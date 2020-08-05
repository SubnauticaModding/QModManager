namespace QModManager.Patching
{
    using BepInEx;
    using Oculus.Newtonsoft.Json;
    using QModManager.API;
    using QModManager.DataStructures;
    using QModManager.Utility;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class QModFactory : IQModFactory
    {
        internal IManifestValidator Validator { get; set; } = new ManifestValidator();

        /// <summary>
        /// Searches through all folders in the provided directory and returns an ordered list of mods to load.<para/>
        /// Mods that cannot be loaded will have an unsuccessful <see cref="QMod.Status"/> value.
        /// </summary>
        /// <param name="qmodsDirectory">The QMods directory</param>
        /// <returns>A new, sorted <see cref="List{QMod}"/> ready to be initialized or skipped.</returns>
        public List<QMod> BuildModLoadingList(string qmodsDirectory)
        {
            if (!Directory.Exists(qmodsDirectory))
            {
                Logger.Info("QMods directory was not found! Creating...");
                Directory.CreateDirectory(qmodsDirectory);

                return new List<QMod>(0);
            }

            string[] subDirectories = Directory.GetDirectories(qmodsDirectory);
            var modSorter = new SortedCollection<string, QMod>();
            var earlyErrors = new List<QMod>(subDirectories.Length);

            LoadModsFromDirectories(subDirectories, modSorter, earlyErrors);

            List<QMod> modsToLoad = modSorter.GetSortedList();

            return CreateModStatusList(earlyErrors, modsToLoad);
        }

        internal void LoadModsFromDirectories(string[] subDirectories, SortedCollection<string, QMod> modSorter, List<QMod> earlyErrors)
        {
            foreach (string subDir in subDirectories.Where(subDir => (new DirectoryInfo(subDir).Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)) // exclude hidden directories
            {
                string[] dllFiles = Directory.GetFiles(subDir, "*.dll", SearchOption.TopDirectoryOnly);

                if (dllFiles.Length < 1)
                    continue;

                string jsonFile = Path.Combine(subDir, "mod.json");

                string folderName = new DirectoryInfo(subDir).Name;

                if (!File.Exists(jsonFile))
                {
                    Logger.Error($"Unable to set up mod in folder \"{folderName}\"");
                    earlyErrors.Add(new QModPlaceholder(folderName, ModStatus.MissingCoreInfo));
                    continue;
                }

                QMod mod = CreateFromJsonManifestFile(subDir);

                if (mod == null)
                {
                    Logger.Error($"Unable to set up mod in folder \"{folderName}\"");
                    earlyErrors.Add(new QModPlaceholder(folderName, ModStatus.MissingCoreInfo));
                    continue;
                }

                this.Validator.CheckRequiredMods(mod);

                Logger.Debug($"Sorting mod {mod.Id}");
                bool added = modSorter.AddSorted(mod);
                if (!added)
                {
                    Logger.Debug($"DuplicateId on mod {mod.Id}");
                    mod.Status = ModStatus.DuplicateIdDetected;
                    earlyErrors.Add(mod);
                }
            }
        }

        internal List<QMod> CreateModStatusList(List<QMod> earlyErrors, List<QMod> modsToLoad)
        {
            var modList = new List<QMod>(modsToLoad.Count + earlyErrors.Count);

            foreach (QMod mod in modsToLoad)
            {
                Logger.Debug($"{mod.Id} ready to load");
                modList.Add(mod);
            }

            foreach (QMod erroredMod in earlyErrors)
            {
                Logger.Debug($"{erroredMod.Id} had an early error");
                modList.Add(erroredMod);
            }

            Logger.Debug($"Validating basic manifest data");
            foreach (QMod mod in modList)
            {
                if (mod.Status == ModStatus.Success)
                    this.Validator.ValidateManifest(mod);
            }

            Logger.Debug($"Loading assemblies");
            foreach (QMod mod in modList)
            {
                if (mod.Status == ModStatus.Success)
                    this.Validator.LoadAssembly(mod);
            }

            Logger.Debug($"Checking mod dependencies");
            foreach (QMod mod in modList)
            {
                if (mod.Status == ModStatus.Success)
                    ValidateDependencies(modList, mod);
            }

            Logger.Debug($"Checking mod patch methods");
            foreach (QMod mod in modList)
            {
                if (mod.Status == ModStatus.Success)
                    this.Validator.FindPatchMethods(mod);
            }


            return modList;
        }

        internal List<PluginInfo> BepInExPlugins { get; set; }
        internal List<PluginInfo> RequiredBepInExPlugins { get; set; } = new List<PluginInfo>();
        private void ValidateDependencies(List<QMod> modsToLoad, QMod mod)
        {
            // Check the mod dependencies

            bool printed = false;
            foreach (RequiredQMod requiredMod in mod.RequiredMods)
            {
                if (!printed)
                {
                    Logger.Debug($"Checking dependencies for '{mod.Id}'");
                    printed = true;
                }

                QMod dependencyQMod = modsToLoad.Find(d => d.Id == requiredMod.Id);
                PluginInfo dependencyPluginInfo = BepInExPlugins.Find(d => d.Metadata.GUID == requiredMod.Id);

                if ((dependencyQMod == null || dependencyQMod.Status != ModStatus.Success) && dependencyPluginInfo == null)
                {
                    // Dependency not found or failed
                    Logger.Error($"{mod.Id} cannot be loaded because it is missing a dependency. Missing mod: {requiredMod.Id}");
                    mod.Status = ModStatus.MissingDependency;
                    break;
                }
                else if (dependencyQMod == null && dependencyPluginInfo != null)
                {
                    if (!RequiredBepInExPlugins.Contains(dependencyPluginInfo))
                        RequiredBepInExPlugins.Add(dependencyPluginInfo);

                    continue;
                }

                if (dependencyQMod.HasDependencies)
                {
                    ValidateDependencies(modsToLoad, dependencyQMod);
                }

                if (dependencyQMod.Status != ModStatus.Success)
                {
                    // Dependency failed to load successfully
                    // Treat it as missing
                    Logger.Error($"{mod.Id} cannot be loaded because its dependency failed to load. Failed mod: {requiredMod.Id}");
                    mod.Status = ModStatus.MissingDependency;
                    break;
                }

                if (dependencyQMod.ParsedVersion < requiredMod.MinimumVersion)
                {
                    // Dependency version is older than the version required by this mod
                    Logger.Error($"{mod.Id} cannot be loaded because its dependency is out of date. Outdated mod: {requiredMod.Id}");
                    mod.Status = ModStatus.OutOfDateDependency;
                    break;
                }
            }
        }

        private static QMod CreateFromJsonManifestFile(string subDirectory)
        {
            string jsonFile = Path.Combine(subDirectory, "mod.json");

            if (!File.Exists(jsonFile))
            {
                return null;
            }

            try
            {
                var deserializer = new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };

                string jsonText = File.ReadAllText(jsonFile);

                using StreamReader sr = new StreamReader(jsonFile);
                using JsonReader reader = new JsonTextReader(sr);
                QMod mod = deserializer.Deserialize<QMod>(reader);

                mod.SubDirectory = subDirectory;

                return mod;
            }
            catch (Exception e)
            {
                Logger.Error($"\"mod.json\" deserialization failed for file \"{jsonFile}\"!");
                Logger.Exception(e);

                return null;
            }
        }
    }
}
