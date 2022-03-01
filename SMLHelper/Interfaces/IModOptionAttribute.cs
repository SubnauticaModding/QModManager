namespace SMLHelper.V2.Interfaces
{
    using Handlers;
    using Json;
    using Options;
    using Options.Attributes;

    /// <summary>
    /// Defines properties for <see cref="ModOptionAttribute"/> derivatives to implement for the purpose of holding
    /// metadata about <see cref="ModOption"/> fields and their generation.
    /// </summary>
    public interface IModOptionAttribute
    {
        /// <summary>
        /// The label to use when displaying the field in the mod's options menu.
        /// </summary>
        string Label { get; set; }

        /// <summary>
        /// The Id to be used for the field in the mod's option menu. If none is specified, one will be automatically generated when
        /// your <see cref="ConfigFile"/> is registered to the <see cref="OptionsPanelHandler"/>. This means it will
        /// change every time the game is launched, but is guaranteed to be unique. If you would like to specify an Id to use for
        /// internal comparisons, you can do so here.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// The order in which to display fields in the mod's option menu, in ascending order. If none is specified, the order will be
        /// automatically set.
        /// </summary>
        int Order { get; set; }

        /// <summary>
        /// An optional tooltip to display for the field.
        /// </summary>
        string Tooltip { get; }

        /// <summary>
        /// An optional id to be parsed with <see cref="Language.Get(string)"/> for the label, allowing for custom language-based strings
        /// via the <see cref="LanguageHandler"/> API.
        /// </summary>
        /// <seealso cref="LanguageHandler.SetLanguageLine(string, string)"/>
        /// <seealso cref="Language.Get(string)"/>
        string LabelLanguageId { get; }
    }
}
