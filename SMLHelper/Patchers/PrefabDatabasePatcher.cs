using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UWE;
using UnityEngine;

namespace SMLHelper.Patchers
{
    public class PrefabDatabasePatcher
    {
        public static void Postfix()
        {
            foreach(var prefab in CustomPrefabHandler.customPrefabs)
            {
                PrefabDatabase.AddToCache(prefab.PrefabFileName, prefab.Object as GameObject);
                PrefabDatabase.prefabFiles[prefab.ClassID] = prefab.PrefabFileName;
            }
        }

        public static void Patch(HarmonyInstance harmony)
        {
            var prefabDatabaseType = typeof(PrefabDatabase);
            var loadPrefabDatabaseMethod = prefabDatabaseType.GetMethod("LoadPrefabDatabase", BindingFlags.Public | BindingFlags.Static);

            harmony.Patch(loadPrefabDatabaseMethod, null,
                new HarmonyMethod(typeof(PrefabDatabasePatcher).GetMethod("Postfix")));
        }
    }
}
