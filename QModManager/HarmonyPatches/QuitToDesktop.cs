namespace QModManager.HarmonyPatches.QuitToDesktop
{
    using Harmony;
    using UnityEngine;
    using UnityEngine.UI;

    [HarmonyPatch(typeof(IngameMenu), nameof(IngameMenu.Start))]
    internal static class IngameMenu_Open_Patch
    {
        internal static GameObject quitConfirmation2;

        [HarmonyPostfix]
        internal static void Postfix(IngameMenu __instance)
        {
            __instance.quitToMainMenuText.text = "Quit to Main Menu";

            var buttonPrefab = __instance.quitToMainMenuButton.GetComponent<Button>();
            var quitButton = GameObject.Instantiate(buttonPrefab, __instance.quitToMainMenuButton.transform.parent);
            quitButton.name = "QuitToDesktop Button";
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(() => QuitDesktopSubscreen(__instance));
            quitButton.GetComponentsInChildren<Text>().Do(t => t.text = "Quit to Desktop");

            var confirmationPrefab = __instance.transform.Find("QuitConfirmation").gameObject;
            var quitConfirmation = GameObject.Instantiate(confirmationPrefab, __instance.transform);
            quitConfirmation.name = "QuitToDesktop Confirmation";
            quitConfirmation.GetComponentsInChildren<Button>()[1].onClick.RemoveAllListeners();
            quitConfirmation.GetComponentsInChildren<Button>()[1].onClick.AddListener(() => __instance.QuitGame(true));

            var confirmationWithSaveWarningPrefab = __instance.transform.Find("QuitConfirmationWithSaveWarning").gameObject;
            quitConfirmation2 = GameObject.Instantiate(confirmationWithSaveWarningPrefab, __instance.transform);
            quitConfirmation2.name = "QuitToDesktop ConfirmationWithSaveWarning";
            quitConfirmation2.GetComponentsInChildren<Button>()[1].onClick.RemoveAllListeners();
            quitConfirmation2.GetComponentsInChildren<Button>()[1].onClick.AddListener(() => __instance.QuitGame(true));
        }

        internal static void QuitDesktopSubscreen(IngameMenu __instance)
        {
            float time = Time.timeSinceLevelLoad - __instance.lastSavedStateTime;
            if (!GameModeUtils.IsPermadeath() && time > __instance.maxSecondsToBeRecentlySaved)
            {
                quitConfirmation2.GetComponentsInChildren<Text>()[1].text = Language.main.GetFormat("TimeSinceLastSave", Utils.PrettifyTime((int)time));
                __instance.ChangeSubscreen("QuitToDesktop ConfirmationWithSaveWarning");
                return;
            }
            __instance.ChangeSubscreen("QuitToDesktop Confirmation");
        }
    }
}
