namespace QModManager.API
{
    using System;
    using QModManager.Utility;

    /// <summary>
    /// Identifies a required mod and an optional minimum version.
    /// </summary>
    public class RequiredQMod
    {
        internal static IVersionParser VersionParserService { get; set; } = new VersionParser();

        private RequiredQMod(string id, bool requiresMinVersion, Version version)
        {
            this.Id = id;
            this.RequiresMinimumVersion = requiresMinVersion;
            this.MinimumVersion = version;
        }

        internal RequiredQMod(string id)
            : this(id, false, VersionParserService.NoVersionParsed)
        {
        }

        internal RequiredQMod(string id, Version minimumVersion)
            : this(id, !VersionParserService.IsAllZeroVersion(minimumVersion), minimumVersion)
        {
        }

        internal RequiredQMod(string id, string minimumVersion)
            : this(id, !string.IsNullOrEmpty(minimumVersion), VersionParserService.GetVersion(minimumVersion))
        {
        }

        /// <summary>
        /// Gets the required mod's ID.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets a value indicating whether the mod must be at a minimum version for compatibility.
        /// </summary>
        public bool RequiresMinimumVersion { get; }

        /// <summary>
        /// Gets the minimum version this mod should be at.<para/>
        /// If <see cref="RequiresMinimumVersion"/> is <c>false</c>, this will return a default value.
        /// </summary>
        public Version MinimumVersion { get; }
    }
}
