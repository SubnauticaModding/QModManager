namespace SMLHelper.V2.Interfaces
{
#if SUBNAUTICA
    using Sprite = Atlas.Sprite;
#elif BELOWZERO
    using Sprite = UnityEngine.Sprite;
#endif

    /// <summary>
    /// A handler interface for everything related to creating new BackgroundTypes.
    /// </summary>
    public interface IBackgroundTypeHandler
    {
        /// <summary>
        /// adds a new <see cref="CraftData.BackgroundType"/> into the game.
        /// </summary>
        /// <param name="backgroundTypeName">the name of the BackgroundType, should not contain special characters.</param>
        /// <param name="backgroundSprite">The sprite for this BackgroundType.</param>
        /// <returns>The new <see cref="CraftData.BackgroundType"/> that's created.</returns>
        CraftData.BackgroundType AddBackgroundType(string backgroundTypeName, Sprite backgroundSprite);

        /// <summary>
        /// Safely looks for a modded Background Type from another mod in the SMLHelper BackgroundTypeCache and outputs its <see cref="CraftData.BackgroundType" /> value when found.
        /// </summary>
        /// <param name="backgroundTypeString">The string used to define the BackgroundType</param>
        /// <param name="modBackgroundType">The BackgroundType enum value of the modded. Defaults to <see cref="CraftData.BackgroundType.Normal" /> when the item was not found.</param>
        /// <returns><see langword="true"/> if the item was found; otherwise <see langword="false"/>.</returns>
        bool TryGetModdedBackgroundType(string backgroundTypeString, out CraftData.BackgroundType modBackgroundType);

        /// <summary>
        /// Safely looks for a modded Background Type from another mod in the SMLHelper BackgroundTypeCache.
        /// </summary>
        /// <param name="backgroundTypeString">The string used to define the BackgroundType.</param>
        /// <returns><see langword="true"/> if the item was found; otherwise <see langword="false"/>.</returns>
        bool ModdedBackgroundTypeExists(string backgroundTypeString);
    }
}
