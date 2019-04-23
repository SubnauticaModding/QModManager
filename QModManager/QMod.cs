using Oculus.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using Logger = QModManager.Utility.Logger;

namespace QModManager
{
    public class QMod
    {
        public string Id = "";
        public string DisplayName = "";
        public string Author = "";
        public string Version = "";
        public string[] Dependencies = new string[] { };
        public Dictionary<string, string> VersionDependencies = new Dictionary<string, string>();
        public string[] LoadBefore = new string[] { };
        public string[] LoadAfter = new string[] { };
        public bool Enable = true;
        public string Game = "Subnautica";
        public string AssemblyName = "";
        public string EntryMethod = "";

        [JsonIgnore] public Assembly LoadedAssembly;
        [JsonIgnore] public Version ParsedVersion;

        [JsonIgnore] internal string ModAssemblyPath;
        [JsonIgnore] internal bool Loaded;
        [JsonIgnore] internal Patcher.Game ParsedGame;
        [JsonIgnore] internal Dictionary<QMod, List<MethodInfo>> MessageReceivers;

        internal static QMod FromJsonFile(string file)
        {
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                };

                string json = File.ReadAllText(file);
                QMod mod = JsonConvert.DeserializeObject<QMod>(json);

                if (mod == null) return null;

                if (mod.Game == "BelowZero") mod.ParsedGame = Patcher.Game.BelowZero;
                else if (mod.Game == "Both") mod.ParsedGame = Patcher.Game.Both;
                else mod.ParsedGame = Patcher.Game.Subnautica;

                try
                {
                    mod.ParsedVersion = new Version(mod.Version);
                }
                catch (Exception e)
                {
                    Logger.Error($"There was an error parsing version \"{mod.Version}\" for mod \"{mod.DisplayName}\"");
                    Debug.LogException(e);
                    mod.ParsedVersion = null;
                }

                return mod;
            }
            catch (Exception e)
            {
                Logger.Error($"\"mod.json\" deserialization failed for file \"{file}\"!");
                Debug.LogException(e);

                return null;
            }
        }
        internal static QMod CreateFakeQMod(string name)
        {
            return new QMod()
            {
                Id = Regex.Replace(name, Patcher.IDRegex, "", RegexOptions.IgnoreCase),
                DisplayName = name,
                Author = "None",
                Version = "None",
                Dependencies = new string[] { },
                VersionDependencies = new Dictionary<string, string>(),
                LoadBefore = new string[] { },
                LoadAfter = new string[] { },
                Enable = false,
                Game = "",
                AssemblyName = "None",
                EntryMethod = "None",
            };
        }

        internal static bool QModValid(QMod mod, string folderName)
        {
            bool success = true;

            if (mod == null)
            {
                Logger.Error($"Skipped a null mod found in folder \"{folderName}\"");

                return false;
            }

            if (string.IsNullOrEmpty(mod.DisplayName))
            {
                Logger.Error($"Mod found in folder \"{folderName}\" is missing a display name!");

                success = false;
            }

            if (string.IsNullOrEmpty(mod.Id))
            {
                Logger.Error($"Mod found in folder \"{folderName}\" is missing an ID!");

                success = false;
            }

            else if (mod.Id != Regex.Replace(mod.Id, Patcher.IDRegex, "", RegexOptions.IgnoreCase))
            {
                Logger.Warn($"Mod found in folder \"{folderName}\" has an invalid ID! All invalid characters have been removed. (This can cause issues!)");
                mod.Id = Regex.Replace(mod.Id, Patcher.IDRegex, "", RegexOptions.IgnoreCase);
            }

            if (string.IsNullOrEmpty(mod.Author))
            {
                Logger.Error($"Mod found in folder \"{folderName}\" is missing an author!");

                success = false;
            }

            if (string.IsNullOrEmpty(mod.Version))
            {
                Logger.Error($"Mod found in folder \"{folderName}\" is missing a version!");

                success = false;
            }

            if (mod.ParsedVersion == null)
            {
                Logger.Warn($"Mod found in folder \"{folderName}\" has an invalid version!");
            }

            if (string.IsNullOrEmpty(mod.AssemblyName))
            {
                Logger.Error($"Mod found in folder \"{folderName}\" is missing an assembly name!");

                success = false;
            }

            else if (!mod.AssemblyName.EndsWith(".dll"))
            {
                Logger.Error($"Mod found in folder \"{folderName}\" is has an invalid assembly name!");

                success = false;
            }

            if (string.IsNullOrEmpty(mod.EntryMethod))
            {
                Logger.Error($"Mod found in folder \"{folderName}\" is missing an entry point!");

                success = false;
            }

            else if (mod.EntryMethod?.Count(c => c == '.') < 2)
            {
                Logger.Error($"Mod found in folder \"{folderName}\" has an invalid entry point!");

                success = false;
            }

            for (int i = 0; i < mod.LoadAfter.Length; i++)
            {
                string good = Regex.Replace(mod.LoadAfter[i], Patcher.IDRegex, "", RegexOptions.IgnoreCase);
                if (mod.LoadAfter[i] != good)
                    mod.LoadAfter[i] = good;
            }

            for (int i = 0; i < mod.LoadBefore.Length; i++)
            {
                string good = Regex.Replace(mod.LoadBefore[i], Patcher.IDRegex, "", RegexOptions.IgnoreCase);
                if (mod.LoadBefore[i] != good)
                    mod.LoadBefore[i] = good;
            }

            Dictionary<string, string> versionDependenciesLoop = new Dictionary<string, string>(mod.VersionDependencies);
            foreach (KeyValuePair<string, string> kvp in versionDependenciesLoop)
            {
                string good = Regex.Replace(kvp.Key, Patcher.IDRegex, "", RegexOptions.IgnoreCase);
                if (kvp.Key != good)
                {
                    mod.VersionDependencies.Remove(kvp.Key);
                    mod.VersionDependencies.Add(good, kvp.Value);
                }
            }

            return success;
        }
    }
}
