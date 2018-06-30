namespace SMLHelper.V2.Handlers
{
    using Patchers;
    using Crafting;
    using System.Collections.Generic;

    /// <summary>
    /// A handler class for creating and editing of crafting trees.
    /// </summary>
    public static class CraftTreeHandler
    {
        /// <summary>
        /// <para>Your first method call to start a new custom crafting tree.</para>
        /// <para>Creating a new CraftTree only makes sense if you're going to use it in a new type of GhostCrafter/Fabricator.</para>
        /// </summary>
        /// <param name="name">The name for the new <see cref="CraftTree.Type" /> enum.</param>
        /// <param name="craftTreeType">The new enum instance for your custom craft tree.</param>
        /// <returns>
        /// <para>The root node for your custom craft tree, as a new <see cref="SmlCraftTreeRoot"/> instance.</para>
        /// <para>Build up your custom crafting tree from this root node.</para>
        /// <para>This tree will automatically patched into the game. No further calls required.</para>
        /// </returns>
        /// <seealso cref="SmlCraftTreeNode"/>
        /// <seealso cref="SmlCraftTreeLinkingNode"/>
        /// <seealso cref="SmlCraftTreeTab"/>
        /// <seealso cref="SmlCraftTreeCraft"/>
        public static SmlCraftTreeRoot CreateCustomCraftTreeAndType(string name, out CraftTree.Type craftTreeType)
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
        public static SmlCraftTreeRoot GetExistingTree(CraftTree.Type Scheme)
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

        /// <summary>
        /// Adds a new crafting node to the root of the specified crafting tree.
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="craftingItem">The item to turn into a crafting node.</param>
        /// <seealso cref="GetExistingTree"/>
        /// <seealso cref="SmlCraftTreeLinkingNode.AddCraftingNode(TechType)"/>
        public static void AddCraftingNodeToRoot(CraftTree.Type craftTree, TechType craftingItem)
        {
            SmlCraftTreeRoot root = GetExistingTree(craftTree);

            if (root == null)
                return;

            root.AddCraftingNode(craftingItem);
        }

        /// <summary>
        /// Adds a collection new crafting nodes to the root of the specified crafting tree.
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="craftingItems">The items to turn into new crafting nodes.</param>
        /// <seealso cref="GetExistingTree"/>
        /// <seealso cref="SmlCraftTreeLinkingNode.AddCraftingNode(TechType[])"/>
        public static void AddCraftingNodeToRoot(CraftTree.Type craftTree, params TechType[] craftingItems)
        {            
            SmlCraftTreeRoot root = GetExistingTree(craftTree);

            if (root == null)
                return;

            root.AddCraftingNode(craftingItems);
        }

        /// <summary>
        /// Adds a new crafting node to the root of the specified crafting tree, at the provided tab location.
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="craftingItem">The item to craft.</param>
        /// <param name="stepsToTab">
        /// <para>The steps to the target tab.</para>
        /// <para>These must match the id value of the CraftNode in the crafting tree you're targeting.</para>
        /// <para>Do not include "root" in this path.</para>
        /// </param>
        /// <seealso cref="GetExistingTree"/>
        /// <seealso cref="SmlCraftTreeRoot.GetTabNode(string[])"/>
        /// <seealso cref="SmlCraftTreeLinkingNode.AddCraftingNode(TechType)"/>
        public static void AddCraftingNodeToTab(CraftTree.Type craftTree, TechType craftingItem, params string[] stepsToTab)
        {
            SmlCraftTreeRoot root = GetExistingTree(craftTree);

            if (root == null)
                return;

            SmlCraftTreeTab tab = root.GetTabNode(stepsToTab);

            if (tab == null)
                return;

            tab.AddCraftingNode(craftingItem);
        }

        /// <summary>
        /// Adds a collection of new crafting nodes to the root of the specified crafting tree, at the provided tab location.
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="craftingItems">The item to craft.</param>
        /// <param name="stepsToTab">
        /// <para>The steps to the target tab.</para>
        /// <para>These must match the id value of the CraftNode in the crafting tree you're targeting.</para>
        /// <para>Do not include "root" in this path.</para>
        /// </param>
        /// <seealso cref="GetExistingTree"/>
        /// <seealso cref="SmlCraftTreeRoot.GetTabNode(string[])"/>
        /// <seealso cref="SmlCraftTreeLinkingNode.AddCraftingNode(IEnumerable{TechType})"/>
        public static void AddCraftingNodeToTab(CraftTree.Type craftTree, IEnumerable<TechType> craftingItems, params string[] stepsToTab)
        {            
            SmlCraftTreeRoot root = GetExistingTree(craftTree);

            if (root == null)
                return;

            SmlCraftTreeTab tab = root.GetTabNode(stepsToTab);

            if (tab == null)
                return;

            tab.AddCraftingNode(craftingItems);
        }
    }
}
