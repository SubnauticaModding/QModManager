namespace SMLHelper.V2.Crafting
{
    /// <summary>
    /// A class that identifies an existing Crafting Tree Node (tab or item) to be removed from the tree.
    /// </summary>
    public class CraftNodeToScrub
    {
        /// <summary>
        /// The fabricator this node belongs to.
        /// </summary>
        public CraftTree.Type Scheme;

        /// <summary>
        /// The path to this node.
        /// </summary>
        public string Path;

        public CraftNodeToScrub(CraftTree.Type scheme, string path)
        {
            Scheme = scheme;
            Path = path;
        }
    }
}
