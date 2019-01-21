using UnityEngine;

namespace QModManager.PrefabDebugger
{

    public class HierarchyItem
    {
        public GameObject source;
        public string oldname;
        public bool opened;

        public HierarchyItem(GameObject _source)
        {
            source = _source;
            oldname = _source.name;
        }
    }
}