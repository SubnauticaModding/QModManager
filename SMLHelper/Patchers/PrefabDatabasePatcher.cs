using Harmony;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UWE;

namespace SMLHelper.Patchers
{
    public class PrefabDatabasePatcher
    {
        private static Dictionary<string, GameObject> prefabsByFileName = new Dictionary<string, GameObject>();
        private static Dictionary<string, GameObject> prefabsByClassId = new Dictionary<string, GameObject>();

        public static void LoadPrefabDatabase_Postfix()
        {
            foreach (var prefab in CustomPrefabHandler.customPrefabs)
            {
                var goPrefab = prefab.GetResource() as GameObject;
                goPrefab.SetActive(false);
                goPrefab.transform.position = new Vector3(5000, 5000, 5000);

                PrefabDatabase.AddToCache(prefab.PrefabFileName, goPrefab);
                PrefabDatabase.prefabFiles[prefab.ClassID] = prefab.PrefabFileName;

                prefabsByFileName[prefab.PrefabFileName] = goPrefab;
                prefabsByClassId[prefab.ClassID] = goPrefab;
            }
        }

        public static bool GetPrefabForFilename_Prefix(string filename, ref GameObject __result)
        {
            foreach(var prefab in prefabsByFileName)
            {
                if(prefab.Key.ToLowerInvariant() == filename.ToLowerInvariant())
                {
                    if(prefab.Value != null)
                    {
                        prefab.Value.SetActive(true);
                        __result = prefab.Value;

                        return false;
                    }
                }
            }

            return true;
        }

        public static bool GetPrefabAsync_Prefix(ref IPrefabRequest __result, string classId)
        {
            foreach(var prefab in prefabsByClassId)
            {
                if(prefab.Key.ToLowerInvariant() == classId.ToLowerInvariant())
                {
                    if(prefab.Value != null)
                    {
                        prefab.Value.SetActive(true);
                        __result = new LoadedPrefabRequest(prefab.Value);

                        return false;
                    }
                }
            }

            return true;
        }

        public static void Patch(HarmonyInstance harmony)
        {
            var prefabDatabaseType = typeof(PrefabDatabase);
            var loadPrefabDatabaseMethod = prefabDatabaseType.GetMethod("LoadPrefabDatabase", BindingFlags.Public | BindingFlags.Static);
            var getPrefabForFilename = prefabDatabaseType.GetMethod("GetPrefabForFilename", BindingFlags.Public | BindingFlags.Static);
            var getPrefabAsync = prefabDatabaseType.GetMethod("GetPrefabAsync", BindingFlags.Public | BindingFlags.Static);

            harmony.Patch(loadPrefabDatabaseMethod, null,
                new HarmonyMethod(typeof(PrefabDatabasePatcher).GetMethod("LoadPrefabDatabase_Postfix")));

            harmony.Patch(getPrefabForFilename, 
                new HarmonyMethod(typeof(PrefabDatabasePatcher).GetMethod("GetPrefabForFilename_Prefix")), null);

            harmony.Patch(getPrefabAsync,
                new HarmonyMethod(typeof(PrefabDatabasePatcher).GetMethod("GetPrefabAsync_Prefix")), null);

            Logger.Log("PrefabDatabasePatcher is done.");
        }
    }
}
