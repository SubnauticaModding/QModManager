namespace QModManager.DataStructures
{
    using System;
    using System.Collections.Generic;

    internal class SortedTree<IdType, DataType, Priority>
        where IdType : IEquatable<IdType>, IComparable<IdType>
        where DataType : ISortable<IdType, Priority>
        where Priority : Enum
    {
        private class MetaSortTree
        {
            internal SortedTreeNode<IdType, DataType, Priority> Root;
            internal readonly IDictionary<IdType, SortedTreeNode<IdType, DataType, Priority>> SortedElements = new Dictionary<IdType, SortedTreeNode<IdType, DataType, Priority>>();
            internal int NodesInError;
            internal int NodeCount => SortedElements.Count - NodesInError;
        }
        private readonly SortedList<Priority, MetaSortTree> SubTrees;
        private readonly IDictionary<IdType, Priority> KnownKeys = new Dictionary<IdType, Priority>();

        internal int Count
        {
            get
            {
                int total = 0;
                foreach (MetaSortTree tree in SubTrees.Values)
                {
                    total += tree.NodeCount;
                }

                return total;
            }
        }

        internal int NodesInError
        {
            get
            {
                int total = 0;
                foreach (MetaSortTree tree in SubTrees.Values)
                {
                    total += tree.NodesInError;
                }

                return total;
            }
        }

        public SortedTree()
        {
            Array priorityValues = Enum.GetValues(typeof(Priority));
            SubTrees = new SortedList<Priority, MetaSortTree>(priorityValues.Length);

            foreach (Priority priority in priorityValues)
            {
                SubTrees.Add(priority, new MetaSortTree());
            };
        }

        public SortResults Add(ISortable<IdType, Priority> data)
        {
            if (IsDuplicateId(data.Id))
            {
                return SortResults.DuplicateId;
            }

            KnownKeys.Add(data.Id, data.LoadPriority);

            var entity = new SortedTreeNode<IdType, DataType, Priority>(data.Id, data, this);

            MetaSortTree subTree = SubTrees[data.LoadPriority];

            if (subTree.Root == null)
            {
                subTree.Root = entity;
                subTree.SortedElements.Add(entity.Id, entity);
                return SortResults.SortAfter;
            }

            SortResults sortResult = subTree.Root.Sort(entity);

            switch (sortResult)
            {
                case SortResults.SortBefore:
                case SortResults.SortAfter:
                    subTree.SortedElements.Add(entity.Id, entity);
                    break;
                default:
                    subTree.NodesInError++;
                    break;
            }

            return sortResult;
        }

        public List<IdType> CreateFlatIndexList()
        {
            var list = new List<IdType>(this.Count);

            foreach (MetaSortTree subTree in SubTrees.Values)
            {
                ClearErrorsCleanTree(subTree);
                CreateFlatIndexList(subTree.Root, list);
            }

            return list;
        }

        private static void CreateFlatIndexList(SortedTreeNode<IdType, DataType, Priority> node, ICollection<IdType> list)
        {
            if (node is null)
            {
                return;
            }

            while (true)
            {
                if (node.LoadBefore != null)
                {
                    CreateFlatIndexList(node.LoadBefore, list);
                }

                if (!node.HasError)
                {
                    list.Add(node.Id);
                }

                if (node.LoadAfter != null)
                {
                    node = node.LoadAfter;
                    continue;
                }

                break;
            }
        }

        private bool IsDuplicateId(IdType id)
        {
            if (KnownKeys.ContainsKey(id))
            {
                MetaSortTree subTree = SubTrees[KnownKeys[id]];
                if (subTree.SortedElements.TryGetValue(id, out SortedTreeNode<IdType, DataType, Priority> dup))
                {
                    dup.Error = ErrorTypes.DuplicateId;
                    subTree.SortedElements.Remove(id);

                    subTree.NodesInError++;
                    return true;
                }
            }

            return false;
        }

        private bool AllDependenciesArePresent(SortedTreeNode<IdType, DataType, Priority> node)
        {
            if (!node.HasDependencies)
            {
                return true;
            }

            bool dependenciesPresent = false;

            foreach (IdType nodeDependency in node.Dependencies)
            {
                foreach (MetaSortTree subTree in SubTrees.Values)
                {
                    if (subTree.SortedElements.ContainsKey(nodeDependency))
                    {
                        dependenciesPresent = true;
                    }
                }
            }

            if (!dependenciesPresent)
            {
                node.Error = ErrorTypes.MissingDepency;
            }

            return dependenciesPresent;
        }

        private void ClearErrorsCleanTree(MetaSortTree subTree)
        {
            var cleanList = new List<SortedTreeNode<IdType, DataType, Priority>>();

            foreach (SortedTreeNode<IdType, DataType, Priority> entity in subTree.SortedElements.Values)
            {
                if (entity.HasError || !AllDependenciesArePresent(entity))
                {
                    continue;
                }

                entity.ClearLinks();
                cleanList.Add(entity);
                KnownKeys.Remove(entity.Id);
            }

            subTree.SortedElements.Clear();
            subTree.Root = null;
            subTree.NodesInError = 0;

            foreach (SortedTreeNode<IdType, DataType, Priority> entity in cleanList)
            {
                Add(entity.Data);
            }
        }
    }
}
