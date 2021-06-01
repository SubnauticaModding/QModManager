using BepInEx;
using System.Collections.Generic;
using UnityEngine;

namespace QModInstaller
{
    using QModManager.API.ModLoading;
    using QModManager.Patching;
    using QModManager.Utility;

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
        internal const string PluginVersion = "1.0.2";

        internal const string SubnauticaProcessName = "Subnautica";
        internal const string SubnauticaZeroProcessName = "SubnauticaZero";

        [System.NonSerialized]
        internal static List<QMod> QModsToLoad;
        private static Initializer Initializer;

        /// <summary>
        /// Prevents a default instance of the <see cref="QMMLoader"/> class from being created 
        /// Also ensures the root bepinex object does not get destroyed if the game reloads for steam.
        /// </summary>
        private void Awake()
        {
            GameObject obj = gameObject;

            while (obj.transform.parent?.gameObject != null)
            {
                obj = obj.transform.parent.gameObject;
            }

            obj.EnsureComponent<SceneCleanerPreserve>();
            DontDestroyOnLoad(obj);

            InitializeQMods();
        }

        private void InitializeQMods()
        {
            if (QModsToLoad is null)
            {
                Logger.LogWarning("QModsToLoad is null!");
                return;
            }

            Patcher.Patch(); // Run QModManager patch

            Initializer = new Initializer(Patcher.CurrentlyRunningGame);
            Initializer.InitializeMods(QModsToLoad, PatchingOrder.MetaPreInitialize);
            Initializer.InitializeMods(QModsToLoad, PatchingOrder.PreInitialize);
            Initializer.InitializeMods(QModsToLoad, PatchingOrder.NormalInitialize);
            Initializer.InitializeMods(QModsToLoad, PatchingOrder.PostInitialize);
            Initializer.InitializeMods(QModsToLoad, PatchingOrder.MetaPostInitialize);

            SummaryLogger.ReportIssues(QModsToLoad);
            SummaryLogger.LogSummaries(QModsToLoad);
            foreach (Dialog dialog in Patcher.Dialogs)
            {
                dialog.Show();
            }
        }
    }
}