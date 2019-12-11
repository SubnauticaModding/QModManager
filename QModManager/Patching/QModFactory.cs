namespace QModManager.Patching
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Oculus.Newtonsoft.Json;
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
            var modSorter = new SortedTree<string, QMod>(subDirectories.Length);
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

                ModStatus status = mod.IsValidForLoading(folderName);

                if (status != ModStatus.Success)
                {
                    earlyErrors.Add(mod, status);
                    continue;
                }

                if (!mod.Enable)
                {
                    earlyErrors.Add(mod, ModStatus.CanceledByUser);
                    continue;
                }

                SortResults sortResult = modSorter.Add(mod);
                switch (sortResult)
                {
                    case SortResults.CircularLoadOrder:
                        earlyErrors.Add(mod, ModStatus.CircularLoadOrder);
                        break;
                    case SortResults.CircularDependency:
                        earlyErrors.Add(mod, ModStatus.CircularDependency);
                        break;
                    case SortResults.DuplicateId:
                        earlyErrors.Add(mod, ModStatus.DuplicateIdDetected);
                        break;
                }
            }

            List<QMod> modsToLoad = modSorter.CreateFlatList(out PairedList<QMod, ErrorTypes> lateErrors);

            PairedList<QMod, ModStatus> modList = CreateModStatusList(earlyErrors, modsToLoad, lateErrors);

            return modList;
        }

        private static PairedList<QMod, ModStatus> CreateModStatusList(PairedList<QMod, ModStatus> earlyErrors, List<QMod> modsToLoad, PairedList<QMod, ErrorTypes> lateErrors)
        {
            var modList = new PairedList<QMod, ModStatus>(modsToLoad.Count + earlyErrors.Count + lateErrors.Count);

            foreach (QMod mod in modsToLoad)
            {
                modList.Add(mod, ModStatus.Success);
            }

            foreach (Pair<QMod, ModStatus> erroredMod in earlyErrors)
            {
                modList.Add(erroredMod.Key, erroredMod.Value);
            }

            foreach (Pair<QMod, ErrorTypes> erroredMod in lateErrors)
            {
                switch (erroredMod.Value)
                {
                    case ErrorTypes.DuplicateId:
                        modList.Add(erroredMod.Key, ModStatus.DuplicateIdDetected);
                        break;
                    case ErrorTypes.CircularDependency:
                        modList.Add(erroredMod.Key, ModStatus.CircularDependency);
                        break;
                    case ErrorTypes.CircularLoadOrder:
                        modList.Add(erroredMod.Key, ModStatus.CircularLoadOrder);
                        break;
                    case ErrorTypes.MissingDepency:
                        modList.Add(erroredMod.Key, ModStatus.MissingDependency);
                        break;
                    default:
                        throw new FatalPatchingException("Invalid error reported by mod sorter");
                }
            }

            foreach (Pair<QMod, ModStatus> pair in modList)
            {
                if (pair.Value != ModStatus.Success)
                    continue;

                QMod mod = pair.Key;
                foreach (RequiredQMod requiredMod in mod.RequiredMods)
                {
                    Pair<QMod, ModStatus> dependency = modList.Find(d => d.Key.Id == requiredMod.Id);

                    if (dependency == null || dependency.Value != ModStatus.Success)
                    {
                        pair.Value = ModStatus.MissingDependency;
                        break;
                    }
                    else if (dependency.Key.ParsedVersion < requiredMod.MinimumVersion)
                    {
                        pair.Value = ModStatus.OutOfDateDependency;
                        break;
                    }
                }
            }

            return modList;
        }

        private static QModLegacy FromJsonFile(string subDirectory)
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
                return JsonConvert.DeserializeObject<QModLegacy>(jsonText);
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
