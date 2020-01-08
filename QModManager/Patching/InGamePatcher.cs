namespace QModManager.Patching
{
    using Harmony;
    using UnityEngine;
    using UnityEngine.UI;

    internal static class InGamePatcher
    {
        [HarmonyPatch(typeof(IngameMenu), nameof(IngameMenu.Open))]
        internal static class IngameMenu_Open_Patch
        {
            internal static Button QuitButton;
            internal static GameObject QuitConfirmation;
            internal static GameObject QuitConfirmationWithSaveWarning;

            [HarmonyPostfix]
            internal static void Postfix(IngameMenu __instance)
            {
                __instance.feedbackButton.interactable = false;
                Transform transform = __instance.transform.Find("Main/ButtonLayout/ButtonFeedback");
                if (transform != null) transform.gameObject.GetComponent<Button>().interactable = false;

                __instance.quitToMainMenuText.text = "Quit to Main Menu";
                if (QuitButton == null)
                {
                    var buttonPrefab = __instance.quitToMainMenuButton.GetComponent<Button>();
                    QuitButton = GameObject.Instantiate(buttonPrefab, __instance.quitToMainMenuButton.transform.parent);
                    QuitButton.name = "QuitToDesktop Button";
                    QuitButton.onClick.RemoveAllListeners();
                    QuitButton.onClick.AddListener(() => QuitDesktopSubscreen(__instance));
                    QuitButton.GetComponentsInChildren<Text>().Do(t => t.text = "Quit to Desktop");

                    var confirmationPrefab = __instance.transform.Find("QuitConfirmation").gameObject;
                    QuitConfirmation = GameObject.Instantiate(confirmationPrefab, __instance.transform);
                    QuitConfirmation.name = "QuitToDesktop Confirmation";
                    QuitConfirmation.GetComponentsInChildren<Button>()[1].onClick.RemoveAllListeners();
                    QuitConfirmation.GetComponentsInChildren<Button>()[1].onClick.AddListener(() => __instance.QuitGame(true));

                    var confirmationWithSaveWarningPrefab = __instance.transform.Find("QuitConfirmationWithSaveWarning").gameObject;
                    QuitConfirmationWithSaveWarning = GameObject.Instantiate(confirmationWithSaveWarningPrefab, __instance.transform);
                    QuitConfirmationWithSaveWarning.name = "QuitToDesktop ConfirmationWithSaveWarning";
                    QuitConfirmationWithSaveWarning.GetComponentsInChildren<Button>()[1].onClick.RemoveAllListeners();
                    QuitConfirmationWithSaveWarning.GetComponentsInChildren<Button>()[1].onClick.AddListener(() => __instance.QuitGame(true));
                }
            }

            internal static void QuitDesktopSubscreen(IngameMenu __instance)
            {
                float time = Time.timeSinceLevelLoad - __instance.lastSavedStateTime;
                if (!GameModeUtils.IsPermadeath() && time > __instance.maxSecondsToBeRecentlySaved)
                {
                    QuitConfirmationWithSaveWarning.GetComponent<Text>().text = Language.main.GetFormat("TimeSinceLastSave", Utils.PrettifyTime((int)time));
                    __instance.ChangeSubscreen("QuitToDesktop ConfirmationWithSaveWarning");
                    return;
                }
                __instance.ChangeSubscreen("QuitToDesktop Confirmation");
            }
        }

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
