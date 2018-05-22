using System.Collections.Generic;
using SMLHelper.Patchers;
using UnityEngine;
using UnityEngine.Assertions;

namespace SMLHelper
{
    /// <summary>
    /// Creating a new CraftTree only makes sense if you're going to use it in a new type of GhostCrafter
    /// </summary>
    public abstract class CustomCraftTreeNode
    {
        internal static bool Initialized = false;

        internal static bool HasCustomTrees { get; set; } = false;

        internal const string InvalidChildAddError = "Only tab and root nodes can contain child nodes.";

        public readonly TreeAction Action;
        public readonly TechType TechType;
        public readonly CraftTree.Type Scheme;
        public readonly string Name;

        internal CustomCraftTreeNode Parent = null;
        internal List<CustomCraftTreeNode> ChildNodes;

        internal readonly CraftNode CraftNode;

        protected CustomCraftTreeNode(CraftTree.Type scheme, string name, TreeAction action, TechType techType, params CustomCraftTreeNode[] childNodes)
        {
            Assert.IsTrue((int)scheme > CraftTreeTypePatcher.startingIndex, "Custom CraftTree types must have an index higher than the in-game types.");

            Scheme = scheme;
            Name = name;
            Action = action;
            TechType = techType;

            // This is the actual class used for the real CraftTree
            CraftNode = new CraftNode(Name, Action, TechType);

            if (childNodes != null)
            {
                AddChildNodes(childNodes);
            }
        }

        /// <summary>
        /// Adds one or more new child nodes to this custom craft tree node.
        /// This node must be either the root node or a tab node to have child nodes.
        /// </summary>
        /// <param name="childnodes">The new child nodes.</param>
        internal void AddChildNodes(params CustomCraftTreeNode[] childnodes)
        {
            Assert.AreNotEqual(TreeAction.Craft, Action, InvalidChildAddError);
            Assert.AreEqual(TechType.None, TechType, InvalidChildAddError);

            if (childnodes?.Length == 0)
                return;

            if (ChildNodes == null)
                ChildNodes = new List<CustomCraftTreeNode>(childnodes);
            else
                ChildNodes.AddRange(childnodes);

            foreach (var child in childnodes)
            {
                // Link nodes
                CraftNode.AddNode(child.CraftNode);
                child.Parent = this;
            }
        }
    }

    /// <summary>
    /// The root node of a custom CraftTree.
    /// </summary>
    public class CustomCraftTreeRoot : CustomCraftTreeNode
    {
        /// <summary>
        /// Creates a root node for a custom crafting tree. Every CraftTree needs one, and only one, root node.
        /// </summary>
        /// <param name="scheme">The new craft tree type.</param>
        /// <param name="childNodes">The child nodes to the root. These must be either tab nodes or craft nodes from here on.</param>
        public CustomCraftTreeRoot(CraftTree.Type scheme, params CustomCraftTreeNode[] childNodes)
            : base(scheme, "Root", TreeAction.None, TechType.None, childNodes)
        {            
            HasCustomTrees = true;
        }

        /// <summary>
        /// Adds one or more new tab nodes to the tree root.        
        /// </summary>
        /// <param name="tabNode">The tab nodes to add.</param>
        public void AddNodes(params CustomCraftTreeTab[] tabNode)
        {
            AddChildNodes(tabNode);
        }

        /// <summary>
        /// Adds one or more new crafting nodes to the tree root.
        /// </summary>
        /// <param name="craftNodes">The crafting nodes to add.</param>
        public void AddNodes(params CustomCraftTreeCraft[] craftNodes)
        {
            AddChildNodes(craftNodes);
        }

        /// <summary>
        /// Dynamically created the CraftTree object for this custom crafting tree.
        /// The CraftNode objects were created and linked as the classes of the CustomCraftTreeFamily were created and linked.
        /// </summary>
        internal CraftTree CraftTree => new CraftTree(Scheme.ToString(), CraftNode);
    }

    /// <summary>
    /// A tab node of a custom CraftTree. Tab nodes help organize crafting nodes by grouping them into categories.
    /// </summary>
    public class CustomCraftTreeTab : CustomCraftTreeNode
    {
        internal string TabLanguageID => $"{CraftTreeTypePatcher.craftTreeTypeToString[Scheme]}Menu_{Name}";

        internal string TabSpriteID => $"{CraftTreeTypePatcher.craftTreeTypeToString[Scheme]}_{Name}";

        /// <summary>
        /// Creates a new tab node for a custom crafting tree.
        /// </summary>
        /// <param name="scheme">The new craft tree type.</param>
        /// <param name="nameID">The name/ID of this node.</param>
        /// <param name="displayText">The hover text to display in-game.</param>
        /// <param name="sprite">The custom sprite to display on this tab node.</param>
        /// <param name="childNodes">The child nodes to this tab node. These must be either tab nodes or craft nodes from here on.</param>
        public CustomCraftTreeTab(CraftTree.Type scheme, string nameID, string displayText, Atlas.Sprite sprite, params CustomCraftTreeNode[] childNodes)
            : base(scheme, nameID, TreeAction.Expand, TechType.None, childNodes)
        {            
            LanguagePatcher.customLines[TabLanguageID] = displayText;

            var custSprite = new CustomSprite(SpriteManager.Group.Category, TabSpriteID, sprite);
            CustomSpriteHandler.customSprites.Add(custSprite);
        }

        /// <summary>
        /// Creates a new tab node for a custom crafting tree. This sets what is being crafted.
        /// </summary>
        /// <param name="scheme">The new craft tree type.</param>
        /// <param name="nameID">The name/ID of this node.</param>
        /// <param name="displayText">The hover text to display in-game.</param>
        /// <param name="sprite">The custom sprite to display on this tab node.</param>
        /// <param name="childNodes">The child nodes to this tab node. These must be either tab nodes or craft nodes from here on.</param>
        public CustomCraftTreeTab(CraftTree.Type scheme, string nameID, string displayText, Sprite sprite, params CustomCraftTreeNode[] childNodes)
            : base(scheme, nameID, TreeAction.Expand, TechType.None, childNodes)
        {
            LanguagePatcher.customLines[TabLanguageID] = displayText;

            var custSprite = new CustomSprite(SpriteManager.Group.Category, TabSpriteID, sprite);
            CustomSpriteHandler.customSprites.Add(custSprite);
        }

        /// <summary>
        /// Adds one or more new tab nodes to this tab.        
        /// </summary>
        /// <param name="tabNode">The tab nodes to add.</param>
        public void AddNodes(params CustomCraftTreeTab[] tabNode)
        {
            AddChildNodes(tabNode);
        }

        /// <summary>
        /// Adds one or more new crafting nodes to this tab.
        /// </summary>
        /// <param name="craftNodes">The crafting nodes to add.</param>
        public void AddNodes(params CustomCraftTreeCraft[] craftNodes)
        {
            AddChildNodes(craftNodes);
        }
    }

    /// <summary>
    /// A crafting node of a custom CrafTree.
    /// </summary>
    public class CustomCraftTreeCraft : CustomCraftTreeNode
    {
        /// <summary>
        /// Creating a new crafting node for the custom crafting tree.
        /// </summary>
        /// <param name="scheme">The new craft tree type.</param>
        /// <param name="techType">The TechType to be crafted.</param>
        public CustomCraftTreeCraft(CraftTree.Type scheme, TechType techType)
            : base(scheme, techType.ToString(), TreeAction.Craft, techType)
        {
        }
    }



}
