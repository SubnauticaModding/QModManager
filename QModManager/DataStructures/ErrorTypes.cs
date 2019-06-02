namespace QModManager.DataStructures
{
    internal enum ErrorTypes
    {
        None,
        DuplicateId,
        CircularDependency,
        CircularLoadOrder,
        MissingDepency,
        MetaOrderConflict,
    }
}
