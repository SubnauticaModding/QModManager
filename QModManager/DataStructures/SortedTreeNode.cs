namespace QModManager.DataStructures
{
    using System;
    using System.Collections.Generic;

    internal class SortedTreeNode<IdType, DataType>
        where IdType : IEquatable<IdType>, IComparable<IdType>
        where DataType : ISortable<IdType>
    {
        internal SortedTreeNode(IdType id, DataType data, SortedCollection<IdType, DataType> tree)
        {
            Id = id;
            Data = data;
            Tree = tree;
        }

        public readonly IdType Id;

        public readonly DataType Data;

        public readonly SortedCollection<IdType, DataType> Tree;

        public IList<IdType> Dependencies => Data.RequiredDependencies;

        public IList<IdType> LoadBefore => Data.LoadBeforePreferences;

        public IList<IdType> LoadAfter => Data.LoadAfterPreferences;

        internal bool HasOrdering => this.Dependencies.Count > 0 || this.LoadBefore.Count > 0 || this.LoadAfter.Count > 0;

        public bool IsRoot => Parent == null && (RightChildNode != null || LeftChildNode != null);

        public bool IsLinked => (Parent != null) || (RightChildNode != null || LeftChildNode != null);

        public SortedTreeNode<IdType, DataType> Parent;

        public SortedTreeNode<IdType, DataType> LeftChildNode;

        public SortedTreeNode<IdType, DataType> RightChildNode;

        public void ClearLinks()
        {
            LeftChildNode = null;
            RightChildNode = null;
        }

        public void SetLeftChild(SortedTreeNode<IdType, DataType> node)
        {
            if (ReferenceEquals(node, this))
                return;

            if (LeftChildNode == null)
            {
                LeftChildNode = node;
                node.Parent = this;
            }
            else
            {
                LeftChildNode.SetRightChild(node);
            }
        }

        public void SetRightChild(SortedTreeNode<IdType, DataType> node)
        {
            if (ReferenceEquals(node, this))
                return;

            if (RightChildNode == null)
            {
                RightChildNode = node;
                node.Parent = this;
            }
            else
            {
                RightChildNode.SetLeftChild(node);
            }
        }
    }
}
