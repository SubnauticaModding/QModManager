using BepInEx;
#if !SUBNAUTICA_STABLE
using HarmonyLib;
#if !BELOWZERO
using System.Collections;
#endif
#endif
using System.Collections.Generic;
using UnityEngine;

namespace QModInstaller.BepInEx.Plugins
{
    using QModManager.API.ModLoading;
    using QModManager.Patching;
    using QModManager.Utility;

    /// <summary>
    /// QMMLoader - simply fires up the QModManager entry point.
    /// </summary>
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInProcess("Subnautica"), BepInProcess("SubnauticaZero")]
    public class QMMLoader : BaseUnityPlugin
    {
        internal const string PluginGuid = "QModManager.QMMLoader";
        internal const string PluginName = "QMMLoader";
        internal const string PluginVersion = "4.2";

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

            PreInitializeQMods();
        }

        private void PreInitializeQMods()
        {
            Patcher.Patch(); // Run QModManager patch

            if (QModsToLoad is null)
            {
                Logger.LogWarning("QModsToLoad is null!");
                return;
            }

            Initializer = new Initializer(Patcher.CurrentlyRunningGame);
            Initializer.InitializeMods(QModsToLoad, PatchingOrder.MetaPreInitialize);
            Initializer.InitializeMods(QModsToLoad, PatchingOrder.PreInitialize);

#if SUBNAUTICA_STABLE
            Initializer.InitializeMods(QModsToLoad, PatchingOrder.NormalInitialize);
            Initializer.InitializeMods(QModsToLoad, PatchingOrder.PostInitialize);
            Initializer.InitializeMods(QModsToLoad, PatchingOrder.MetaPostInitialize);

            SummaryLogger.ReportIssues(QModsToLoad);
            SummaryLogger.LogSummaries(QModsToLoad);
            foreach (Dialog dialog in Patcher.Dialogs)
            {
                dialog.Show();
            }
#else
            var harmony = new Harmony(PluginGuid);
            harmony.Patch(
                AccessTools.Method(
#if SUBNAUTICA
                    typeof(PlatformUtils), nameof(PlatformUtils.PlatformInitAsync)
#elif BELOWZERO
                    typeof(SpriteManager), nameof(SpriteManager.OnLoadedSpriteAtlases)
#endif
                    ),
                    postfix: new HarmonyMethod(AccessTools.Method(typeof(QMMLoader), nameof(QMMLoader.InitializeQMods)))
                );
#endif
        }

#if SUBNAUTICA_EXP
        private static IEnumerator InitializeQMods(IEnumerator result)
        {
            yield return result;

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
#elif BELOWZERO
        private static void InitializeQMods() 
        {
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
#endif
    }
}