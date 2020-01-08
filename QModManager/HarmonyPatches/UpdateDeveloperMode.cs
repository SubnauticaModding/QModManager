namespace QModManager.HarmonyPatches.UpdateDeveloperMode
{
    using Harmony;

    [HarmonyPatch(typeof(IngameMenu), nameof(IngameMenu.Update))]
    internal static class IngameMenu_Update
    {
        [HarmonyPostfix]
        internal static void Postfix(IngameMenu __instance)
        {
            if (__instance.developerMode != OptionsManager.DevMode)
            {
                IngameMenu.main.developerMode = OptionsManager.DevMode;
                IngameMenu.main.developerButton.gameObject.SetActive(OptionsManager.DevMode);
            }
        }
    }
}
