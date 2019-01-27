using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

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