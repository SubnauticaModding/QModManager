namespace QModManager
{
    using Harmony;
    using QModManager.Utility;
    using UnityEngine.Events;

    internal static class OptionsManager
    {
        internal static int ModsTab;



        [HarmonyPatch(typeof(uGUI_OptionsPanel), "AddTabs")]
        internal static class OptionsPatch
        {
            [HarmonyPostfix]
            internal static void Postfix(uGUI_OptionsPanel __instance)
            {
                ModsTab = __instance.AddTab("Mods");
                __instance.AddHeading(ModsTab, "QModManager");

                __instance.AddToggleOption(ModsTab, Config.FIELDS.CHECK_FOR_UPDATES, Config.Get(Config.FIELDS.CHECK_FOR_UPDATES, true), 
                    new UnityAction<bool>(toggleVal => Config.Set(Config.FIELDS.CHECK_FOR_UPDATES, toggleVal)));

                __instance.AddToggleOption(ModsTab, Config.FIELDS.ENABLE_CONSOLE, Config.Get(Config.FIELDS.ENABLE_CONSOLE, false),
                    new UnityAction<bool>(toggleVal =>
                    {
                        Config.Set(Config.FIELDS.ENABLE_CONSOLE, toggleVal);
                        DevConsole.disableConsole = !toggleVal;
                    }));

                __instance.AddToggleOption(ModsTab, Config.FIELDS.ENABLE_DEBUG_LOGS, Config.Get(Config.FIELDS.ENABLE_DEBUG_LOGS, false),
                    new UnityAction<bool>(toggleVal => Config.Set(Config.FIELDS.ENABLE_DEBUG_LOGS, toggleVal)));

                __instance.AddToggleOption(ModsTab, Config.FIELDS.ENABLE_DEV_MODE, Config.Get(Config.FIELDS.ENABLE_DEV_MODE, false), 
                    new UnityAction<bool>(toggleVal => Config.Set(Config.FIELDS.ENABLE_DEV_MODE, toggleVal)));
            }
        }
    }
}
