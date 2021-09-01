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
        internal const string PluginVersion = "4.3.0";

        private void Awake()
        {
            var harmony = new Harmony(PluginGuid);
            harmony.Patch(AccessTools.Method("MirrorInternalLogs.Util.LibcHelper:Format"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(LogFilter), nameof(LogFilter.LibcHelper_Format_Postfix))));
        }

        private readonly static Regex[] DirtyRegexPatterns = new Regex[] {
            new Regex(@"([\r\n]+)?(\(Filename: .*\))", RegexOptions.Compiled),
            new Regex(@"([\r\n]+)?(Replacing cell.*)", RegexOptions.Compiled),
            new Regex(@"([\r\n]+)?(Resetting cell with.*)", RegexOptions.Compiled),
            new Regex(@"([\r\n]+)?(Fallback handler could not load.*)", RegexOptions.Compiled),
            new Regex(@"([\r\n]+)?(Heartbeat CSV.*,[0-9])", RegexOptions.Compiled),
            new Regex(@"([\r\n]+)?(L[0-9]: .*)", RegexOptions.Compiled),
            new Regex(@"([\r\n]+)?(Kinematic body only supports Speculative Continuous collision detection)", RegexOptions.Compiled)
        };

        private static void LibcHelper_Format_Postfix(ref string __result)
        {
            bool match = false;
            foreach (Regex pattern in DirtyRegexPatterns)
            {
                if (pattern.IsMatch(__result))
                {
                    __result = pattern.Replace(__result, string.Empty).Trim();
                    match = true;
                }
            }

            // if our filtering resulted in an empty string, return null so that MirrorInternalLogs will skip the line
            if (match && string.IsNullOrWhiteSpace(__result))
            {
                __result = null;
            }
        }
    }
}
