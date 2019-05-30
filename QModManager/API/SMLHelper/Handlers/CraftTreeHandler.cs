namespace QModManager.API.SMLHelper.Handlers
{
    using Crafting;
    using Interfaces;
    using Patchers;
    using Utility;
    using System;
    using UnityEngine;
    using QModManager.Utility;

    /// <summary>
    /// A handler class for creating and editing of crafting trees.
    /// </summary>
    public class CraftTreeHandler : ICraftTreeHandler
    {
        /// <summary>
        /// Main entry point for all calls to this handler.
        /// </summary>
        public static ICraftTreeHandler Main { get; } = new CraftTreeHandler();

        private CraftTreeHandler() { }

        #region Static Methods

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
            return Main.CreateCustomCraftTreeAndType(name, out craftTreeType);
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
            return Main.ModdedCraftTreeTypeExists(craftTreeString);
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
        public static void AddCraftingNode(CraftTree.Type craftTree, TechType craftingItem, params string[] stepsToTab)
        {
            Main.AddCraftingNode(craftTree, craftingItem, stepsToTab);
        }

        /// <summary>
        /// Adds a new crafting node to the root of the specified crafting tree
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="craftingItem">The item to craft.</param>        
        public static void AddCraftingNode(CraftTree.Type craftTree, TechType craftingItem)
        {
            Main.AddCraftingNode(craftTree, craftingItem);
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
            Main.AddTabNode(craftTree, name, displayName, sprite);
        }

        /// <summary>
        /// Adds a new tab node to the root of the specified crafting tree.
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="name">The ID of the tab node. Must be unique!</param>
        /// <param name="displayName">The display name of the tab, which will show up when you hover your mouse on the tab.</param>
        /// <param name="sprite">The sprite of the tab.</param>        
        public static void AddTabNode(CraftTree.Type craftTree, string name, string displayName, Sprite sprite)
        {
            Main.AddTabNode(craftTree, name, displayName, sprite);
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
            Main.AddTabNode(craftTree, name, displayName, sprite, stepsToTab);
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
        public static void AddTabNode(CraftTree.Type craftTree, string name, string displayName, Sprite sprite, params string[] stepsToTab)
        {
            Main.AddTabNode(craftTree, name, displayName, sprite, stepsToTab);
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
        public static void RemoveNode(CraftTree.Type craftTree, params string[] stepsToNode)
        {
            Main.RemoveNode(craftTree, stepsToNode);
        }

        #endregion

        #region Interface Methods

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
        ModCraftTreeRoot ICraftTreeHandler.CreateCustomCraftTreeAndType(string name, out CraftTree.Type craftTreeType)
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
        bool ICraftTreeHandler.ModdedCraftTreeTypeExists(string craftTreeString)
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
        void ICraftTreeHandler.AddCraftingNode(CraftTree.Type craftTree, TechType craftingItem, params string[] stepsToTab)
        {
            ValidateStandardCraftTree(craftTree);
            CraftTreePatcher.CraftingNodes.Add(new CraftingNode(stepsToTab, craftTree, craftingItem));
        }

        /// <summary>
        /// Adds a new crafting node to the root of the specified crafting tree
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="craftingItem">The item to craft.</param>
        void ICraftTreeHandler.AddCraftingNode(CraftTree.Type craftTree, TechType craftingItem)
        {
            ValidateStandardCraftTree(craftTree);
            CraftTreePatcher.CraftingNodes.Add(new CraftingNode(new string[0], craftTree, craftingItem));
        }

        /// <summary>
        /// Adds a new tab node to the root of the specified crafting tree.
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="name">The ID of the tab node. Must be unique!</param>
        /// <param name="displayName">The display name of the tab, which will show up when you hover your mouse on the tab.</param>
        /// <param name="sprite">The sprite of the tab.</param>        
        void ICraftTreeHandler.AddTabNode(CraftTree.Type craftTree, string name, string displayName, Atlas.Sprite sprite)
        {
            ValidateStandardCraftTree(craftTree);
            string modName = ReflectionHelper.CallingAssemblyByStackTrace().GetName().Name;

            CraftTreePatcher.TabNodes.Add(new TabNode(new string[0], craftTree, sprite, modName, name, displayName));
        }

        /// <summary>
        /// Adds a new tab node to the root of the specified crafting tree.
        /// </summary>
        /// <param name="craftTree">The target craft tree to edit.</param>
        /// <param name="name">The ID of the tab node. Must be unique!</param>
        /// <param name="displayName">The display name of the tab, which will show up when you hover your mouse on the tab.</param>
        /// <param name="sprite">The sprite of the tab.</param>
        void ICraftTreeHandler.AddTabNode(CraftTree.Type craftTree, string name, string displayName, UnityEngine.Sprite sprite)
        {
            ValidateStandardCraftTree(craftTree);
            string modName = ReflectionHelper.CallingAssemblyByStackTrace().GetName().Name;

            CraftTreePatcher.TabNodes.Add(new TabNode(new string[0], craftTree, new Atlas.Sprite(sprite), modName, name, displayName));
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
        void ICraftTreeHandler.AddTabNode(CraftTree.Type craftTree, string name, string displayName, Atlas.Sprite sprite, params string[] stepsToTab)
        {
            ValidateStandardCraftTree(craftTree);
            string modName = ReflectionHelper.CallingAssemblyByStackTrace().GetName().Name;

            CraftTreePatcher.TabNodes.Add(new TabNode(stepsToTab, craftTree, sprite, modName, name, displayName));
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
        void ICraftTreeHandler.AddTabNode(CraftTree.Type craftTree, string name, string displayName, UnityEngine.Sprite sprite, params string[] stepsToTab)
        {
            ValidateStandardCraftTree(craftTree);            
            string modName = ReflectionHelper.CallingAssemblyByStackTrace().GetName().Name;

            CraftTreePatcher.TabNodes.Add(new TabNode(stepsToTab, craftTree, new Atlas.Sprite(sprite), modName, name, displayName));
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
        void ICraftTreeHandler.RemoveNode(CraftTree.Type craftTree, params string[] stepsToNode)
        {
            ValidateStandardCraftTree(craftTree);
            CraftTreePatcher.NodesToRemove.Add(new Node(stepsToNode, craftTree));
        }

        #endregion

        private static void ValidateStandardCraftTree(CraftTree.Type craftTree)
        {
            switch (craftTree)
            {
                case CraftTree.Type.Fabricator:
                case CraftTree.Type.Constructor:
                case CraftTree.Type.Workbench:
                case CraftTree.Type.SeamothUpgrades:
                case CraftTree.Type.MapRoom:
                case CraftTree.Type.Centrifuge:
                case CraftTree.Type.CyclopsFabricator:
                case CraftTree.Type.Rocket:
                    break; // Okay
                case CraftTree.Type.Unused1:                    
                case CraftTree.Type.Unused2:                    
                case CraftTree.Type.None:
                default:
                    throw new ArgumentException($"{nameof(craftTree)} value of '{craftTree}' does not correspond to a standard crafting tree. This method is intended for use only with standard crafting trees, not custom ones or unused ones.");
            }   
        }
    }
}
