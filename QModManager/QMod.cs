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
        public class VersionDependency
        {
            public string Dependency;
            public string Operator;
            public Version Version;

            public string RawVersion;
        }

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
        [JsonIgnore] public List<VersionDependency> ParsedVersionDependencies = new List<VersionDependency>();

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

                try
                {
                    mod.ParsedVersion = new Version(mod.Version);
                }
                catch
                {
                    mod.ParsedVersion = null;
                }

                if (mod.VersionDependencies.Count > 0)
                {
                    foreach (KeyValuePair<string, string> dependency in mod.VersionDependencies)
                    {
                        string version = dependency.Value.Trim();
                        string vOperator = "";

                        Match match = Regex.Match(version, "^([<>]=?|!?=)");
                        if (match.Success)
                        {
                            vOperator = version.Substring(0, match.Value.Length);
                            version = version.Substring(match.Value.Length).Trim();
                        }

                        VersionDependency vd = new VersionDependency() { RawVersion = version, Operator = vOperator, Dependency = dependency.Key };

                        mod.ParsedVersionDependencies.Add(vd);
                    }
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
                Id = Regex.Replace(name, "[^0-9a-z_]", "", RegexOptions.IgnoreCase),
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

            else if (mod.Id != Regex.Replace(mod.Id, "[^0-9a-z_]", "", RegexOptions.IgnoreCase))
            {
                Logger.Warn($"Mod found in folder \"{folderName}\" has an invalid ID! All invalid characters have been removed. (This can cause issues!)");
                mod.Id = Regex.Replace(mod.Id, "[^0-9a-z_]", "", RegexOptions.IgnoreCase);
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

            if (mod.ParsedVersionDependencies.Count > 0)
            {
                foreach (VersionDependency dependency in mod.ParsedVersionDependencies)
                {
                    try
                    {
                        dependency.Version = new Version(dependency.RawVersion);
                    }
                    catch
                    {
                        dependency.Version = null;
                        if (!string.IsNullOrEmpty(dependency.Operator) && dependency.Operator != "=")
                            Logger.Error($"Mod in folder \"{folderName}\" has an invalid version dependency for \"{dependency.Dependency}\": \"{dependency.Operator}\" is not a valid operator for version \"{dependency.RawVersion}\"");

                        success = false;
                    }
                }
            }

            return success;
        }
    }
}
