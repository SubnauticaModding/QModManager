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
        
        public static void SetTechTypeName(TechType techType, string text)
            => SetLanguageLine(techType.ToString(), text);
        
        public static void SetTechTypeTooltip(TechType techType, string text)
            => SetLanguageLine($"Tooltip_{techType.ToString()}", text);
    }
}
