using Oculus.Newtonsoft.Json;
using QModManager.API;
using QModManager.Utility;
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
    /// <summary>
    /// A class containing information about a mod
    /// </summary>
    public class QMod : IQMod
    {
        /// <summary>
        /// The dummy <see cref="QMod"/> which is used to represent QModManager
        /// </summary>
        public static QMod QModManagerQMod { get; } = new QMod()
        {
            AssemblyName = "QModInstaller.dll",
            Author = "the QModManager dev team",
            Dependencies = new string[] { },
            DisplayName = "QModManager",
            Enable = true,
            EntryMethod = null,
            Game = "Both",
            Id = "QModManager",
            LoadAfter = new string[] { },
            LoadBefore = new string[] { },
            Loaded = true,
            LoadedAssembly = Assembly.GetExecutingAssembly(),
            //MessageReceivers = new Dictionary<IQMod, List<MethodInfo>>(),
            ModAssemblyPath = Assembly.GetExecutingAssembly().Location,
            ParsedGame = Patcher.Game.Both,
            ParsedVersion = Assembly.GetExecutingAssembly().GetName().Version,
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToStringParsed(),
            VersionDependencies = new Dictionary<string, string>(),
        };

        internal QMod() { }

        /// <summary>
        /// The ID of the mod <para/>
        /// Can only contain alphanumeric characters and underscores: (<see langword="a-z"/>, <see langword="A-Z"/>, <see langword="0-9"/>, <see langword="_"/>)
        /// </summary>
        public string Id { get; set; } = "";

        /// <summary>
        /// The display name of the mod
        /// </summary>
        public string DisplayName { get; set; } = "";

        /// <summary>
        /// The author of the mod
        /// </summary>
        public string Author { get; set; } = "";

        /// <summary>
        /// The version of the mod <para/>
        /// Should be have this form: <see langword="MAJOR"/>.<see langword="MINOR"/>.<see langword="BUILD"/>.<see langword="REVISION"/>
        /// </summary>
        public string Version { get; set; } = "";

        /// <summary>
        /// The dependencies of the mod <para/>
        /// If you also want to specify the version of required dependencies, see <see cref="VersionDependencies"/>
        /// </summary>
        public string[] Dependencies { get; set; } = new string[] { };

        /// <summary>
        /// The version dependencies of the mod <para/>
        /// </summary>
        public Dictionary<string, string> VersionDependencies { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// A list of mods before which this mod will load
        /// </summary>
        public string[] LoadBefore { get; set; } = new string[] { };

        /// <summary>
        /// A list of mods after which this mod will load
        /// </summary>
        public string[] LoadAfter { get; set; } = new string[] { };

        /// <summary>
        /// Whether or not this mod is enabled
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// The game of this mod <para/>
        /// Should be <see langword="Subnautica"/>, <see langword="BelowZero"/>, or <see langword="Both"/>
        /// </summary>
        public string Game { get; set; } = "Subnautica";

        /// <summary>
        /// The assembly name of the mod (including <see langword=".dll"/>)
        /// </summary>
        public string AssemblyName { get; set; } = "";

        /// <summary>
        /// The entry method of the mod <para/>
        /// Should have this form: <see langword="NAMESPACE"/>.<see langword="CLASS"/>.<see langword="METHOD"/>
        /// </summary>
        public string EntryMethod { get; set; } = "";

        /// <summary>
        /// The assembly of this mod <para/>
        /// Check if <see langword="null"/> before using
        /// </summary>
        [JsonIgnore] public Assembly LoadedAssembly { get; set; }

        /// <summary>
        /// The parsed <see cref="Version"/> of this mod
        /// </summary>
        [JsonIgnore] public Version ParsedVersion { get; set; }

        /// <summary>
        /// The parsed <see cref="Patcher.Game"/> of this mod
        /// </summary>
        [JsonIgnore] public Patcher.Game ParsedGame { get; set; }

        /// <summary>
        /// The dll path of this mod
        /// </summary>
        [JsonIgnore] public string ModAssemblyPath { get; set; }

        /// <summary>
        /// Whether or not this mod is loaded
        /// </summary>
        [JsonIgnore] public bool Loaded { get; set; }

        //// <summary>
        //// The <see cref="MessageReceiver"/>s and <see cref="GlobalMessageReceiver"/>s defined in this mod
        //// </summary>
        //[JsonIgnore] public Dictionary<IQMod, List<MethodInfo>> MessageReceivers { get; set; }

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
                    Logger.Exception(e);
                    mod.ParsedVersion = null;
                }

                return mod;
            }
            catch (Exception e)
            {
                Logger.Error($"\"mod.json\" deserialization failed for file \"{file}\"!");
                Logger.Exception(e);

                return null;
            }
        }

        internal static QMod CreateFakeQMod(string name)
        {
            return new QMod()
            {
                Id = Patcher.IDRegex.Replace(name, ""),
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
            else if (mod.Id != Patcher.IDRegex.Replace(mod.Id, ""))
            {
                Logger.Warn($"Mod found in folder \"{folderName}\" has an invalid ID! All invalid characters have been removed. (This can cause issues!)");
                mod.Id = Patcher.IDRegex.Replace(mod.Id, "");
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
                string good = Patcher.IDRegex.Replace(mod.LoadAfter[i], "");
                if (mod.LoadAfter[i] != good)
                    mod.LoadAfter[i] = good;
            }

            for (int i = 0; i < mod.LoadBefore.Length; i++)
            {
                string good = Patcher.IDRegex.Replace(mod.LoadBefore[i], "");
                if (mod.LoadBefore[i] != good)
                    mod.LoadBefore[i] = good;
            }

            Dictionary<string, string> versionDependenciesLoop = new Dictionary<string, string>(mod.VersionDependencies);
            foreach (KeyValuePair<string, string> kvp in versionDependenciesLoop)
            {
                string good = Patcher.IDRegex.Replace(kvp.Key, "");
                if (kvp.Key != good)
                {
                    mod.VersionDependencies.Remove(kvp.Key);
                    mod.VersionDependencies.Add(good, kvp.Value);
                }
            }

            return success;
        }
    }

    /// <summary>
    /// A read-only <see cref="QMod"/>
    /// </summary>
    public interface IQMod
    {
        /// <summary>
        /// The ID of the mod <para/>
        /// Can only contain alphanumeric characters and underscores: (<see langword="a-z"/>, <see langword="A-Z"/>, <see langword="0-9"/>, <see langword="_"/>)
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The display name of the mod
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// The author of the mod
        /// </summary>
        string Author { get; }

        /// <summary>
        /// The version of the mod <para/>
        /// Should be have this form: <see langword="MAJOR"/>.<see langword="MINOR"/>.<see langword="BUILD"/>.<see langword="REVISION"/>
        /// </summary>
        string Version { get; }

        /// <summary>
        /// The dependencies of the mod <para/>
        /// If you also want to specify the version of required dependencies, see <see cref="VersionDependencies"/>
        /// </summary>
        string[] Dependencies { get; }

        /// <summary>
        /// The version dependencies of the mod <para/>
        /// </summary>
        Dictionary<string, string> VersionDependencies { get; }

        /// <summary>
        /// A list of mods before which this mod will load
        /// </summary>
        string[] LoadBefore { get; }

        /// <summary>
        /// A list of mods after which this mod will load
        /// </summary>
        string[] LoadAfter { get; }

        /// <summary>
        /// Whether or not this mod is enabled
        /// </summary>
        bool Enable { get; }

        /// <summary>
        /// The game of this mod <para/>
        /// Should be <see langword="Subnautica"/>, <see langword="BelowZero"/>, or <see langword="Both"/>
        /// </summary>
        string Game { get; }

        /// <summary>
        /// The assembly name of the mod (including <see langword=".dll"/>)
        /// </summary>
        string AssemblyName { get; }

        /// <summary>
        /// The entry method of the mod <para/>
        /// Should have this form: <see langword="NAMESPACE"/>.<see langword="CLASS"/>.<see langword="METHOD"/>
        /// </summary>
        string EntryMethod { get; }

        /// <summary>
        /// The assembly of this mod <para/>
        /// Check if <see langword="null"/> before using
        /// </summary>
        Assembly LoadedAssembly { get; }

        /// <summary>
        /// The parsed <see cref="Version"/> of this mod
        /// </summary>
        Version ParsedVersion { get; }

        /// <summary>
        /// The parsed <see cref="Patcher.game"/> of this mod
        /// </summary>
        Patcher.Game ParsedGame { get; }

        /// <summary>
        /// The dll path of this mod
        /// </summary>
        string ModAssemblyPath { get; }

        /// <summary>
        /// Whether or not this mod is loaded
        /// </summary>
        bool Loaded { get; }

        //// <summary>
        //// The <see cref="MessageReceiver"/>s and <see cref="GlobalMessageReceiver"/>s defined in this mod
        //// </summary>
        //Dictionary<IQMod, List<MethodInfo>> MessageReceivers { get; }
    }
}
