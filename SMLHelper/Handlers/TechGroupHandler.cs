namespace SMLHelper.V2.Handlers
{
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Interfaces;
    using SMLHelper.V2.Patchers.EnumPatching;
    using SMLHelper.V2.Utility;

    /// <summary>
    /// A handler class for everything related to creating new TechGroups.
    /// </summary>
    public class TechGroupHandler : ITechGroupHandler
    {
        /// <summary>
        /// Main entry point for all calls to this handler.
        /// </summary>
        public static ITechGroupHandler Main { get; } = new TechGroupHandler();

        private TechGroupHandler()
        {
            // Hide constructor
        }

        /// <summary>
        /// Adds a new <see cref="TechGroup" /> into the game.
        /// </summary>
        /// <param name="techGroupName">The name of the TechGroup. Should not contain special characters.</param>
        /// <param name="displayName">The display name of the TechGroup. Can be anything.</param>
        /// <returns>
        /// The new <see cref="TechGroup" /> that is created.
        /// </returns>
        public TechGroup AddTechGroup(string techGroupName, string displayName)
        {
            TechGroup techGroup = TechGroupPatcher.AddTechGroup(techGroupName);

            LanguageHandler.SetLanguageLine("Group" + techGroupName, displayName);

            return techGroup;
        }

        /// <summary>
        /// Safely looks for a modded group from another mod in the SMLHelper TechGroupCache.
        /// </summary>
        /// <param name="techGroupString">The string used to define the techgroup.</param>
        /// <returns>
        ///   <c>True</c> if the item was found; Otherwise <c>false</c>.
        /// </returns>
        public bool ModdedTechGroupExists(string techGroupString)
        {
            EnumTypeCache cache = TechGroupPatcher.cacheManager.RequestCacheForTypeName(techGroupString, false);

            if (cache != null) // Item Found
            {
                return true;
            }
            else // Mod not present or not yet loaded
            {
                return false;
            }
        }

        /// <summary>
        /// Safely looks for a modded item from another mod in the SMLHelper TechGroupCache and outputs its <see cref="TechGroup" /> value when found.
        /// </summary>
        /// <param name="techGroupString">The string used to define the techgroup.</param>
        /// <param name="modTechGroup">The TechGroup enum value of the modded. Defaults to <see cref="TechGroup.Uncategorized" /> when the item was not found.</param>
        /// <returns>
        ///   <c>True</c> if the item was found; Otherwise <c>false</c>.
        /// </returns>
        public bool TryGetModdedTechGroup(string techGroupString, out TechGroup modTechGroup)
        {
            EnumTypeCache cache = TechGroupPatcher.cacheManager.RequestCacheForTypeName(techGroupString, false);

            if (cache != null) // Item Found
            {
                modTechGroup = (TechGroup)cache.Index;
                return true;
            }
            else // Mod not present or not yet loaded
            {
                modTechGroup = TechGroup.Uncategorized;
                return false;
            }
        }
    }
}
