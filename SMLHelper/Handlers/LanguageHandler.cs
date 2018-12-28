namespace SMLHelper.V2.Handlers
{
    using Patchers;

    public class LanguageHandler
    {
        /// <summary>
        /// Allows you to define a language entry into the game.
        /// </summary>
        /// <param name="lineId">The ID of the entry, this is what is used to get the actual text.</param>
        /// <param name="text">The actual text related to the entry.</param>
        public static void SetLanguageLine(string lineId, string text)
        {
            LanguagePatcher.customLines[lineId] = text;
        }
        
        /// <summary>
        /// Allows you to set the display name of a specific <see cref="TechType"/>.
        /// </summary>
        /// <param name="techType">The <see cref="TechType"/> whose display name that is to be changed.</param>
        /// <param name="text">The new display name for the chosen <see cref="TechType"/>.</param>
        public static void SetTechTypeName(TechType techType, string text)
            => SetLanguageLine(techType.AsString(), text);

        /// <summary>
        /// Allows you to set the tooltip of a specific <see cref="TechType"/>.
        /// </summary>
        /// <param name="techType">The <see cref="TechType"/> whose tooltip that is to be changed.</param>
        /// <param name="text">The new tooltip for the chosen <see cref="TechType"/>.</param>
        public static void SetTechTypeTooltip(TechType techType, string text)
            => SetLanguageLine($"Tooltip_{techType.AsString()}", text);
    }
}
