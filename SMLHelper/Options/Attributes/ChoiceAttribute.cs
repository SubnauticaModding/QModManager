namespace SMLHelper.V2.Options.Attributes
{
    using Json;
    using System;

    /// <summary>
    /// Attribute used to signify the decorated member should be represented in the mod's options menu as a
    /// <see cref="ModChoiceOption"/>. Works for either <see cref="int"/> index-based, <see cref="string"/>-based, or
    /// <see cref="Enum"/>-based members.
    /// </summary>
    /// <remarks>
    /// <see cref="Enum"/> choices can also be parsed from their values by merely omitting the <see cref="ChoiceAttribute"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// using SMLHelper.V2.Json;
    /// using SMLHelper.V2.Options;
    /// 
    /// public enum CustomChoice { One, Two, Three }
    /// 
    /// [Menu("My Options Menu")]
    /// public class Config : ConfigFile
    /// {
    ///     [Choice("My index-based choice", "One", "Two", "Three")]
    ///     public int MyIndexBasedChoice;
    ///     
    ///     [Choice]
    ///     public CustomChoice MyEnumBasedChoice;
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="MenuAttribute"/>
    /// <seealso cref="ModChoiceOption"/>
    /// <seealso cref="ConfigFile"/>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ChoiceAttribute : ModOptionAttribute
    {
        /// <summary>
        /// The list of options that will be displayed.
        /// </summary>
        public string[] Options { get; set; }

        /// <summary>
        /// Attribute used to signify the decorated member should be represented in the mod's options menu as a
        /// <see cref="ModChoiceOption"/>. Works for either <see cref="int"/> index-based, <see cref="string"/>-based, or
        /// <see cref="Enum"/>-based members.
        /// </summary>
        /// <remarks>
        /// <see cref="Enum"/> choices can also be parsed from their values by merely omitting the <paramref name="options"/>.
        /// </remarks>
        /// <param name="label">The label for the choice. If none is set, the name of the member will be used.</param>
        /// <param name="options">The list of options for the user to choose from.</param>
        public ChoiceAttribute(string label = null, params string[] options) : base(label)
        {
            Options = options;
        }

        /// <summary>
        /// Attribute used to signify the decorated member should be represented in the mod's options menu as a
        /// <see cref="ModChoiceOption"/>. Works for either <see cref="int"/> index-based, <see cref="string"/>-based, or
        /// <see cref="Enum"/>-based members.
        /// </summary>
        /// <remarks>
        /// <see cref="Enum"/> choices can also be parsed from their values by merely omitting the <paramref name="options"/>.
        /// </remarks>
        /// <param name="options">The list of options for the user to choose from.</param>
        public ChoiceAttribute(string[] options) : this(null, options) { }

        /// <summary>
        /// Attribute used to signify the decorated member should be represented in the mod's options menu as a
        /// <see cref="ModChoiceOption"/>. Works for either <see cref="int"/> index-based, <see cref="string"/>-based, or
        /// <see cref="Enum"/>-based members.
        /// </summary>
        public ChoiceAttribute() { }
    }
}
