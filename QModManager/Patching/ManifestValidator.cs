namespace QModManager.Patching
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using QModManager.API;
    using QModManager.API.ModLoading;
    using QModManager.Utility;

    internal class ManifestValidator
    {
        internal static readonly Regex VersionRegex = new Regex(@"(((\d+)\.?)+)");

        internal static readonly Dictionary<string, ModStatus> ProhibitedModIDs = new Dictionary<string, ModStatus>()
        {
            { "QModManager", ModStatus.BannedID },
            { "QModInstaller", ModStatus.BannedID },
            { "EnableAchievements", ModStatus.Merged },
        };

        public ModStatus ValidateManifest(QMod mod, string subDirectory)
        {
            Logger.Debug($"Validating mod in '{subDirectory}'");
            if (string.IsNullOrEmpty(mod.Id) ||
                string.IsNullOrEmpty(mod.DisplayName) ||
                string.IsNullOrEmpty(mod.Author))
                return mod.Status = ModStatus.MissingCoreInfo;

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
                    return mod.Status = ModStatus.FailedIdentifyingGame;
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

                return mod.Status = ModStatus.InvalidCoreInfo;
            }

            string modAssemblyPath = Path.Combine(subDirectory, mod.AssemblyName);

            if (string.IsNullOrEmpty(modAssemblyPath) || !File.Exists(modAssemblyPath))
            {
                Logger.Debug($"Did not find a DLL at {modAssemblyPath}");
                return mod.Status = ModStatus.MissingAssemblyFile;
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
                    return mod.Status = ModStatus.FailedLoadingAssemblyFile;
                }
            }

            try
            {
                ModStatus patchMethodResults = FindPatchMethods(mod);

                if (patchMethodResults != ModStatus.Success)
                    return mod.Status = patchMethodResults;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                return mod.Status = ModStatus.MissingPatchMethod;
            }

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
                    else if (System.Version.TryParse(cleanVersion, out Version version))
                    {
                        versionedDependencies.Add(new RequiredQMod(item.Key, version));
                    }
                    else
                    {
                        versionedDependencies.Add(new RequiredQMod(item.Key));
                    }
                }
            }

            if (!mod.Enable)
                return mod.Status = ModStatus.CanceledByUser;

            return mod.Status = ModStatus.Success;
        }

        internal ModStatus FindPatchMethods(QMod qMod)
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
                                    patch.ValidateSecretPassword(method, qMod);
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

            if (qMod.PatchMethods.Count == 0)
                return ModStatus.MissingPatchMethod;

            return ModStatus.Success;
        }
    }
}
