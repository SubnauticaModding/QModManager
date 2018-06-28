namespace SMLHelper.V2.Handlers
{
    using Patchers;
    using Utilities;

    /// <summary>
    /// A handler class with features to aid in making your mod safely cross-compatible with another without DLL references.
    /// </summary>
    /// <remarks>
    /// There's no guarante in which order SMLHelper dependent mods are loaded,
    /// so if two mods are added at the same time, it may take a second game load for both to be visible to each other.
    /// </remarks>
    public static class CrossModSupportHandler
    {
        /// <summary>
        /// Safely looks for a modded item from another mod in the SMLHelper TechTypeCache and outputs its <see cref="TechType" /> value when found.
        /// </summary>
        /// <param name="techtypeString">The string used to define the modded item's new techtype.</param>
        /// <param name="modTechType">The TechType enum value of the modded. Defaults to <see cref="TechType.None" /> when the item was not found.</param>
        /// <returns>
        ///   <c>True</c> if the item was found; Otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// There's no guarante in which order SMLHelper dependent mods are loaded,
        /// so if two mods are added at the same time, it may take a second game load for both to be visible to each other.
        /// </remarks>
        public static bool TryGetModdedTechType(string techtypeString, out TechType modTechType)
        {
            EnumTypeCache cache = TechTypePatcher.cacheManager.GetCacheForTypeName(techtypeString);

            if (cache != null) // Item Found
            {
                modTechType = (TechType)cache.Index;
                return true;
            }
            else // Mod not present or not yet loaded
            {
                modTechType = TechType.None;
                return false;
            }
        }

        /// <summary>
        /// Safely looks for a modded item from another mod in the SMLHelper TechTypeCache.
        /// </summary>
        /// <param name="techtypeString">The string used to define the modded item's new techtype.</param>
        /// <returns>
        ///   <c>True</c> if the item was found; Otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// There's no guarante in which order SMLHelper dependent mods are loaded,
        /// so if two mods are added at the same time, it may take a second game load for both to be visible to each other.
        /// </remarks>
        public static bool ModdedTechTypeExists(string techtypeString)
        {
            EnumTypeCache cache = TechTypePatcher.cacheManager.GetCacheForTypeName(techtypeString);
            return cache != null;
        }

        /// <summary>
        /// Safely looks for a modded CraftTree Type from another mod in the SMLHelper CraftTreeTypeCache.
        /// </summary>
        /// <param name="craftTreeString">The string used to define the modded item's new techtype.</param>
        /// <returns>
        ///   <c>True</c> if the craft tree was found; Otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// There's no guarante in which order SMLHelper dependent mods are loaded,
        /// so if two mods are added at the same time, it may take a second game load for both to be visible to each other.
        /// </remarks>
        public static bool ModdedCraftTreeTypeExists(string craftTreeString)
        {
            EnumTypeCache cache = CraftTreeTypePatcher.cacheManager.GetCacheForTypeName(craftTreeString);
            return cache != null;
        }
    }
}
