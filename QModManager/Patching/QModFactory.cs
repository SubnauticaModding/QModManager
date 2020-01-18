namespace QModManager.Patching
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Oculus.Newtonsoft.Json;
    using QModManager.API;
    using QModManager.API.ModLoading;
    using QModManager.DataStructures;
    using QModManager.Utility;

    internal class QModFactory : IQModFactory
    {
        internal static readonly ManifestValidator Validator = new ManifestValidator();

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

            // Load Mods - Checking for duplicates
            LoadModsFromDirectories(subDirectories, modSorter, earlyErrors);

            // Sort
            List<QMod> modsToLoad = modSorter.GetSortedList();

            ValidateManifests(earlyErrors, modsToLoad);

            return CreateModStatusList(earlyErrors, modsToLoad);
        }

        internal static void LoadModsFromDirectories(string[] subDirectories, SortedCollection<string, QMod> modSorter, List<QMod> earlyErrors)
        {
            foreach (string subDir in subDirectories)
            {
                string[] dllFiles = Directory.GetFiles(subDir, "*.dll", SearchOption.TopDirectoryOnly);

                if (dllFiles.Length < 1)
                    continue;

                string jsonFile = Path.Combine(subDir, "mod.json");

                string folderName = new DirectoryInfo(subDir).Name;

                if (!File.Exists(jsonFile))
                {
                    Logger.Error($"Unable to set up mod in folder \"{folderName}\"");
                    earlyErrors.Add(new QModPlaceholder(folderName, ModStatus.InvalidCoreInfo));
                    continue;
                }

                QMod mod = CreateFromJsonManifestFile(subDir);

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

        private static void ValidateManifests(List<QMod> earlyErrors, List<QMod> modsToLoad)
        {
            foreach (QMod mod in modsToLoad)
            {
                ModStatus status = Validator.ValidateManifest(mod, mod.SubDirectory);

                if (status != ModStatus.Success)
                {
                    Logger.Debug($"Mod '{mod.Id}' will not be loaded");
                    earlyErrors.Add(mod);
                    continue;
                }
            }
        }

        internal static List<QMod> CreateModStatusList(List<QMod> earlyErrors, List<QMod> modsToLoad)
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

            foreach (QMod mod in modList)
            {
                if (mod.Status != ModStatus.Success)
                    continue;

                if (mod.RequiredMods == null)
                    continue;

                foreach (RequiredQMod requiredMod in mod.RequiredMods)
                {
                    QMod dependency = modsToLoad.Find(d => d.Id == requiredMod.Id);

                    if (dependency == null || dependency.Status != ModStatus.Success)
                    {
                        mod.Status = ModStatus.MissingDependency;
                        break;
                    }

                    if (dependency.ParsedVersion < requiredMod.MinimumVersion)
                    {
                        mod.Status = ModStatus.OutOfDateDependency;
                        break;
                    }
                }
            }

            return modList;
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
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };

                string jsonText = File.ReadAllText(jsonFile);

                QMod mod = JsonConvert.DeserializeObject<QMod>(jsonText);

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
