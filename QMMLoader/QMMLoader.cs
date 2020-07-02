using BepInEx;
using BepInEx.Logging;

namespace QMMLoader
{
    /// <summary>
    /// QMMLoader - simply fires up the QModManager entry point.
    /// </summary>
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInProcess(SubnauticaProcessName)]
    [BepInProcess(SubnauticaZeroProcessName)]
    public class QMMLoader : BaseUnityPlugin
    {
        internal const string PluginGuid = "QMMLoader";
        internal const string PluginName = "QMMLoader";
        internal const string PluginVersion = "1.0";

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

        private bool initialized = false;
        private void Initialize()
        {
            if (!initialized && Main != null && Main == this)
            {
                initialized = true;

                QModManager.Patching.Patcher.Patch();
            }
        }
    }
}
