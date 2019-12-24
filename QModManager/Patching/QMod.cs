namespace QModManager.Patching
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Oculus.Newtonsoft.Json;
    using QModManager.API;
    using QModManager.API.ModLoading;
    using QModManager.DataStructures;

    [JsonObject(MemberSerialization.OptIn)]
    internal class QMod : ISortable<string>, IQMod, IQModSerialiable
    {
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

        public QModGame SupportedGame { get; internal set; }

        public IEnumerable<RequiredQMod> RequiredMods { get; internal set; }

        public IEnumerable<string> ModsToLoadBefore => this.LoadBeforePreferences;

        public IEnumerable<string> ModsToLoadAfter => this.LoadAfterPreferences;

        public Assembly LoadedAssembly { get; internal set; }

        public Version ParsedVersion { get; internal set; }

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
    }
}
