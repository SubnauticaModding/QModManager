namespace QModManager.API.ModLoading.Internal
{
    using Advanced;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal interface IQMod
    {
        string Id { get; }
        string DisplayName { get; }
        string Author { get; }
        Game Game { get; }
        Dictionary<MetaPatchOrder, MethodInfo> PatchMethods { get; }
        Dictionary<string, Version> Dependencies { get; }
        IEnumerable<string> LoadBefore { get; }
        IEnumerable<string> LoadAfter { get; }
        Assembly LoadedAssembly { get; }
        string AssemblyName { get; }
        Version Version { get; }

        bool Enable { get; set; }
        bool Loaded { get; set; }

        bool Validate(string folderName);
    }
}