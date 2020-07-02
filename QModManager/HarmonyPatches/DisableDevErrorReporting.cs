namespace QModManager.HarmonyPatches.DisableDevErrorReporting
{
    using HarmonyLib;
    using UnityEngine;

    [HarmonyPatch(typeof(SentrySdk), nameof(SentrySdk.Start))]
    internal static class SentrySdk_Start_Patch
    {
        // This patch destroys any SentrySdk object
        // The SentrySdk class is the class that sends errors logged with Debug.LogError or Debug.LogException to the Subnautica developers

        [HarmonyPrefix]
        internal static bool Prefix(SentrySdk __instance)
        {
            GameObject.Destroy(__instance);
            return false;
        }
    }
}
