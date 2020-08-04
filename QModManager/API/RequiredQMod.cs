namespace QModManager.API
{
    using System;

    /// <summary>
    /// Identifies a required mod and an optional minimum version.
    /// </summary>
    public class RequiredQMod
    {
        internal RequiredQMod(string id)
        {
            this.Id = id;
            this.RequiresMinimumVersion = false;
            this.MinimumVersion = new Version(0, 0, 0, 0);
        }

        internal RequiredQMod(string id, string minimumVersion)
        {
            this.Id = id;
            this.RequiresMinimumVersion = true;

            int groups = minimumVersion.Split('.').Length;
            while (groups++ < 4)
            {
                minimumVersion += ".0";
            }

            this.MinimumVersion = Version.Parse(minimumVersion);
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
