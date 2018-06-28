namespace SMLHelper.V2.Handlers
{
    using System;
    using System.IO;
    using Assets;
    using UnityEngine;
    using Logger = V2.Logger;

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
        public static void RegisterSprite(SpriteManager.Group group, string id, Sprite sprite)
        {
            ModSprite.Sprites.Add(new ModSprite(group, id, sprite));
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="type">The techtype paired to this sprite.</param>
        /// <param name="sprite">The sprite to be added.</param>
        public static void RegisterSprite(TechType type, Sprite sprite)
        {
            ModSprite.Sprites.Add(new ModSprite(type, sprite));
        }

        /// <summary>
        /// Registers a new sprite to the game.
        /// </summary>
        /// <param name="type">The techtype paired to this sprite.</param>
        /// <param name="filePathToImage">The file path to image to be converted into a sprite.</param>
        /// <param name="format"><para>The texture format. By default, this uses <see cref="TextureFormat.BC7" />.</para>
        /// <para>https://docs.unity3d.com/ScriptReference/TextureFormat.BC7.html</para>
        /// <para>Don't change this unless you really know what you're doing.</para></param>
        /// <seealso cref="LoadTextureFromFile(string, TextureFormat)" />
        public static void RegisterSprite(TechType type, string filePathToImage, TextureFormat format = TextureFormat.BC7)
        {
            Texture2D textureFromImage = LoadTextureFromFile(filePathToImage, format);

            if (textureFromImage == null)
                return;
            
            var sprite = new Atlas.Sprite(textureFromImage);

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
        /// <seealso cref="LoadTextureFromFile(string, TextureFormat)" />
        public static void RegisterSprite(SpriteManager.Group group, string id, string filePathToImage, TextureFormat format = TextureFormat.BC7)
        {
            Texture2D textureFromImage = LoadTextureFromFile(filePathToImage, format);

            if (textureFromImage == null)
                return;

            var sprite = new Atlas.Sprite(textureFromImage);

            RegisterSprite(group, id, sprite);
        }
        
        /// <summary>
        /// Creates a new <see cref="Texture2D" /> from an image file.
        /// </summary>
        /// <param name="filePathToImage">The path to the image file.</param>
        /// <param name="format">
        /// <para>The texture format. By default, this uses <see cref="TextureFormat.BC7" />.</para>
        /// <para>https://docs.unity3d.com/ScriptReference/TextureFormat.BC7.html</para>
        /// <para>Don't change this unless you really know what you're doing.</para>
        /// </param>
        /// <returns>Will return a new <see cref="Texture2D"/> instance if the file exists; Otherwise returns null.</returns>
        /// <remarks>
        /// Ripped from: https://github.com/RandyKnapp/SubnauticaModSystem/blob/master/SubnauticaModSystem/Common/Utility/ImageUtils.cs
        /// </remarks>
        public static Texture2D LoadTextureFromFile(string filePathToImage, TextureFormat format = TextureFormat.BC7)
        {
            if (File.Exists(filePathToImage))
            {
                byte[] imageBytes = File.ReadAllBytes(filePathToImage);
                Texture2D texture2D = new Texture2D(2, 2, format, false);
                if (texture2D.LoadImage(imageBytes))
                {
                    return texture2D;
                }
            }
            else
            {
                Logger.Log("ERROR on LoadTextureFromFile call. File not found at " + filePathToImage);
            }

            return null;
        }
    }
}
