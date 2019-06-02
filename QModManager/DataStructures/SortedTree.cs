namespace QModManager.DataStructures
{
    using System;
    using System.Collections.Generic;

    internal class SortedTree<IdType, DataType>
        where IdType : IEquatable<IdType>, IComparable<IdType>
        where DataType : ISortable<IdType>
    {
        private SortedTreeNode<IdType, DataType> Root;
        private readonly IDictionary<IdType, SortedTreeNode<IdType, DataType>> SortedElements;
        private readonly ICollection<IdType> KnownKeys = new HashSet<IdType>();
        private List<IdType> erroredIds = new List<IdType>();
        
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
            if (IsDuplicateId(data.Id))
            {
                return SortResults.DuplicateId;
            }

            KnownKeys.Add(data.Id);

            var entity = new SortedTreeNode<IdType, DataType>(data.Id, data, this);

            if (Root == null)
            {
                Root = entity;
                SortedElements.Add(entity.Id, entity);
                return SortResults.SortAfter;
            }

            SortResults sortResult = Root.Sort(entity);

            switch (sortResult)
            {
                case SortResults.SortBefore:
                case SortResults.SortAfter:
                    SortedElements.Add(entity.Id, entity);
                    break;
                default:
                    NodesInError++;
                    break;
            }

            return sortResult;
        }

        public List<IdType> GetErrors()
        {
            return erroredIds;
        }

        public List<IdType> CreateFlatIndexList()
        {
            var list = new List<IdType>(this.NodeCount);

            erroredIds = ClearErrorsCleanTree();
            CreateFlatIndexList(Root, list);

            return list;
        }

        private static void CreateFlatIndexList(SortedTreeNode<IdType, DataType> node, ICollection<IdType> list)
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

        public List<DataType> CreateFlatValueList()
        {
            var list = new List<DataType>(this.NodeCount);

            erroredIds = ClearErrorsCleanTree();
            CreateFlatValueList(Root, list);

            return list;
        }

        private static void CreateFlatValueList(SortedTreeNode<IdType, DataType> node, ICollection<DataType> list)
        {
            if (node is null)
            {
                return;
            }

            while (true)
            {
                if (node.LoadBefore != null)
                {
                    CreateFlatValueList(node.LoadBefore, list);
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

        private bool IsDuplicateId(IdType id)
        {
            if (KnownKeys.Contains(id))
            {
                if (SortedElements.TryGetValue(id, out SortedTreeNode<IdType, DataType> dup))
                {
                    dup.Error = ErrorTypes.DuplicateId;
                    SortedElements.Remove(id);

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

            bool dependenciesPresent = false;

            foreach (IdType nodeDependency in node.Dependencies)
            {
                if (SortedElements.ContainsKey(nodeDependency))
                {
                    dependenciesPresent = true;
                }
            }

            if (!dependenciesPresent)
            {
                node.Error = ErrorTypes.MissingDepency;
            }

            return dependenciesPresent;
        }

        private List<IdType> ClearErrorsCleanTree()
        {
            var errors = new List<IdType>(erroredIds);
            var cleanList = new List<SortedTreeNode<IdType, DataType>>();

            foreach (SortedTreeNode<IdType, DataType> entity in SortedElements.Values)
            {
                if (entity.HasError || !AllDependenciesArePresent(entity))
                {
                    continue;
                }

                entity.ClearLinks();
                cleanList.Add(entity);
                errors.Add(entity.Id);
                KnownKeys.Remove(entity.Id);
            }

            SortedElements.Clear();
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
