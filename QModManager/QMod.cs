using Oculus.Newtonsoft.Json;
using System;
using System.IO;
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
    }
}
