using BepInEx;
#if !SUBNAUTICA_STABLE
using HarmonyLib;
using System.Collections;
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
        internal const string PluginVersion = "4.3.0";

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
                    typeof(PlatformUtils), nameof(PlatformUtils.PlatformInitAsync)
                    ),
                    postfix: new HarmonyMethod(AccessTools.Method(typeof(QMMLoader), nameof(QMMLoader.InitializeQMods)))
                );
        }

        private static IEnumerator InitializeQMods(IEnumerator result)
        {
            while (result.MoveNext())
            {
                yield return result;
            }

#if BELOWZERO
            if(!SpriteManager.hasInitialized)
                yield return new WaitUntil(()=>SpriteManager.hasInitialized);
#endif

            Initializer.InitializeMods(QModsToLoad, PatchingOrder.NormalInitialize);
            Initializer.InitializeMods(QModsToLoad, PatchingOrder.PostInitialize);
            Initializer.InitializeMods(QModsToLoad, PatchingOrder.MetaPostInitialize);

            SummaryLogger.ReportIssues(QModsToLoad);
            SummaryLogger.LogSummaries(QModsToLoad);
            foreach (Dialog dialog in Patcher.Dialogs)
            {
                dialog.Show();
            }
#endif
        }
    }
}