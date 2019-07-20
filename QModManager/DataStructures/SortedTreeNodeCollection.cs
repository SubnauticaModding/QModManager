namespace QModManager.DataStructures
{
    using System;
    using System.Collections.Generic;

    internal class SortedTreeNodeCollection<IdType, DataType> : Dictionary<IdType, SortedTreeNode<IdType, DataType>>
        where IdType : IEquatable<IdType>, IComparable<IdType>
        where DataType : ISortable<IdType>
    {
        public ICollection<IdType> FindIds(Predicate<SortedTreeNode<IdType, DataType>> predicate)
        {
            var list = new List<IdType>(this.Count);

            foreach (SortedTreeNode<IdType, DataType> item in this.Values)
            {
                if (predicate.Invoke(item))
                    list.Add(item.Id);
            }

            return list;
        }

        public ICollection<SortedTreeNode<IdType, DataType>> FindNodes(Predicate<SortedTreeNode<IdType, DataType>> predicate)
        {
            var list = new List<SortedTreeNode<IdType, DataType>>(this.Count);

            foreach (SortedTreeNode<IdType, DataType> item in this.Values)
            {
                if (predicate.Invoke(item))
                    list.Add(item);
            }

            return list;
        }
    }
}
