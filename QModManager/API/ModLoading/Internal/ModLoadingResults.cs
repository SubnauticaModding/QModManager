namespace QModManager.API.ModLoading.Internal
{
    internal enum ModLoadingResults
    {
        Success,
        NoMethodToExecute,
        Failure,
        AlreadyLoaded,
        CurrentGameNotSupported,        
    }
}
