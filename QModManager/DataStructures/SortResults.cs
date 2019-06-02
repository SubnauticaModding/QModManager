namespace QModManager.DataStructures
{
    internal enum SortResults : int
    {
        NoSortPreference = 0, // Okay
        SortBefore = 1, // Okay
        SortAfter = 2, // Okay
        CircularLoadOrder = 3, // Error
        CircularDependency = 6, // Error
        DuplicateId = 12, // Error        
    }
}
