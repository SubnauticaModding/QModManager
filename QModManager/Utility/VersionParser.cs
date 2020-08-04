namespace QModManager.Utility
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Defines a service for parsing version strings.
    /// </summary>
    public interface IVersionParser
    {
        /// <summary>
        /// The default version zero used when no version could be parsed.
        /// </summary>
        Version NoVersionParsed { get; }

        /// <summary>
        /// Returns a new <see cref="Version"/> based on the provided string value, with all 4 groups populated.
        /// </summary>
        /// <param name="versionString">The version string to parse.</param>
        /// <returns>A new <see cref="Version"/> with all empty groups populated with 0.</returns>
        Version GetVersion(string versionString);

        /// <summary>
        /// Checks if the provided version is all zeros.
        /// </summary>
        /// <param name="version">The version to check.</param>
        /// <returns>True if this matches version 0.0.0.0</returns>
        bool IsAllZeroVersion(Version version);
    }

    /// <summary>
    /// A service that handles parsing <see cref="string"/> values into <see cref="Version"/> objects.
    /// </summary>
    public class VersionParser : IVersionParser
    {
        private static readonly Version VersionZeroSingleton = new Version(0, 0, 0, 0);

        /// <summary>
        /// The regex used to sanitize incoming version strings.
        /// ^(((\d+)\.?){0,3}\d+)$
        /// </summary>
        public static readonly Regex VersionRegex = new Regex(@"^(((\d+)\.?){0,3}\d+)$");

        /// <summary>
        /// The default version zero used when no version could be parsed.
        /// </summary>
        public Version NoVersionParsed => new Version(0, 0, 0, 0);

        /// <summary>
        /// Returns a new <see cref="Version"/> based on the provided string value, with all 4 groups populated.
        /// </summary>
        /// <param name="versionString">The version string to parse. This must match <seealso cref="VersionRegex"/>.</param>
        /// <returns>A new <see cref="Version"/> with all empty groups populated with 0.</returns>
        /// <example>
        /// "2.8" will be parsed as "2.8.0.0"
        /// </example>
        public Version GetVersion(string versionString)
        {
            if (!VersionRegex.IsMatch(versionString))
            {
                return NoVersionParsed;
            }

            versionString = VersionRegex.Matches(versionString)?[0]?.Value;

            int groups = versionString.Split('.').Length;
            while (groups++ < 4)
            {
                versionString += ".0";
            }

            if (Version.TryParse(versionString, out Version parsedVersion))
            {
                return parsedVersion;
            }

            return NoVersionParsed;
        }

        /// <summary>
        /// Checks if the provided version is all zeros.
        /// </summary>
        /// <param name="version">The version to check.</param>
        /// <returns>True if this matches version 0.0.0.0</returns>
        public bool IsAllZeroVersion(Version version)
        {
            return version == VersionZeroSingleton;
        }
    }
}
