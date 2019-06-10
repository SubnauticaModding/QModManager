namespace QModManager.API.ModLoading
{
    internal enum ModStatus
    {
        Success,
        CanceledByAuthor,
        CanceledByUser,
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
        MissingCoreInfo,
        InvalidCoreInfo,
        MissingAssemblyFile,
        FailedLoadingAssemblyFile,
        UnidentifiedMod,
    }
}
