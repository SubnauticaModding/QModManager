namespace QModManager.Patching
{
    using Newtonsoft.Json;
    using QModManager.API;
    using QModManager.DataStructures;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    [JsonObject(MemberSerialization.OptIn)]
    internal class QMod : ISortable<string>, IQMod, IQModSerialiable
    {
        internal object instance = null;

        #region JSON & IQModSerialiable

        public QMod()
        {
            // Empty public constructor for JSON
        }

        [JsonProperty]
        public string Id { get; set; }

        [JsonProperty]
        public string DisplayName { get; set; }

        [JsonProperty]
        public string Author { get; set; }

        [JsonProperty]
        public string Version { get; set; }

        [JsonProperty]
        public string[] Dependencies { get; set; } = new string[0];

        [JsonProperty]
        public Dictionary<string, string> VersionDependencies { get; set; } = new Dictionary<string, string>();

        [JsonProperty]
        public string[] LoadBefore { get; set; } = new string[0];

        [JsonProperty]
        public string[] LoadAfter { get; set; } = new string[0];

        [JsonProperty]
        public bool Enable { get; set; } = true;

        [JsonProperty]
        public string Game { get; set; } = $"{QModGame.Subnautica}";

        [JsonProperty]
        public string AssemblyName { get; set; }

        [JsonProperty]
        public string EntryMethod { get; set; }

        public bool HasDependencies => Dependencies.Length > 0 || VersionDependencies.Count > 0;

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
                if (LoadedAssembly != null)
                {
                    AssemblyName[] references = this.LoadedAssembly.GetReferencedAssemblies();
                    return references.Any(reference => reference.FullName == "0Harmony, Version=1.0.9.1, Culture=neutral, PublicKeyToken=null" || (reference.Name.StartsWith("0Harmony") && reference.Version < new Version(2, 0)));
                }

                return false;
            }
        }

        public Dictionary<PatchingOrder, QModPatchMethod> PatchMethods { get; } = new Dictionary<PatchingOrder, QModPatchMethod>();

        public IList<string> RequiredDependencies { get; } = new List<string>();

        public IList<string> LoadBeforePreferences { get; } = new List<string>();

        public IList<string> LoadAfterPreferences { get; } = new List<string>();

        internal ModStatus Status { get; set; }

        internal string SubDirectory { get; set; }
    }
}
