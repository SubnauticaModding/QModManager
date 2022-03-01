namespace SMLHelper.V2.Json
{
    using System;

    /// <summary>
    /// Attribute used to specify the filename and subfolder for a <see cref="ConfigFile"/>.
    /// </summary>
    /// <remarks>
    /// When used alongside the <see cref="ConfigFile(string, string)"/> constructor, this attribute is ignored.
    /// </remarks>
    /// <example>
    /// <code>
    /// using SMLHelper.V2.Json;
    /// 
    /// [ConfigFile("options", "configs")]
    /// public class Config : ConfigFile
    /// {
    ///     public float MyFloatValue;
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ConfigFileAttribute : Attribute
    {
        /// <summary>
        /// The filename to use for the <see cref="ConfigFile"/>.
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// The subfolder within the mod's folder for the <see cref="ConfigFile"/>.
        /// </summary>
        public string Subfolder { get; set; } = string.Empty;

        /// <summary>
        /// Used to specify the filename for the <see cref="ConfigFile"/>.
        /// </summary>
        /// <remarks>
        /// When used alongside the <see cref="ConfigFile(string, string)"/> constructor, this attribute is ignored.
        /// </remarks>
        /// <param name="filename">The filename to use for the <see cref="ConfigFile"/>.</param>
        public ConfigFileAttribute(string filename = "config")
        {
            if (string.IsNullOrEmpty(filename))
                filename = "config";

            Filename = filename;
        }
    }
}
