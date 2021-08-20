namespace QModManager
{
    using HarmonyLib;
    using QModManager.Utility;
    using UnityEngine.Events;

    internal static class OptionsManager
    {
        internal static int ModsTab;

        [HarmonyPatch(typeof(uGUI_OptionsPanel), nameof(uGUI_OptionsPanel.AddTabs))]
        internal static class OptionsPatch
        {
            [HarmonyPostfix]
            internal static void Postfix(uGUI_OptionsPanel __instance)
            {
                ModsTab = __instance.AddTab("Mods");
                __instance.AddHeading(ModsTab, "QModManager");

                __instance.AddToggleOption(ModsTab, "Check for updates", Config.CheckForUpdates, new UnityAction<bool>(value => Config.CheckForUpdates = value));

                __instance.AddToggleOption(ModsTab, "Enable console", Config.EnableConsole, new UnityAction<bool>(value =>
                {
                    Config.EnableConsole = value;
#if SUBNAUTICA
                    DevConsole.disableConsole = !value;
                    UnityEngine.PlayerPrefs.SetInt("UWE.DisableConsole", value ? 0 : 1);
#else
                    PlatformUtils.SetDevToolsEnabled(value);
#endif
                }));

                __instance.AddToggleOption(ModsTab, "Enable debug logs", Config.EnableDebugLogs, new UnityAction<bool>(value => Config.EnableDebugLogs = value));

                __instance.AddToggleOption(ModsTab, "Enable developer mode", Config.EnableDevMode, new UnityAction<bool>(value => Config.EnableDevMode = value));
            }
        }
    }
}
