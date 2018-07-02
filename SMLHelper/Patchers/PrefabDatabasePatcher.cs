namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using System.Reflection;
    using System.Collections.Generic;
    using UnityEngine;
    using MonoBehaviours;
    using Assets;
    using UWE;
    using Logger = V2.Logger;

    internal class PrefabDatabasePatcher
    {
        internal static void LoadPrefabDatabase_Postfix()
        {
            foreach(var prefab in ModPrefab.Prefabs)
            {
                PrefabDatabase.prefabFiles[prefab.ClassID] = prefab.PrefabFileName;
            }
        }

        internal static bool GetPrefabForFilename_Prefix(string filename, ref GameObject __result)
        {
            foreach(var prefab in ModPrefab.Prefabs)
            {
                if(prefab.PrefabFileName.ToLowerInvariant() == filename.ToLowerInvariant())
                {
                    var go = prefab.GetGameObjectInternal();
                    __result = go;

                    return false;
                }
            }

            return true;
        }

        internal static bool GetPrefabAsync_Prefix(ref IPrefabRequest __result, string classId)
        {
            foreach (var prefab in ModPrefab.Prefabs)
            {
                if (prefab.ClassID.ToLowerInvariant() == classId.ToLowerInvariant())
                {
                    var go = prefab.GetGameObjectInternal();
                    __result = new LoadedPrefabRequest(go);

                    return false;
                }
            }

            return true;
        }

        internal static void Patch(HarmonyInstance harmony)
        {
            var prefabDatabaseType = typeof(PrefabDatabase);
            var loadPrefabDatabase = prefabDatabaseType.GetMethod("LoadPrefabDatabase", BindingFlags.Public | BindingFlags.Static);
            var getPrefabForFilename = prefabDatabaseType.GetMethod("GetPrefabForFilename", BindingFlags.Public | BindingFlags.Static);
            var getPrefabAsync = prefabDatabaseType.GetMethod("GetPrefabAsync", BindingFlags.Public | BindingFlags.Static);

            harmony.Patch(loadPrefabDatabase, null,
                new HarmonyMethod(typeof(PrefabDatabasePatcher).GetMethod("LoadPrefabDatabase_Postfix", BindingFlags.NonPublic | BindingFlags.Static)));

            harmony.Patch(getPrefabForFilename, 
                new HarmonyMethod(typeof(PrefabDatabasePatcher).GetMethod("GetPrefabForFilename_Prefix", BindingFlags.Static | BindingFlags.NonPublic)), null);

            harmony.Patch(getPrefabAsync,
                new HarmonyMethod(typeof(PrefabDatabasePatcher).GetMethod("GetPrefabAsync_Prefix", BindingFlags.Static | BindingFlags.NonPublic)), null);

            Logger.Log("PrefabDatabasePatcher is done.");
        }
    }
}
