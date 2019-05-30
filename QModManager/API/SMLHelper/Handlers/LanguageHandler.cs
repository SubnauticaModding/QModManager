namespace QModManager.API.SMLHelper.Handlers
{
    using Interfaces;
    using Patchers;
    using QModManager.Utility;

    /// <summary>
    /// A handler class for managing language lines
    /// </summary>
    public class LanguageHandler : ILanguageHandler
    {
        public static ILanguageHandler Main { get; } = new LanguageHandler();

        private LanguageHandler() { }

        #region Static Methods

        /// <summary>
        /// Allows you to define a language entry into the game.
        /// </summary>
        /// <param name="lineId">The ID of the entry, this is what is used to get the actual text.</param>
        /// <param name="text">The actual text related to the entry.</param>
        public static void SetLanguageLine(string lineId, string text)
        {
            Main.SetLanguageLine(lineId, text);
        }

        /// <summary>
        /// Allows you to set the display name of a specific <see cref="TechType"/>.
        /// </summary>
        /// <param name="techType">The <see cref="TechType"/> whose display name that is to be changed.</param>
        /// <param name="text">The new display name for the chosen <see cref="TechType"/>.</param>
        public static void SetTechTypeName(TechType techType, string text)
        {
            Main.SetTechTypeName(techType, text);
        }

        /// <summary>
        /// Allows you to set the tooltip of a specific <see cref="TechType"/>.
        /// </summary>
        /// <param name="techType">The <see cref="TechType"/> whose tooltip that is to be changed.</param>
        /// <param name="text">The new tooltip for the chosen <see cref="TechType"/>.</param>
        public static void SetTechTypeTooltip(TechType techType, string text)
        {
            Main.SetTechTypeTooltip(techType, text);
        }

        #endregion

        #region Interface Methods

        /// <summary>
        /// Allows you to define a language entry into the game.
        /// </summary>
        /// <param name="lineId">The ID of the entry, this is what is used to get the actual text.</param>
        /// <param name="text">The actual text related to the entry.</param>
        void ILanguageHandler.SetLanguageLine(string lineId, string text)
        {
            string modName = ReflectionHelper.CallingAssemblyByStackTrace().GetName().Name;

            LanguagePatcher.AddCustomLanguageLine(modName, lineId, text);
        }

        /// <summary>
        /// Allows you to set the display name of a specific <see cref="TechType"/>.
        /// </summary>
        /// <param name="techType">The <see cref="TechType"/> whose display name that is to be changed.</param>
        /// <param name="text">The new display name for the chosen <see cref="TechType"/>.</param>
        void ILanguageHandler.SetTechTypeName(TechType techType, string text)
        {
            string modName = ReflectionHelper.CallingAssemblyByStackTrace().GetName().Name;

            LanguagePatcher.AddCustomLanguageLine(modName, techType.AsString(), text);
        }

        /// <summary>
        /// Allows you to set the tooltip of a specific <see cref="TechType"/>.
        /// </summary>
        /// <param name="techType">The <see cref="TechType"/> whose tooltip that is to be changed.</param>
        /// <param name="text">The new tooltip for the chosen <see cref="TechType"/>.</param>
        void ILanguageHandler.SetTechTypeTooltip(TechType techType, string text)
        {
            string modName = ReflectionHelper.CallingAssemblyByStackTrace().GetName().Name;

            LanguagePatcher.AddCustomLanguageLine(modName, $"Tooltip_{techType.AsString()}", text);
        }

        #endregion
    }
}
