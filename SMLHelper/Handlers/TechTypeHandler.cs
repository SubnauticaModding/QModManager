namespace SMLHelper.V2.Handlers
{
    using System.Collections.Generic;
    using System.Reflection;
    using Patchers;
    using Assets;
    using Utility;

    /// <summary>
    /// A handler class for everything related to creating new TechTypes.
    /// </summary>
    public static class TechTypeHandler
    {
        /// <summary>
        /// Adds a new <see cref="TechType"/> into the game.
        /// </summary>
        /// <param name="internalName">The internal name of the TechType. Should not contain special characters.</param>
        /// <param name="displayName">The display name of the TechType. Can be anything.</param>
        /// <param name="tooltip">The tooltip, displayed when hovered in an inventory. Can be anything.</param>
        /// <param name="unlockAtStart">Whether this TechType should be unlocked on game start, or not. By default, <c>true</c>.</param>
        /// <returns>The new <see cref="TechType"/> that is created.</returns>
        public static TechType AddTechType(string internalName, string displayName, string tooltip, bool unlockAtStart = true)
        {
            string modName = Assembly.GetCallingAssembly().GetName().Name;
            return AddTechType(modName, internalName, displayName, tooltip, unlockAtStart);
        }

        internal static TechType AddTechType(string modName, string internalName, string displayName, string tooltip, bool unlockAtStart = true)
        {
            // Register the TechType.
            TechType techType = TechTypePatcher.AddTechType(internalName);

            // Register Language lines.
            LanguagePatcher.AddCustomLanguageLine(modName, internalName, displayName);
            LanguagePatcher.AddCustomLanguageLine(modName, "Tooltip_" + internalName, tooltip);

            Dictionary<TechType, string> valueToString = TooltipFactory.techTypeTooltipStrings.valueToString;
            valueToString[techType] = "Tooltip_" + internalName;

            // Unlock the TechType on start
            if (unlockAtStart)
                KnownTechPatcher.UnlockedAtStart.Add(techType);

            // Return the new TechType.
            return techType;
        }

        /// <summary>
        /// Adds a new <see cref="TechType"/> into the game, with a sprite.
        /// </summary>
        /// <param name="internalName">The internal name of the TechType. Should not contain special characters.</param>
        /// <param name="displayName">The display name of the TechType. Can be anything.</param>
        /// <param name="tooltip">The tooltip, displayed when hovered in an inventory. Can be anything.</param>
        /// <param name="sprite">The sprite that will related to this TechType.</param>
        /// <param name="unlockAtStart">Whether this TechType should be unlocked on game start, or not. By default, <c>true</c>.</param>
        /// <returns>The new <see cref="TechType"/> that is created.</returns>
        public static TechType AddTechType(string internalName, string displayName, string tooltip, Atlas.Sprite sprite, bool unlockAtStart = true)
        {
            string modName = Assembly.GetCallingAssembly().GetName().Name;

            // Register the TechType using overload.
            TechType techType = AddTechType(internalName, displayName, tooltip, unlockAtStart);

            // Register the Sprite
            if(sprite != null)
                ModSprite.Add(SpriteManager.Group.None, internalName, sprite);

            // Return the new TechType
            return techType;
        }

        /// <summary>
        /// Adds a new <see cref="TechType"/> into the game, with a sprite.
        /// </summary>
        /// <param name="internalName">The internal name of the TechType. Should not contain special characters.</param>
        /// <param name="displayName">The display name of the TechType. Can be anything.</param>
        /// <param name="tooltip">The tooltip, displayed when hovered in an inventory. Can be anything.</param>
        /// <param name="sprite">The sprite that will related to this TechType.</param>
        /// <param name="unlockAtStart">Whether this TechType should be unlocked on game start, or not. By default, <c>true</c>.</param>
        /// <returns>The new <see cref="TechType"/> that is created.</returns>
        public static TechType AddTechType(string internalName, string displayName, string tooltip, UnityEngine.Sprite sprite, bool unlockAtStart = true)
        {
            string modName = Assembly.GetCallingAssembly().GetName().Name;

            // Register the TechType using overload.
            TechType techType = AddTechType(internalName, displayName, tooltip, unlockAtStart);

            // Register the Sprite
            if (sprite != null)
                ModSprite.Add(SpriteManager.Group.None, internalName, new Atlas.Sprite(sprite));

            // Return the new TechType
            return techType;
        }

        /// <summary>
        /// Safely looks for a modded item from another mod in the SMLHelper TechTypeCache and outputs its <see cref="TechType" /> value when found.
        /// </summary>
        /// <param name="techtypeString">The string used to define the modded item's new techtype.</param>
        /// <param name="modTechType">The TechType enum value of the modded. Defaults to <see cref="TechType.None" /> when the item was not found.</param>
        /// <returns>
        ///   <c>True</c> if the item was found; Otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// There's no guarantee in which order SMLHelper dependent mods are loaded,
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
        /// There's no guarantee in which order SMLHelper dependent mods are loaded,
        /// so if two mods are added at the same time, it may take a second game load for both to be visible to each other.
        /// </remarks>
        public static bool ModdedTechTypeExists(string techtypeString)
        {
            EnumTypeCache cache = TechTypePatcher.cacheManager.GetCacheForTypeName(techtypeString);
            return cache != null;
        }
    }
}
