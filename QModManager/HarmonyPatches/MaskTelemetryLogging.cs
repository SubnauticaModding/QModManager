namespace QModManager.HarmonyPatches
{
    using HarmonyLib;
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Networking;

    [HarmonyPatch(typeof(Telemetry), nameof(Telemetry.SendSesionStart))]
    static class MaskTelemetryLogging
    {
        public static IEnumerator Postfix(IEnumerator values, Telemetry __instance, string setPlatformName, string setUserId)
        {
            yield return __instance.platformServices.TryEnsureServerAccessAsync(false);
            if (!__instance.platformServices.CanAccessServers())
            {
                yield break;
            }
            __instance.platformName = (string.IsNullOrEmpty(setPlatformName) ? "Null" : setPlatformName);
            __instance.userId = (string.IsNullOrEmpty(setUserId) ? "Null" : setUserId);
            __instance.csId = SNUtils.GetPlasticChangeSetOfBuild(0);
            WWWForm wwwform = new WWWForm();
            wwwform.AddField("product_id", Telemetry.productId);
            wwwform.AddField("platform", __instance.platformName);
            wwwform.AddField("platform_user_id", __instance.userId);
            wwwform.AddField("cs_id", __instance.csId);
            wwwform.AddField("language", Language.main.GetCurrentLanguage());
            wwwform.AddField("arguments", string.Join(", ", Environment.GetCommandLineArgs()));
            wwwform.AddField("used_cheats", DevConsole.HasUsedConsole().ToString());
            wwwform.AddField("gpu_name", SystemInfo.graphicsDeviceName);
            wwwform.AddField("gpu_memory", SystemInfo.graphicsMemorySize);
            wwwform.AddField("gpu_api", SystemInfo.graphicsDeviceType.ToString());
            wwwform.AddField("cpu_name", SystemInfo.processorType);
            wwwform.AddField("system_memory", SystemInfo.systemMemorySize);
            wwwform.AddField("system_os", SystemInfo.operatingSystem);
            wwwform.AddField("quality", QualitySettings.GetQualityLevel());
            wwwform.AddField("res_x", Screen.width);
            wwwform.AddField("res_y", Screen.height);
            UnityWebRequest webRequest = UnityWebRequest.Post(string.Format("{0}/session-start", "https://analytics.unknownworlds.com/api"), wwwform);
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                SessionStartResponse sessionStartResponse = SessionStartResponse.CreateFromJSON(webRequest.downloadHandler.text);
                __instance.sessionId = sessionStartResponse.session_id;
                Debug.LogFormat("Telemetry session started. Platform: '{0}', UserId: ->Masked by QModManager for privacy reason<-, SessionId: {1}", new object[] { __instance.platformName, __instance.sessionId });
            }
            yield break;
        }
    }
}
