using Harmony;
using QModManager.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace QModManager
{
    internal static class OptionsManager
    {
        [HarmonyPatch(typeof(uGUI_OptionsPanel), "AddTabs")]
        private static class OptionsPatch
        {
            [HarmonyPostfix]
            private static void Postfix(uGUI_OptionsPanel __instance)
            {
                int modsTab = __instance.AddTab("Mods");
                __instance.AddHeading(modsTab, "QModManager");

                bool enableDebugLogs = PlayerPrefsExtra.GetBool("QModManager_EnableDebugLogs", false);
                __instance.AddToggleOption(modsTab, "Enable debug logs", enableDebugLogs,
                    new UnityAction<bool>(toggleVal => PlayerPrefsExtra.SetBool("QModManager_EnableDebugLogs", toggleVal)));

                bool updateCheck = PlayerPrefsExtra.GetBool("QModManager_EnableUpdateCheck", true);
                __instance.AddToggleOption(modsTab, "Check for updates", updateCheck,
                    new UnityAction<bool>(toggleVal => PlayerPrefsExtra.SetBool("QModManager_EnableUpdateCheck", toggleVal)));

                bool enableDebugger = PlayerPrefsExtra.GetBool("QModManager_PrefabDebugger_Enable", true);
                __instance.AddToggleOption(modsTab, "Enable prefab debugger", enableDebugger,
                    new UnityAction<bool>(toggleVal => PlayerPrefsExtra.SetBool("QModManager_PrefabDebugger_Enable", toggleVal)));
            }
        }
    }
}
