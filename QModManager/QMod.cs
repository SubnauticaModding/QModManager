using Oculus.Newtonsoft.Json;
using System;
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
        public string[] LoadBefore = new string[] { };
        public string[] LoadAfter = new string[] { };
        public bool Enable = true;
        public string Game = "Subnautica";
        public string AssemblyName = "";
        public string EntryMethod = "";

        [JsonIgnore] internal Assembly LoadedAssembly;
        [JsonIgnore] internal string ModAssemblyPath;
        [JsonIgnore] internal bool Loaded;
        [JsonIgnore] internal Patcher.Game ParsedGame;

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
                Id = Regex.Replace(name, "[^0-9a-z_]", "", RegexOptions.IgnoreCase),
                DisplayName = name,
                Author = "None",
                Version = "None",
                Dependencies = new string[] { },
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
            if (mod == null)
            {
                Logger.Error($"Skipped a null mod found in folder \"{folderName}\"");

                return false;
            }

            if (string.IsNullOrEmpty(mod.DisplayName))
            {
                Logger.Error($"Mod found in folder \"{folderName}\" is missing a display name!");

                return false;
            }

            if (string.IsNullOrEmpty(mod.Id))
            {
                Logger.Error($"Mod found in folder \"{folderName}\" is missing an ID!");

                return false;
            }

            if (mod.Id != Regex.Replace(mod.Id, "[^0-9a-z_]", "", RegexOptions.IgnoreCase))
            {
                Logger.Warn($"Mod found in folder \"{folderName}\" has an invalid ID! All invalid characters have been removed. (This can cause issues!)");
                mod.Id = Regex.Replace(mod.Id, "[^0-9a-z_]", "", RegexOptions.IgnoreCase);
            }

            if (string.IsNullOrEmpty(mod.Author))
            {
                Logger.Error($"Mod found in folder \"{folderName}\" is missing an author!");

                return false;
            }

            if (string.IsNullOrEmpty(mod.Version))
            {
                Logger.Error($"Mod found in folder \"{folderName}\" is missing a version!");

                return false;
            }

            if (string.IsNullOrEmpty(mod.AssemblyName))
            {
                Logger.Error($"Mod found in folder \"{folderName}\" is missing an assembly name!");

                return false;
            }

            if (string.IsNullOrEmpty(mod.EntryMethod))
            {
                Logger.Error($"Mod found in folder \"{folderName}\" is missing an entry point!");

                return false;
            }

            if (mod.EntryMethod.Count(c => c == '.') < 2)
            {
                Logger.Error($"Mod found in folder \"{folderName}\" has a badly-formatted entry point!");

                return false;
            }

            return true;
        }
    }
}
