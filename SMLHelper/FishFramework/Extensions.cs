using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SMLHelper.V2.FishFramework
{
    public static class Extensions
    {
        public static T AddOrGet<T>(this GameObject obj) where T : Component
        {
            T comp = obj.GetComponent<T>();
            if (!comp)
            {
                comp = obj.AddComponent<T>();
            }

            return comp;
        }
    }
}
