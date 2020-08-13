using System.Collections.Generic;
using BepInEx;

namespace QModManager.Patching
{
    internal interface IPluginCollection
    {
        bool IsKnownPlugin(string id);

        void MarkAsRequired(string id);

        IEnumerable<PluginInfo> AllPlugins { get; }
    }
}
