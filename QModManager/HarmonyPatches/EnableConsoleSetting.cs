namespace QModManager.HarmonyPatches.EnableConsoleSetting
{
    using Harmony;
    using QModManager.Utility;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using UnityEngine;

    [HarmonyPatch(typeof(DevConsole), nameof(DevConsole.Awake))]
    internal static class DevConsole_Awake_Patch
    {
        // This patch toggles the console based on the mod option

        [HarmonyPostfix]
        internal static void Postfix()
        {
            DevConsole.disableConsole = !Config.EnableConsole;
            PlayerPrefs.SetInt("UWE.DisableConsole", Config.EnableConsole ? 0 : 1);
        }
    }

    [HarmonyPatch(typeof(PlayerPrefsUtils), nameof(PlayerPrefsUtils.PrefsToggle))]
    internal static class PlayerPrefsUtils_PrefsToggle_Patch
    {
        // This patch syncronizes the "Disable console" UI element in the F3 debug menu

        [HarmonyPostfix]
        public static void Postfix(bool __result, string key)
        {
            if (key != "UWE.DisableConsole") return;

            Config.EnableConsole = !__result;

            return;
        }
    }
}
