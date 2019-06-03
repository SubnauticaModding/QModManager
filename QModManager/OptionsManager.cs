namespace QModManager
{
    using Harmony;
    using QModManager.API.SMLHelper.Utility;
    using UnityEngine.Events;

    internal static class OptionsManager
    {
        internal static int ModsTab;

        [HarmonyPatch(typeof(uGUI_OptionsPanel), "AddTabs")]
        private static class OptionsPatch
        {
            [HarmonyPostfix]
            private static void Postfix(uGUI_OptionsPanel __instance)
            {
                ModsTab = __instance.AddTab("Mods");
                __instance.AddHeading(ModsTab, "QModManager");

                AddOptions(__instance);
            }
        }

        private static void AddOptions(uGUI_OptionsPanel options)
        {
            bool enableDebugLogs = PlayerPrefsExtra.GetBool("QModManager_EnableDebugLogs", false);
            options.AddToggleOption(ModsTab, "Enable debug logs", enableDebugLogs,
                new UnityAction<bool>(toggleVal => PlayerPrefsExtra.SetBool("QModManager_EnableDebugLogs", toggleVal)));

            bool updateCheck = PlayerPrefsExtra.GetBool("QModManager_EnableUpdateCheck", true);
            options.AddToggleOption(ModsTab, "Check for updates", updateCheck,
                new UnityAction<bool>(toggleVal => PlayerPrefsExtra.SetBool("QModManager_EnableUpdateCheck", toggleVal)));
        }

        /*
        
        internal static bool DebuggerEnabled { get => PlayerPrefsExtra.GetBool("QModManager_PrefabDebugger_EnableExperimental", false); }
        
        private static void AddDebuggerOptions(uGUI_OptionsPanel options)
        {
            bool enableDebugger = PlayerPrefsExtra.GetBool("QModManager_PrefabDebugger_EnableExperimental", false);
            options.AddToggleOption(ModsTab, "Enable prefab debugger (experimental)", enableDebugger,
                new UnityAction<bool>(toggleVal => PlayerPrefsExtra.SetBool("QModManager_PrefabDebugger_EnableExperimental", toggleVal)));

            bool enableDebuggerOld = PlayerPrefsExtra.GetBool("QModManager_PrefabDebugger_Enable", true);
            options.AddToggleOption(ModsTab, "Enable prefab debugger", enableDebuggerOld,
                new UnityAction<bool>(toggleVal => PlayerPrefsExtra.SetBool("QModManager_PrefabDebugger_Enable", toggleVal)));
        }
        
        */
    }
}
