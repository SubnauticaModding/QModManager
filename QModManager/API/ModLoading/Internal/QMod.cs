namespace QModManager.API.ModLoading.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal class QMod
    {
        private readonly QModCoreInfo _modInfo;
        private readonly QModLoadBefore[] _loadBefores;
        private readonly QModLoadAfter[] _loadAfters;

        internal QMod(QModCoreInfo modInfo, Type originatingType, Assembly loadedAssembly)
        {
            _modInfo = modInfo;


            var dependencies = (QModDependency[])originatingType.GetCustomAttributes(typeof(QModDependency), false);
            foreach (QModDependency dependency in dependencies)
            {
                this.Dependencies.Add(dependency.RequiredMod, dependency.MinimumVersion);
            }

            _loadBefores = (QModLoadBefore[])originatingType.GetCustomAttributes(typeof(QModLoadBefore), false);
            _loadAfters = (QModLoadAfter[])originatingType.GetCustomAttributes(typeof(QModLoadAfter), false);

            this.LoadedAssembly = loadedAssembly;
        }

        internal string Id => _modInfo.Id;
        internal string DisplayName => _modInfo.DisplayName;
        internal string Author => _modInfo.Author;
        internal Patcher.Game Game => _modInfo.Game;
        internal string EntryMethod => _modInfo.PatchMethod;

        internal Dictionary<string, Version> Dependencies { get; } = new Dictionary<string, Version>();

        internal IEnumerable<string> LoadBefore
        {
            get
            {
                foreach (QModLoadBefore loadBefore in _loadBefores)
                {
                    yield return loadBefore.OtherMod;
                }
            }
        }

        internal IEnumerable<string> LoadAfter
        {
            get
            {
                foreach (QModLoadAfter loadAfter in _loadAfters)
                {
                    yield return loadAfter.OtherMod;
                }
            }
        }

        internal string AssemblyName => this.LoadedAssembly.GetName().Name;
        
        internal Assembly LoadedAssembly { get; }

        // TODO
        internal bool Enable { get; } = true;
        internal bool Loaded { get; set; }
        internal List<string> Errors { get; } = new List<string>();
        internal bool HasError => this.Errors.Count > 0;
    }
}
