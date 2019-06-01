namespace QModManager.Examples
{
    using QModManager.API.ModLoading;

    [QModCoreInfo(id: "QExampleMod", 
                  displayName: "Example Mod", 
                  author: "QModManager Dev Team",
                  developedFor: DevelopedFor.Subnautica,
                  patchMethod: nameof(PatchMyMod))]
    [QModLoadBefore("SomeMod")]
    [QModLoadBefore("SomeMod2")]
    [QModLoadAfter("SomeOtherMod")]
    [QModDependency("ApiMod")]
    [QModDependency("OtherAPIMod", "2.2.1")]
    internal static class PatchExample
    {
        public static void PatchMyMod()
        {
            // Mod patching happens here
        }
    }
}
