namespace SMLHelper.V2.Handlers
{
    using Assets;

    public class SpriteHandler
    {
        /// <summary>
        /// Registers a Sprite into the game.
        /// </summary>
        /// <param name="sprite"></param>
        /// <seealso cref="ModSprite"/>
        public static void RegisterSprite(ModSprite sprite)
        {
            ModSprite.Sprites.Add(sprite);
        }
    }
}
