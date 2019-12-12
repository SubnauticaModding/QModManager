namespace QModManager.DataStructures
{
    using System;
    using System.Collections.Generic;
    using QModManager.Utility;

    internal class SortedTree<IdType, DataType>
        where IdType : IEquatable<IdType>, IComparable<IdType>
        where DataType : ISortable<IdType>
    {
        private SortedTreeNode<IdType, DataType> Root;
        private readonly IDictionary<IdType, SortedTreeNode<IdType, DataType>> SortedElements;
        private readonly ICollection<IdType> KnownKeys = new HashSet<IdType>();

        internal int NodesInError;
        internal int NodeCount => SortedElements.Count - NodesInError;

        public SortedTree()
        {
            SortedElements = new Dictionary<IdType, SortedTreeNode<IdType, DataType>>();
        }

        public SortedTree(int capacity)
        {
            SortedElements = new Dictionary<IdType, SortedTreeNode<IdType, DataType>>(capacity);
        }

        public SortResults Add(DataType data)
        {
            if (IsDuplicate(data))
            {
                return SortResults.DuplicateId;
            }

            var entity = new SortedTreeNode<IdType, DataType>(data.Id, data, this);

            if (Root == null)
            {
                Root = entity;
                KnownKeys.Add(data.Id);
                SortedElements.Add(entity.Id, entity);
                return SortResults.SortAfter;
            }

            SortResults sortResult = Root.Sort(entity);

            switch (sortResult)
            {
                case SortResults.SortBefore:
                case SortResults.SortAfter:
                    KnownKeys.Add(data.Id);
                    SortedElements.Add(entity.Id, entity);
                    break;
                default:
                    NodesInError++;
                    break;
            }

            return sortResult;
        }

        public List<IdType> CreateFlatIndexList(out PairedList<DataType, ErrorTypes> erroredList)
        {
            List<DataType> list = CreateFlatList(out erroredList);

            var retList = new List<IdType>(list.Count);
            foreach (DataType data in list)
            {
                retList.Add(data.Id);
            }

            return retList;
        }

        public List<DataType> CreateFlatList(out PairedList<DataType, ErrorTypes> erroredList)
        {
            Logger.Debug($"CreateFlatList called with {NodesInError} early errors");
            var list = new List<DataType>(this.NodeCount);
            erroredList = ClearErrorsCleanTree();
            CreateFlatList(Root, list);

            return list;
        }

        private static void CreateFlatList(SortedTreeNode<IdType, DataType> node, List<DataType> list)
        {
            if (node is null)
            {
                return;
            }

            while (true)
            {
                if (node.LoadBefore != null)
                {
                    CreateFlatList(node.LoadBefore, list);
                }

                if (!node.HasError)
                {
                    list.Add(node.Data);
                }

                if (node.LoadAfter != null)
                {
                    node = node.LoadAfter;
                    continue;
                }

                break;
            }
        }

        private bool IsDuplicate(DataType other)
        {
            if (KnownKeys.Contains(other.Id))
            {
                if (SortedElements.TryGetValue(other.Id, out SortedTreeNode<IdType, DataType> dup))
                {
                    if (ReferenceEquals(dup, other))
                        return false;

                    dup.Error = ErrorTypes.DuplicateId;
                    SortedElements.Remove(other.Id);

                    NodesInError++;
                    return true;
                }
            }

            return false;
        }

        private bool AllDependenciesArePresent(SortedTreeNode<IdType, DataType> node)
        {
            if (!node.HasDependencies)
            {
                return true;
            }

            int missingDependencies = node.Dependencies.Count;

            foreach (IdType nodeDependency in node.Dependencies)
            {
                if (SortedElements.ContainsKey(nodeDependency))
                {
                    missingDependencies--;
                }
            }

            if (missingDependencies > 0)
            {
                node.Error = ErrorTypes.MissingDepency;
                return false;
            }

            return true;
        }

        private PairedList<DataType, ErrorTypes> ClearErrorsCleanTree()
        {
            var errors = new PairedList<DataType, ErrorTypes>();
            var cleanList = new List<SortedTreeNode<IdType, DataType>>();

            foreach (SortedTreeNode<IdType, DataType> entity in SortedElements.Values)
            {
                if (entity.HasError || !AllDependenciesArePresent(entity))
                {
                    entity.ClearLinks();
                    errors.Add(entity.Data, entity.Error);
                }
                else
                {
                    cleanList.Add(entity);
                }
            }

            SortedElements.Clear();
            KnownKeys.Clear();
            Root = null;
            NodesInError = 0;

            foreach (SortedTreeNode<IdType, DataType> entity in cleanList)
            {
                Add(entity.Data);
            }

            return errors;
        }
    }
}
