namespace QModManager.Patching
{
    internal enum ModStatus
    {
        Obsolete = -4,
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
        MissingManifest = 12,
        ManifestParsingError = 13,
        InvalidCoreInfo = 14,
        MissingAssemblyFile = 15,
        FailedLoadingAssemblyFile = 16,
        UnidentifiedMod = 17,
        BannedID = 18,
    }
}
