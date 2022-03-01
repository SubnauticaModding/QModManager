namespace SMLHelper.V2.Interfaces
{
    /// <summary>
    /// A handler class for everything related to creating new TechTypes.
    /// </summary>
    public interface ITechTypeHandler
    {
        /// <summary>
        /// Adds a new <see cref="TechType"/> into the game. This new techtype will be unlocked at the start of a the game.
        /// </summary>
        /// <param name="internalName">The internal name of the TechType. Should not contain special characters.</param>
        /// <param name="displayName">The display name of the TechType. Can be anything.</param>
        /// <param name="tooltip">The tooltip, displayed when hovered in an inventory. Can be anything.</param>
        /// <returns>The new <see cref="TechType"/> that is created.</returns>
        TechType AddTechType(string internalName, string displayName, string tooltip);

        /// <summary>
        /// Adds a new <see cref="TechType"/> into the game.
        /// </summary>
        /// <param name="internalName">The internal name of the TechType. Should not contain special characters.</param>
        /// <param name="displayName">The display name of the TechType. Can be anything.</param>
        /// <param name="tooltip">The tooltip, displayed when hovered in an inventory. Can be anything.</param>
        /// <param name="unlockAtStart">Whether this TechType should be unlocked on game start, or not. By default, <c>true</c>.</param>
        /// <returns>The new <see cref="TechType"/> that is created.</returns>
        TechType AddTechType(string internalName, string displayName, string tooltip, bool unlockAtStart);

#if SUBNAUTICA
        /// <summary>
        /// Adds a new <see cref="TechType"/> into the game, with a sprite. This new techtype will be unlocked at the start of a the game.
        /// </summary>
        /// <param name="internalName">The internal name of the TechType. Should not contain special characters.</param>
        /// <param name="displayName">The display name of the TechType. Can be anything.</param>
        /// <param name="tooltip">The tooltip, displayed when hovered in an inventory. Can be anything.</param>
        /// <param name="sprite">The sprite that will related to this TechType.</param>
        /// <returns>The new <see cref="TechType"/> that is created.</returns>
        TechType AddTechType(string internalName, string displayName, string tooltip, Atlas.Sprite sprite);

        /// <summary>
        /// Adds a new <see cref="TechType"/> into the game, with a sprite.
        /// </summary>
        /// <param name="internalName">The internal name of the TechType. Should not contain special characters.</param>
        /// <param name="displayName">The display name of the TechType. Can be anything.</param>
        /// <param name="tooltip">The tooltip, displayed when hovered in an inventory. Can be anything.</param>
        /// <param name="sprite">The sprite that will related to this TechType.</param>
        /// <param name="unlockAtStart">Whether this TechType should be unlocked on game start, or not. By default, <c>true</c>.</param>
        /// <returns>The new <see cref="TechType"/> that is created.</returns>
        TechType AddTechType(string internalName, string displayName, string tooltip, Atlas.Sprite sprite, bool unlockAtStart);
        
#endif
        /// <summary>
        /// Adds a new <see cref="TechType"/> into the game, with a sprite. This new techtype will be unlocked at the start of a the game.
        /// </summary>
        /// <param name="internalName">The internal name of the TechType. Should not contain special characters.</param>
        /// <param name="displayName">The display name of the TechType. Can be anything.</param>
        /// <param name="tooltip">The tooltip, displayed when hovered in an inventory. Can be anything.</param>
        /// <param name="sprite">The sprite that will related to this TechType.</param>
        /// <returns>The new <see cref="TechType"/> that is created.</returns>
        TechType AddTechType(string internalName, string displayName, string tooltip, UnityEngine.Sprite sprite);

        /// <summary>
        /// Adds a new <see cref="TechType"/> into the game, with a sprite.
        /// </summary>
        /// <param name="internalName">The internal name of the TechType. Should not contain special characters.</param>
        /// <param name="displayName">The display name of the TechType. Can be anything.</param>
        /// <param name="tooltip">The tooltip, displayed when hovered in an inventory. Can be anything.</param>
        /// <param name="sprite">The sprite that will related to this TechType.</param>
        /// <param name="unlockAtStart">Whether this TechType should be unlocked on game start, or not. By default, <c>true</c>.</param>
        /// <returns>The new <see cref="TechType"/> that is created.</returns>
        TechType AddTechType(string internalName, string displayName, string tooltip, UnityEngine.Sprite sprite, bool unlockAtStart);

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
        bool TryGetModdedTechType(string techtypeString, out TechType modTechType);

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
        bool ModdedTechTypeExists(string techtypeString);
    }
}
