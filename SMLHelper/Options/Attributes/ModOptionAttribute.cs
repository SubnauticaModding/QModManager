namespace SMLHelper.V2.Options.Attributes
{
    using Handlers;
    using Interfaces;
    using Json;
    using System;

    /// <summary>
    /// Abstract base attribute used to signify the decorated public member should generate a <see cref="ModOption"/>
    /// in a mod's options menu.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public abstract class ModOptionAttribute : Attribute, IModOptionAttribute
    {
        /// <summary>
        /// The label to use when displaying the field in the mod's options menu. If <see cref="LabelLanguageId"/> is set, this
        /// will be ignored.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The Id to be used for the field in the mod's option menu. If none is specified, one will be automatically generated when
        /// your <see cref="ConfigFile"/> is registered to the <see cref="OptionsPanelHandler"/>. This means it will
        /// change every time the game is launched, but is guaranteed to be unique. If you would like to specify an Id to use for
        /// internal comparisons, you can do so here.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString().Replace('-', '_');

        /// <summary>
        /// The order in which to display fields in the mod's option menu, in ascending order. If none is specified, the order will be
        /// automatically set.
        /// </summary>
        public int Order { get; set; } = i++;
        private static int i = 0;

        /// <summary>
        /// An optional tooltip to display for the field.
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// An optional id to be parsed with <see cref="Language.Get(string)"/> for the label, allowing for custom language-based strings
        /// via the <see cref="LanguageHandler"/> API. If this is set, it will take precedence.
        /// </summary>
        /// <seealso cref="LanguageHandler.SetLanguageLine(string, string)"/>
        /// <seealso cref="Language.Get(string)"/>
        public string LabelLanguageId { get; set; }

        /// <summary>
        /// Signifies the decorated member should be represented in the mod's options menu as a <see cref="ModOption"/>
        /// with an optional label.
        /// </summary>
        /// <param name="label">The label to display. If none is set, the name of the member will be used. If <see cref="LabelLanguageId"/>
        /// is set, this will be ignored.</param>
        protected ModOptionAttribute(string label = null)
        {
            Label = label;
        }
    }
}
