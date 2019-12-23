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

    internal class QModFactory
    {
        internal PairedList<QMod, ModStatus> BuildModLoadingList(string qmodsDirectory)
        {
            if (!Directory.Exists(qmodsDirectory))
            {
                Logger.Info("QMods directory was not found! Creating...");
                Directory.CreateDirectory(qmodsDirectory);

                return new PairedList<QMod, ModStatus>(0);
            }

            string[] subDirectories = Directory.GetDirectories(qmodsDirectory);
            var modSorter = new SortedCollection<string, QMod>();
            var earlyErrors = new PairedList<QMod, ModStatus>(subDirectories.Length);

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
                    earlyErrors.Add(new QModPlaceholder(folderName), ModStatus.InvalidCoreInfo);
                    continue;
                }

                QMod mod = FromJsonFile(subDir);

                ModStatus status = mod.IsValidForLoading(subDir);

                if (status != ModStatus.Success)
                {
                    Logger.Debug($"Mod '{mod.Id}' had invalid core data");
                    earlyErrors.Add(mod, status);
                    continue;
                }

                if (!mod.Enable)
                {
                    earlyErrors.Add(mod, ModStatus.CanceledByUser);
                    continue;
                }

                Logger.Debug($"Sorting mod {mod.Id}");
                bool added = modSorter.AddSorted(mod);
                if (!added)
                {
                    Logger.Debug($"DuplicateId on mod {mod.Id}");
                    earlyErrors.Add(mod, ModStatus.DuplicateIdDetected);
                }
            }

            List<QMod> modsToLoad = modSorter.GetSortedList();

            PairedList<QMod, ModStatus> modList = CreateModStatusList(earlyErrors, modsToLoad);

            return modList;
        }

        private static PairedList<QMod, ModStatus> CreateModStatusList(
            PairedList<QMod, ModStatus> earlyErrors, List<QMod> modsToLoad)
        {
            var modList = new PairedList<QMod, ModStatus>(modsToLoad.Count + earlyErrors.Count);

            foreach (QMod mod in modsToLoad)
            {
                Logger.Debug($"{mod.Id} ready to load");
                modList.Add(mod, ModStatus.Success);
            }

            foreach (Pair<QMod, ModStatus> erroredMod in earlyErrors)
            {
                Logger.Debug($"{erroredMod.Key.Id} had an early error");
                modList.Add(erroredMod.Key, erroredMod.Value);
            }

            foreach (Pair<QMod, ModStatus> pair in modList)
            {
                if (pair.Value != ModStatus.Success)
                    continue;

                QMod mod = pair.Key;

                if (mod.RequiredMods == null)
                    continue;

                foreach (RequiredQMod requiredMod in mod.RequiredMods)
                {
                    Pair<QMod, ModStatus> dependency = modList.Find(d => d.Key.Id == requiredMod.Id);

                    if (dependency == null || dependency.Key == null)
                    {
                        pair.Value = ModStatus.MissingDependency;
                        break;
                    }

                    if (dependency.Value != ModStatus.Success)
                    {
                        pair.Value = ModStatus.MissingDependency;
                        break;
                    }

                    if (dependency.Key.ParsedVersion < requiredMod.MinimumVersion)
                    {
                        pair.Value = ModStatus.OutOfDateDependency;
                        break;
                    }
                }
            }

            return modList;
        }

        private static QModJson FromJsonFile(string subDirectory)
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
                return JsonConvert.DeserializeObject<QModJson>(jsonText);
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
