namespace QModManager.DataStructures
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Requirements to function within the <seealso cref="SortedCollection{IdType, DataType}"/>
    /// </summary>
    /// <typeparam name="IdType">The ID type.</typeparam>
    internal interface ISortable<IdType>
        where IdType : IEquatable<IdType>, IComparable<IdType>
    {
        IdType Id { get; }

        IList<IdType> RequiredDependencies { get; }
        IList<IdType> LoadBeforePreferences { get; }
        IList<IdType> LoadAfterPreferences { get; }
    }
}
