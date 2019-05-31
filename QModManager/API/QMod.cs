using Oculus.Newtonsoft.Json;
using QModManager.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace QModManager.API
{
    /// <summary>
    /// An enum which contains possible values for <see cref="QMod.Game"/>
    /// </summary>
    [Flags]
    public enum Game
    {
        /// <summary>
        /// No game was detected <para/>
        /// In theory, this should never be the case
        /// </summary>
        None = 0b00,
        /// <summary>
        /// Subnautica was detected
        /// </summary>
        Subnautica = 0b01,
        /// <summary>
        /// Below Zero was detected
        /// </summary>
        BelowZero = 0b10,
        /// <summary>
        /// Both games were detected <para/>
        /// In theory, this should never be the case
        /// </summary>
        Both = Subnautica | BelowZero,
    }

    /// <summary>
    /// An enum which can be used by modders to tell QModManager about the state of their mod
    /// </summary>
    public enum State
    {
        /// <summary>
        /// The mod loaded successfully
        /// </summary>
        OK,
        /// <summary>
        /// The mod encountered an error and was not loaded
        /// </summary>
        Error,
        /// <summary>
        /// The mod decided to cancel its load process
        /// </summary>
        Cancel,
    }

    /// <summary>
    /// An interface containing all of the properties of a mod
    /// </summary>
    public interface IQMod : IQModBase, IQModDependencies, IQModLoadOrder { }

    /// <summary>
    /// An interface containing properties related to a mod's base fields
    /// </summary>
    public interface IQModBase
    {
        /*
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
        */

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
        /// The version of the mod
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// The game of this mod
        /// </summary>
        Game Game { get; }

        /// <summary>
        /// Stage one of mod loading <para/>
        /// Do all your initialization here
        /// </summary>
        /// <returns>The state of the mod after pre-initialization</returns>
        State PreInit();

        /// <summary>
        /// Stage two of mod loading <para/>
        /// Do all your API calls and cross-mod stuff here
        /// </summary>
        /// <returns>The state of the mod after initialization</returns>
        State Init();

        /// <summary>
        /// Final stage of mod loading <para/>
        /// Finish up loading here
        /// </summary>
        /// <returns>The state of the mod after post-initialization</returns>
        State PostInit();
    }

    /// <summary>
    /// An interface containing properties related to a mod's dependencies
    /// </summary>
    public interface IQModDependencies
    {
        /// <summary>
        /// The dependencies of the mod <para/>
        /// If you also want to specify the version of required dependencies, see <see cref="VersionDependencies"/>
        /// </summary>
        IEnumerable<string> Dependencies { get; }

        /// <summary>
        /// The version dependencies of the mod <para/>
        /// </summary>
        IDictionary<string, string> VersionDependencies { get; }
    }

    /// <summary>
    /// An interface containing properties related to a mod's loading order
    /// </summary>
    public interface IQModLoadOrder
    {
        /// <summary>
        /// A list of mods before which this mod will load
        /// </summary>
        IEnumerable<string> LoadBefore { get; }

        /// <summary>
        /// A list of mods after which this mod will load
        /// </summary>
        IEnumerable<string> LoadAfter { get; }
    }

    internal class JsonQMod : IQMod
    {
        public string Id { get; set; }

        public string DisplayName { get; set; }

        public string Author { get; set; }

        public Version Version { get; set; }

        public Game Game { get; set; }

        public IEnumerable<string> LoadBefore { get; set; }

        public IEnumerable<string> LoadAfter { get; set; }

        public IEnumerable<string> Dependencies { get; set; }

        public IDictionary<string, string> VersionDependencies { get; set; }

        public bool Enable { get; set; }

        public string AssemblyName { get; set; }

        public string EntryMethod { get; set; }

        public Assembly LoadedAssembly { get; set; }

        public string ModAssemblyPath { get; set; }

        public bool Loaded { get; set; }

        public State PreInit() => State.OK;

        public State Init() => State.OK;

        public State PostInit() => State.OK;

        internal static JsonQMod FromJsonFile(string file)
        {
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                };

                string json = File.ReadAllText(file);
                Dictionary<string, dynamic> dict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);

                if (dict == null) return null;

                JsonQMod mod = new JsonQMod();

                if (dict.TryGetValue("Id", out dynamic id) && id is string)
                {
                    mod.Id = id;
                }
                else
                {
                    mod.Id = null;
                }

                if (dict.TryGetValue("DisplayName", out dynamic displayName) && displayName is string)
                {
                    mod.DisplayName = displayName;
                }
                else
                {
                    mod.DisplayName = null;
                }

                if (dict.TryGetValue("Author", out dynamic author) && author is string)
                {
                    mod.Author = author;
                }
                else
                {
                    mod.Author = null;
                }

                if (dict.TryGetValue("Version", out dynamic version) && version is string)
                {
                    try
                    {
                        mod.Version = new Version(version);
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"There was an error parsing version \"{version}\" for mod \"{mod.DisplayName}\"");
                        Logger.Exception(e);
                        mod.Version = null;
                    }
                }
                else
                {
                    mod.Version = null;
                }

                if (dict.TryGetValue("Dependencies", out dynamic dependencies) && dependencies is string[])
                {
                    mod.Dependencies = dependencies;
                }
                else
                {
                    mod.Dependencies = new string[0];
                }

                if (dict.TryGetValue("VersionDependencies", out dynamic versionDependencies) && versionDependencies is Dictionary<string, string>)
                {
                    mod.VersionDependencies = versionDependencies;
                }
                else
                {
                    mod.VersionDependencies = new Dictionary<string, string>();
                }

                if (dict.TryGetValue("LoadBefore", out dynamic loadBefore) && loadBefore is string[])
                {
                    mod.LoadBefore = loadBefore;
                }
                else
                {
                    mod.LoadBefore = new string[0];
                }

                if (dict.TryGetValue("LoadAfter", out dynamic loadAfter) && loadAfter is string[])
                {
                    mod.LoadAfter = loadAfter;
                }
                else
                {
                    mod.LoadAfter = new string[0];
                }

                if (dict.TryGetValue("Enable", out dynamic enable) && enable is bool)
                {
                    mod.Enable = enable;
                }
                else
                {
                    mod.Enable = true;
                }

                if (dict.TryGetValue("Game", out dynamic game) && game is string)
                {
                    if (game == "BelowZero") mod.Game = Game.BelowZero;
                    else if (game == "Both") mod.Game = Game.Both;
                    else mod.Game = Game.Subnautica;
                }
                else
                {
                    mod.Game = Game.Subnautica;
                }

                if (dict.TryGetValue("AssemblyName", out dynamic assemblyName) && assemblyName is string)
                {
                    mod.AssemblyName = assemblyName;
                }
                else
                {
                    mod.AssemblyName = null;
                }

                if (dict.TryGetValue("EntryMethod", out dynamic entryMethod) && entryMethod is string)
                {
                    mod.EntryMethod = entryMethod;
                }
                else
                {
                    mod.EntryMethod = null;
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

        internal static IQMod CreateFakeQMod(string name)
        {
            return new JsonQMod()
            {
                Id = Patcher.IDRegex.Replace(name, ""),
                DisplayName = name,
                Author = "None",
                Version = new Version(1, 0, 0),
                Game = Game.Subnautica,
                Dependencies = new string[] { },
                VersionDependencies = new Dictionary<string, string>(),
                LoadBefore = new string[] { },
                LoadAfter = new string[] { },
                Enable = false,
                AssemblyName = "None",
                EntryMethod = "None",
                Loaded = false,
            };
        }

        internal static bool QModValid(JsonQMod mod, string folderName)
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

            if (mod.Version == null)
            {
                Logger.Error($"Mod found in folder \"{folderName}\" has an invalid version! Defaulting to 1.0.0");
                mod.Version = new Version(1, 0, 0);
            }

            if (string.IsNullOrEmpty(mod.AssemblyName))
            {
                Logger.Error($"Mod found in folder \"{folderName}\" is missing an assembly name!");

                success = false;
            }
            else if (!mod.AssemblyName.EndsWith(".dll"))
            {
                Logger.Error($"Mod found in folder \"{folderName}\" has an invalid assembly name!");

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

            string[] loadBefore = mod.LoadBefore.ToArray();
            for (int i = 0; i < loadBefore.Length; i++)
            {
                string good = Patcher.IDRegex.Replace(loadBefore[i], "");
                if (loadBefore[i] != good)
                    loadBefore[i] = good;
            }
            mod.LoadBefore = loadBefore.AsEnumerable();

            string[] loadAfter = mod.LoadAfter.ToArray();
            for (int i = 0; i < loadAfter.Length; i++)
            {
                string good = Patcher.IDRegex.Replace(loadAfter[i], "");
                if (loadAfter[i] != good)
                    loadAfter[i] = good;
            }
            mod.LoadAfter = loadAfter.AsEnumerable();

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
}
