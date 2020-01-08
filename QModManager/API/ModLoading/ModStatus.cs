namespace QModManager.API.ModLoading
{
    internal enum ModStatus
    {
        Success,
        //CanceledByAuthor,
        CanceledByUser,
        PatchMethodFailed,
        TooManyPatchMethods,
        MissingPatchMethod,
        //CircularLoadOrder,
        //CircularDependency,
        MissingDependency,
        OutOfDateDependency,
        CurrentGameNotSupported,
        FailedIdentifyingGame,
        DuplicateIdDetected,
        DuplicatePatchAttemptDetected,
        MissingCoreInfo,
        InvalidCoreInfo,
        MissingAssemblyFile,
        FailedLoadingAssemblyFile,
        UnidentifiedMod,
        BannedID,
        Merged,
    }
}
