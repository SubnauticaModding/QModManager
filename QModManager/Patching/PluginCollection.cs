namespace QModManager.Patching
{
    using System.Collections.Generic;
    using BepInEx;

    internal class PluginCollection : IPluginCollection
    {
        private readonly Dictionary<PluginInfo, bool> _pluginsRequiredDictionary;
        private readonly Dictionary<string, PluginInfo> _pluginsIdDictionary;

        public PluginCollection()
            : this(new PluginInfo[] { })
        {
        }

        public PluginCollection(IEnumerable<PluginInfo> plugins)
        {
            _pluginsRequiredDictionary = new Dictionary<PluginInfo, bool>();
            _pluginsIdDictionary = new Dictionary<string, PluginInfo>();

            foreach (var plugin in plugins)
            {
                _pluginsRequiredDictionary.Add(plugin, false);
                _pluginsIdDictionary.Add(plugin.Metadata.GUID, plugin);
            }

        }

        public IEnumerable<PluginInfo> AllPlugins => _pluginsIdDictionary.Values;

        public bool IsKnownPlugin(string id)
        {
            return _pluginsIdDictionary.ContainsKey(id);
        }

        public void MarkAsRequired(string id)
        {
            PluginInfo plugin = _pluginsIdDictionary[id];
            _pluginsRequiredDictionary[plugin] = true;
        }
    }
}
