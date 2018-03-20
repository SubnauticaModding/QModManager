using Harmony;
using System.Reflection;
using UnityEngine;
using UWE;

namespace SMLHelper.Patchers
{
    public class PrefabDatabasePatcher
    {
        public static void Postfix()
        {
            foreach (var prefab in CustomPrefabHandler.customPrefabs)
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
            Logger.Log("PrefabDatabasePatcher is done.");
        }
    }
}
