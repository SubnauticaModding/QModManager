namespace SMLHelper.V2.Handlers
{
    using Assets;

    /// <summary>
    /// A handler class for adding custom sprites into the game.
    /// </summary>
    public static class SpriteHandler
    {
        /// <summary>
        /// Registers a Sprite into the game.
        /// </summary>
        /// <param name="sprite">The sprite to be added.</param>
        public static void RegisterSprite(ModSprite sprite)
        {
            ModSprite.Sprites.Add(sprite);
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="type">The techtype paired to this sprite.</param>
        /// <param name="sprite">The sprite to be added.</param>
        public static void RegisterSprite(TechType type, Atlas.Sprite sprite)
        {
            ModSprite.Sprites.Add(new ModSprite(type, sprite));
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="group">The sprite group.</param>
        /// <param name="id">The sprite internal identifier.</param>
        /// <param name="sprite">The sprite to be added.</param>
        public static void RegisterSprite(SpriteManager.Group group, string id, Atlas.Sprite sprite)
        {
            ModSprite.Sprites.Add(new ModSprite(group, id, sprite));
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="group">The sprite group this sprite will be added to.</param>
        /// <param name="id">The sprite internal identifier.</param>
        /// <param name="sprite">The sprite to be added.</param>
        public static void RegisterSprite(SpriteManager.Group group, string id, UnityEngine.Sprite sprite)
        {
            ModSprite.Sprites.Add(new ModSprite(group, id, sprite));
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="type">The techtype paired to this sprite.</param>
        /// <param name="sprite">The sprite to be added.</param>
        public static void RegisterSprite(TechType type, UnityEngine.Sprite sprite)
        {
            ModSprite.Sprites.Add(new ModSprite(type, sprite));
        }
    }
}
