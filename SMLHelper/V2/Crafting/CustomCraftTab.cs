namespace SMLHelper.V2.Crafting
{
    using Patchers;

    /// <summary>
    /// A class that represents a new tab node to be added to an existing tree.
    /// </summary>
    public class CustomCraftTab
    {
        public string SpriteId
        {
            get
            {
                return Scheme.ToString() + "_" + System.IO.Path.GetFileName(Path);
            }
        }

        public string LanguageId
        {
            get
            {
                return Scheme.ToString() + "Menu_" + System.IO.Path.GetFileName(Path);
            }
        }

        /// <summary>
        /// The fabricator this node will belong to.
        /// </summary>
        public CraftTree.Type Scheme;

        /// <summary>
        /// The path to where this node should be added.
        /// </summary>
        public string Path;

        /// <summary>
        /// The name ID for this tab node.
        /// </summary>
        public string Name;

        /// <summary>
        /// The sprite to be used on this tab node.
        /// </summary>
        public CustomSprite Sprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomCraftTab"/> class for adding a new tab node to an existing crafting tree.
        /// </summary>
        /// <param name="path">The path to where this node should be added.</param>
        /// <param name="name">The name for this tab node.</param>
        /// <param name="scheme">The fabricator this node will belong to.</param>
        /// <param name="sprite">The sprite to be used on this tab node.</param>
        public CustomCraftTab(string path, string name, CraftTree.Type scheme, Atlas.Sprite sprite)
        {
            Path = path;
            Name = name;
            Scheme = scheme;
            Sprite = new CustomSprite(SpriteManager.Group.Category, SpriteId, sprite);

            LanguagePatcher.customLines[LanguageId] = name;
            CustomSpriteHandler.customSprites.Add(Sprite);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomCraftTab"/> class for adding a new tab node to an existing crafting tree.
        /// </summary>
        /// <param name="path">The path to where this node should be added.</param>
        /// <param name="name">The name for this tab node.</param>
        /// <param name="scheme">The fabricator this node will belong to.</param>
        /// <param name="sprite">The sprite to be used on this tab node.</param>
        public CustomCraftTab(string path, string name, CraftTree.Type scheme, UnityEngine.Sprite sprite) : this(path, name, scheme, new Atlas.Sprite(sprite, false))
        {
        }
    }
}