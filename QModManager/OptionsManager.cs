using Harmony;
using QModManager.Utility;
using UnityEngine.Events;

namespace QModManager
{
    internal static class OptionsManager
    {
        internal static bool DebuggerEnabled { get => PlayerPrefsExtra.GetBool("QModManager_PrefabDebugger_EnableExperimental", false); }

        internal static int ModsTab;

        [HarmonyPatch(typeof(uGUI_OptionsPanel), "AddTabs")]
        internal static class OptionsPatch
        {
            [HarmonyPostfix]
            internal static void Postfix(uGUI_OptionsPanel __instance)
            {
                ModsTab = __instance.AddTab("Mods");
                __instance.AddHeading(ModsTab, "QModManager");

                bool enableDebugLogs = PlayerPrefsExtra.GetBool("QModManager_EnableDebugLogs", false);
                __instance.AddToggleOption(ModsTab, "Enable debug logs", enableDebugLogs,
                    new UnityAction<bool>(toggleVal => PlayerPrefsExtra.SetBool("QModManager_EnableDebugLogs", toggleVal)));

                bool updateCheck = PlayerPrefsExtra.GetBool("QModManager_EnableUpdateCheck", true);
                __instance.AddToggleOption(ModsTab, "Check for updates", updateCheck,
                    new UnityAction<bool>(toggleVal => PlayerPrefsExtra.SetBool("QModManager_EnableUpdateCheck", toggleVal)));

                /*
                bool enableDebugger = PlayerPrefsExtra.GetBool("QModManager_PrefabDebugger_EnableExperimental", false);
                __instance.AddToggleOption(ModsTab, "Enable prefab debugger (experimental)", enableDebugger,
                    new UnityAction<bool>(toggleVal => PlayerPrefsExtra.SetBool("QModManager_PrefabDebugger_EnableExperimental", toggleVal)));

                bool enableDebugger = PlayerPrefsExtra.GetBool("QModManager_PrefabDebugger_Enable", true);
                __instance.AddToggleOption(modsTab, "Enable prefab debugger", enableDebugger,
                    new UnityAction<bool>(toggleVal => PlayerPrefsExtra.SetBool("QModManager_PrefabDebugger_Enable", toggleVal)));
                */
            }
        }
    }
}
