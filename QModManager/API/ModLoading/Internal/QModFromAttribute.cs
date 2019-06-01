namespace QModManager.API.ModLoading.Internal
{
    using Advanced;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Utility;

    internal class QModFromAttribute : IQMod
    {
        internal QModFromAttribute(QModCoreInfo modInfo, Type originatingType, Assembly loadedAssembly)
        {
            // Basic mod info
            this.Id = modInfo.Id;
            this.DisplayName = modInfo.DisplayName;
            this.Author = modInfo.Author;
            this.Game = modInfo.SupportedGame;

            // Dependencies
            this.Dependencies = GetDependencies(originatingType);

            // Load order
            this.LoadBefore = GetOrderedMods<QModLoadBefore>(originatingType);
            this.LoadAfter = GetOrderedMods<QModLoadAfter>(originatingType);

            // Patch methods
            this.PatchMethods = GetPatchMethods(originatingType);

            // Assembly info
            AssemblyName assemblyName = loadedAssembly.GetName();
            this.AssemblyName = assemblyName.Name;
            this.Version = assemblyName.Version;
            this.LoadedAssembly = loadedAssembly;
        }

        #region IQMod

        public string Id { get; private set; }

        public string DisplayName { get; }

        public string Author { get; }

        public Game Game { get; }

        public Dictionary<MetaPatchOrder, MethodInfo> PatchMethods { get; }

        public Dictionary<string, Version> Dependencies { get; }

        public IEnumerable<string> LoadBefore { get; }

        public IEnumerable<string> LoadAfter { get; }

        public Assembly LoadedAssembly { get; }

        public string AssemblyName { get; }

        public Version Version { get; }

        public bool Enable { get; set; } = true; // TODO

        public bool Loaded { get; set; }

        public bool Validate(string folderName)
        {
            bool success = true;
            if (string.IsNullOrEmpty(this.Id))
            {
                Logger.Error($"Mod found in folder \"{folderName}\" is missing an ID!");
                success = false;
            }
            else
            {
                string cleanId = Patcher.IDRegex.Replace(this.Id, string.Empty);
                if (this.Id != cleanId)
                {
                    Logger.Warn($"Mod found in folder \"{folderName}\" has an invalid ID! All invalid characters have been removed. (This can cause issues!)");
                    this.Id = cleanId;
                }
            }

            if (string.IsNullOrEmpty(this.DisplayName))
            {
                Logger.Error($"Mod found in folder \"{folderName}\" is missing a display name!");
                success = false;
            }

            if (string.IsNullOrEmpty(this.Author))
            {
                Logger.Error($"Mod found in folder \"{folderName}\" is missing an author!");
                success = false;
            }

            if (this.Version == null)
            {
                Logger.Warn($"Mod found in folder \"{folderName}\" has an invalid version!");
            }

            if (string.IsNullOrEmpty(this.AssemblyName))
            {
                Logger.Error($"Mod found in folder \"{folderName}\" is missing an assembly name!");
                success = false;
            }

            if (this.PatchMethods.Count == 0)
            {
                Logger.Error($"Mod found in folder \"{folderName}\" is missing a patch method!");
                success = false;
            }

            return success;
        }

        #endregion

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

        private static Dictionary<MetaPatchOrder, MethodInfo> GetPatchMethods(Type originatingType)
        {
            var dictionary = new Dictionary<MetaPatchOrder, MethodInfo>(3);

            MethodInfo[] methods = originatingType.GetMethods(BindingFlags.Public);
            foreach (MethodInfo method in methods)
            {
                var patchMethods = (QModPatchMethod[])method.GetCustomAttributes(typeof(QModPatchMethod), false);
                foreach (QModPatchMethod patchmethod in patchMethods)
                {
                    dictionary.Add(patchmethod.PatchOrder, method);
                }
            }

            return dictionary;
        }
    }
}
