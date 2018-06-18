namespace SMLHelper.V2.Crafting
{
    using Patchers;
    using Util;

    /// <summary>
    /// A class that represents a new item node to be added to an existing tree.
    /// </summary>
    public class CustomCraftNode
    {
        /// <summary>
        /// The item ID of the item to be crafted.
        /// </summary>
        public TechType TechType;

        /// <summary>
        /// The fabricator this node will belong to.
        /// </summary>
        public CraftTree.Type Scheme;

        /// <summary>
        /// The path to where this node should be added.
        /// </summary>
        public string Path;

        internal readonly bool ItemExists;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomCraftNode"/> class for adding a new item node to an existing crafting tree.
        /// </summary>
        /// <param name="techType">The item ID of the item to be crafted.</param>
        /// <param name="scheme">The fabricator this node will belong to.</param>
        /// <param name="path">The path to where this node should be added.</param>
        public CustomCraftNode(TechType techType, CraftTree.Type scheme, string path)
        {
            TechType = techType;
            Scheme = scheme;
            Path = path;
            ItemExists = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomCraftNode"/> class for adding a new node for a modded item to an existing crafting tree.
        /// </summary>
        /// <param name="moddedTechTypeName">The TechType string of the modded item.</param>
        /// <param name="scheme">The fabricator this node will belong to.</param>
        /// <param name="path">The path to where this node should be added.</param>
        public CustomCraftNode(string moddedTechTypeName, CraftTree.Type scheme, string path)
        {            
            Scheme = scheme;
            Path = path;

            EnumTypeCache cache = TechTypePatcher.cacheManager.GetCacheForTypeName(moddedTechTypeName);

            ItemExists = cache != null;

            if (ItemExists)
            {
                TechType = (TechType)cache.Index;
            }
        }
    }
}
