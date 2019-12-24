namespace QModManager.Patching
{
    internal enum ModLoadingResults
    {
        Success,
        NoMethodToExecute,
        Failure,
        AlreadyLoaded,
        CurrentGameNotSupported,
        CancledByModAuthor
    }
}
