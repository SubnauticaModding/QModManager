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

        public bool AllDependenciesPresent(ICollection<IdType> otherNodes)
        {
            foreach (IdType item in this.Dependencies)
            {
                if (!otherNodes.Contains(item))
                    return false;
            }

            return true;
        }

        public IList<IdType> Dependencies => Data.RequiredDependencies;

        public IList<IdType> LoadBefore => Data.LoadBeforePreferences;

        public IList<IdType> LoadAfter => Data.LoadAfterPreferences;

        internal bool HasOrdering => this.Dependencies.Count > 0 || this.LoadBefore.Count > 0 || this.LoadAfter.Count > 0;

        public bool IsRoot => Parent == null && (NodeAfter != null || NodeBefore != null);

        public bool IsLinked => (Parent != null) || (NodeAfter != null || NodeBefore != null);

        public SortedTreeNode<IdType, DataType> Parent;

        public SortedTreeNode<IdType, DataType> NodeBefore;

        public SortedTreeNode<IdType, DataType> NodeAfter;

        public void ClearLinks()
        {
            NodeBefore = null;
            NodeAfter = null;
        }

        public void SetNodeBefore(SortedTreeNode<IdType, DataType> node)
        {
            if (ReferenceEquals(node, this))
                return;

            if (NodeBefore == null)
            {
                NodeBefore = node;
                node.Parent = this;
            }
            else
            {
                NodeBefore.SetNodeAfter(node);
            }
        }

        public void SetNodeAfter(SortedTreeNode<IdType, DataType> node)
        {
            if (ReferenceEquals(node, this))
                return;

            if (NodeAfter == null)
            {
                NodeAfter = node;
                node.Parent = this;
            }
            else
            {
                NodeAfter.SetNodeBefore(node);
            }
        }
    }
}
