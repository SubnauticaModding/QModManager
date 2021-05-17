namespace QModManager.HarmonyPatches.DisableDevErrorReporting
{
    using HarmonyLib;
    using System.Collections;
    using UnityEngine;

    [HarmonyPatch]
    internal static class SentrySdk_Start_Patch
    {
        // This patch destroys any SentrySdk object
        // The SentrySdk class is the class that sends errors logged with Debug.LogError or Debug.LogException to the Subnautica developers

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SentrySdk), nameof(SentrySdk.Start))]
        internal static bool Prefix(SentrySdk __instance)
        {
            GameObject.Destroy(__instance);
            return false;
        }

#if !SUBNAUTICA_STABLE
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SystemsSpawner), nameof(SystemsSpawner.SetupSingleton))]
        internal static IEnumerator Postfix(IEnumerator enumerator)
        {
            yield return null;
            yield break;
        }
#endif
    }
}
