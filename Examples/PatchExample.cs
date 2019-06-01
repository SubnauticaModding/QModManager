namespace QModManager.Examples
{
    using QModManager.API.ModLoading;

    [QModCoreInfo("QExampleMod", "Example Mod", "QModManager Dev Team", API.Game.Subnautica)]
    [QModLoadBefore("SomeMod")]
    [QModLoadBefore("SomeMod2")]
    [QModLoadAfter("SomeOtherMod")]
    [QModDependency("ApiMod")]
    [QModDependency("OtherAPIMod", 2, 2, 1)]
    internal static class PatchExample
    {
        [QModPatchMethod]
        public static void PatchMyMod()
        {
            // Mod patching happens here
        }
    }
}
