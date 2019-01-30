using UnityEngine;

namespace QModManager.Debugger
{
    public class HierarchyItem
    {
        public GameObject source;
        public bool opened;

        public HierarchyItem(GameObject _source)
        {
            source = _source;
        }
    }
}