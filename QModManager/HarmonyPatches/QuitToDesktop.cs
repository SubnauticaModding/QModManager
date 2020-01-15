// Original mod by RandyKnapp: https://github.com/RandyKnapp/SubnauticaModSystem/blob/3b10593994f18e9015aa480d92b393f78fc4922e/SubnauticaModSystem/QuitToDesktop/Patches/IngameMenu_Open_Patch.cs
namespace QModManager.HarmonyPatches.QuitToDesktop
{
    using Harmony;
    using UnityEngine;
    using UnityEngine.UI;

    [HarmonyPatch(typeof(IngameMenu), nameof(IngameMenu.Start))]
    internal static class IngameMenu_Start_Patch
    {
        // This patch creates the UI elements for the Quit to Desktop button

        internal static Button quitButton;
        internal static GameObject quitConfirmation;
        internal static GameObject quitConfirmation2;

        [HarmonyPostfix]
        internal static void Postfix(IngameMenu __instance)
        {
            var buttonPrefab = __instance.quitToMainMenuButton.GetComponent<Button>();
            quitButton = GameObject.Instantiate(buttonPrefab, __instance.quitToMainMenuButton.transform.parent);
            quitButton.name = "QuitToDesktop Button";
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(() => QuitDesktopSubscreen(__instance));

            var confirmationPrefab = __instance.transform.Find("QuitConfirmation").gameObject;
            quitConfirmation = GameObject.Instantiate(confirmationPrefab, __instance.transform);
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
                // We can't use Language.main directly since it's a field in Subnautica and a property in Below Zero
                Language languageMain;
                try
                {
                    // We can't use nameof(Language.main) since it will throw a field not found exception in Below Zero
                    languageMain = AccessTools.Field(typeof(Language), "main").GetValue(null) as Language;
                }
                catch
                {
                    try
                    {
                        languageMain = AccessTools.Property(typeof(Language), "main").GetValue(null, null) as Language;
                    }
                    catch
                    {
                        quitButton.GetComponentsInChildren<Text>().Do(t => t.text = "ERROR");
                        quitButton.interactable = false;
                        return;
                    }
                }

                quitConfirmation2.GetComponentsInChildren<Text>()[1].text = languageMain.GetFormat("TimeSinceLastSave", Utils.PrettifyTime((int)time));
                __instance.ChangeSubscreen("QuitToDesktop ConfirmationWithSaveWarning");
                return;
            }
            __instance.ChangeSubscreen("QuitToDesktop Confirmation");
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
            IngameMenu_Start_Patch.quitButton.interactable = !GameModeUtils.IsPermadeath();
            IngameMenu_Start_Patch.quitButton.GetComponentsInChildren<Text>().Do(t => t.text = "Quit to Desktop");
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
