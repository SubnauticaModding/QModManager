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
        /// <para>The root node for your custom craft tree, as a new <see cref="ModCraftTreeRoot"/> instance.</para>
        /// <para>Build up your custom crafting tree from this root node.</para>
        /// <para>This tree will automatically patched into the game. No further calls required.</para>
        /// </returns>
        /// <seealso cref="ModCraftTreeNode"/>
        /// <seealso cref="ModCraftTreeLinkingNode"/>
        /// <seealso cref="ModCraftTreeTab"/>
        /// <seealso cref="ModCraftTreeCraft"/>
        public static ModCraftTreeRoot CreateCustomCraftTreeAndType(string name, out CraftTree.Type craftTreeType)
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
        public static ModCraftTreeRoot GetExistingTree(CraftTree.Type Scheme)
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
        /// <seealso cref="ModCraftTreeRoot.GetTabNode(string[])"/>
        /// <seealso cref="ModCraftTreeLinkingNode.AddCraftingNode(TechType)"/>
        public static void AddCraftingNode(CraftTree.Type craftTree, TechType craftingItem, params string[] stepsToTab)
        {
            ModCraftTreeRoot root = GetExistingTree(craftTree);

            if (root == null)
                return;

            ModCraftTreeTab tab = root.GetTabNode(stepsToTab);

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
        /// <seealso cref="ModCraftTreeRoot.GetTabNode(string[])"/>
        /// <seealso cref="ModCraftTreeLinkingNode.AddCraftingNode(IEnumerable{TechType})"/>
        public static void AddCraftingNode(CraftTree.Type craftTree, IEnumerable<TechType> craftingItems, params string[] stepsToTab)
        {            
            ModCraftTreeRoot root = GetExistingTree(craftTree);

            if (root == null)
                return;

            ModCraftTreeTab tab = root.GetTabNode(stepsToTab);

            if (tab == null)
                return;

            tab.AddCraftingNode(craftingItems);
        }

        /// <summary>
        /// Adds a new crafting node to the root of the specified crafting tree
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="craftingItem">The item to craft.</param>
        /// <seealso cref="GetExistingTree"/>
        /// <seealso cref="ModCraftTreeRoot.GetTabNode(string[])"/>
        /// <seealso cref="ModCraftTreeLinkingNode.AddCraftingNode(TechType)"/>
        public static void AddCraftingNode(CraftTree.Type craftTree, TechType craftingItem)
        {
            ModCraftTreeRoot root = GetExistingTree(craftTree);

            if (root == null)
                return;

            root.AddCraftingNode(craftingItem);
        }

        /// <summary>
        /// Adds a collection of new crafting nodes to the root of the specified crafting tree.
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="craftingItems">The item to craft.</param>
        /// <seealso cref="GetExistingTree"/>
        /// <seealso cref="ModCraftTreeRoot.GetTabNode(string[])"/>
        /// <seealso cref="ModCraftTreeLinkingNode.AddCraftingNode(IEnumerable{TechType})"/>
        public static void AddCraftingNode(CraftTree.Type craftTree, IEnumerable<TechType> craftingItems)
        {
            ModCraftTreeRoot root = GetExistingTree(craftTree);

            if (root == null)
                return;

            root.AddCraftingNode(craftingItems);
        }

        /// <summary>
        /// Adds a new tab node to the root of the specified crafting tree.
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="name">The ID of the tab node. Must be unique!</param>
        /// <param name="displayName">The display name of the tab, which will show up when you hover your mouse on the tab.</param>
        /// <param name="sprite">The sprite of the tab.</param>
        public static void AddTabNode(CraftTree.Type craftTree, string name, string displayName, Atlas.Sprite sprite)
        {
            ModCraftTreeRoot root = GetExistingTree(craftTree);

            if (root == null)
                return;

            root.AddTabNode(name, displayName, sprite);
        }

        /// <summary>
        /// Adds a new tab node to the root of the specified crafting tree.
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="name">The ID of the tab node. Must be unique!</param>
        /// <param name="displayName">The display name of the tab, which will show up when you hover your mouse on the tab.</param>
        /// <param name="sprite">The sprite of the tab.</param>
        public static void AddTabNode(CraftTree.Type craftTree, string name, string displayName, UnityEngine.Sprite sprite)
        {
            ModCraftTreeRoot root = GetExistingTree(craftTree);

            if (root == null)
                return;

            root.AddTabNode(name, displayName, sprite);
        }

        /// <summary>
        /// Adds a new tab node to the root of the specified crafting tree, at the specified tab location.
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="name">The ID of the tab node. Must be unique!</param>
        /// <param name="displayName">The display name of the tab, which will show up when you hover your mouse on the tab.</param>
        /// <param name="sprite">The sprite of the tab.</param>
        /// <param name="stepsToTab">
        /// <para>The steps to the target tab.</para>
        /// <para>These must match the id value of the CraftNode in the crafting tree you're targeting.</para>
        /// <para>Do not include "root" in this path.</para>
        /// </param>
        public static void AddTabNode(CraftTree.Type craftTree, string name, string displayName, Atlas.Sprite sprite, params string[] stepsToTab)
        {
            ModCraftTreeRoot root = GetExistingTree(craftTree);

            if (root == null)
                return;

            ModCraftTreeTab tab = root.GetTabNode(stepsToTab);

            if (tab == null)
                return;

            tab.AddTabNode(name, displayName, sprite);
        }

        /// <summary>
        /// Adds a new tab node to the root of the specified crafting tree, at the specified tab location.
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="name">The ID of the tab node. Must be unique!</param>
        /// <param name="displayName">The display name of the tab, which will show up when you hover your mouse on the tab.</param>
        /// <param name="sprite">The sprite of the tab.</param>
        /// <param name="stepsToTab">
        /// <para>The steps to the target tab.</para>
        /// <para>These must match the id value of the CraftNode in the crafting tree you're targeting.</para>
        /// <para>Do not include "root" in this path.</para>
        /// </param>
        public static void AddTabNode(CraftTree.Type craftTree, string name, string displayName, UnityEngine.Sprite sprite, params string[] stepsToTab)
        {
            ModCraftTreeRoot root = GetExistingTree(craftTree);

            if (root == null)
                return;

            ModCraftTreeTab tab = root.GetTabNode(stepsToTab);

            if (tab == null)
                return;

            tab.AddTabNode(name, displayName, sprite);
        }

        /// <summary>
        /// Removes a node at the specified node location. Can be used with both tabs and normal craft nodes.
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="stepsToNode">
        /// <para>The steps to the target node.</para>
        /// <para>These must match the id value of the CraftNode in the crafting tree you're targeting.</para>
        /// <para>Do not include "root" in this path.</para>
        /// </param>
        public static void RemoveNode(CraftTree.Type craftTree, params string[] stepsToNode)
        {
            ModCraftTreeRoot root = GetExistingTree(craftTree);

            if (root == null)
                return;

            ModCraftTreeNode node = root.GetNode(stepsToNode);

            if (node == null)
                return;

            node.RemoveNode();
        }
    }
}
