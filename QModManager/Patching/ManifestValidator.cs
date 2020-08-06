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

    internal class ManifestValidator : IManifestValidator
    {
        internal static IVersionParser VersionParserService { get; set; } = new VersionParser();

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

            Logger.Debug($"Validating mod '{mod.Id}'");
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
            {
                mod.Status = reason;
                return;
            }

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
                mod.ParsedVersion = VersionParserService.GetVersion(mod.Version);
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
            var requiredMods = new Dictionary<string, RequiredQMod>(mod.VersionDependencies.Count + mod.Dependencies.Length);

            foreach (string id in mod.Dependencies)
            {
                mod.RequiredDependencies.Add(id);
                requiredMods.Add(id, new RequiredQMod(id));
            }

            foreach (string id in mod.LoadBefore)
                mod.LoadBeforePreferences.Add(id);

            foreach (string id in mod.LoadAfter)
                mod.LoadAfterPreferences.Add(id);

            if (mod.VersionDependencies.Count > 0)
            {
                foreach (KeyValuePair<string, string> item in mod.VersionDependencies)
                {
                    string id = item.Key;
                    string versionString = item.Value;

                    Version version = VersionParserService.GetVersion(versionString);

                    requiredMods[id] = new RequiredQMod(id, version);

                    mod.RequiredDependencies.Add(id);
                }
            }

            mod.RequiredMods = requiredMods.Values;

            if (Logger.DebugLogsEnabled)
            {
                string GetModList(IEnumerable<string> modIds)
                {
                    string modList = string.Empty;
                    foreach (var id in modIds)
                        modList += $"{id} ";

                    return modList;
                }

                if (requiredMods.Count > 0)
                    Logger.Debug($"{mod.Id} has required mods: {GetModList(mod.RequiredMods.Select(mod => mod.Id))}");

                if (mod.LoadBeforePreferences.Count > 0)
                    Logger.Debug($"{mod.Id} should load before: {GetModList(mod.LoadBeforePreferences)}");

                if (mod.LoadAfterPreferences.Count > 0)
                    Logger.Debug($"{mod.Id} should load after: {GetModList(mod.LoadAfterPreferences)}");
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

                    MethodInfo jsonPatchMethod = qMod.LoadedAssembly.GetType(entryType)?.GetMethod(entryMethod, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

                    if (jsonPatchMethod != null && jsonPatchMethod.GetParameters().Length == 0)
                    {
                        qMod.PatchMethods[PatchingOrder.NormalInitialize] = new QModPatchMethod(jsonPatchMethod, qMod, PatchingOrder.NormalInitialize);
                        return ModStatus.Success;
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
                                    continue;
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
