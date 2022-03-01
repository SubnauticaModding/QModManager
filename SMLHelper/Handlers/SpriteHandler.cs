namespace SMLHelper.V2.Handlers
{
    using Assets;
    using Interfaces;
    using UnityEngine;
    using Utility;

    /// <summary>
    /// A handler class for adding custom sprites into the game.
    /// </summary>
    public class SpriteHandler : ISpriteHandler
    {
        /// <summary>
        /// Main entry point for all calls to this handler.
        /// </summary>
        public static ISpriteHandler Main { get; } = new SpriteHandler();

        private SpriteHandler()
        {
            // Hide constructor
        }

        #region Static Methods

#if SUBNAUTICA

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="type">The techtype paired to this sprite.</param>
        /// <param name="sprite">The sprite to be added.</param>
        public static void RegisterSprite(TechType type, Atlas.Sprite sprite)
        {
            Main.RegisterSprite(type, sprite);
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="group">The sprite group.</param>
        /// <param name="id">The sprite internal identifier.</param>
        /// <param name="sprite">The sprite to be added.</param>
        public static void RegisterSprite(SpriteManager.Group group, string id, Atlas.Sprite sprite)
        {
            Main.RegisterSprite(group, id, sprite);
        }
#endif
        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="group">The sprite group this sprite will be added to.</param>
        /// <param name="id">The sprite internal identifier.</param>
        /// <param name="sprite">The sprite to be added.</param>
        public static void RegisterSprite(SpriteManager.Group group, string id, Sprite sprite)
        {
            Main.RegisterSprite(group, id, sprite);
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="type">The techtype paired to this sprite.</param>
        /// <param name="sprite">The sprite to be added.</param>
        public static void RegisterSprite(TechType type, Sprite sprite)
        {
            Main.RegisterSprite(type, sprite);
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="type">The techtype paired to this sprite.</param>
        /// <param name="filePathToImage">The file path to image to be converted into a sprite.</param>
        /// <seealso cref="ImageUtils.LoadSpriteFromFile(string, TextureFormat)" />
        public static void RegisterSprite(TechType type, string filePathToImage)
        {
            Main.RegisterSprite(type, filePathToImage);
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="type">The techtype paired to this sprite.</param>
        /// <param name="filePathToImage">The file path to image to be converted into a sprite.</param>
        /// <param name="format"><para>The texture format. By default, this uses <see cref="TextureFormat.BC7" />.</para>
        /// <para>https://docs.unity3d.com/ScriptReference/TextureFormat.BC7.html</para>
        /// <para>Don't change this unless you really know what you're doing.</para></param>
        /// <seealso cref="ImageUtils.LoadSpriteFromFile(string, TextureFormat)" />
        public static void RegisterSprite(TechType type, string filePathToImage, TextureFormat format)
        {
            Main.RegisterSprite(type, filePathToImage, format);
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="group">The sprite group.</param>
        /// <param name="id">The sprite internal identifier.</param>
        /// <param name="filePathToImage">The file path to image.</param>
        /// <seealso cref="ImageUtils.LoadSpriteFromFile(string, TextureFormat)" />
        public static void RegisterSprite(SpriteManager.Group group, string id, string filePathToImage)
        {
            Main.RegisterSprite(group, id, filePathToImage);
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
        /// <seealso cref="ImageUtils.LoadSpriteFromFile(string, TextureFormat)" />
        public static void RegisterSprite(SpriteManager.Group group, string id, string filePathToImage, TextureFormat format)
        {
            Main.RegisterSprite(group, id, filePathToImage, format);
        }

        #endregion

        #region Interface Methods

#if SUBNAUTICA

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="type">The techtype paired to this sprite.</param>
        /// <param name="sprite">The sprite to be added.</param>
        void ISpriteHandler.RegisterSprite(TechType type, Atlas.Sprite sprite)
        {
            ModSprite.Add(SpriteManager.Group.None, type.AsString(), sprite);
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="group">The sprite group.</param>
        /// <param name="id">The sprite internal identifier.</param>
        /// <param name="sprite">The sprite to be added.</param>
        void ISpriteHandler.RegisterSprite(SpriteManager.Group group, string id, Atlas.Sprite sprite)
        {
            ModSprite.Add(group, id, sprite);
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="group">The sprite group this sprite will be added to.</param>
        /// <param name="id">The sprite internal identifier.</param>
        /// <param name="sprite">The sprite to be added.</param>
        void ISpriteHandler.RegisterSprite(SpriteManager.Group group, string id, Sprite sprite)
        {
            ModSprite.Add(group, id, new Atlas.Sprite(sprite));
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="type">The techtype paired to this sprite.</param>
        /// <param name="sprite">The sprite to be added.</param>
        void ISpriteHandler.RegisterSprite(TechType type, Sprite sprite)
        {
            ModSprite.Add(SpriteManager.Group.None, type.AsString(), new Atlas.Sprite(sprite));
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="type">The techtype paired to this sprite.</param>
        /// <param name="filePathToImage">The file path to image to be converted into a sprite.</param>
        /// <seealso cref="ImageUtils.LoadSpriteFromFile(string, TextureFormat)" />
        void ISpriteHandler.RegisterSprite(TechType type, string filePathToImage)
        {
            Atlas.Sprite sprite = ImageUtils.LoadSpriteFromFile(filePathToImage, TextureFormat.BC7);

            Main.RegisterSprite(type, sprite);
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="type">The techtype paired to this sprite.</param>
        /// <param name="filePathToImage">The file path to image to be converted into a sprite.</param>
        /// <param name="format"><para>The texture format. By default, this uses <see cref="TextureFormat.BC7" />.</para>
        /// <para>https://docs.unity3d.com/ScriptReference/TextureFormat.BC7.html</para>
        /// <para>Don't change this unless you really know what you're doing.</para></param>
        /// <seealso cref="ImageUtils.LoadSpriteFromFile(string, TextureFormat)" />
        void ISpriteHandler.RegisterSprite(TechType type, string filePathToImage, TextureFormat format)
        {
            Atlas.Sprite sprite = ImageUtils.LoadSpriteFromFile(filePathToImage, format);

            Main.RegisterSprite(type, sprite);
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="group">The sprite group.</param>
        /// <param name="id">The sprite internal identifier.</param>
        /// <param name="filePathToImage">The file path to image.</param>
        /// <seealso cref="ImageUtils.LoadSpriteFromFile(string, TextureFormat)" />
        void ISpriteHandler.RegisterSprite(SpriteManager.Group group, string id, string filePathToImage)
        {
            Atlas.Sprite sprite = ImageUtils.LoadSpriteFromFile(filePathToImage, TextureFormat.BC7);

            Main.RegisterSprite(group, id, sprite);
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
        /// <seealso cref="ImageUtils.LoadSpriteFromFile(string, TextureFormat)" />
        void ISpriteHandler.RegisterSprite(SpriteManager.Group group, string id, string filePathToImage, TextureFormat format)
        {
            Atlas.Sprite sprite = ImageUtils.LoadSpriteFromFile(filePathToImage, format);

            Main.RegisterSprite(group, id, sprite);
        }

#elif BELOWZERO
        
        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="group">The sprite group this sprite will be added to.</param>
        /// <param name="id">The sprite internal identifier.</param>
        /// <param name="sprite">The sprite to be added.</param>
        void ISpriteHandler.RegisterSprite(SpriteManager.Group group, string id, Sprite sprite)
        {
            ModSprite.Add(group, id, sprite);
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="type">The techtype paired to this sprite.</param>
        /// <param name="sprite">The sprite to be added.</param>
        void ISpriteHandler.RegisterSprite(TechType type, Sprite sprite)
        {
            ModSprite.Add(SpriteManager.Group.None, type.AsString(), sprite);
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="type">The techtype paired to this sprite.</param>
        /// <param name="filePathToImage">The file path to image to be converted into a sprite.</param>
        /// <seealso cref="ImageUtils.LoadSpriteFromFile(string, TextureFormat)" />
        void ISpriteHandler.RegisterSprite(TechType type, string filePathToImage)
        {
            Sprite sprite = ImageUtils.LoadSpriteFromFile(filePathToImage, TextureFormat.BC7);

            Main.RegisterSprite(type, sprite);
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="type">The techtype paired to this sprite.</param>
        /// <param name="filePathToImage">The file path to image to be converted into a sprite.</param>
        /// <param name="format"><para>The texture format. By default, this uses <see cref="TextureFormat.BC7" />.</para>
        /// <para>https://docs.unity3d.com/ScriptReference/TextureFormat.BC7.html</para>
        /// <para>Don't change this unless you really know what you're doing.</para></param>
        /// <seealso cref="ImageUtils.LoadSpriteFromFile(string, TextureFormat)" />
        void ISpriteHandler.RegisterSprite(TechType type, string filePathToImage, TextureFormat format)
        {
            Sprite sprite = ImageUtils.LoadSpriteFromFile(filePathToImage, format);

            Main.RegisterSprite(type, sprite);
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="group">The sprite group.</param>
        /// <param name="id">The sprite internal identifier.</param>
        /// <param name="filePathToImage">The file path to image.</param>
        /// <seealso cref="ImageUtils.LoadSpriteFromFile(string, TextureFormat)" />
        void ISpriteHandler.RegisterSprite(SpriteManager.Group group, string id, string filePathToImage)
        {
            Sprite sprite = ImageUtils.LoadSpriteFromFile(filePathToImage, TextureFormat.BC7);

            Main.RegisterSprite(group, id, sprite);
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
        /// <seealso cref="ImageUtils.LoadSpriteFromFile(string, TextureFormat)" />
        void ISpriteHandler.RegisterSprite(SpriteManager.Group group, string id, string filePathToImage, TextureFormat format)
        {
            Sprite sprite = ImageUtils.LoadSpriteFromFile(filePathToImage, format);

            Main.RegisterSprite(group, id, sprite);
        }


#endif
        #endregion
    }
}
