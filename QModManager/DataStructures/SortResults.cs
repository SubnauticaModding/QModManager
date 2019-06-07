namespace QModManager.DataStructures
{
    internal enum SortResults : int
    {
        NoSortPreference, // Okay
        SortBefore, // Okay
        SortAfter, // Okay
        CircularLoadOrder, // Error
        CircularDependency, // Error
        DuplicateId, // Error        
    }
}
