using SMLHelper.Patchers;
using SMLHelper.Util;
using UnityEngine;
using UnityEngine.Assertions;

namespace SMLHelper
{
    /// <summary>
    /// Basic data structure of a custom crafting tree node.
    /// </summary>
    public abstract class CustomCraftTreeNode
    {
        internal static bool Initialized = false;
        internal static bool HasCustomTrees { get; set; } = false;        

        protected readonly TreeAction Action;
        protected readonly TechType TechType;
        protected readonly string Name;
        protected readonly CraftNode CraftNode;

        protected CustomCraftTreeLinkingNode Parent = null;

        protected virtual CraftTree.Type Scheme => this.Parent.Scheme;        
        protected virtual string SchemeAsString => this.Parent.SchemeAsString;

        protected CustomCraftTreeNode(string name, TreeAction action, TechType techType)
        {
            Name = name;
            Action = action;
            TechType = techType;

            // This is the actual class used for the real CraftTree
            CraftNode = new CraftNode(Name, Action, TechType);
        }

        internal virtual void LinkToParent(CustomCraftTreeLinkingNode parent)
        {
            parent.CraftNode.AddNode(this.CraftNode);
            this.Parent = parent;
        }
    }

    /// <summary>
    /// Abstract class that provides methods for adding new nodes into the custom crafting tree.
    /// </summary>
    public abstract class CustomCraftTreeLinkingNode : CustomCraftTreeNode
    {
        protected CustomCraftTreeLinkingNode(string name, TreeAction action, TechType techType)
            : base(name, action, techType)
        {
        }

        /// <summary>
        /// Creates a new tab node for the custom crafting tree and links it to the calling node.
        /// </summary>
        /// <param name="nameID">The name/ID of this node.</param>
        /// <param name="displayText">The hover text to display in-game.</param>
        /// <param name="sprite">The custom sprite to display on this tab node.</param>
        /// <param name="childNodes">The child nodes to this tab node. These must be either tab nodes or craft nodes from here on.</param>
        public CustomCraftTreeTab AddTabNode(string nameID, string displayText, Atlas.Sprite sprite)
        {
            var tabNode = new CustomCraftTreeTab(nameID, displayText, sprite);
            tabNode.LinkToParent(this);
            return tabNode;
        }

        /// <summary>
        /// Creates a new tab node for the custom crafting tree and links it to the calling node.
        /// </summary>
        /// <param name="nameID">The name/ID of this node.</param>
        /// <param name="displayText">The hover text to display in-game.</param>
        /// <param name="sprite">The custom sprite to display on this tab node.</param>
        /// <param name="childNodes">The child nodes to this tab node. These must be either tab nodes or craft nodes from here on.</param>
        public CustomCraftTreeTab AddTabNode(string nameID, string displayText, Sprite sprite)
        {
            var tabNode = new CustomCraftTreeTab(nameID, displayText, sprite);
            tabNode.LinkToParent(this);
            return tabNode;
        }

        /// <summary>
        /// Creates a new crafting node for the custom crafting tree and links it to the calling node.
        /// </summary>
        /// <param name="techType">The TechType to be crafted.</param>
        public void AddCraftingNode(TechType techType)
        {
            var craftNode = new CustomCraftTreeCraft(techType);
            craftNode.LinkToParent(this);
        }

        /// <summary>
        /// Creates a collection of new crafting nodes for the custom crafting tree and links it to the calling node.
        /// </summary>
        /// <param name="techType">The TechType to be crafted.</param>
        public void AddCraftingNode(params TechType[] techTypes)
        {
            foreach (var tType in techTypes)
            {
                this.AddCraftingNode(tType);
            }
        }

        /// <summary>
        /// Creates a new crafting node for a modded item for custom crafting tree and links it to the calling node.
        /// </summary>
        /// <param name="techType">The name of the custom TechType to be crafted.</param>
        /// <remarks>If the player doesn't have the mod for this TechType installed, then nothing will happen.</remarks>
        public void AddModdedCraftingNode(string moddedTechTypeName)
        {
            EnumTypeCache cache = TechTypePatcher.cacheManager.GetCacheForTypeName(moddedTechTypeName);

            if (cache != null)
            {
                var techType = (TechType)cache.Index;
                var craftNode = new CustomCraftTreeCraft(techType);
                craftNode.LinkToParent(this);
            }
        }
    }

    /// <summary>
    /// The root node of a custom CraftTree. The whole tree starts here.
    /// </summary>
    public class CustomCraftTreeRoot : CustomCraftTreeLinkingNode
    {
        private readonly string _schemeAsString;
        private readonly CraftTree.Type _scheme;

        protected override string SchemeAsString => _schemeAsString;
        protected override CraftTree.Type Scheme => _scheme;

        internal CustomCraftTreeRoot(CraftTree.Type scheme, string schemeAsString)
            : base("Root", TreeAction.None, TechType.None)
        {
            Assert.IsTrue((int)scheme > CraftTreeTypePatcher.startingIndex, "Custom CraftTree types must have an index higher than the in-game types.");

            _schemeAsString = schemeAsString;
            _scheme = scheme;
            HasCustomTrees = true;
        }

        /// <summary>
        /// Dynamically creates the CraftTree object for this custom crafting tree.
        /// The CraftNode objects were created and linked as the classes of the CustomCraftTreeFamily were created and linked.
        /// </summary>
        internal CraftTree CraftTree => new CraftTree(_schemeAsString, CraftNode);
    }

    /// <summary>
    /// A tab node of a custom CraftTree. Tab nodes help organize crafting nodes by grouping them into categories.
    /// </summary>
    public class CustomCraftTreeTab : CustomCraftTreeLinkingNode
    {
        private readonly string DisplayText;
        private readonly Atlas.Sprite Asprite;
        private readonly Sprite Usprite;

        internal CustomCraftTreeTab(string nameID, string displayText, Atlas.Sprite sprite)
            : base(nameID, TreeAction.Expand, TechType.None)
        {
            DisplayText = displayText;
            Asprite = sprite;
            Usprite = null;
        }

        internal CustomCraftTreeTab(string nameID, string displayText, Sprite sprite)
            : base(nameID, TreeAction.Expand, TechType.None)
        {
            DisplayText = displayText;
            Asprite = null;
            Usprite = sprite;
        }

        internal override void LinkToParent(CustomCraftTreeLinkingNode parent)
        {
            base.LinkToParent(parent);

            string tabLanguageID = $"{SchemeAsString}Menu_{Name}";

            LanguagePatcher.customLines[tabLanguageID] = DisplayText;

            string spriteID = $"{SchemeAsString}_{Name}";

            CustomSprite custSprite;
            if (Asprite != null)
            {
                custSprite = new CustomSprite(SpriteManager.Group.Category, spriteID, Asprite);
            }
            else
            {
                custSprite = new CustomSprite(SpriteManager.Group.Category, spriteID, Usprite);
            }

            CustomSpriteHandler.customSprites.Add(custSprite);
        }
    }

    /// <summary>
    /// A crafting node of a custom CrafTree. This is the last node on a tree; The one that actuall crafts something.
    /// </summary>
    public class CustomCraftTreeCraft : CustomCraftTreeNode
    {
        internal CustomCraftTreeCraft(TechType techType)
            : base(techType.ToString(), TreeAction.Craft, techType)
        {
        }
    }



}
