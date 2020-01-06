namespace QModManager
{
    using Harmony;
    using UnityEngine;
    using UnityEngine.Events;
    using System.IO;
    using System;

    internal static class PlayerPrefsExtra
    {
        internal static bool GetBool(string key, bool defaultValue = false)
        {
            return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 0 ? false : true;
        }
        internal static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }
    }

    internal static class OptionsManager
    {
        internal static int ModsTab;

        internal static bool DevMode = false;

        [HarmonyPatch(typeof(uGUI_OptionsPanel), "AddTabs")]
        internal static class OptionsPatch
        {
            [HarmonyPostfix]
            internal static void Postfix(uGUI_OptionsPanel __instance)
            {
                ModsTab = __instance.AddTab("Mods");
                __instance.AddHeading(ModsTab, "QModManager");

                __instance.AddToggleOption(ModsTab, "Enable debug logs", Utility.Logger.EnableDebugLogging,
                    new UnityAction<bool>(toggleVal => ChangeDebugLogs(toggleVal)));

                __instance.AddToggleOption(ModsTab, "Check for updates", PlayerPrefsExtra.GetBool("QModManager_EnableUpdateCheck", true),
                    new UnityAction<bool>(toggleVal => PlayerPrefsExtra.SetBool("QModManager_EnableUpdateCheck", toggleVal)));

                __instance.AddToggleOption(ModsTab, "Enable developer mode", DevMode, new UnityAction<bool>(toggleVal => DevMode = toggleVal));
            }
        }

        internal static void ChangeDebugLogs(bool value)
        {
            string path = Path.Combine(Environment.CurrentDirectory, "QModDebug.txt");
            if (value && !File.Exists(path)) File.WriteAllText(path, "1");
            if (!value && File.Exists(path)) File.Delete(path);
        }
    }
}
