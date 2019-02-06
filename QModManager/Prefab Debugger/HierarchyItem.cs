using UnityEngine;

namespace QModManager.Debugger
{
    internal class HierarchyItem
    {
        internal GameObject source;
        internal bool opened;

        internal HierarchyItem(GameObject _source)
        {
            source = _source;
        }
    }
}