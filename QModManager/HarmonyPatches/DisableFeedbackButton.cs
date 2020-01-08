namespace QModManager.HarmonyPatches.DisableFeedbackButton
{
    using Harmony;
    using UnityEngine;
    using UnityEngine.UI;

    [HarmonyPatch(typeof(IngameMenu), nameof(IngameMenu.Start))]
    internal static class IngameMenu_Start
    {
        [HarmonyPostfix]
        internal static void Postfix(IngameMenu __instance)
        {
            __instance.feedbackButton.interactable = false;
            Transform transform = __instance.transform.Find("Main/ButtonLayout/ButtonFeedback");
            if (transform != null) transform.GetComponent<Button>().interactable = false;
        }
    }

    [HarmonyPatch(typeof(uGUI_FeedbackCollector), nameof(uGUI_FeedbackCollector.Update))]
    internal static class uGUI_FeedbackCollector_Update
    {
        [HarmonyPostfix]
        internal static void Postfix(uGUI_FeedbackCollector __instance)
        {
            if (__instance.repliesPanel.activeSelf) __instance.repliesPanel.SetActive(false);
            if (__instance.root.activeSelf) __instance.root.SetActive(false);
            if (__instance.hint.gameObject.activeSelf) __instance.hint.gameObject.SetActive(false);
        }
    }
}
