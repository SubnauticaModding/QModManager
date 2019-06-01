namespace QModManager.Examples
{
    using QModManager.API.ModLoading;

    [QModCoreInfo("QExampleMod", "Example Mod", "QModManager Dev Team", API.QModGame.Subnautica)]
    [QModLoadBefore("SomeMod")]
    [QModLoadBefore("SomeMod2")]
    [QModLoadAfter("SomeOtherMod")]
    [QModDependency("ApiMod")]
    [QModDependency("OtherAPIMod", 2, 2, 1)]
    internal static class PatchExample
    {
        [QModPrePatchMethod]
        public static PatchResults PatchMyModEarly()
        {
            // Early mod patching happens here
            return PatchResults.OK;
        }

        [QModPatchMethod]
        public static void PatchMyMod()
        {
            // Mod patching happens here
        }

        [QModPostPatchMethod]
        public static void PatchMyModLate()
        {
            // Late mod patching happens here
        }
    }
}
