namespace QModManager.API.SMLHelper.Crafting
{
    using System.Collections.Generic;
    using System.Linq;
    using Assets;
    using Patchers;
    using UnityEngine;
    using UnityEngine.Assertions;
    using Utility;

    /// <summary>
    /// Basic data structure of a crafting tree node.
    /// </summary>
    public abstract class ModCraftTreeNode
    {
        internal static bool Initialized = false;
        internal static bool HasCustomTrees { get; set; } = false;

        /// <summary>
        /// The action this node takes in the crafting tree.
        /// </summary>
        public readonly TreeAction Action;

        /// <summary>
        /// The tech type ID associated to this node.
        /// For item nodes, it is the item ID to be crafted.
        /// For root and tab nodes, this is always <see cref="TechType.None"/>.
        /// </summary>
        public readonly TechType TechType;

        /// <summary>
        /// The name ID for this tab node.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The the in-game representation of this node.
        /// </summary>
        public CraftNode CraftNode;

        protected ModCraftTreeLinkingNode Parent = null;

        protected virtual CraftTree.Type Scheme => this.Parent.Scheme;
        protected virtual string SchemeAsString => this.Parent.SchemeAsString;

        protected ModCraftTreeNode(string name, TreeAction action, TechType techType)
        {
            Name = name;
            Action = action;
            TechType = techType;

            // This is the actual class used for the real CraftTree
            CraftNode = new CraftNode(Name, Action, TechType);
        }

        /// <summary>
        /// Removes the calling node from parent. 
        /// </summary>
        public void RemoveNode()
        {
            Assert.IsNotNull(this.Parent, "No parent found to remove node from!");
            Assert.IsNotNull(this.Parent.CraftNode, "No CraftNode found on parent!");

            if (this is ModCraftTreeLinkingNode)
            {
                ModCraftTreeLinkingNode linkingNode = this as ModCraftTreeLinkingNode;
                foreach (ModCraftTreeNode cNode in linkingNode.ChildNodes)
                {
                    cNode.RemoveNode();
                }
            }

            this.Parent.ChildNodes.Remove(this);
            this.Parent.CraftNode.RemoveNode(this.CraftNode);
            this.Parent = null;
        }

        internal virtual void LinkToParent(ModCraftTreeLinkingNode parent)
        {
            parent.CraftNode.AddNode(this.CraftNode);
            this.Parent = parent;
        }
    }

    /// <summary>
    /// Abstract class that provides methods for adding new nodes into the crafting tree.
    /// </summary>
    /// <seealso cref="ModCraftTreeNode" />
    public abstract class ModCraftTreeLinkingNode : ModCraftTreeNode
    {
        /// <summary>
        /// The child nodes linked bellow this node.
        /// </summary>
        public readonly List<ModCraftTreeNode> ChildNodes = new List<ModCraftTreeNode>();

        protected ModCraftTreeLinkingNode(string name, TreeAction action, TechType techType)
            : base(name, action, techType)
        {
        }

        /// <summary>
        /// Creates a new tab node for the crafting tree and links it to the calling node.
        /// </summary>
        /// <param name="nameID">The name/ID of this node.</param>
        /// <param name="displayText">The hover text to display in-game.</param>
        /// <param name="sprite">The custom sprite to display on this tab node.</param>
        /// <returns>A new tab node linked to the root node and ready to use.</returns>
        public ModCraftTreeTab AddTabNode(string nameID, string displayText, Atlas.Sprite sprite)
        {
            string modName = ReflectionHelper.CallingAssemblyNameByStackTrace();

            var tabNode = new ModCraftTreeTab(modName, nameID, displayText, sprite);
            tabNode.LinkToParent(this);

            ChildNodes.Add(tabNode);

            return tabNode;
        }

        /// <summary>
        /// Creates a new tab node for the crafting tree and links it to the calling node.
        /// </summary>
        /// <param name="nameID">The name/ID of this node.</param>
        /// <param name="displayText">The hover text to display in-game.</param>
        /// <param name="sprite">The custom sprite to display on this tab node.</param>
        /// <returns>A new tab node linked to the root node and ready to use.</returns>
        public ModCraftTreeTab AddTabNode(string nameID, string displayText, Sprite sprite)
        {
            string modName = ReflectionHelper.CallingAssemblyNameByStackTrace();

            var tabNode = new ModCraftTreeTab(modName, nameID, displayText, sprite);
            tabNode.LinkToParent(this);

            ChildNodes.Add(tabNode);

            return tabNode;
        }

        /// <summary>
        /// Creates a new tab node for the crafting tree and links it to the calling node.
        /// </summary>
        /// <param name="nameID">The name/ID of this node.</param>
        /// <returns>A new tab node linked to the root node and ready to use.</returns>
        public ModCraftTreeTab AddTabNode(string nameID)
        {
            string modName = ReflectionHelper.CallingAssemblyNameByStackTrace();

            var tabNode = new ModCraftTreeTab(modName, nameID);
            tabNode.LinkToParent(this);

            ChildNodes.Add(tabNode);

            return tabNode;
        }

