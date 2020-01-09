// Original mod by RandyKnapp
namespace QModManager.HarmonyPatches.QuitToDesktop
{
    using Harmony;
    using UnityEngine;
    using UnityEngine.UI;

    internal static class QTD
    {
        internal static Button quitButton;
        internal static GameObject quitConfirmation;
        internal static GameObject quitConfirmation2;

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

    [HarmonyPatch(typeof(IngameMenu), nameof(IngameMenu.Start))]
    internal static class IngameMenu_Start_Patch
    {
        // This patch creates the UI elements for the Quit to Desktop button

        [HarmonyPostfix]
        internal static void Postfix(IngameMenu __instance)
        {
            var buttonPrefab = __instance.quitToMainMenuButton.GetComponent<Button>();
            QTD.quitButton = GameObject.Instantiate(buttonPrefab, __instance.quitToMainMenuButton.transform.parent);
            QTD.quitButton.name = "QuitToDesktop Button";
            QTD.quitButton.onClick.RemoveAllListeners();
            QTD.quitButton.onClick.AddListener(() => QTD.QuitDesktopSubscreen(__instance));

            var confirmationPrefab = __instance.transform.Find("QuitConfirmation").gameObject;
            QTD.quitConfirmation = GameObject.Instantiate(confirmationPrefab, __instance.transform);
            QTD.quitConfirmation.name = "QuitToDesktop Confirmation";
            QTD.quitConfirmation.GetComponentsInChildren<Button>()[1].onClick.RemoveAllListeners();
            QTD.quitConfirmation.GetComponentsInChildren<Button>()[1].onClick.AddListener(() => __instance.QuitGame(true));

            var confirmationWithSaveWarningPrefab = __instance.transform.Find("QuitConfirmationWithSaveWarning").gameObject;
            QTD.quitConfirmation2 = GameObject.Instantiate(confirmationWithSaveWarningPrefab, __instance.transform);
            QTD.quitConfirmation2.name = "QuitToDesktop ConfirmationWithSaveWarning";
            QTD.quitConfirmation2.GetComponentsInChildren<Button>()[1].onClick.RemoveAllListeners();
            QTD.quitConfirmation2.GetComponentsInChildren<Button>()[1].onClick.AddListener(() => __instance.QuitGame(true));
        }
    }

    [HarmonyPatch(typeof(IngameMenu), nameof(IngameMenu.Open))]
    internal static class IngameMenu_Open_Patch
    {
        // This patch disables the QTD button in hardcore mode and changes the names of the two quit buttons based on game mode

        [HarmonyPostfix]
        internal static void Postfix(IngameMenu __instance)
        {
            __instance.quitToMainMenuText.text = $"{(GameModeUtils.IsPermadeath() ? "Save & " : "")}Quit to Main Menu";
            QTD.quitButton.interactable = !GameModeUtils.IsPermadeath();
            QTD.quitButton.GetComponentsInChildren<Text>().Do(t => t.text = "Quit to Desktop");
        }
    }

    /*
    [HarmonyPatch(typeof(IngameMenu), nameof(IngameMenu.QuitGame))]
    internal static class IngameMenu_QuitGame
    {
        // This is a pass-through patch

        [HarmonyPrefix]
        internal static bool Prefix(IngameMenu __instance, bool quitToDesktop)
        {
            __instance.StartCoroutine(IngameMenu_QuitGameAsync.Passthrough(__instance.QuitGameAsync(quitToDesktop), quitToDesktop));
            return false;
        }
    }

    internal static class IngameMenu_QuitGameAsync
    {
        // This patch makes QTD work in hardcore mode

        internal static IEnumerator Passthrough(IEnumerator original, bool quitToDesktop)
        {
            int n = 0;
            while (original.MoveNext())
            {
                yield return original.Current;

                if (quitToDesktop && original.Current is IEnumerator)
                    yield return new WaitForSecondsRealtime(2);

                n++;
            }
        }
    }
    */
}
