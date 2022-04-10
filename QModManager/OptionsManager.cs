namespace QModManager
{
    using HarmonyLib;
    using QModManager.API;
    using QModManager.Patching;
    using QModManager.Utility;
    using System.Reflection;
    using UnityEngine.Events;
    using System;
    using System.IO;

    internal static class OptionsManager
    {
        internal static int ModsTab;
        internal static int ModListTab;

        [HarmonyPatch(typeof(uGUI_OptionsPanel), nameof(uGUI_OptionsPanel.AddTabs))]
        internal static class OptionsPatch
        {
            [HarmonyPostfix]
            internal static void Postfix(uGUI_OptionsPanel __instance)
            {
                #region Mod Config
                ModsTab = __instance.AddTab("Mods");
                __instance.AddHeading(ModsTab, "QModManager");

                MethodInfo AddToggleOption = null; 
                if(Patcher.CurrentlyRunningGame == QModGame.Subnautica)
                {
                    AddToggleOption = typeof(uGUI_OptionsPanel).GetMethod(nameof(AddToggleOption), new System.Type[] { typeof(int), typeof(string), typeof(bool), typeof(UnityAction<bool>) });
                    AddToggleOption.Invoke(__instance, new object[] { ModsTab, "Check for updates", Config.CheckForUpdates, new UnityAction<bool>(value => Config.CheckForUpdates = value) });

                    AddToggleOption.Invoke(__instance, new object[] { ModsTab, "Enable console", Config.EnableConsole, new UnityAction<bool>(value =>
                {
                    Config.EnableConsole = value;
#if SUBNAUTICA_STABLE
                    DevConsole.disableConsole = !value;
                    UnityEngine.PlayerPrefs.SetInt("UWE.DisableConsole", value ? 0 : 1);
#elif BELOWZERO || SUBNAUTICA_EXP
                    PlatformUtils.SetDevToolsEnabled(value);
#endif
                }) });

                    AddToggleOption.Invoke(__instance, new object[] { ModsTab, "Enable debug logs", Config.EnableDebugLogs, new UnityAction<bool>(value => Config.EnableDebugLogs = value) });
                    AddToggleOption.Invoke(__instance, new object[] { ModsTab, "Enable developer mode", Config.EnableDevMode, new UnityAction<bool>(value => Config.EnableDevMode = value) });
                }
                else
                {
                    AddToggleOption = typeof(uGUI_OptionsPanel).GetMethod(nameof(AddToggleOption), new System.Type[] { typeof(int), typeof(string), typeof(bool), typeof(UnityAction<bool>), typeof(string) });
                    AddToggleOption.Invoke(__instance, new object[] { ModsTab, "Check for updates", Config.CheckForUpdates, new UnityAction<bool>(value => Config.CheckForUpdates = value), null });

                    AddToggleOption.Invoke(__instance, new object[] { ModsTab, "Enable console", Config.EnableConsole, new UnityAction<bool>(value =>
                {
                    Config.EnableConsole = value;
#if SUBNAUTICA_STABLE
                    DevConsole.disableConsole = !value;
                    UnityEngine.PlayerPrefs.SetInt("UWE.DisableConsole", value ? 0 : 1);
#elif BELOWZERO || SUBNAUTICA_EXP
                    PlatformUtils.SetDevToolsEnabled(value);
#endif
                }), null });

                    AddToggleOption.Invoke(__instance, new object[] { ModsTab, "Enable debug logs", Config.EnableDebugLogs, new UnityAction<bool>(value => Config.EnableDebugLogs = value), null });
                    AddToggleOption.Invoke(__instance, new object[] { ModsTab, "Enable developer mode", Config.EnableDevMode, new UnityAction<bool>(value => Config.EnableDevMode = value), null });
                }
                #endregion Mod Config

                #region Mod List
                ModListTab = __instance.AddTab("Loaded Mods List");

#if SUBNAUTICA_STABLE
                __instance.AddHeading(ModListTab, $"QModManager Stable for Subnautica running version {Assembly.GetExecutingAssembly().GetName().Version.ToStringParsed()}");
#elif SUBNAUTICA_EXP
                __instance.AddHeading(ModListTab, $"QModManager Experimental for Subnautica running version {Assembly.GetExecutingAssembly().GetName().Version.ToStringParsed()}");
#elif BELOWZERO_STABLE
                __instance.AddHeading(ModListTab, $"QModManager Stable for Below Zero running version {Assembly.GetExecutingAssembly().GetName().Version.ToStringParsed()}");
#elif BELOWZERO_EXP
                __instance.AddHeading(ModListTab, $"QModManager Experimental for Below Zero running version {Assembly.GetExecutingAssembly().GetName().Version.ToStringParsed()}");
#endif
                __instance.AddHeading(ModListTab, $"- - List of currently running Mods - -");
                var mods = QModServices.Main.GetAllMods();
                int modsactivecounter = 0;

                //Maybe add algorithm to sort Mods after Name
                foreach (var mod in mods)
                {
                    if(mod.Enable)
                    {
                        __instance.AddHeading(ModListTab, $"{mod.DisplayName} version {mod.ParsedVersion.ToString()} from {mod.Author}");
                        modsactivecounter++;
                    }
                }
                __instance.AddHeading(ModListTab, $"Statistics: {modsactivecounter} Mods activated from {mods.Count} loaded");
                #endregion Mod List
            }
        }
    }
}
