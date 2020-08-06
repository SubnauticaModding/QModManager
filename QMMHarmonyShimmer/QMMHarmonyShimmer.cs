using BepInEx;
using BepInEx.Logging;
using Mono.Cecil;
using QModManager.QMMHarmonyShimmer.Patches;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace QModManager.QMMHarmonyShimmer
{
    /// <summary>
    /// Based on the work of IPALoaderX: https://github.com/BepInEx/IPALoaderX/tree/v1.2.2/BepInEx.IPAHarmonyShimmer
    /// A patcher which runs ahead of any BepInEx plugins (eg. QMMLoader) to identify QMods referencing older versions of Harmony than the
    /// Harmony shipped with BepInEx, backs them up and creates a patched copy which references new Harmony, shimming the outdated Harmony
    /// API.
    /// </summary>
    [Obsolete("Should not be used!", true)]
    public static class QMMHarmonyShimmer
    {
        internal static string QMMLoaderPluginPath => Path.Combine(Paths.BepInExRootPath, "plugins", "QModManager");
        internal static string QModInstallerPath => Path.Combine(QMMLoaderPluginPath, "QModInstaller.dll");
        internal static string QModsPath => Path.Combine(Paths.GameRootPath, "QMods");
        internal static string QModBackupsPath => Path.Combine(QModsPath, ".backups");

        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("QMMHarmonyShimmer");

        /// <summary>
        /// Called from BepInEx while patching, our entry point for patching.
        /// Do not change the method name as it is identified by BepInEx. Method must remain public.
        /// </summary>
        [Obsolete("Should not be used!", true)]
        public static void Initialize()
        {
            ApplyHarmonyPatches();
            InitAssemblyResolver();
            ApplyShims();
        }

        private static void ApplyHarmonyPatches()
        {
            var harmony = new HarmonyLib.Harmony("QMMLoader");
            harmony.PatchAll();
        }

        private static void InitAssemblyResolver()
        {
            var resolver = new DefaultAssemblyResolver();
            readerParameters = new ReaderParameters { AssemblyResolver = resolver };
            resolver.ResolveFailure += ResolveAssemblies;
        }

        private static ReaderParameters readerParameters;
        private static AssemblyDefinition ResolveAssemblies(object sender, AssemblyNameReference assemblyNameReference)
        {
            var name = new AssemblyName(assemblyNameReference.FullName);

            if (Utility.TryResolveDllAssembly(name, Paths.BepInExAssemblyDirectory, readerParameters, out var assembly) || // First try BepInEx assemblies
                Utility.TryResolveDllAssembly(name, QMMLoaderPluginPath, readerParameters, out assembly) || // Then QMMLoader assemblies
                Utility.TryResolveDllAssembly(name, QModsPath, readerParameters, out assembly) || // Then QMods
                Utility.TryResolveDllAssembly(name, Paths.ManagedPath, readerParameters, out assembly)) // Finally, Subnautica Managed assemblies
            {
                return assembly;
            }
            else if (assemblyNameReference.Name == "0Harmony_Shim")
            {
                return AssemblyDefinition.ReadAssembly(Assembly.GetExecutingAssembly().Location);
            }
            else
            {
                return null;
            }
        }

        private static void ApplyShims()
        {
            ShimQMods();
        }

        private static void ShimQMods()
        {
            if (!Directory.Exists(QModsPath))
            {
                Logger.LogInfo("No QMods folder found! Mod shimming aborted.");
                return;
            }

            Logger.LogInfo("Shimming QMods...");
            var harmonyTypes = new HashSet<string>();
            using (var shimmerAssemblyDefinition = AssemblyDefinition.ReadAssembly(Assembly.GetExecutingAssembly().Location))
            {
                foreach (var type in shimmerAssemblyDefinition.MainModule.Types)
                {
                    if (type.Namespace.StartsWith("Harmony"))
                    {
                        harmonyTypes.Add(type.FullName);
                    }
                }
            }

            var harmonyFullTypes = new Dictionary<string, string>();
            using (var harmonyAssemblyDefinition = AssemblyDefinition.ReadAssembly(Path.Combine(Paths.BepInExAssemblyDirectory, "0Harmony.dll")))
            {
                foreach (var type in harmonyAssemblyDefinition.MainModule.Types)
                {
                    if (type.Namespace.StartsWith("HarmonyLib"))
                    {
                        harmonyFullTypes.Add(type.Name, type.Namespace);
                    }
                }
            }

            foreach (var subfolderPath in Directory.GetDirectories(QModsPath))
            {
                foreach (string filePath in Directory.GetFiles(subfolderPath, "*.dll", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        using (var stream = new MemoryStream(File.ReadAllBytes(filePath)))
                        {
                            var assemblyDefinition = AssemblyDefinition.ReadAssembly(stream);

                            if (assemblyDefinition.MainModule.AssemblyResolver is DefaultAssemblyResolver qModResolver)
                            {
                                qModResolver.ResolveFailure += ResolveAssemblies;
                            }

                            var harmonyShimReference = new AssemblyNameReference("0Harmony_Shim", new Version(1, 1, 0, 0));
                            var harmony2Reference = new AssemblyNameReference("0Harmony", new Version(2, 0, 0, 0));
                            var outdatedHarmonyReference = assemblyDefinition.MainModule.AssemblyReferences
                                .FirstOrDefault(reference => reference.Name.StartsWith("0Harmony") && reference.Name != "0Harmony_Shim" && reference.Version.Major <= 1);

                            bool shimmed = false;

                            if (assemblyDefinition.MainModule.Types.Any(type => harmonyTypes.Contains(type.FullName)))
                            {   // Unmerge the assembly
                                shimmed = true;
                                Logger.LogInfo($"Unmerging {Path.GetFileNameWithoutExtension(filePath)}");
                                assemblyDefinition.MainModule.AssemblyReferences.Add(harmony2Reference);
                                assemblyDefinition.MainModule.AssemblyReferences.Add(harmonyShimReference);

                                foreach (var typeDefinition in assemblyDefinition.MainModule.Types.ToList())
                                {
                                    string @namespace = null;
                                    if (harmonyTypes.Contains(typeDefinition.FullName) ||
                                        (typeDefinition.Namespace.StartsWith("Harmony") && harmonyFullTypes.TryGetValue(typeDefinition.Name, out @namespace)))
                                    {
                                        assemblyDefinition.MainModule.Types.Remove(typeDefinition);
                                        typeDefinition.Scope = harmonyTypes.Contains(typeDefinition.FullName) ? harmonyShimReference : harmony2Reference;
                                        typeDefinition.GetType().GetField("module", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(typeDefinition, assemblyDefinition.MainModule);
                                        typeDefinition.MetadataToken = new MetadataToken(TokenType.TypeRef, 0);

                                        if (@namespace != null)
                                        {
                                            typeDefinition.Namespace = @namespace;
                                        }

                                        foreach (var method in typeDefinition.Methods)
                                        {
                                            method.MetadataToken = new MetadataToken(TokenType.MemberRef, 0);
                                        }

                                        foreach (var member in typeDefinition.Fields.Cast<IMemberDefinition>().Concat(typeDefinition.Methods.Cast<IMemberDefinition>()))
                                        {
                                            member.MetadataToken = new MetadataToken(TokenType.MemberRef, 0);
                                        }
                                    }
                                }
                            }
                            else if (outdatedHarmonyReference != null)
                            {   // Shim Harmony
                                shimmed = true;
                                Logger.LogInfo($"Shimming {Path.GetFileNameWithoutExtension(filePath)}");
                                outdatedHarmonyReference.Name = "0Harmony_Shim";
                                assemblyDefinition.MainModule.AssemblyReferences.Add(harmony2Reference);

                                foreach (var typeReference in assemblyDefinition.MainModule.GetTypeReferences())
                                {
                                    if (typeReference.Namespace.StartsWith("Harmony") &&
                                        !harmonyTypes.Contains(typeReference.FullName) &&
                                        harmonyFullTypes.TryGetValue(typeReference.Name, out var @namespace))
                                    {
                                        typeReference.Namespace = @namespace;
                                        typeReference.Scope = harmony2Reference;
                                    }
                                }

                                foreach (var memberReference in assemblyDefinition.MainModule.GetMemberReferences())
                                {
                                    if (memberReference is MethodReference methodReference &&
                                        methodReference.DeclaringType.Name == "HarmonyInstance" &&
                                        methodReference.DeclaringType.Scope == outdatedHarmonyReference &&
                                        methodReference.Name == "Patch" &&
                                        methodReference.ReturnType.FullName == "System.Void")
                                    {
                                        methodReference.Name = "PatchVoid";
                                    }
                                }
                            }

                            if (shimmed)
                            {   // backup the original to QMods\.backups and write the patched back to the QMods folder, where QModManager loads from.
                                var pathPart = filePath.Substring(QModsPath.Length + 1);
                                var backupPath = Path.Combine(QModBackupsPath, pathPart);
                                Logger.LogInfo($"Backing up {Path.GetFileNameWithoutExtension(filePath)} to {backupPath}. Original path: {filePath}");
                                var backupDirInfo = Directory.CreateDirectory(QModBackupsPath);
                                if ((backupDirInfo.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                                {
                                    backupDirInfo.Attributes |= FileAttributes.Hidden;
                                }
                                Directory.CreateDirectory(Path.GetDirectoryName(backupPath));
                                File.Copy(filePath, backupPath, true);

                                TypeDefinitionIsDefinitionPatch.definitionsAreReferences = true;
                                assemblyDefinition.Write(filePath);
                                TypeDefinitionIsDefinitionPatch.definitionsAreReferences = false;
                            }
                        }
                    }
                    catch (BadImageFormatException)
                    {
                        if (Path.GetFileName(filePath).Contains("0Harmony"))
                            Logger.LogWarning($"QMod in folder {Path.GetDirectoryName(subfolderPath)} is shipping its own version of Harmony! " +
                                $"This is NOT RECOMMENDED and can lead to serious compatibility issues with other mods! " +
                                $"If you are a mod author, please do not ship Harmony with your mods, and instead rely on QModManager to load it for you.");
                        else
                            Logger.LogInfo($"Cannot shim {Path.GetFileName(filePath)} as it is not a valid assembly, skipping...");
                        continue;
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Failed to shim {Path.GetFileName(filePath)}");
                        Logger.LogError(e);
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Called from BepInEx while patching, runs after all patches complete.
        /// Do not change the method name as it is identified by BepInEx. Method must remain public.
        /// </summary>
        [Obsolete("Should not be used!", true)]
        public static void Finish()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, resolveEventArgs) =>
            {
                return new AssemblyName(resolveEventArgs.Name).Name == "0Harmony_Shim" // Redirect references to the shim to this assembly,
                    ? Assembly.GetExecutingAssembly()                                  // since we are emulating the API.
                    : null;
            };
        }

        /// <summary>
        /// For BepInEx to identify your patcher as a patcher, it must match the patcher contract as outlined in the BepInEx docs:
        /// https://bepinex.github.io/bepinex_docs/v5.0/articles/dev_guide/preloader_patchers.html#patcher-contract
        /// It must contain a list of managed assemblies to patch as a public static <see cref="IEnumerable{T}"/> property named TargetDLLs
        /// </summary>
        [Obsolete("Should not be used!", true)]
        public static IEnumerable<string> TargetDLLs { get; } = new string[0];

        /// <summary>
        /// For BepInEx to identify your patcher as a patcher, it must match the patcher contract as outlined in the BepInEx docs:
        /// https://bepinex.github.io/bepinex_docs/v5.0/articles/dev_guide/preloader_patchers.html#patcher-contract
        /// It must contain a public static void method named Patch which receives an <see cref="AssemblyDefinition"/> argument,
        /// which patches each of the target assemblies in the TargetDLLs list.
        /// 
        /// We don't actually need to patch any of the managed assemblies, so we are providing an empty method here, and instead dynamically patch
        /// assemblies in the QMods directory via the <see cref="ShimQMods"/> method.
        /// </summary>
        /// <param name="ad"></param>
        [Obsolete("Should not be used!", true)]
        public static void Patch(AssemblyDefinition ad) { }
    }
}
