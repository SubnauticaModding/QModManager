namespace QModManager.API.SMLHelper.Interfaces
{
    using Crafting;
    using Handlers;
    using UnityEngine;

    /// <summary>
    /// Interface for <see cref="CraftTreeHandler"/> <para/>
    /// Can be used for dependency injection
    /// </summary>
    public interface ICraftTreeHandler
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
        /// <para>This tree will be automatically patched into the game. No further calls into <see cref="ICraftTreeHandler"/> required.</para>
        /// </returns>
        /// <seealso cref="ModCraftTreeNode"/>
        /// <seealso cref="ModCraftTreeLinkingNode"/>
        /// <seealso cref="ModCraftTreeTab"/>
        /// <seealso cref="ModCraftTreeCraft"/>
        ModCraftTreeRoot CreateCustomCraftTreeAndType(string name, out CraftTree.Type craftTreeType);

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
        bool ModdedCraftTreeTypeExists(string craftTreeString);

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
        void AddCraftingNode(CraftTree.Type craftTree, TechType craftingItem, params string[] stepsToTab);

        /// <summary>
        /// Adds a new crafting node to the root of the specified crafting tree
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="craftingItem">The item to craft.</param>        
        void AddCraftingNode(CraftTree.Type craftTree, TechType craftingItem);

        /// <summary>
        /// Adds a new tab node to the root of the specified crafting tree.
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="name">The ID of the tab node. Must be unique!</param>
        /// <param name="displayName">The display name of the tab, which will show up when you hover your mouse on the tab.</param>
        /// <param name="sprite">The sprite of the tab.</param>        
        void AddTabNode(CraftTree.Type craftTree, string name, string displayName, Atlas.Sprite sprite);

        /// <summary>
        /// Adds a new tab node to the root of the specified crafting tree.
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="name">The ID of the tab node. Must be unique!</param>
        /// <param name="displayName">The display name of the tab, which will show up when you hover your mouse on the tab.</param>
        /// <param name="sprite">The sprite of the tab.</param>        
        void AddTabNode(CraftTree.Type craftTree, string name, string displayName, Sprite sprite);

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
        void AddTabNode(CraftTree.Type craftTree, string name, string displayName, Atlas.Sprite sprite, params string[] stepsToTab);

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
        void AddTabNode(CraftTree.Type craftTree, string name, string displayName, Sprite sprite, params string[] stepsToTab);

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
        void RemoveNode(CraftTree.Type craftTree, params string[] stepsToNode);
    }
}
