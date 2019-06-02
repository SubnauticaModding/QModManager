namespace QModManager.API.ModLoading.Internal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Oculus.Newtonsoft.Json;
    using QModManager.DataStructures;
    using QModManager.Utility;

    [JsonObject(MemberSerialization.OptIn)]
    internal class QMod : IQMod, IQModSerialiable, IQModLoadable, ISortable<string>
    {
        /// <summary>
        /// The dummy <see cref="QMod"/> which is used to represent QModManager
        /// </summary>
        internal static QMod QModManager { get; } = new QMod
        {
            Id = "QModManager",
            DisplayName = "QModManager",
            Author = "QModManager Dev Team",
            LoadedAssembly = Assembly.GetExecutingAssembly(),
            ParsedGame = API.Game.Both,
            IsValid = true,
            Enable = true
        };

        private bool _isValidChecked = false;
        private bool _validationResults = false;
        private Assembly _loadedAssembly;
        private Version _modVersion;
        private Game _moddedGame = API.Game.None;
        private Dictionary<string, Version> _requiredMods;

        public QMod()
        {
        }

        internal QMod(QModCoreInfo modInfo, Type originatingType, Assembly loadedAssembly, string subDirectory)
        {
            this.ModDirectory = subDirectory;

            // Basic mod info
            this.Id = modInfo.Id;
            this.DisplayName = modInfo.DisplayName;
            this.Author = modInfo.Author;
            this.ParsedGame = modInfo.SupportedGame;

            // Dependencies
            this.RequiredMods = GetDependencies(originatingType);

            // Load order
            this.LoadBefore = GetOrderedMods<QModLoadBefore>(originatingType);
            this.LoadAfter = GetOrderedMods<QModLoadAfter>(originatingType);

            // Patch methods
            this.PatchMethods = GetPatchMethods(originatingType);

            // Assembly info
            this.LoadedAssembly = loadedAssembly;
        }

        internal QMod(string name)
        {
            this.Id = Patcher.IDRegex.Replace(name, "");
            this.DisplayName = name;
            this.Author = "Unknown";
            this.ParsedGame = API.Game.None;
            this.Enable = false;
            this.IsValid = false;
        }

        [JsonProperty(Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string DisplayName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Author { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Version { get; set; }

        [JsonProperty(Required = Required.Default)]
        public string[] Dependencies { get; set; } = new string[0];

        [JsonProperty(Required = Required.Default)]
        public Dictionary<string, string> VersionDependencies { get; set; } = new Dictionary<string, string>();

        [JsonProperty(Required = Required.Default)]
        public string[] LoadBefore { get; set; } = new string[0];

        [JsonProperty(Required = Required.Default)]
        public string[] LoadAfter { get; set; } = new string[0];

        [JsonProperty(Required = Required.DisallowNull)]
        public string Game { get; set; } = $"{API.Game.Subnautica}";

        [JsonProperty(Required = Required.Always)]
        public string AssemblyName { get; set; }

        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool Enable { get; set; } = true;

        [JsonProperty(Required = Required.Always)]
        public string EntryMethod { get; set; }

        public string ModDirectory { get; private set; }

        public Dictionary<string, Version> RequiredMods
        {
            get => _requiredMods;
            private set
            {
                _requiredMods = value;
                this.Dependencies = new string[_requiredMods.Count];

                int i = 0;
                foreach (KeyValuePair<string, Version> dependency in _requiredMods)
                {
                    this.Dependencies[i] = dependency.Key;

                    if (!IsDefaultVersion(dependency.Value))
                        this.VersionDependencies.Add(dependency.Key, dependency.Value.ToStringParsed());

                    i++;
                }
            }
        }

        public Game ParsedGame
        {
            get => _moddedGame;
            private set
            {
                _moddedGame = value;
                this.Game = $"{_moddedGame}";
            }
        }

        public bool IsLoaded
        {
            get
            {
                if (this.PatchMethods.Count == 0)
                    return false;

                foreach (PatchMethod patchingMethod in this.PatchMethods.Values)
                {
                    if (!patchingMethod.IsPatched)
                        return false;
                }

                return true;
            }
        }

        public bool IsValid
        {
            get
            {
                if (_isValidChecked)
                    return _validationResults;

                _isValidChecked = true;
                return _validationResults = Validate();
            }
            private set
            {
                _isValidChecked = true;
                _validationResults = value;
            }
        }

        public Assembly LoadedAssembly
        {
            get => _loadedAssembly;
            set
            {
                _loadedAssembly = value;

                AssemblyName assemblyName = _loadedAssembly.GetName();
                this.AssemblyName = assemblyName.Name;
                this.ParsedVersion = assemblyName.Version;
            }
        }

        public Version ParsedVersion
        {
            get => _modVersion;
            private set
            {
                _modVersion = value;
                this.Version = _modVersion.ToStringParsed();
            }
        }

        public Dictionary<PatchingOrder, PatchMethod> PatchMethods { get; } = new Dictionary<PatchingOrder, PatchMethod>();

        public ICollection<string> DependencyCollection { get; private set; }
        public ICollection<string> LoadBeforeCollection { get; private set; }
        public ICollection<string> LoadAfterCollection { get; private set; }

        public ModLoadingResults TryLoading(PatchingOrder order)
        {
            if (!this.PatchMethods.TryGetValue(order, out PatchMethod patchMethod))
                return ModLoadingResults.NoMethodToExecute;

            if (patchMethod.IsPatched)
                return ModLoadingResults.AlreadyLoaded;

            try
            {
                patchMethod.Method.Invoke(this.LoadedAssembly, new object[] { });
            }
            catch (ArgumentNullException e)
            {
                Logger.Error($"Could not parse entry method \"{this.AssemblyName}\" for mod \"{this.Id}\"");
                Logger.Exception(e);

                return ModLoadingResults.Failure;
            }
            catch (TargetInvocationException e)
            {
                Logger.Error($"Invoking the specified entry method \"{patchMethod.Method.Name}\" failed for mod \"{this.Id}\"");
                Logger.Exception(e);
                return ModLoadingResults.Failure;
            }
            catch (Exception e)
            {
                Logger.Error($"An unexpected error occurred whilst trying to load mod \"{this.Id}\"");
                Logger.Exception(e);
                return ModLoadingResults.Failure;
            }

            if (QModAPI.ErroredMods.Contains(this?.LoadedAssembly))
            {
                Logger.Error($"Mod \"{this.Id}\" could not be loaded.");
                QModAPI.ErroredMods.Remove(this?.LoadedAssembly);
                return ModLoadingResults.Failure;
            }

            Logger.Info($"Loaded mod \"{this.Id}\"");
            patchMethod.IsPatched = true;

            return ModLoadingResults.Success;
        }

        public bool TryCompletingJsonLoading(string subDirectory)
        {
            switch (this.Game)
            {
                case "BelowZero":
                    _moddedGame = API.Game.BelowZero;
                    break;
                case "Both":
                    _moddedGame = API.Game.Both;
                    break;
                case "Subnautica":
                default:
                    _moddedGame = API.Game.Subnautica;
                    break;
            }

            try
            {
                this.ParsedVersion = new Version(this.Version);
            }
            catch (Exception vEx)
            {
                Logger.Error($"There was an error parsing version \"{this.Version}\" for mod \"{this.DisplayName}\"");
                Logger.Exception(vEx);

                this.IsValid = false;

                return false;
            }

            string modAssemblyPath = Path.Combine(subDirectory, this.AssemblyName);

            if (string.IsNullOrEmpty(modAssemblyPath) || !File.Exists(modAssemblyPath))
            {
                Logger.Error($"No matching dll found at \"{modAssemblyPath}\" for mod \"{this.DisplayName}\"");
                this.IsValid = false;

                return false;
            }
            else
            {
                try
                {
                    this.LoadedAssembly = Assembly.LoadFrom(modAssemblyPath);
                }
                catch (Exception aEx)
                {
                    Logger.Error($"Failed loading the dll found at \"{modAssemblyPath}\" for mod \"{this.DisplayName}\"");
                    Logger.Exception(aEx);
                    return false;
                }
            }

            MethodInfo patchMethod = GetPatchMethod(this.EntryMethod, this.LoadedAssembly);

            if (patchMethod != null)
                this.PatchMethods.Add(PatchingOrder.NormalInitialize, new PatchMethod(patchMethod));

            if (this.PatchMethods.Count == 0)
                return false;

            this.DependencyCollection = new HashSet<string>(this.Dependencies);
            this.LoadBeforeCollection = new HashSet<string>(this.LoadBefore);
            this.LoadAfterCollection = new HashSet<string>(this.LoadAfter);

            return true;
        }

        private bool Validate()
        {
            throw new NotImplementedException();
        }

        private static Dictionary<string, Version> GetDependencies(Type originatingType)
        {
            var dependencies = (QModDependency[])originatingType.GetCustomAttributes(typeof(QModDependency), false);
            var dictionary = new Dictionary<string, Version>();
            foreach (QModDependency dependency in dependencies)
            {
                dictionary.Add(dependency.RequiredMod, dependency.MinimumVersion);
            }

            return dictionary;
        }

        private static string[] GetOrderedMods<T>(Type originatingType) where T : IModOrder
        {
            object[] others = originatingType.GetCustomAttributes(typeof(T), false);

            int length = others.Length;
            string[] array = new string[length];

            for (int i = 0; i < length; i++)
                array[i] = (others[i] as IModOrder).OtherMod;

            return array;
        }

        private static Dictionary<PatchingOrder, PatchMethod> GetPatchMethods(Type originatingType)
        {
            var dictionary = new Dictionary<PatchingOrder, PatchMethod>(3);

            MethodInfo[] methods = originatingType.GetMethods(BindingFlags.Public);
            foreach (MethodInfo method in methods)
            {
                var patchMethods = (QModPatchMethod[])method.GetCustomAttributes(typeof(QModPatchMethod), false);
                foreach (QModPatchMethod patchmethod in patchMethods)
                {
                    dictionary.Add(patchmethod.PatchOrder, new PatchMethod(method));
                }
            }

            return dictionary;
        }

        private static bool IsDefaultVersion(Version version)
        {
            return version.Major == 0 && version.Minor == 0 && version.Revision == 0 && version.Build == 0;
        }

        private static MethodInfo GetPatchMethod(string methodPath, Assembly assembly)
        {
            string[] entryMethodSig = methodPath.Split('.');
            string entryType = string.Join(".", entryMethodSig.Take(entryMethodSig.Length - 1).ToArray());
            string entryMethod = entryMethodSig[entryMethodSig.Length - 1];

            return assembly.GetType(entryType).GetMethod(entryMethod);
        }
    }
}
