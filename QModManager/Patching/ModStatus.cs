namespace QModManager.Patching
{
    internal enum ModStatus
    {
        Merged = -3,
        //CanceledByAuthor = -2,
        CanceledByUser = -1,
        Success = 0,
        PatchMethodFailed = 1,
        TooManyPatchMethods = 2,
        MissingPatchMethod = 3,
        //CircularLoadOrder = 4,
        //CircularDependency = 5,
        MissingDependency = 6,
        OutOfDateDependency = 7,
        CurrentGameNotSupported = 8,
        FailedIdentifyingGame = 9,
        DuplicateIdDetected = 10,
        DuplicatePatchAttemptDetected = 11,
        MissingCoreInfo = 12,
        InvalidCoreInfo = 13,
        MissingAssemblyFile = 14,
        FailedLoadingAssemblyFile = 15,
        UnidentifiedMod = 16,
        BannedID = 17,
    }
}
