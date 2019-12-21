namespace QModManager.Examples
{
    // Include this using
    using QModManager.API.ModLoading;

    [QModCore]
    public static class ExampleMod
    {
        [QModPatch]
        public static void Patch()
        {
            // Perform standard patching here
        }
    }
}
