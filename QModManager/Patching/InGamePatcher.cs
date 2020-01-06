using Harmony;

namespace QModManager.Patching
{
    internal static class InGamePatcher
    {
        [HarmonyPatch(typeof(IngameMenu), nameof(IngameMenu.Update))]
        internal static class IngameMenu_Update_Patch
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
}
