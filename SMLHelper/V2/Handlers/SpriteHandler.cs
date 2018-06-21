namespace SMLHelper.V2.Handlers
{
    using Assets;

    /// <summary>
    /// A handler class for adding custom sprites into the game.
    /// </summary>
    public class SpriteHandler
    {
        /// <summary>
        /// Registers a Sprite into the game.
        /// </summary>
        /// <param name="sprite">The sprite to be added.</param>
        public static void RegisterSprite(ModSprite sprite)
        {
            ModSprite.Sprites.Add(sprite);
        }
    }
}
