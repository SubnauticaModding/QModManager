namespace QModManager.HarmonyPatches.UpdateDeveloperMode
{
    using Harmony;
    using QModManager.Utility;

    [HarmonyPatch(typeof(IngameMenu), nameof(IngameMenu.Open))]
    internal static class IngameMenu_Open_Patch
    {
        // This patch updates developer mode based on the mod option

        [HarmonyPostfix]
        internal static void Postfix(IngameMenu __instance)
        {
            var devMode = Config.EnableDevMode;

            IngameMenu.main.developerMode = devMode;
            IngameMenu.main.developerButton.gameObject.SetActive(devMode);
        }
    }
}
