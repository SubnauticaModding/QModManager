using BepInEx;
using System;
using UnityEngine;

namespace QModInstaller.BepInEx.Plugins
{
    /// <summary>
    /// QMMLoader - simply fires up the QModManager entry point.
    /// </summary>
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInProcess("Subnautica"), BepInProcess("SubnauticaZero")]
    public class QMMLoader : BaseUnityPlugin
    {
        internal const string PluginGuid = "QModManager.QMMLoader";
        internal const string PluginName = "QMMLoader";
        internal const string PluginVersion = "4.1.4";

        /// <summary>
        /// Prevents a default instance of the <see cref="QMMLoader"/> class from being created 
        /// Also ensures the root bepinex object does not get destroyed if the game reloads for steam.
        /// </summary>
        private void Awake()
        {
            GameObject obj = gameObject;

            while (obj.transform.parent?.gameObject != null)
                obj = obj.transform.parent.gameObject;

            obj.EnsureComponent<SceneCleanerPreserve>();
            DontDestroyOnLoad(obj);
        }
    }
}