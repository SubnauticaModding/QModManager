namespace QModManager.Patching
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Oculus.Newtonsoft.Json;
    using QModManager.API;
    using QModManager.API.ModLoading;
    using QModManager.DataStructures;
    using QModManager.Utility;

    [JsonObject(MemberSerialization.OptIn)]
    internal class QMod : ISortable<string>, IQMod, IQModSerialiable
    {
        internal static readonly Regex VersionRegex = new Regex(@"(((\d+)\.?)+)");
        internal static readonly PatchMethodFinder patchMethodFinder = new PatchMethodFinder();

        internal object instance = null;

        #region JSON & IQModSerialiable

        public QMod()
        {
            // Empty public constructor for JSON
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

        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool Enable { get; set; } = true;

        [JsonProperty(Required = Required.DisallowNull)]
        public string Game { get; set; } = $"{QModGame.Subnautica}";

        [JsonProperty(Required = Required.Always)]
        public string AssemblyName { get; set; }

        [JsonProperty(Required = Required.Default)]
        public string EntryMethod { get; set; }

        #endregion

        public QModGame SupportedGame { get; protected set; }

        public IEnumerable<RequiredQMod> RequiredMods { get; protected set; }

        public IEnumerable<string> ModsToLoadBefore => this.LoadBeforePreferences;

        public IEnumerable<string> ModsToLoadAfter => this.LoadAfterPreferences;

        public Assembly LoadedAssembly { get; set; }

        public Version ParsedVersion { get; set; }

        public bool IsLoaded
        {
            get
            {
                if (this.PatchMethods.Count == 0)
                    return false;

                foreach (QModPatchMethod patchingMethod in this.PatchMethods.Values)
                {
                    if (!patchingMethod.IsPatched)
                        return false;
                }

                return true;
            }
        }

        public bool HarmonyOutdated
        {
            get
            {
                if (this.LoadedAssembly != null)
                {
                    AssemblyName[] references = this.LoadedAssembly.GetReferencedAssemblies();
                    foreach (AssemblyName reference in references)
                    {
                        if (reference.FullName == "0Harmony, Version=1.0.9.1, Culture=neutral, PublicKeyToken=null")
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public Dictionary<PatchingOrder, QModPatchMethod> PatchMethods { get; } = new Dictionary<PatchingOrder, QModPatchMethod>();

        public IList<string> RequiredDependencies { get; } = new List<string>();

        public IList<string> LoadBeforePreferences { get; } = new List<string>();

        public IList<string> LoadAfterPreferences { get; } = new List<string>();

        internal ModStatus Status { get; set; }

        public virtual ModStatus ValidateManifest(string subDirectory)
        {
            if (string.IsNullOrEmpty(this.Id) ||
                string.IsNullOrEmpty(this.DisplayName) ||
                string.IsNullOrEmpty(this.Author))
                return this.Status = ModStatus.MissingCoreInfo;

            switch (this.Game)
            {
                case "BelowZero":
                    this.SupportedGame = QModGame.BelowZero;
                    break;
                case "Both":
                    this.SupportedGame = QModGame.Both;
                    break;
                case "Subnautica":
                    this.SupportedGame = QModGame.Subnautica;
                    break;
                default:
                    return this.Status = ModStatus.FailedIdentifyingGame;
            }

            try
            {
                if (System.Version.TryParse(this.Version, out Version version))
                    this.ParsedVersion = version;
            }
            catch (Exception vEx)
            {
                Logger.Error($"There was an error parsing version \"{this.Version}\" for mod \"{this.DisplayName}\"");
                Logger.Exception(vEx);

                return this.Status = ModStatus.InvalidCoreInfo;
            }

            string modAssemblyPath = Path.Combine(subDirectory, this.AssemblyName);

            if (string.IsNullOrEmpty(modAssemblyPath) || !File.Exists(modAssemblyPath))
            {
                Logger.Debug($"Did not find a DLL at {modAssemblyPath}");
                return this.Status = ModStatus.MissingAssemblyFile;
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
                    return this.Status = ModStatus.FailedLoadingAssemblyFile;
                }
            }

            ModStatus patchMethodResults = patchMethodFinder.LoadPatchMethods(this);

            if (patchMethodResults != ModStatus.Success)
                return this.Status = patchMethodResults;

            foreach (string item in this.Dependencies)
                this.RequiredDependencies.Add(item);

            foreach (string item in this.LoadBefore)
                this.LoadBeforePreferences.Add(item);

            foreach (string item in this.LoadAfter)
                this.LoadAfterPreferences.Add(item);

            if (this.VersionDependencies.Count > 0)
            {
                var versionedDependencies = new List<RequiredQMod>(this.VersionDependencies.Count);
                foreach (KeyValuePair<string, string> item in this.VersionDependencies)
                {
                    string cleanVersion = VersionRegex.Matches(item.Value)?[0]?.Value;

                    if (string.IsNullOrEmpty(cleanVersion))
                    {
                        versionedDependencies.Add(new RequiredQMod(item.Key));
                    }
                    else if (System.Version.TryParse(cleanVersion, out Version version))
                    {
                        versionedDependencies.Add(new RequiredQMod(item.Key, version));
                    }
                    else
                    {
                        versionedDependencies.Add(new RequiredQMod(item.Key));
                    }
                }
            }

            if (!this.Enable)
                return this.Status = ModStatus.CanceledByUser;

            return this.Status = ModStatus.Success;
        }

        public virtual ModLoadingResults TryLoading(PatchingOrder order, QModGame currentGame)
        {
            if (this.Status != ModStatus.Success)
                return ModLoadingResults.Failure;

            if ((this.SupportedGame & currentGame) == QModGame.None)
            {
                this.PatchMethods.Clear(); // Do not attempt any other patch methods
                return ModLoadingResults.CurrentGameNotSupported;
            }

            if (this.PatchMethods.Count == 0 || !this.PatchMethods.TryGetValue(order, out QModPatchMethod patchMethod))
                return ModLoadingResults.NoMethodToExecute;

            if (patchMethod.IsPatched)
                return ModLoadingResults.AlreadyLoaded;

            Logger.Debug($"Starting patch method for mod \"{this.Id}\" at {order}");


            if (patchMethod.TryInvoke())
            {
                Logger.Debug($"Completed patch method for mod \"{this.Id}\" at {order}");
                return ModLoadingResults.Success;
            }
            else
            {
                this.PatchMethods.Clear(); // Do not attempt any other patch methods
                return ModLoadingResults.Failure;
            }
        }
    }
}
