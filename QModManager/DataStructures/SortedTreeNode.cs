namespace QModManager.DataStructures
{
    using System;
    using System.Collections.Generic;

    internal class SortedTreeNode<IdType, DataType>
        where IdType : IEquatable<IdType>, IComparable<IdType>
        where DataType : ISortable<IdType>
    {
        public SortedTreeNode(IdType id, DataType data, SortedTree<IdType, DataType> tree)
        {
            Id = id;
            Data = data;
            Tree = tree;
        }

        public readonly IdType Id;

        public readonly DataType Data;

        public readonly SortedTree<IdType, DataType> Tree;

        public ErrorTypes Error { get; set; } = ErrorTypes.None;

        public bool HasError => this.Error != ErrorTypes.None;

        public ICollection<IdType> Dependencies => Data.DependencyCollection;

        public ICollection<IdType> LoadBeforeRequirements => Data.LoadBeforeCollection;

        public ICollection<IdType> LoadAfterRequirements => Data.LoadAfterCollection;

        internal bool HasOrdering => this.Dependencies.Count > 0 || this.LoadBeforeRequirements.Count > 0 || this.LoadAfterRequirements.Count > 0;

        internal bool HasDependencies => this.Dependencies.Count > 0;

        public int NodesAddedBefore { get; private set; }
        public int NodesAddedAfter { get; private set; }

        public SortedTreeNode<IdType, DataType> Parent { get; protected set; }

        public SortedTreeNode<IdType, DataType> LoadBefore { get; protected set; }
        public SortedTreeNode<IdType, DataType> LoadAfter { get; protected set; }

        public void ClearLinks()
        {
            this.Parent = null;
            this.LoadBefore = null;
            this.LoadAfter = null;
        }

        internal bool RequiresBefore(IdType other)
        {
            return this.LoadBeforeRequirements.Contains(other);
        }

        internal bool RequiresAfter(IdType other)
        {
            return this.LoadAfterRequirements.Contains(other);
        }

        internal bool DependsOn(IdType other)
        {
            return this.Dependencies.Contains(other);
        }

        internal static SortResults CompareLoadOrder(SortedTreeNode<IdType, DataType> entity, SortedTreeNode<IdType, DataType> other)
        {
            if (entity.HasError || other.HasError)
            {
                return SortResults.NoSortPreference;
            }

            if (entity.Id.Equals(other.Id))
            {
                entity.Error = ErrorTypes.DuplicateId;
                other.Error = ErrorTypes.DuplicateId;
                return SortResults.DuplicateId;
            }

            bool entityIsDependentOnOther = entity.DependsOn(other.Id);
            bool otherIsDependentOnEntity = other.DependsOn(entity.Id);

            if (entityIsDependentOnOther && otherIsDependentOnEntity)
            {
                entity.Error = ErrorTypes.CircularDependency;
                other.Error = ErrorTypes.CircularDependency;
                return SortResults.CircularDependency;
            }

            if (entityIsDependentOnOther)
            {
                return SortResults.SortAfter;
            }

            if (otherIsDependentOnEntity)
            {
                return SortResults.SortBefore;
            }

            if ((entity.RequiresBefore(other.Id) && other.RequiresBefore(entity.Id)) ||
                (entity.RequiresAfter(other.Id) && other.RequiresAfter(entity.Id)))
            {
                entity.Error = ErrorTypes.CircularLoadOrder;
                other.Error = ErrorTypes.CircularLoadOrder;
                return SortResults.CircularLoadOrder;
            }

            if (entity.RequiresBefore(other.Id) || other.RequiresAfter(entity.Id))
            {
                return NextLevelCompareBefore(entity, other);
            }

            if (entity.RequiresAfter(other.Id) || other.RequiresBefore(entity.Id))
            {
                return NextLevelCompareAfter(entity, other);
            }

            SortResults subResultB = SortResults.NoSortPreference;
            SortResults subResultA = SortResults.NoSortPreference;

            if (entity.LoadBefore != null && entity.LoadBefore != other)
            {
                subResultB = CompareLoadOrder(entity.LoadBefore, other);
            }

            if (entity.LoadAfter != null && entity.LoadAfter != other)
            {
                subResultA = CompareLoadOrder(entity.LoadAfter, other);
            }

            SortResults splitCheckResult = subResultA + (int)subResultB;

            if (splitCheckResult <= SortResults.SortAfter)
                return splitCheckResult;

            return SortResults.CircularLoadOrder;
        }

        private static SortResults NextLevelCompareAfter(SortedTreeNode<IdType, DataType> entity, SortedTreeNode<IdType, DataType> other)
        {
            if (entity.LoadBefore != null && entity.LoadBefore != other)
            {
                SortResults subResult = CompareLoadOrder(entity.LoadBefore, other);

                switch (subResult)
                {
                    case SortResults.SortBefore:
                        other.Error = ErrorTypes.CircularLoadOrder;
                        entity.ChainInCircularLoadOrder();
                        return SortResults.CircularLoadOrder;
                    case SortResults.NoSortPreference:
                        return SortResults.SortAfter;
                    default:
                        return subResult;
                }
            }

            return SortResults.SortAfter;
        }

        private static SortResults NextLevelCompareBefore(SortedTreeNode<IdType, DataType> entity, SortedTreeNode<IdType, DataType> other)
        {
            if (entity.LoadAfter != null && entity.LoadAfter != other)
            {
                SortResults subResult = CompareLoadOrder(entity.LoadAfter, other);

                switch (subResult)
                {
                    case SortResults.SortAfter:
                        other.Error = ErrorTypes.CircularLoadOrder;
                        entity.ChainInCircularLoadOrder();
                        return SortResults.CircularLoadOrder;
                    case SortResults.NoSortPreference:
                        return SortResults.SortBefore;
                    default:
                        return subResult;
                }
            }

            return SortResults.SortBefore;
        }

        public SortResults Sort(SortedTreeNode<IdType, DataType> other, bool testing = false)
        {
            if (this == other)
            {
                return SortResults.NoSortPreference;
            }

            SortResults topLevelResult = CompareLoadOrder(this, other);

            SortResults midLevelResult = SortResults.NoSortPreference;
            switch (topLevelResult)
            {
                case SortResults.DuplicateId:
                case SortResults.CircularDependency:
                case SortResults.CircularLoadOrder:
                    return topLevelResult;
                case SortResults.NoSortPreference:

                    const bool TestSort = true;
                    if (this.LoadBefore != null && this.LoadAfter != null)
                    {
                        SortResults testAfterResult = SortAfter(other, TestSort);
                        SortResults testBeforeResult = SortBefore(other, TestSort);

                        if (testAfterResult > SortResults.NoSortPreference)
                        {
                            return testAfterResult;
                        }

                        if (testBeforeResult > SortResults.NoSortPreference)
                        {
                            return testBeforeResult;
                        }

                        midLevelResult = testAfterResult > testBeforeResult
                            ? testAfterResult
                            : testBeforeResult;
                    }
                    else if (this.LoadBefore == null && this.LoadAfter != null)
                    {
                        SortResults testAfterResult = SortAfter(other, TestSort);

                        if (testAfterResult > SortResults.NoSortPreference)
                        {
                            return testAfterResult;
                        }

                        midLevelResult = testAfterResult;
                    }
                    else if (this.LoadAfter == null && this.LoadBefore != null)
                    {
                        SortResults testBeforeResult = SortBefore(other, TestSort);

                        if (testBeforeResult > SortResults.NoSortPreference)
                        {
                            return testBeforeResult;
                        }

                        midLevelResult = testBeforeResult;
                    }

                    if (midLevelResult == SortResults.NoSortPreference)
                    {
                        midLevelResult = SortAfter(other, testing);
                    }

                    break;
                case SortResults.SortBefore:
                    midLevelResult = SortBefore(other, testing);
                    break;
                case SortResults.SortAfter:
                    midLevelResult = SortAfter(other, testing);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!testing)
            {
                switch (midLevelResult)
                {
                    case SortResults.SortBefore:
                        this.NodesAddedBefore++;
                        break;
                    case SortResults.SortAfter:
                        this.NodesAddedAfter++;
                        break;
                }
            }

            return midLevelResult;
        }

        public SortResults SortAfter(SortedTreeNode<IdType, DataType> other, bool testing)
        {
            if (this.LoadAfter == null)
            {
                if (!testing)
                {
                    this.LoadAfter = other;
                    other.Parent = this;
                }

                return SortResults.SortAfter;
            }

            return this.LoadAfter.Sort(other);
        }

        public SortResults SortBefore(SortedTreeNode<IdType, DataType> other, bool testing)
        {
            if (this.LoadBefore == null)
            {
                if (!testing)
                {
                    this.LoadBefore = other;
                    other.Parent = this;
                }

                return SortResults.SortBefore;
            }

            return this.LoadBefore.Sort(other);
        }

        protected void ChainInCircularLoadOrder()
        {
            if (this.HasOrdering)
            {
                this.Error = ErrorTypes.CircularLoadOrder;
            }

            this.LoadBefore?.ChainInCircularLoadOrder();
            this.LoadAfter?.ChainInCircularLoadOrder();
        }
    }
}
