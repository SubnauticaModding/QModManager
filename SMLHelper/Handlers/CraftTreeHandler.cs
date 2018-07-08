namespace SMLHelper.V2.Handlers
{
    using Patchers;
    using Crafting;
    using System.Collections.Generic;
    using Assets;

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
        /// Adds a new crafting node to the root of the specified crafting tree, at the provided tab location.
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="craftingItem">The item to craft.</param>
        /// <param name="stepsToTab">
        /// <para>The steps to the target tab.</para>
        /// <para>These must match the id value of the CraftNode in the crafting tree you're targeting.</para>
        /// <para>Do not include "root" in this path.</para>
        /// </param>
        /// <seealso cref="ModCraftTreeRoot.GetTabNode(string[])"/>
        /// <seealso cref="ModCraftTreeLinkingNode.AddCraftingNode(TechType)"/>
        public static void AddCraftingNode(CraftTree.Type craftTree, TechType craftingItem, params string[] stepsToTab)
        {
            // Add to game
            CraftTreePatcher.CraftingNodes.Add(new CraftingNode(stepsToTab, craftTree, craftingItem));
        }

        /// <summary>
        /// Adds a new crafting node to the root of the specified crafting tree
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="craftingItem">The item to craft.</param>
        /// <seealso cref="ModCraftTreeRoot.GetTabNode(string[])"/>
        /// <seealso cref="ModCraftTreeLinkingNode.AddCraftingNode(TechType)"/>
        public static void AddCraftingNode(CraftTree.Type craftTree, TechType craftingItem)
        {
            CraftTreePatcher.CraftingNodes.Add(new CraftingNode(new string[0], craftTree, craftingItem));
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
            // Adds sprites and language lines too.
            CraftTreePatcher.TabNodes.Add(new TabNode(new string[0], craftTree, sprite, name, displayName));
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
            CraftTreePatcher.TabNodes.Add(new TabNode(new string[0], craftTree, new Atlas.Sprite(sprite), name, displayName));
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
            CraftTreePatcher.TabNodes.Add(new TabNode(stepsToTab, craftTree, sprite, name, displayName));
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
            CraftTreePatcher.TabNodes.Add(new TabNode(stepsToTab, craftTree, new Atlas.Sprite(sprite), name, displayName));
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
            CraftTreePatcher.NodesToRemove.Add(new Node(stepsToNode, craftTree));
        }
    }
}
