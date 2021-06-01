using BepInEx;
using HarmonyLib;
using System.Text.RegularExpressions;

namespace QModInstaller.BepInEx.Plugins
{
    /// <summary>
    /// Handles filtering noisy logs from the QModManager logs.
    /// </summary>
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInProcess("Subnautica"), BepInProcess("SubnauticaZero")]
    internal class LogFilter : BaseUnityPlugin
    {
        internal const string PluginGuid = "QModManager.LogFilter";
        internal const string PluginName = PluginGuid;
        internal const string PluginVersion = "4.1.4";

        private void Awake()
        {
            var harmony = new Harmony(PluginGuid);
            harmony.Patch(AccessTools.Method("MirrorInternalLogs.Util.LibcHelper:Format"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(LogFilter), nameof(LogFilter.LibcHelper_Format_Postfix))));
        }

        private readonly static Regex[] DirtyRegexPatterns = new Regex[] {
            new Regex(@"([\r\n]+)?(\(Filename: .*\))$", RegexOptions.Compiled | RegexOptions.Multiline),
            new Regex(@"^(Replacing cell.*)$", RegexOptions.Compiled | RegexOptions.Multiline),
            new Regex(@"^(Resetting cell with.*)$", RegexOptions.Compiled | RegexOptions.Multiline),
            new Regex(@"^(PerformGarbage.*)$", RegexOptions.Compiled | RegexOptions.Multiline),
            new Regex(@"^(Fallback handler could not load.*)$", RegexOptions.Compiled | RegexOptions.Multiline),
            new Regex(@"^(Heartbeat CSV.*,[0-9])$", RegexOptions.Compiled | RegexOptions.Multiline),
            new Regex(@"^(L0: PerformGarbageCollection ->.*)$", RegexOptions.Compiled | RegexOptions.Multiline),
            new Regex(@"^(L0: CellManager::EstimateBytes.*)$", RegexOptions.Compiled | RegexOptions.Multiline),
            new Regex(@"^(Kinematic body only supports Speculative Continuous collision detection.*)$", RegexOptions.Compiled | RegexOptions.Multiline),
        };

        private static void LibcHelper_Format_Postfix(ref string __result)
        {
            foreach (Regex pattern in DirtyRegexPatterns)
            {
                __result = pattern.Replace(__result, string.Empty).Trim();
            }
        }
    }
}
