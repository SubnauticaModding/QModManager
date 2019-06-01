namespace QModManager.API.ModLoading.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal class QModCore : QMod, IQMod
    {
        private readonly bool tooManyPatchMethods = false;

        internal QModCore(QModCoreInfo modInfo, Type originatingType, Assembly loadedAssembly)
        {
            // Basic mod info
            this.Id = modInfo.Id;
            this.DisplayName = modInfo.DisplayName;
            this.Author = modInfo.Author;
            this.SupportedGame = modInfo.SupportedGame;

            // Dependencies
            this.RequiredMods = GetDependencies(originatingType);

            // Load Before
            foreach (string item in GetOrderedMods<QModLoadBefore>(originatingType))
                this.LoadBeforeCollection.Add(item);

            // Load After
            foreach (string item in GetOrderedMods<QModLoadAfter>(originatingType))
                this.LoadAfterCollection.Add(item);

            // Patch methods
            foreach (QPatchMethod qpatch in GetPatchMethods(originatingType))
            {
                if (this.PatchMethods.ContainsKey(qpatch.Order))
                {
                    tooManyPatchMethods = true;
                    this.PatchMethods.Clear(); // Too many methods specified for the current PatchingOrder
                    break;
                }

                this.PatchMethods.Add(qpatch.Order, qpatch);
            }            

            // Assembly info
            this.LoadedAssembly = loadedAssembly;
            this.ParsedVersion = loadedAssembly.GetName().Version;
        }

        protected override ModStatus Validate(string subDirectory)
        {
            if (this.SupportedGame == QModGame.None ||
                this.ParsedVersion == null)
                return ModStatus.InvalidCoreInfo;

            if (this.LoadedAssembly == null)
                return ModStatus.FailedLoadingAssemblyFile;

            if (tooManyPatchMethods)
                return ModStatus.TooManyPatchMethods;

            if (this.PatchMethods.Count == 0)
                return ModStatus.MissingPatchMethod;

            return ModStatus.Success;
        }

        private List<RequiredQMod> GetDependencies(Type originatingType)
        {
            var dependencies = (QModDependency[])originatingType.GetCustomAttributes(typeof(QModDependency), false);
            var list = new List<RequiredQMod>(dependencies.Length);

            foreach (QModDependency dependency in dependencies)
            {
                if (IsDefaultVersion(dependency.MinimumVersion))
                    list.Add(new RequiredQMod(dependency.RequiredMod));
                else
                    list.Add(new RequiredQMod(dependency.RequiredMod, dependency.MinimumVersion));
            }

            return list;
        }

        private IEnumerable<string> GetOrderedMods<T>(Type originatingType)
            where T : Attribute, IModOrder
        {
            object[] others = originatingType.GetCustomAttributes(typeof(T), false);

            foreach (IModOrder entry in others)
            {
                yield return entry.OtherMod;
            }
        }

        private IEnumerable<QPatchMethod> GetPatchMethods(Type originatingType)
        {
            MethodInfo[] methods = originatingType.GetMethods(BindingFlags.Public);
            foreach (MethodInfo method in methods)
            {
                object[] patchMethods = method.GetCustomAttributes(typeof(QModPatchAttributeBase), false);
                foreach (QModPatchAttributeBase attribute in patchMethods)
                {
                    yield return new QPatchMethod(method, this, attribute.PatchOrder);
                }
            }
        }
    }
}
