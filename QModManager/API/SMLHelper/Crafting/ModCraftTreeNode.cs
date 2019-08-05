namespace QModManager.API.SMLHelper.Crafting
{
    using UnityEngine.Assertions;

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
        /// The in-game representation of this node.
        /// </summary>
        public CraftNode CraftNode;

        internal ModCraftTreeLinkingNode Parent = null;

        internal virtual CraftTree.Type Scheme => this.Parent.Scheme;
        internal virtual string SchemeAsString => this.Parent.SchemeAsString;

        internal ModCraftTreeNode(string name, TreeAction action, TechType techType)
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
}
