namespace SMLHelper.V2.Handlers
{
    using Assets;
    using UnityEngine;
    using Utility;

    /// <summary>
    /// A handler class for adding custom sprites into the game.
    /// </summary>
    public static class SpriteHandler
    {
        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="type">The techtype paired to this sprite.</param>
        /// <param name="sprite">The sprite to be added.</param>
        public static void RegisterSprite(TechType type, Atlas.Sprite sprite)
        {
            ModSprite.Add(SpriteManager.Group.None, type.AsString(), sprite);
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="group">The sprite group.</param>
        /// <param name="id">The sprite internal identifier.</param>
        /// <param name="sprite">The sprite to be added.</param>
        public static void RegisterSprite(SpriteManager.Group group, string id, Atlas.Sprite sprite)
        {
            ModSprite.Add(group, id, sprite);
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="group">The sprite group this sprite will be added to.</param>
        /// <param name="id">The sprite internal identifier.</param>
        /// <param name="sprite">The sprite to be added.</param>
        public static void RegisterSprite(SpriteManager.Group group, string id, Sprite sprite)
        {
            ModSprite.Add(group, id, new Atlas.Sprite(sprite));
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="type">The techtype paired to this sprite.</param>
        /// <param name="sprite">The sprite to be added.</param>
        public static void RegisterSprite(TechType type, Sprite sprite)
        {
            ModSprite.Add(SpriteManager.Group.None, type.AsString(), new Atlas.Sprite(sprite));
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="type">The techtype paired to this sprite.</param>
        /// <param name="filePathToImage">The file path to image to be converted into a sprite.</param>
        /// <param name="format"><para>The texture format. By default, this uses <see cref="TextureFormat.BC7" />.</para>
        /// <para>https://docs.unity3d.com/ScriptReference/TextureFormat.BC7.html</para>
        /// <para>Don't change this unless you really know what you're doing.</para></param>
        /// <seealso cref="ImageUtils.LoadTextureFromFile(string, TextureFormat)" />
        public static void RegisterSprite(TechType type, string filePathToImage, TextureFormat format = TextureFormat.BC7)
        {
            var sprite = ImageUtils.LoadSpriteFromFile(filePathToImage, format);

            RegisterSprite(type, sprite);
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="group">The sprite group.</param>
        /// <param name="id">The sprite internal identifier.</param>
        /// <param name="filePathToImage">The file path to image.</param>
        /// <param name="format"><para>The texture format. By default, this uses <see cref="TextureFormat.BC7" />.</para>
        /// <para>https://docs.unity3d.com/ScriptReference/TextureFormat.BC7.html</para>
        /// <para>Don't change this unless you really know what you're doing.</para></param>
        /// <seealso cref="ImageUtils.LoadTextureFromFile(string, TextureFormat)" />
        public static void RegisterSprite(SpriteManager.Group group, string id, string filePathToImage, TextureFormat format = TextureFormat.BC7)
        {
            var sprite = ImageUtils.LoadSpriteFromFile(filePathToImage, format);

            RegisterSprite(group, id, sprite);
        }
    }
}
