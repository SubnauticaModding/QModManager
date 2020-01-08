namespace QModManager.Patching
{
    using Harmony;
    using UnityEngine;
    using UnityEngine.UI;

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
                __instance.feedbackButton.interactable = false;
                Transform transform = __instance.transform.Find("Main/ButtonLayout/ButtonFeedback");
                if (transform != null) transform.gameObject.GetComponent<Button>().interactable = false;
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

        [HarmonyPatch(typeof(uGUI_FeedbackCollector), nameof(uGUI_FeedbackCollector.Update))]
        internal static class uGUI_FeedbackCollector_Update_Patch
        {
            [HarmonyPostfix]
            internal static void Postfix(uGUI_FeedbackCollector __instance)
            {
                if (__instance.repliesPanel.activeSelf) __instance.repliesPanel.SetActive(false);
                if (__instance.root.activeSelf) __instance.root.SetActive(false);
            }
        }
    }
}
