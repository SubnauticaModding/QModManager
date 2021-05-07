namespace QModInstaller
{
    using BepInEx;
    using System;
    using UnityEngine;

    /// <summary>
    /// QMMLoader - simply fires up the QModManager entry point.
    /// </summary>
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInProcess(SubnauticaProcessName)]
    [BepInProcess(SubnauticaZeroProcessName)]
    public class QMMLoader: BaseUnityPlugin
    {
        internal const string PluginGuid = "QModManager.QMMLoader";
        internal const string PluginName = "QMMLoader";
        internal const string PluginVersion = "1.0.2";

        internal const string SubnauticaProcessName = "Subnautica";
        internal const string SubnauticaZeroProcessName = "SubnauticaZero";

        /// <summary>
        /// Prevents a default instance of the <see cref="QMMLoader"/> class from being created 
        /// Also ensures the root bepinex object does not get destroyed if the game reloads for steam.
        /// </summary>
        [Obsolete("DO NOT USE!", true)]
        private QMMLoader()
        {
            GameObject obj = gameObject;

            while(obj.transform.parent?.gameObject != null)
                obj = obj.transform.parent.gameObject;

            DontDestroyOnLoad(obj);
        }
    }
}