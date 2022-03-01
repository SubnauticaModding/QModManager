namespace SMLHelper.V2.Handlers
{
    using System.Collections.Generic;
    using Interfaces;
    using Utility;
    using Patchers.EnumPatching;
#if SUBNAUTICA
    using Sprite = Atlas.Sprite;
#elif BELOWZERO
    using Sprite = UnityEngine.Sprite;
#endif

    /// <summary>
    /// A handler for everything related to creating new BackgroundTypes.
    /// </summary>
    public class BackgroundTypeHandler : IBackgroundTypeHandler
    {
        internal static readonly Dictionary<CraftData.BackgroundType, Sprite> BackgroundSprites = new Dictionary<CraftData.BackgroundType, Sprite>();

        /// <summary>
        /// Main entry point for all calls to this handler.
        /// </summary>
        public static IBackgroundTypeHandler Main { get; } = new BackgroundTypeHandler();

        private BackgroundTypeHandler()
        {
            // Hide constructor
        }

        #region Interface Implementations

        /// <summary>
        /// adds a new <see cref="CraftData.BackgroundType"/> into the game.
        /// </summary>
        /// <param name="backgroundTypeName">the name of the BackgroundType, should not contain special characters.</param>
        /// <param name="backgroundSprite">The sprite for this BackgroundType.</param>
        /// <returns>The new <see cref="CraftData.BackgroundType"/> that's created.</returns>
        CraftData.BackgroundType IBackgroundTypeHandler.AddBackgroundType(string backgroundTypeName, Sprite backgroundSprite)
        {
            var backgroundType = BackgroundTypePatcher.AddBackgroundType(backgroundTypeName);

            BackgroundSprites[backgroundType] = backgroundSprite;

            return backgroundType;
        }

        /// <summary>
        /// Safely looks for a modded Background Type from another mod in the SMLHelper BackgroundTypeCache and outputs its <see cref="CraftData.BackgroundType" /> value when found.
        /// </summary>
        /// <param name="backgroundTypeString">The string used to define the BackgroundType</param>
        /// <param name="modBackgroundType">The BackgroundType enum value of the modded. Defaults to <see cref="CraftData.BackgroundType.Normal" /> when the item was not found.</param>
        /// <returns><see langword="true"/> if the item was found; otherwise <see langword="false"/>.</returns>
        bool IBackgroundTypeHandler.TryGetModdedBackgroundType(string backgroundTypeString, out CraftData.BackgroundType modBackgroundType)
        {
            EnumTypeCache cache = BackgroundTypePatcher.cacheManager.RequestCacheForTypeName(backgroundTypeString, false);

            if (cache != null) // Item Found
            {
                modBackgroundType = (CraftData.BackgroundType)cache.Index;
                return true;
            }
            else // Mod not present or not yet loaded
            {
                modBackgroundType = CraftData.BackgroundType.Normal;
                return false;
            }
        }

        /// <summary>
        /// Safely looks for a modded Background Type from another mod in the SMLHelper BackgroundTypeCache.
        /// </summary>
        /// <param name="backgroundTypeString">The string used to define the BackgroundType.</param>
        /// <returns><see langword="true"/> if the item was found; otherwise <see langword="false"/>.</returns>
        bool IBackgroundTypeHandler.ModdedBackgroundTypeExists(string backgroundTypeString)
        {
            EnumTypeCache cache = BackgroundTypePatcher.cacheManager.RequestCacheForTypeName(backgroundTypeString, false);

            if (cache != null) // Item Found
            {
                return true;
            }
            else // Mod not present or not yet loaded
            {
                return false;
            }
        }
        #endregion

        #region Static Methods

        /// <summary>
        /// adds a new <see cref="CraftData.BackgroundType"/> into the game.
        /// </summary>
        /// <param name="backgroundTypeName">the name of the BackgroundType, should not contain special characters.</param>
        /// <param name="backgroundSprite">The sprite for this BackgroundType.</param>
        /// <returns>The new <see cref="CraftData.BackgroundType"/> that's created.</returns>
        /// <returns></returns>
        public static CraftData.BackgroundType AddBackgroundType(string backgroundTypeName, Sprite backgroundSprite)
        {
            return Main.AddBackgroundType(backgroundTypeName, backgroundSprite);
        }

        /// <summary>
        /// Safely looks for a modded Background Type from another mod in the SMLHelper BackgroundTypeCache and outputs its <see cref="CraftData.BackgroundType" /> value when found.
        /// </summary>
        /// <param name="backgroundTypeString">The string used to define the BackgroundType</param>
        /// <param name="modBackgroundType">The BackgroundType enum value of the modded. Defaults to <see cref="CraftData.BackgroundType.Normal" /> when the item was not found.</param>
        /// <returns><see langword="true"/> if the item was found; otherwise <see langword="false"/>.</returns>
        public static bool TryGetModdedBackgroundType(string backgroundTypeString, out CraftData.BackgroundType modBackgroundType)
        {
            return Main.TryGetModdedBackgroundType(backgroundTypeString, out modBackgroundType);
        }

        /// <summary>
        /// Safely looks for a modded Background Type from another mod in the SMLHelper BackgroundTypeCache.
        /// </summary>
        /// <param name="backgroundTypeString">The string used to define the BackgroundType.</param>
        /// <returns><see langword="true"/> if the item was found; otherwise <see langword="false"/>.</returns>
        public static bool ModdedBackgroundTypeExists(string backgroundTypeString)
        {
            return Main.ModdedBackgroundTypeExists(backgroundTypeString);
        }

        #endregion
    }
}
