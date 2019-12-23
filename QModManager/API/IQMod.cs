namespace QModManager.API
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// All public data about a QMod.
    /// </summary>
    public interface IQMod
    {
        /// <summary>
        /// The ID of the mod <para/>
        /// Can only contain alphanumeric characters and underscores: (<see langword="a-z"/>, <see langword="A-Z"/>, <see langword="0-9"/>, <see langword="_"/>)
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The display name of the mod
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// The author of the mod
        /// </summary>
        string Author { get; }

        /// <summary>
        /// The game this mod was developed for.
        /// </summary>
        QModGame SupportedGame { get; }

        /// <summary>
        /// The dependencies of the mod and their optional minimum required version
        /// </summary>
        IEnumerable<RequiredQMod> RequiredMods { get; }

        /// <summary>
        /// A list of mods, before which, this mod will load
        /// </summary>
        IEnumerable<string> ModsToLoadBefore { get; }

        /// <summary>
        /// A list of mods, after which, this mod will load
        /// </summary>
        IEnumerable<string> ModsToLoadAfter { get; }

        /// <summary>
        /// The assembly of this mod <para/>
        /// Check if <see langword="null"/> before using
        /// </summary>
        Assembly LoadedAssembly { get; }

        /// <summary>
        /// The assembly name of the mod
        /// </summary>
        string AssemblyName { get; }

        /// <summary>
        /// The version of the mod.
        /// </summary>
        Version ParsedVersion { get; }

        /// <summary>
        /// Whether or not this mod is enabled
        /// </summary>
        bool Enable { get; }

        /// <summary>
        /// Whether or not this mod has been loaded
        /// </summary>
        bool IsLoaded { get; }
    }
}
