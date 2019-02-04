using Harmony;
using UnityEngine;
using UnityEngine.Events;

namespace QModManager
{
    internal static class Options
    {
        [HarmonyPatch(typeof(uGUI_OptionsPanel), "AddTabs")]
        private static class OptionsPatch
        {
            [HarmonyPostfix]
            private static void Postfix(uGUI_OptionsPanel __instance)
            {
                bool updateCheck = PlayerPrefs.GetInt("QModManager_EnableUpdateCheck", 1) == 0 ? false : true;
                bool enableDebugger = PlayerPrefs.GetInt("QModManager_PrefabDebugger_Enable", 0) == 0 ? false : true;
                int modsTab = __instance.AddTab("Mods");
                __instance.AddHeading(modsTab, "QModManager");
                __instance.AddToggleOption(modsTab, "Check for updates", updateCheck,
                    new UnityAction<bool>((bool toggleVal) => PlayerPrefs.SetInt("QModManager_EnableUpdateCheck", toggleVal ? 1 : 0)));
                __instance.AddToggleOption(modsTab, "Enable prefab debugger", enableDebugger,
                    new UnityAction<bool>((bool toggleVal) => PlayerPrefs.SetInt("QModManager_PrefabDebugger_Enable", toggleVal ? 1 : 0)));
            }
        }
    }
}
