using BepInEx;
using QModManager.API;

namespace QModManager
{
    [BepInPlugin("QModManager.QModPluginGenerator", "QModManager.QModPluginGenerator", "1.0")]
    public class QModPlugin : BaseUnityPlugin
    {
        public IQMod QMod { get; private set; }

        void Awake()
        {
            if (QModPluginGenerator.QModsToLoadById.TryGetValue(Info.Metadata.GUID, out var mod))
            {
                QMod = mod;
            }
            else
            {
                Logger.LogError($"Could not find QMod with ID: {Info.Metadata.GUID}");
                DestroyImmediate(this);
            }
        }
    }
}