        /// <summary>
        /// Gets the tab from the calling node.
        /// </summary>
        /// <param name="nameID">The name id of the tab to get.</param>
        /// <returns></returns>
        public ModCraftTreeTab GetTabNode(string nameID)
        {
            foreach (ModCraftTreeTab node in ChildNodes)
            {
                if (node == null) continue;

                if (node.Name == nameID && node.Action == TreeAction.Expand)
                {
                    var tab = (ModCraftTreeTab)node;
                    return tab;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the crafting node from the calling node.
        /// </summary>
        /// <param name="techType">The TechType whose node to get.</param>
        /// <returns></returns>
        public ModCraftTreeCraft GetCraftingNode(TechType techType)
        {
            foreach (ModCraftTreeNode node in ChildNodes)
            {
                if (node == null) continue;

                if (node.TechType == techType && node.Action == TreeAction.Craft)
                {
                    var craftNode = (ModCraftTreeCraft)node;
                    return craftNode;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the node associated with the ID specified. Used if you don't know whether node is a tab or a craft node.
        /// </summary>
        /// <param name="nameID"></param>
        /// <returns></returns>
        public ModCraftTreeNode GetNode(string nameID)
        {
            foreach (ModCraftTreeNode node in ChildNodes)
            {
                if (node == null) continue;

                if (node.Name == nameID)
                    return node;
            }

            return null;
        }

        /// <summary>
        /// Creates a new crafting node for the crafting tree and links it to the calling node.
        /// </summary>
        /// <param name="techType">The TechType to be crafted.</param>
        public void AddCraftingNode(TechType techType)
        {
            Assert.AreNotEqual(TechType.None, techType, "Attempt to add TechType.None as a crafting node.");

            var craftNode = new ModCraftTreeCraft(techType);
            craftNode.LinkToParent(this);

            ChildNodes.Add(craftNode);
        }

        /// <summary>
        /// Creates a collection of new crafting nodes for the crafting tree and links it to the calling node.
        /// </summary>
        /// <param name="techTypes">The TechTypes to be crafted.</param>
        public void AddCraftingNode(params TechType[] techTypes) => AddCraftingNode(techTypes.AsEnumerable());

        /// <summary>
        /// Creates a collection of new crafting nodes for the crafting tree and links it to the calling node.
        /// </summary>
        /// <param name="techTypes">The TechTypes to be crafted.</param>
        public void AddCraftingNode(IEnumerable<TechType> techTypes)
        {
            foreach (TechType tType in techTypes)
            {
                Assert.AreNotEqual(TechType.None, tType, "Attempt to add TechType.None as a crafting node.");
                this.AddCraftingNode(tType);
            }
        }

        /// <summary>
        /// <para>Creates a new crafting node for a modded item and links it to the calling node.</para>
        /// <para>If the modded item isn't present for the player, this call is safely ignored.</para>
        /// </summary>
        /// <param name="moddedTechTypeName">The internal name of the custom TechType to be crafted.</param>
        /// <remarks>
        /// If the player doesn't have the mod for this TechType installed, then nothing will happen.
        /// </remarks>
        public void AddModdedCraftingNode(string moddedTechTypeName)
        {
            EnumTypeCache cache = TechTypePatcher.cacheManager.GetCacheForTypeName(moddedTechTypeName);

            if (cache != null)
            {
                var techType = (TechType)cache.Index;
                var craftNode = new ModCraftTreeCraft(techType);
                craftNode.LinkToParent(this);

                ChildNodes.Add(craftNode);
            }
        }
    }

    /// <summary>
    /// The root node of a CraftTree. The whole tree starts here.
    /// </summary>    
    /// <seealso cref="ModCraftTreeLinkingNode" />
    public class ModCraftTreeRoot : ModCraftTreeLinkingNode
    {
        private readonly string _schemeAsString;
        private readonly CraftTree.Type _scheme;

        protected override string SchemeAsString => _schemeAsString;
        protected override CraftTree.Type Scheme => _scheme;

        internal ModCraftTreeRoot(CraftTree.Type scheme, string schemeAsString)
            : base("Root", TreeAction.None, TechType.None)
        {
            Assert.IsTrue((int)scheme > CraftTreeTypePatcher.startingIndex, "Custom CraftTree types must have an index higher than the in-game types.");

            _schemeAsString = schemeAsString;
            _scheme = scheme;
            HasCustomTrees = true;
        }

        /// <summary>
        /// Dynamically creates the CraftTree object for this crafting tree.
        /// The CraftNode objects were created and linked as the classes of the ModCraftTreeFamily were created and linked.
        /// </summary>
        internal CraftTree CraftTree => new CraftTree(_schemeAsString, CraftNode);

        /// <summary>
        /// Populates a new ModCraftTreeRoot from a CraftNode tree.
        /// </summary>
        /// <param name="tree">The tree to create the ModCraftTreeRoot from.</param>
        /// <param name="root"></param>
        internal static void CreateFromExistingTree(CraftNode tree, ref ModCraftTreeLinkingNode root)
        {
            foreach (CraftNode node in tree)
            {
                if (node.action == TreeAction.Expand)
                {
                    ModCraftTreeTab tab = root.AddTabNode(node.id);
                    var thing = (ModCraftTreeLinkingNode)tab;
                    CreateFromExistingTree(node, ref thing);
                }

                if (node.action == TreeAction.Craft)
                {
                    var techType = TechType.None;
                    TechTypeExtensions.FromString(node.id, out techType, false);

                    if (node.id == "SeamothHullModule2") techType = TechType.VehicleHullModule2;
                    else if (node.id == "SeamothHullModule3") techType = TechType.VehicleHullModule3;

                    root.AddCraftingNode(techType);
                }
            }
        }

        /// <summary>
        /// Gets the tab node at the specified path from the root.
        /// </summary>
        /// <param name="stepsToTab">
        /// <para>The steps to the target tab.</para>
        /// <para>These must match the id value of the CraftNode in the crafting tree you're targeting.</para>
        /// <para>Do not include "root" in this path.</para>
        /// </param>
        /// <returns>If the specified tab node is found, returns that <see cref="ModCraftTreeTab"/>; Otherwise, returns null.</returns>
        public ModCraftTreeTab GetTabNode(params string[] stepsToTab)
        {
            ModCraftTreeTab tab = base.GetTabNode(stepsToTab[0]);

            for (int i = 1; i < stepsToTab.Length && tab != null; i++)
            {
                tab = tab.GetTabNode(stepsToTab[i]);
            }

            return tab;
        }

        /// <summary>
        /// Gets the node at the specified path from the root.
        /// </summary>
        /// <param name="stepsToNode">
        /// <para>The steps to the target tab.</para>
        /// <para>These must match the id value of the CraftNode in the crafting tree you're targeting.</para>
        /// <para>Do not include "root" in this path.</para>
        /// </param>
        /// <returns>If the specified tab node is found, returns that <see cref="ModCraftTreeNode"/>; Otherwise, returns null.</returns>
        public ModCraftTreeNode GetNode(params string[] stepsToNode)
        {
            if (stepsToNode.Length == 1)
            {
                return base.GetNode(stepsToNode[0]);
            }

            string nodeID = stepsToNode[stepsToNode.Length - 1];
            string[] stepsToTab = stepsToNode.Take(stepsToNode.Length - 1).ToArray();
            ModCraftTreeTab tab = GetTabNode(stepsToTab);

            if (tab == null) return null;

            return tab.GetNode(nodeID);
        }
    }

    /// <summary>
    /// A tab node of a CraftTree. Tab nodes help organize crafting nodes by grouping them into categories.
    /// </summary>
    /// <seealso cref="ModCraftTreeLinkingNode" />
    public class ModCraftTreeTab : ModCraftTreeLinkingNode
    {
        private readonly string DisplayText;
        private readonly Atlas.Sprite Asprite;
        private readonly Sprite Usprite;
        private readonly bool IsExistingTab;
        private readonly string ModName;

        internal ModCraftTreeTab(string modName, string nameID, string displayText, Atlas.Sprite sprite)
            : base(nameID, TreeAction.Expand, TechType.None)
        {
            DisplayText = displayText;
            Asprite = sprite;
            Usprite = null;
            ModName = modName;
        }

        internal ModCraftTreeTab(string modName, string nameID, string displayText, Sprite sprite)
            : base(nameID, TreeAction.Expand, TechType.None)
        {
            DisplayText = displayText;
            Asprite = null;
            Usprite = sprite;
            ModName = modName;
        }

        internal ModCraftTreeTab(string modName, string nameID)
            : base(nameID, TreeAction.Expand, TechType.None)
        {
            IsExistingTab = true;
            ModName = modName;
        }

        internal override void LinkToParent(ModCraftTreeLinkingNode parent)
        {
            base.LinkToParent(parent);

            if (IsExistingTab) return;

            LanguagePatcher.AddCustomLanguageLine(ModName, $"{base.SchemeAsString}Menu_{Name}", DisplayText);

            string spriteID = $"{SchemeAsString}_{Name}";

            ModSprite modSprite;
            if (Asprite != null)
            {
                modSprite = new ModSprite(SpriteManager.Group.Category, spriteID, Asprite);
            }
            else
            {
                modSprite = new ModSprite(SpriteManager.Group.Category, spriteID, Usprite);

            }

            ModSprite.Add(modSprite);
        }
    }

    /// <summary>
    /// A crafting node of a CraftTree. This is the last node on a tree; The one that actually crafts something.
    /// </summary>
    /// <seealso cref="ModCraftTreeNode" />
    public class ModCraftTreeCraft : ModCraftTreeNode
    {
        internal ModCraftTreeCraft(TechType techType)
            : base(techType.AsString(), TreeAction.Craft, techType)
        {
        }
    }
}
