using BepInEx;
using BepInEx.Logging;
using Mono.Cecil;
using QMMLoader.QMMHarmonyShimmer.Patches;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace QMMLoader.QMMHarmonyShimmer
{
    /// <summary>
    /// Based on the work of IPALoaderX: https://github.com/BepInEx/IPALoaderX/tree/v1.2.2/BepInEx.IPAHarmonyShimmer
    /// </summary>
    public static class QMMHarmonyShimmer
    {
        internal const string SubnauticaProcessName = "Subnautica";
        internal const string SubnauticaZeroProcessName = "SubnauticaZero";

        internal static string QModInstallerPath => Path.Combine(Path.Combine(Path.Combine(Paths.BepInExRootPath, "plugins"), "QMMLoader"), "QModInstaller.dll");
        internal static string QModsPath => Path.Combine(Paths.GameRootPath, "QMods");

        public static IEnumerable<string> TargetDLLs { get; } = new[] { "Assembly-CSharp.dll" };

        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("QMMHarmonyShim");

        private static ReaderParameters readerParameters;
        private static AssemblyDefinition ResolveAssemblies(object sender, AssemblyNameReference assemblyNameReference)
        {
            var name = new AssemblyName(assemblyNameReference.FullName);

            if (Utility.TryResolveDllAssembly(name, Paths.BepInExAssemblyDirectory, readerParameters, out var assembly) ||
                Utility.TryResolveDllAssembly(name, QModsPath, readerParameters, out assembly) ||
                Utility.TryResolveDllAssembly(name, Paths.ManagedPath, readerParameters, out assembly))
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

        public static void ApplyHarmonyPatches()
        {
            var harmony = new HarmonyLib.Harmony("QMMLoader");
            harmony.PatchAll();
        }

        public static void InitAssemblyResolver()
        {
            var resolver = new DefaultAssemblyResolver();
            readerParameters = new ReaderParameters { AssemblyResolver = resolver };
            resolver.ResolveFailure += ResolveAssemblies;
        }

        public static void ApplyShims()
        {
            ShimQMods();
        }

        public static void ShimQMods()
        {
            if (!Directory.Exists(QModsPath))
            {
                Logger.LogInfo("No QMods folder found! Mod shimming aborted.");
                return;
            }

            Logger.LogDebug("Shimming QMods...");
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

            var backupDirectory = Path.Combine(Paths.GameRootPath, "QMods_backup");
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
                            {
                                var pathPart = filePath.Substring(QModsPath.Length + 1);
                                var backupPath = Path.Combine(backupDirectory, pathPart);
                                Logger.LogInfo($"Backing up {Path.GetFileNameWithoutExtension(filePath)} to {backupPath}. Original path: {filePath}");
                                Directory.CreateDirectory(Path.GetDirectoryName(backupPath));
                                File.Copy(filePath, backupPath, true);

                                TypeDefinitionIsDefinitionPatch.definitionsAreReferences = true;
                                assemblyDefinition.Write(filePath);
                                TypeDefinitionIsDefinitionPatch.definitionsAreReferences = false;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Failed to shim {Path.GetFileName(filePath)}");
                        Logger.LogError(e);
                    }
                }
            }
        }

        public static void Initialize()
        {
            ApplyHarmonyPatches();
            InitAssemblyResolver();
            ApplyShims();
        }

        public static void Patch(AssemblyDefinition ad) { }

        public static void Finish()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, resolveEventArgs) =>
            {
                return new AssemblyName(resolveEventArgs.Name).Name == "0Harmony_Shim"
                    ? Assembly.GetExecutingAssembly()
                    : null;
            };
        }
    }
}
