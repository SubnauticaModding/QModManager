using CraftTreeTypePatcher2 = SMLHelper.V2.Patchers.CraftTreeTypePatcher;

namespace SMLHelper.Patchers
{
    public class CraftTreeTypePatcher
    {
        /// <summary>
        /// Your first method call to start a new custom crafting tree.
        /// Creating a new CraftTree only makes sense if you're going to use it in a new type of GhostCrafter/Fabricator.
        /// </summary>
        /// <param name="name">The name for the new <see cref="CraftTree.Type"/> enum.</param>
        /// <param name="cratfTreeType">The new enum instance for your custom craft tree.</param>
        /// <returns>A new root node for your custom craft tree.</returns>
        /// <remarks>This node is automatically assigned to <see cref="CraftTreePatcher.CustomTrees"/>.</remarks>
        public static CustomCraftTreeRoot CreateCustomCraftTreeAndType(string name, out CraftTree.Type cratfTreeType)
        {
            return CraftTreeTypePatcher2.CreateCustomCraftTreeAndType(name, out cratfTreeType).GetV1RootNode();
        }

    }
}
