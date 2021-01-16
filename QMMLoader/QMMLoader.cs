using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using QModManager.API.ModLoading;
using QModManager.Utility;
using System.Linq;
using System.Reflection;

namespace QModManager
{
    /// <summary>
    /// QMMLoader - simply fires up the QModManager entry point.
    /// </summary>
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInProcess(SubnauticaProcessName)]
    [BepInProcess(SubnauticaZeroProcessName)]
    public class QMMLoader : BaseUnityPlugin
    {
        internal const string PluginGuid = "QModManager.QMMLoader";
        internal const string PluginName = "QMMLoader";
        internal const string PluginVersion = "1.0.1";

        internal const string SubnauticaProcessName = "Subnautica";
        internal const string SubnauticaZeroProcessName = "SubnauticaZero";

        /// <summary>
        /// Static singleton instance of QMMLoader
        /// </summary>
        public static QMMLoader Main;

        internal new ManualLogSource Logger => base.Logger;

        private void Awake()
        {
            if (Main == null && this != null)
            {
                Main = this;
                Initialize();
            }
            else
            {
                DestroyImmediate(this);
            }
        }

        private static Harmony harmony;
        private static MethodInfo entryPointTarget = AccessTools.Method(typeof(StartScreen), nameof(StartScreen.LoadMainMenu));
        private static MethodInfo entryPointPatch = AccessTools.Method(typeof(QMMLoader), nameof(QMMLoader.InitializeQModManager));
        private void Initialize()
        {
            if (harmony == null && Main != null && Main == this)
            {
                harmony = new Harmony("QModManager.QMMLoader");
                harmony.Patch(entryPointTarget, postfix: new HarmonyMethod(entryPointPatch));
            }
        }

        private static void InitializeQModManager()
        {
            Patching.Patcher.Patch(); // Run QModManager patch
            InitializeQMods();
            harmony.Unpatch(entryPointTarget, entryPointPatch); // kill this Harmony patch just to be sure it never happens twice
        }

        private static void InitializeQMods()
        {
            var modsToLoad = QModPluginGenerator.QModsToLoad.ToList();

            var initializer = new Initializer(Patching.Patcher.CurrentlyRunningGame);
            initializer.InitializeMods(modsToLoad);

            SummaryLogger.ReportIssues(modsToLoad);

            SummaryLogger.LogSummaries(modsToLoad);
        }
    }
}
