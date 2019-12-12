namespace QModManager.Patching
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Oculus.Newtonsoft.Json;
    using QModManager.API;
    using QModManager.API.ModLoading;
    using QModManager.Utility;

    [JsonObject(MemberSerialization.OptIn)]
    internal class QModLegacy : QMod, IQMod, IQModSerialiable
    {
        public QModLegacy()
        {
            // Empty public constructor for JSON
        }

        [JsonProperty(Required = Required.Always)]
        public override string Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public override string DisplayName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public override string Author { get; set; }

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
        public string Game { get; set; } = $"{QModGame.Subnautica}";

        [JsonProperty(Required = Required.Always)]
        public override string AssemblyName { get; set; }

        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public override bool Enable { get; set; } = true;

        [JsonProperty(Required = Required.Always)]
        public string EntryMethod { get; set; }

        protected override ModStatus Validate(string subDirectory)
        {
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
                    return ModStatus.FailedIdentifyingGame;
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

                return ModStatus.InvalidCoreInfo;
            }

            string modAssemblyPath = Path.Combine(subDirectory, this.AssemblyName);

            if (string.IsNullOrEmpty(modAssemblyPath) || !File.Exists(modAssemblyPath))
            {
                Logger.Debug($"Did not find a DLL at {modAssemblyPath}");
                return ModStatus.MissingAssemblyFile;
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
                    return ModStatus.FailedLoadingAssemblyFile;
                }
            }

            MethodInfo patchMethod = GetPatchMethod(this.EntryMethod, this.LoadedAssembly);

            if (patchMethod != null && patchMethod.GetParameters().Length == 0)
                this.PatchMethods.Add(PatchingOrder.NormalInitialize, new QModPatchMethod(patchMethod, this, PatchingOrder.NormalInitialize));

            if (this.PatchMethods.Count == 0)
                return ModStatus.MissingPatchMethod;

            foreach (string item in this.Dependencies)
                this.DependencyCollection.Add(item);

            foreach (string item in this.LoadBefore)
                this.LoadBeforeCollection.Add(item);

            foreach (string item in this.LoadAfter)
                this.LoadAfterCollection.Add(item);

            var versionedDependencies = new List<RequiredQMod>(this.VersionDependencies.Count);
            foreach (KeyValuePair<string, string> item in this.VersionDependencies)
            {
                string cleanVersion = QMod.VersionRegex.Matches(item.Value)?[0]?.Value;

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
            
            return ModStatus.Success;
        }

        private MethodInfo GetPatchMethod(string methodPath, Assembly assembly)
        {
            string[] entryMethodSig = methodPath.Split('.');
            string entryType = string.Join(".", entryMethodSig.Take(entryMethodSig.Length - 1).ToArray());
            string entryMethod = entryMethodSig[entryMethodSig.Length - 1];

            return assembly.GetType(entryType).GetMethod(entryMethod);
        }
    }
}
