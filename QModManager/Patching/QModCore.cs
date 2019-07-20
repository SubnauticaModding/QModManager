namespace QModManager.Patching
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using QModManager.API;
    using QModManager.API.ModLoading;

    internal class QModCore : QMod, IQMod
    {
        private readonly bool tooManyPatchMethods = false;        

        internal QModCore(string dllFile, string typeName)
        {
            // Now we can load the assembly into the current domain
            this.LoadedAssembly = Assembly.Load(dllFile);
            this.ParsedVersion = this.LoadedAssembly.GetName().Version;

            Type originatingType = this.LoadedAssembly.GetType(typeName);

            if (originatingType == null)
                throw new FatalPatchingException("QModCore somehow failed to find the originating type");

            QModCoreInfo modInfo = null;

            object[] coreInfos = originatingType.GetCustomAttributes(typeof(QModCoreInfo), false);

            if (coreInfos.Length == 1)            
                modInfo = (QModCoreInfo)coreInfos[0];            

            if (modInfo == null)
                throw new FatalPatchingException("QModCore somehow failed to find QModCoreInfo");

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
            foreach (QModPatchMethod qpatch in GetPatchMethods(originatingType))
            {
                if (this.PatchMethods.ContainsKey(qpatch.Order))
                {
                    tooManyPatchMethods = true;
                    this.PatchMethods.Clear(); // Too many methods specified for the current PatchingOrder
                    break;
                }

                this.PatchMethods.Add(qpatch.Order, qpatch);
            }
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

        private IEnumerable<QModPatchMethod> GetPatchMethods(Type originatingType)
        {
            MethodInfo[] methods = originatingType.GetMethods(BindingFlags.Public);
            foreach (MethodInfo method in methods)
            {
                object[] patchMethods = method.GetCustomAttributes(typeof(QModPatchAttributeBase), false);
                foreach (QModPatchAttributeBase attribute in patchMethods)
                {
                    if (method.GetParameters().Length == 0)
                        yield return new QModPatchMethod(method, this, attribute.PatchOrder);
                }
            }
        }
    }
}
