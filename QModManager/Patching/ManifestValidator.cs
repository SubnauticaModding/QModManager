namespace QModManager.Patching
{
    using QModManager.API;
    using QModManager.API.ModLoading;
    using QModManager.Utility;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    internal class ManifestValidator : IManifestValidator
    {
        internal static readonly Regex VersionRegex = new Regex(@"(((\d+)\.?)+)");

        internal static readonly Dictionary<string, ModStatus> ProhibitedModIDs = new Dictionary<string, ModStatus>()
        {
            { "QModManager", ModStatus.BannedID },
            { "QModInstaller", ModStatus.BannedID },
            { "EnableAchievements", ModStatus.Merged },
        };

        public void ValidateManifest(QMod mod)
        {
            if (mod.Status != ModStatus.Success)
                return;

            if (mod.PatchMethods.Count > 0)
                return;

            Logger.Debug($"Validating mod in '{mod.SubDirectory}'");
            if (string.IsNullOrEmpty(mod.Id) ||
                string.IsNullOrEmpty(mod.DisplayName) ||
                string.IsNullOrEmpty(mod.Author))
            {
                mod.Status = ModStatus.MissingCoreInfo;
                return;
            }

            if (!mod.Enable)
            {
                mod.Status = ModStatus.CanceledByUser;
                return;
            }

            if (ProhibitedModIDs.TryGetValue(mod.Id, out ModStatus reason))
                return mod.Status = reason;

            switch (mod.Game)
            {
                case "BelowZero":
                    mod.SupportedGame = QModGame.BelowZero;
                    break;
                case "Both":
                    mod.SupportedGame = QModGame.Both;
                    break;
                case "Subnautica":
                    mod.SupportedGame = QModGame.Subnautica;
                    break;
                default:
                {
                    mod.Status = ModStatus.FailedIdentifyingGame;
                    return;
                }
            }

            try
            {
                if (Version.TryParse(mod.Version, out Version version))
                    mod.ParsedVersion = version;
            }
            catch (Exception vEx)
            {
                Logger.Error($"There was an error parsing version \"{mod.Version}\" for mod \"{mod.DisplayName}\"");
                Logger.Exception(vEx);

                mod.Status = ModStatus.InvalidCoreInfo;
                return;
            }

            string modAssemblyPath = Path.Combine(mod.SubDirectory, mod.AssemblyName);

            if (string.IsNullOrEmpty(modAssemblyPath) || !File.Exists(modAssemblyPath))
            {
                Logger.Debug($"Did not find a DLL at {modAssemblyPath}");
                mod.Status = ModStatus.MissingAssemblyFile;
                return;
            }
            else
            {
                try
                {
                    mod.LoadedAssembly = Assembly.LoadFrom(modAssemblyPath);
                }
                catch (Exception aEx)
                {
                    Logger.Error($"Failed loading the dll found at \"{modAssemblyPath}\" for mod \"{mod.DisplayName}\"");
                    Logger.Exception(aEx);
                    mod.Status = ModStatus.FailedLoadingAssemblyFile;
                    return;
                }
            }

            ModStatus patchMethodResults = FindPatchMethods(mod);

            if (patchMethodResults != ModStatus.Success)
            {
                mod.Status = patchMethodResults;
                return;
            }
        }

        public void CheckRequiredMods(QMod mod)
        {
            foreach (string item in mod.Dependencies)
                mod.RequiredDependencies.Add(item);

            foreach (string item in mod.LoadBefore)
                mod.LoadBeforePreferences.Add(item);

            foreach (string item in mod.LoadAfter)
                mod.LoadAfterPreferences.Add(item);

            if (mod.VersionDependencies.Count > 0)
            {
                var versionedDependencies = new List<RequiredQMod>(mod.VersionDependencies.Count);
                foreach (KeyValuePair<string, string> item in mod.VersionDependencies)
                {
                    string cleanVersion = VersionRegex.Matches(item.Value)?[0]?.Value;

                    if (string.IsNullOrEmpty(cleanVersion))
                    {
                        versionedDependencies.Add(new RequiredQMod(item.Key));
                    }
                    else if (Version.TryParse(cleanVersion, out Version version))
                    {
                        versionedDependencies.Add(new RequiredQMod(item.Key, version));
                    }
                    else
                    {
                        versionedDependencies.Add(new RequiredQMod(item.Key));
                    }
                }

                mod.RequiredMods = versionedDependencies;
            }
        }

        internal ModStatus FindPatchMethods(QMod qMod)
        {
            try
            {
                if (!string.IsNullOrEmpty(qMod.EntryMethod))
                {
                    // Legacy
                    string[] entryMethodSig = qMod.EntryMethod.Split('.');
                    string entryType = string.Join(".", entryMethodSig.Take(entryMethodSig.Length - 1).ToArray());
                    string entryMethod = entryMethodSig[entryMethodSig.Length - 1];

                    MethodInfo jsonPatchMethod = qMod.LoadedAssembly.GetType(entryType).GetMethod(entryMethod, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

                    if (jsonPatchMethod != null && jsonPatchMethod.GetParameters().Length == 0)
                    {
                        qMod.PatchMethods[PatchingOrder.NormalInitialize] = new QModPatchMethod(jsonPatchMethod, qMod, PatchingOrder.NormalInitialize);
                    }
                }

                // QMM 3.0
                foreach (Type type in qMod.LoadedAssembly.GetTypes())
                {
                    if (type.IsNotPublic || type.IsEnum || type.ContainsGenericParameters)
                        continue;

                    foreach (QModCoreAttribute core in type.GetCustomAttributes(typeof(QModCoreAttribute), false))
                    {
                        foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
                        {
                            foreach (QModPatchAttributeBase patch in method.GetCustomAttributes(typeof(QModPatchAttributeBase), false))
                            {
                                switch (patch.PatchOrder)
                                {
                                    case PatchingOrder.MetaPreInitialize:
                                    case PatchingOrder.MetaPostInitialize:
                                        if (!patch.ValidateSecretPassword(method, qMod))
                                        {
                                            Logger.Error($"The mod {qMod.Id} has an invalid priority patching password.");
                                            qMod.PatchMethods.Clear();
                                            return ModStatus.InvalidCoreInfo;
                                        }
                                        break;
                                }

                                if (qMod.PatchMethods.TryGetValue(patch.PatchOrder, out QModPatchMethod extra))
                                {
                                    if (extra.Method.Name != method.Name)
                                        return ModStatus.TooManyPatchMethods;
                                }
                                else
                                {
                                    qMod.PatchMethods[patch.PatchOrder] = new QModPatchMethod(method, qMod, patch.PatchOrder);
                                }
                            }
                        }
                    }
                }
            }
            catch (TypeLoadException tlEx)
            {
                Logger.Debug($"Unable to load types for '{qMod.Id}': " + tlEx.Message);
                return ModStatus.MissingDependency;
            }
            catch (MissingMethodException mmEx)
            {
                Logger.Debug($"Unable to find patch method for '{qMod.Id}': " + mmEx.Message);
                return ModStatus.MissingDependency;
            }
            catch (ReflectionTypeLoadException rtle)
            {
                Logger.Debug($"Unable to load types for '{qMod.Id}': " + rtle.Message);
                return ModStatus.MissingDependency;
            }

            if (qMod.PatchMethods.Count == 0)
                return ModStatus.MissingPatchMethod;

            return ModStatus.Success;
        }
    }
}
