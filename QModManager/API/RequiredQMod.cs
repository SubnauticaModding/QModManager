namespace QModManager.API
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Identifies a required mod and an optional minimum version.
    /// </summary>
    public class RequiredQMod
    {
        internal static readonly Regex VersionRegex = new Regex(@"^(((\d+)\.?){0,3}\d+)$");

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

            var cleanVersion = CleanVersionString(minimumVersion);

            this.MinimumVersion = Version.Parse(cleanVersion);
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

        internal static string CleanVersionString(string versionString)
        {
            if (!VersionRegex.IsMatch(versionString))
            {
                return null;
            }

            versionString = VersionRegex.Matches(versionString)?[0]?.Value;

            int groups = versionString.Split('.').Length;
            while (groups++ < 4)
            {
                versionString += ".0";
            }

            return versionString;
        }
    }
}
