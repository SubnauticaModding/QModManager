namespace SMLHelper.V2.Handlers
{
    using Patchers;

    /// <summary>
    /// A class for handling creating and editing of crafting trees.
    /// </summary>
    public class CraftTreeHandler
    {
        /// <summary>
        /// Your first method call to start a new custom crafting tree.
        /// Creating a new CraftTree only makes sense if you're going to use it in a new type of GhostCrafter/Fabricator.
        /// </summary>
        /// <param name="name">The name for the new <see cref="CraftTree.Type" /> enum.</param>
        /// <param name="craftTreeType">The new enum instance for your custom craft tree.</param>
        /// <returns>A new root node for your custom craft tree.</returns>
        /// <remarks>This node is automatically assigned to <see cref="CraftTreePatcher.CustomTrees" />.</remarks>
        public static Crafting.CustomCraftTreeRoot CreateCustomCraftTreeAndType(string name, out CraftTree.Type craftTreeType)
        {
            return CraftTreeTypePatcher.CreateCustomCraftTreeAndType(name, out craftTreeType);
        }

        /// <summary>
        /// Returns a CustomCraftTreeRoot for an existing CraftTree.Type scheme, which you can edit to your liking.
        /// </summary>
        /// <param name="Scheme">The scheme whose craft tree to get.</param>
        /// <returns>The CustomCraftTreeRoot for that scheme.</returns>
        public static Crafting.CustomCraftTreeRoot GetExistingTree(CraftTree.Type Scheme)
        {
            switch(Scheme)
            {
                case CraftTree.Type.Fabricator:
                    return CraftTreePatcher.FabricatorTree;

                case CraftTree.Type.CyclopsFabricator:
                    return CraftTreePatcher.CyclopsFabricatorTree;

                case CraftTree.Type.MapRoom:
                    return CraftTreePatcher.MapRoomTree;

                case CraftTree.Type.Rocket:
                    return CraftTreePatcher.RocketTree;

                case CraftTree.Type.Workbench:
                    return CraftTreePatcher.WorkbenchTree;

                case CraftTree.Type.Constructor:
                    return CraftTreePatcher.ConstructorTree;

                case CraftTree.Type.SeamothUpgrades:
                    return CraftTreePatcher.SeamothUpgradesTree;
            }

            return null;
        }
    }
}
