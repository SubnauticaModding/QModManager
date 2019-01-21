using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace BlueFire.Debugger
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