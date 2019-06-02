namespace QModManager.DataStructures
{
    using System;
    using System.Collections.Generic;

    internal interface ISortable<IdType, Priority>
        where IdType : IEquatable<IdType>, IComparable<IdType>
        where Priority : Enum
    {
        IdType Id { get; }

        ICollection<IdType> Dependencies { get; }
        ICollection<IdType> LoadBeforeRequirements { get; }
        ICollection<IdType> LoadAfterRequirements { get; }

        Priority LoadPriority { get; }
    }
}
