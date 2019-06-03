namespace QModManager.API.ModLoading.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Enforces all the elements required to load a QMod.
    /// </summary>
    internal interface IQModLoadable
    {
        string Id { get; }
        string ModDirectory { get; }
        bool IsLoaded { get; }
        bool IsValid { get; }
        Assembly LoadedAssembly { get; }
        string AssemblyName { get; }
        Version ParsedVersion { get; }
        Dictionary<PatchingOrder, PatchMethod> PatchMethods { get; }
        ModLoadingResults TryLoading(PatchingOrder order, Game currentGame);
    }
}
