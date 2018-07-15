namespace SMLHelper.V2.Handlers
{
    using Crafting;
    using Patchers;
    using Utility;
    using UnityEngine.Assertions;

    /// <summary>
    /// A handler class for creating and editing of crafting trees.
    /// </summary>
    public static class CraftTreeHandler
    {
        /// <summary>
        /// <para>Your first method call to start a new custom crafting tree.</para>
        /// <para>Creating a new CraftTree only makes sense if you're going to use it in a new type of <see cref="GhostCrafter"/>.</para>
        /// </summary>
        /// <param name="name">The name for the new <see cref="CraftTree.Type" /> enum.</param>
        /// <param name="craftTreeType">The new enum instance for your custom craft tree type.</param>
        /// <returns>
        /// <para>Returns the root node for your custom craft tree, as a new <see cref="ModCraftTreeRoot"/> instance.</para>
        /// <para>Build up your custom crafting tree from this root node.</para>
        /// <para>This tree will be automatically patched into the game. No further calls into <see cref="CraftTreeHandler"/> required.</para>
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
        /// Safely looks for a modded CraftTree Type from another mod in the SMLHelper CraftTreeTypeCache.
        /// </summary>
        /// <param name="craftTreeString">The string used to define the modded item's new techtype.</param>
        /// <returns>
        ///   <c>True</c> if the craft tree was found; Otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// There's no guarantee in which order SMLHelper dependent mods are loaded,
        /// so if two mods are added at the same time, it may take a second game load for both to be visible to each other.
        /// </remarks>
        public static bool ModdedCraftTreeTypeExists(string craftTreeString)
        {
            EnumTypeCache cache = CraftTreeTypePatcher.cacheManager.GetCacheForTypeName(craftTreeString);
            return cache != null;
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
        /// <exception cref="AssertionException">This method is intended for use only with standard crafting trees, not custom ones.</exception>
        public static void AddCraftingNode(CraftTree.Type craftTree, TechType craftingItem, params string[] stepsToTab)
        {
            Assert.IsTrue(craftTree <= CraftTree.Type.Rocket, $"{nameof(AddCraftingNode)} is intended for use only with standard crafting trees, not custom ones.");
            CraftTreePatcher.CraftingNodes.Add(new CraftingNode(stepsToTab, craftTree, craftingItem));
        }

        /// <summary>
        /// Adds a new crafting node to the root of the specified crafting tree
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="craftingItem">The item to craft.</param>
        /// <exception cref="AssertionException">This method is intended for use only with standard crafting trees, not custom ones.</exception>
        public static void AddCraftingNode(CraftTree.Type craftTree, TechType craftingItem)
        {
            Assert.IsTrue(craftTree <= CraftTree.Type.Rocket, $"{nameof(AddCraftingNode)} is intended for use only with standard crafting trees, not custom ones.");
            CraftTreePatcher.CraftingNodes.Add(new CraftingNode(new string[0], craftTree, craftingItem));
        }

        /// <summary>
        /// Adds a new tab node to the root of the specified crafting tree.
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="name">The ID of the tab node. Must be unique!</param>
        /// <param name="displayName">The display name of the tab, which will show up when you hover your mouse on the tab.</param>
        /// <param name="sprite">The sprite of the tab.</param>
        /// <exception cref="AssertionException">This method is intended for use only with standard crafting trees, not custom ones.</exception>
        public static void AddTabNode(CraftTree.Type craftTree, string name, string displayName, Atlas.Sprite sprite)
        {
            Assert.IsTrue(craftTree <= CraftTree.Type.Rocket, $"{nameof(AddTabNode)} is intended for use only with standard crafting trees, not custom ones.");
            CraftTreePatcher.TabNodes.Add(new TabNode(new string[0], craftTree, sprite, name, displayName));
        }

        /// <summary>
        /// Adds a new tab node to the root of the specified crafting tree.
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="name">The ID of the tab node. Must be unique!</param>
        /// <param name="displayName">The display name of the tab, which will show up when you hover your mouse on the tab.</param>
        /// <param name="sprite">The sprite of the tab.</param>
        /// <exception cref="AssertionException">This method is intended for use only with standard crafting trees, not custom ones.</exception>
        public static void AddTabNode(CraftTree.Type craftTree, string name, string displayName, UnityEngine.Sprite sprite)
        {
            Assert.IsTrue(craftTree <= CraftTree.Type.Rocket, $"{nameof(AddTabNode)} is intended for use only with standard crafting trees, not custom ones.");
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
        /// <exception cref="AssertionException">This method is intended for use only with standard crafting trees, not custom ones.</exception>
        public static void AddTabNode(CraftTree.Type craftTree, string name, string displayName, Atlas.Sprite sprite, params string[] stepsToTab)
        {
            Assert.IsTrue(craftTree <= CraftTree.Type.Rocket, $"{nameof(AddTabNode)} is intended for use only with standard crafting trees, not custom ones.");
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
        /// <exception cref="AssertionException">This method is intended for use only with standard crafting trees, not custom ones.</exception>
        public static void AddTabNode(CraftTree.Type craftTree, string name, string displayName, UnityEngine.Sprite sprite, params string[] stepsToTab)
        {
            Assert.IsTrue(craftTree <= CraftTree.Type.Rocket, $"{nameof(AddTabNode)} is intended for use only with standard crafting trees, not custom ones.");
            CraftTreePatcher.TabNodes.Add(new TabNode(stepsToTab, craftTree, new Atlas.Sprite(sprite), name, displayName));
        }

        /// <summary>
        /// <para>Removes a node at the specified node location. Can be used to remove either tabs or craft nodes.</para>
        /// <para>If a tab node is selected, all child nodes to it will also be removed.</para>
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="stepsToNode">
        /// <para>The steps to the target node.</para>
        /// <para>These must match the id value of the CraftNode in the crafting tree you're targeting.</para>
        /// <para>This means matching the id of the crafted item or the id of the tab name.</para>
        /// <para>Do not include "root" in this path.</para>
        /// </param>
        /// <exception cref="AssertionException">This method is intended for use only with standard crafting trees, not custom ones.</exception>
        public static void RemoveNode(CraftTree.Type craftTree, params string[] stepsToNode)
        {
            Assert.IsTrue(craftTree <= CraftTree.Type.Rocket, $"{nameof(RemoveNode)} is intended for use only with standard crafting trees, not custom ones.");
            CraftTreePatcher.NodesToRemove.Add(new Node(stepsToNode, craftTree));
        }
    }
}
