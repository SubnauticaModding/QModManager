namespace SMLHelper.V2.Handlers
{
    using Patchers;
    using Crafting;

    /// <summary>
    /// A handler class for creating and editing of crafting trees.
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
        public static CustomCraftTreeRoot CreateCustomCraftTreeAndType(string name, out CraftTree.Type craftTreeType)
        {
            return CraftTreeTypePatcher.CreateCustomCraftTreeAndType(name, out craftTreeType);
        }

        /// <summary>
        /// <para>Returns a CustomCraftTreeRoot for an existing CraftTree.Type scheme, which you can edit to your liking.</para>
        /// <para>Valid schemes are:</para>
        /// <para>Standard Fabricator:<see cref="CraftTree.Type.Fabricator"/></para>
        /// <para>Cyclops Fabricator:<see cref="CraftTree.Type.CyclopsFabricator"/></para>
        /// <para>Scanner Room Fabricator:<see cref="CraftTree.Type.MapRoom"/></para>
        /// <para>Neptune Rocket Fabricator:<see cref="CraftTree.Type.Rocket"/></para>
        /// <para>Modification Station:<see cref="CraftTree.Type.Workbench"/></para>
        /// <para>Mobile Vehicle Bay:<see cref="CraftTree.Type.Constructor"/></para>
        /// <para>Vehicle Upgrade Console:<see cref="CraftTree.Type.SeamothUpgrades"/></para>
        /// </summary>
        /// <param name="Scheme">The fabricator scheme whose craft tree to get.</param>
        /// <returns>The CustomCraftTreeRoot for the given <see cref="CraftTree.Type"/> when valid; Otherwise returns null.</returns>
        public static CustomCraftTreeRoot GetExistingTree(CraftTree.Type Scheme)
        {
            switch (Scheme)
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
