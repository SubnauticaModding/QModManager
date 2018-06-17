using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLHelper.V2.Patchers;

namespace SMLHelper.V2.Crafting
{
    public class CraftTreeNode
    {
        protected readonly string Name;
        protected readonly TreeAction TreeAction;
        protected readonly TechType TechType;

        protected CraftTreeNode Parent;

        internal CraftNode CraftNode;

        public CraftTreeNode(TreeAction action, string name, TechType techType)
        {
            Name = name;
            TreeAction = action;
            TechType = techType;

            CraftNode = new CraftNode(Name, TreeAction, TechType);
        }

        public void LinkToParent(CraftTreeNode parent)
        {
            this.Parent = parent;
            this.Parent.CraftNode.AddNode(this.CraftNode);
        }
    }

    public class CraftTreeTab : CraftTreeNode
    {
        private readonly string DisplayText;
        private readonly CustomSprite Sprite;
        private readonly CraftTree.Type Scheme;

        public CraftTreeTab(CraftTree.Type scheme, string name, string displayText, UnityEngine.Sprite sprite) : base(TreeAction.Expand, name, TechType.None)
        {
            Scheme = scheme;
            DisplayText = displayText;
            Sprite = new CustomSprite(SpriteManager.Group.Category, $"{Scheme.ToString()}_{Name}", sprite);

            LanguagePatcher.customLines.Add($"{Scheme.ToString()}Menu_{Name}", displayText);
            CustomSpriteHandler.customSprites.Add(Sprite);
        }

        public CraftTreeTab(CraftTree.Type scheme, string name, string displayText, Atlas.Sprite sprite) : base(TreeAction.Expand, name, TechType.None)
        {
            Scheme = scheme;
            DisplayText = displayText;
            Sprite = new CustomSprite(SpriteManager.Group.Category, $"{Scheme.ToString()}_{Name}", sprite);

            LanguagePatcher.customLines.Add($"{Scheme.ToString()}Menu_{Name}", displayText);
            CustomSpriteHandler.customSprites.Add(Sprite);
        }

        public void AddCraftingNode(TechType techType)
        {
            var node = new CraftTreeCraftNode(techType);
            node.LinkToParent(this);
        }

        public void AddCraftingNode(params TechType[] techTypes)
        {
            foreach (var tType in techTypes)
            {
                AddCraftingNode(tType);
            }
        }

        public CraftTreeTab AddTabNode(string name, string displayName, UnityEngine.Sprite sprite)
        {
            var node = new CraftTreeTab(Scheme, name, displayName, sprite);
            node.LinkToParent(this);

            return node;
        }

        public CraftTreeTab AddTabNode(string name, string displayName, Atlas.Sprite sprite)
        {
            var node = new CraftTreeTab(Scheme, name, displayName, sprite);
            node.LinkToParent(this);

            return node;
        }
    }

    public class CraftTreeCraftNode : CraftTreeNode
    {
        public CraftTreeCraftNode(TechType techType) : base(TreeAction.Craft, techType.AsString(), techType)
        {
        }
    }
}
