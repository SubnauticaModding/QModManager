namespace QModManager.HarmonyPatches.EnableConsoleSetting
{
    using HarmonyLib;
    using QModManager.Utility;
    using UnityEngine;

    [HarmonyPatch(typeof(DevConsole), nameof(DevConsole.Update))]
    internal static class DevConsole_Update_Patch
    {
        // This patch toggles the console based on the mod option

        [HarmonyPostfix]
        internal static void Postfix()
        {
            if(DevConsole.disableConsole != !Config.EnableConsole)
            {
                DevConsole.disableConsole = !Config.EnableConsole;
                PlayerPrefs.SetInt("UWE.DisableConsole", Config.EnableConsole ? 0 : 1);
            }
        }
    }

    [HarmonyPatch(typeof(PlayerPrefsUtils), nameof(PlayerPrefsUtils.PrefsToggle))]
    internal static class PlayerPrefsUtils_PrefsToggle_Patch
    {
        // This patch syncronizes the "Disable console" UI element in the F3 debug menu

        [HarmonyPostfix]
        public static void Postfix(bool defaultVal, string key, string label, ref bool __result)
        {
            if (key == "UWE.DisableConsole")
            {
                Config.EnableConsole = !__result;
            }
        }
    }
}
