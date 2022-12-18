namespace QModManager.HarmonyPatches.EnableConsoleSetting
{
    using HarmonyLib;
    using QModManager.Utility;

    [HarmonyPatch(typeof(DevConsole), nameof(DevConsole.Awake))]
    internal static class DevConsole_Awake_Patch
    {
        // This patch toggles the console based on the mod option

        [HarmonyPostfix]
        internal static void Postfix()
        {
            if (PlatformUtils.GetDevToolsEnabled() != Config.EnableConsole)
            {
                PlatformUtils.SetDevToolsEnabled(Config.EnableConsole);
            }
        }
    }
}
