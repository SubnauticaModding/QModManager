namespace SMLHelper.V2.Options.Attributes
{
    using Json;
    using System;

    /// <summary>
    /// Attribute used to signify the decorated <see cref="bool"/> should be represented in the mod's
    /// option menu as a <see cref="ModToggleOption"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// using SMLHelper.V2.Json;
    /// using SMLHelper.V2.Options;
    /// 
    /// [Menu("My Options Menu")]
    /// public class Config : ConfigFile
    /// {
    ///     [Toggle("My Toggle")]
    ///     public bool MyToggle;
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="MenuAttribute"/>
    /// <seealso cref="ConfigFile"/>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ToggleAttribute : ModOptionAttribute
    {
        /// <summary>
        /// Signifies the decorated <see cref="bool"/> should be represented in the mod's option menu
        /// as a <see cref="ModToggleOption"/>.
        /// </summary>
        /// <param name="label">The label for the toggle.</param>
        public ToggleAttribute(string label = null) : base(label) { }

        /// <summary>
        /// Signifies the decorated <see cref="bool"/> should be represented in the mod's option menu
        /// as a <see cref="ModToggleOption"/>.
        /// </summary>
        public ToggleAttribute() { }
    }
}
