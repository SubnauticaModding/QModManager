namespace QModManager.Patching
{
    using Harmony;
    using UnityEngine;

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

        [HarmonyPatch(typeof(SentrySdk), nameof(SentrySdk.Start))]
        internal static class SentrySdk_Start_Patch
        {
            [HarmonyPrefix]
            internal static bool Prefix(SentrySdk __instance)
            {
                GameObject.Destroy(__instance);
                return false;
            }
        }
    }
}
