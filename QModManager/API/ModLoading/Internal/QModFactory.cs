namespace QModManager.API.ModLoading.Internal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Oculus.Newtonsoft.Json;
    using QModManager.DataStructures;
    using QModManager.Utility;

    internal class QModFactory
    {
        private SortedTree<string, QMod> sortedMods;
        private List<QMod> unloadedMods;
        private string[] subDirectories;

        internal int FailedToCreate { get; private set; }

        internal List<QMod> BuildModLoadingList(string qmodsDirectory)
        {
            if (!Directory.Exists(qmodsDirectory))
            {
                Logger.Info("QMods directory was not found! Creating...");
                Directory.CreateDirectory(qmodsDirectory);

                return new List<QMod>(0);
            }

            subDirectories = Directory.GetDirectories(qmodsDirectory);
            sortedMods = new SortedTree<string, QMod>(subDirectories.Length);
            unloadedMods = new List<QMod>((subDirectories.Length / 2) + 1);

            foreach (string subDir in subDirectories)
            {
                string[] dllFiles = Directory.GetFiles(subDir, "*.dll", SearchOption.TopDirectoryOnly);

                if (dllFiles.Length < 1)
                    continue;

                QMod mod;

                string jsonFile = Path.Combine(subDir, "mod.json");

                if (File.Exists(jsonFile))
                    mod = FromJsonFile(subDir);
                else
                    mod = FromDll(subDir, dllFiles);

                string folderName = new DirectoryInfo(subDir).Name;

                if (mod == null)
                {
                    Logger.Error($"Unable to set up mod in folder \"{folderName}\"");

                    continue;
                }
                else if (!mod.IsValid)
                {
                    unloadedMods.Add(mod);

                    continue;
                }

                if (!mod.Enable)
                {
                    Logger.Info($"Mod \"{mod.DisplayName}\" is disabled via config, skipping...");

                    continue;
                }

                SortResults result = sortedMods.Add(mod);

                switch (result)
                {
                    case SortResults.CircularLoadOrder:

                        break;
                    case SortResults.CircularDependency:

                        break;
                    case SortResults.DuplicateId:
                        // TODO Report error
                        break;
                }
            }

            List<QMod> modsToLoad = sortedMods.CreateFlatValueList();

            this.FailedToCreate = ReportErrors(sortedMods.GetErrors());
            CheckOldHarmony(modsToLoad);

            return modsToLoad;
        }

        private static int ReportErrors(List<string> erroredMods)
        {
            if (erroredMods.Count == 0)
                return 0;

            string display = "The following mods could not be loaded: ";
            int maxDisplay = Math.Min(5, erroredMods.Count);

            for (int i = 0; i < maxDisplay; i++)
            {
                display += $", {erroredMods[i]}";
            }

            if (erroredMods.Count > 5)
                display += $", and {erroredMods.Count - 5} others";

            display += ". Check the log for details.";

            Dialog.Show(display, Dialog.Button.SeeLog, Dialog.Button.Close, false);

            return erroredMods.Count;
        }

        private static void CheckOldHarmony(List<QMod> loadedMods)
        {
            var modsThatUseOldHarmony = new List<QMod>();

            foreach (QMod mod in loadedMods)
            {
                AssemblyName[] references = mod.LoadedAssembly.GetReferencedAssemblies();
                foreach (AssemblyName reference in references)
                {
                    if (reference.FullName == "0Harmony, Version=1.0.9.1, Culture=neutral, PublicKeyToken=null")
                    {
                        modsThatUseOldHarmony.Add(mod);
                        break;
                    }
                }
            }

            if (modsThatUseOldHarmony.Count > 0)
            {
                Logger.Warn($"Some mods are using an old version of harmony! This will NOT cause any problems, but it's not recommended:");
                foreach (IQMod mod in modsThatUseOldHarmony)
                {
                    Console.WriteLine($"- {mod.DisplayName} ({mod.Id})");
                }
            }
        }

        private static QMod FromDll(string subDirectory, string[] dllFilePaths)
        {
            foreach (string dllFile in dllFilePaths)
            {
                var assembly = Assembly.LoadFrom(dllFile);

                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    object[] coreInfos = type.GetCustomAttributes(typeof(QModCoreInfo), false);
                    if (coreInfos.Length == 1)
                    {
                        var mod = new QMod((QModCoreInfo)coreInfos[0], type, assembly, subDirectory);

                        if (mod.IsValid)
                            return mod;
                    }
                }
            }

            return null;
        }

        private static QMod FromJsonFile(string subDirectory)
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

                if (mod == null)
                    return null;

                bool success = mod.TryCompletingJsonLoading(subDirectory) && mod.IsValid;

                if (!success)
                    return null;

                return mod;
            }
            catch (Exception e)
            {
                Logger.Error($"\"mod.json\" deserialization failed for file \"{jsonFile}\"!");
                Logger.Exception(e);

                return null;
            }
        }

        private static QMod FakePlaceholder(string name)
        {
            return new QMod(name);
        }
    }
}
