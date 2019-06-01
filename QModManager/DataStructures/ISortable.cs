namespace QModManager.DataStructures
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Requirements to function within the <seealso cref="SortedTree{IdType, DataType}"/>
    /// </summary>
    /// <typeparam name="IdType">The ID type.</typeparam>
    internal interface ISortable<IdType>
        where IdType : IEquatable<IdType>, IComparable<IdType>
    {
        IdType Id { get; }

        ICollection<IdType> DependencyCollection { get; }
        ICollection<IdType> LoadBeforeCollection { get; }
        ICollection<IdType> LoadAfterCollection { get; }
    }
}
