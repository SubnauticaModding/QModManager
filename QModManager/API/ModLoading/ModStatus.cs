namespace QModManager.API.ModLoading
{
    internal enum ModStatus
    {
        Success,
        CanceledByAuthor,
        PatchMethodFailed,
        TooManyPatchMethods,
        MissingPatchMethod,
        CircularLoadOrder,
        CircularDependency,
        MissingDependency,
        CurrentGameNotSupported,
        FailedIdentifyingGame,
        DuplicateIdDetected,
        DuplicatePatchAttemptDetected,
        MissingCoreData,
        InvalidCoreData,
        MissingAssemblyFile,
        FailedLoadingAssemblyFile,
        UnidentifiedMod,
    }
}
