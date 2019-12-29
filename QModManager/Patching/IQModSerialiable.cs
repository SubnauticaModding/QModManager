namespace QModManager.Patching
{
    using System.Collections.Generic;

    /// <summary>
    /// Enforces the requirements of the mod.json file for legacy mod loading.
    /// </summary>
    internal interface IQModSerialiable
    {
        string Id { get; set; }
        string DisplayName { get; set; }
        string Author { get; set; }
        string Version { get; set; }
        string[] Dependencies { get; set; }
        Dictionary<string, string> VersionDependencies { get; set; }
        string[] LoadBefore { get; set; }
        string[] LoadAfter { get; set; }
        bool Enable { get; set; }
        string Game { get; set; }
        string AssemblyName { get; set; }
        string EntryMethod { get; set; }
    }
}
