﻿namespace QModManager.Patching
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using QModManager.API;
    using QModManager.API.ModLoading;
    using QModManager.DataStructures;
    using QModManager.Utility;

    internal abstract class QMod : ISortable<string>
    {
        internal static readonly Regex VersionRegex = new Regex(@"(((\d+)\.?)+)");

        public virtual string Id { get; set; }

        public virtual string DisplayName { get; set; }

        public virtual string Author { get; set; }

        public virtual QModGame SupportedGame { get; protected set; }

        public IEnumerable<RequiredQMod> RequiredMods { get; protected set; }

        public IEnumerable<string> ModsToLoadBefore => this.LoadBeforeCollection;

        public IEnumerable<string> ModsToLoadAfter => this.LoadAfterCollection;

        public Assembly LoadedAssembly { get; set; }

        public virtual string AssemblyName { get; set; }

        public Version ParsedVersion { get; set; }

        public virtual bool Enable { get; set; } = true;

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

        public ICollection<string> DependencyCollection { get; } = new List<string>();

        public ICollection<string> LoadBeforeCollection { get; } = new List<string>();

        public ICollection<string> LoadAfterCollection { get; } = new List<string>();

        public ModStatus IsValidForLoading(string subDirectory)
        {
            if (string.IsNullOrEmpty(this.Id) ||
                string.IsNullOrEmpty(this.DisplayName) ||
                string.IsNullOrEmpty(this.Author))
                return ModStatus.MissingCoreInfo;

            return Validate(subDirectory);
        }

        protected abstract ModStatus Validate(string subDirectory);

        public virtual ModLoadingResults TryLoading(PatchingOrder order, QModGame currentGame)
        {
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
            PatchResults result = patchMethod.TryInvoke();
            switch (result)
            {
                case PatchResults.OK:
                    Logger.Debug($"Completed patch method for mod \"{this.Id}\" at {order}");
                    return ModLoadingResults.Success;

                case PatchResults.Error:
                    this.PatchMethods.Clear(); // Do not attempt any other patch methods
                    return ModLoadingResults.Failure;

                case PatchResults.ModderCanceled:
                    this.PatchMethods.Clear(); // Do not attempt any other patch methods
                    return ModLoadingResults.CancledByModAuthor;
            }

            return ModLoadingResults.Failure;
        }

        protected static bool IsDefaultVersion(Version version)
        {
            return version.Major == 0 && version.Minor == 0 && version.Revision == 0 && version.Build == 0;
        }

        protected static bool MinimumVersionMet(Version currentVersion, Version minimumVersion)
        {
            return currentVersion >= minimumVersion;
        }
    }
}
