namespace QModManager.Patching
{
    internal enum ModStatus
    {
        Merged = -3,
        //CanceledByAuthor = -2,
        CanceledByUser = -1,
        Success = 0,
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
    }
}
