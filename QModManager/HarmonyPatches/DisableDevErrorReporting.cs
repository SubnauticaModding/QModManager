namespace QModManager.HarmonyPatches.DisableDevErrorReporting
{
    using Harmony;
    using UnityEngine;

    [HarmonyPatch(typeof(SentrySdk), nameof(SentrySdk.Start))]
    internal static class SentrySdk_Start
    {
        [HarmonyPrefix]
        internal static bool Prefix(SentrySdk __instance)
        {
            GameObject.Destroy(__instance);
            return false;
        }
    }
}
