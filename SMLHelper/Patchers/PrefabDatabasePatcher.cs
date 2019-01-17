namespace SMLHelper.V2.Patchers
{
    using System;
    using System.Reflection;
    using Assets;
    using Harmony;
    using UnityEngine;
    using UWE;
    using Logger = V2.Logger;

    internal class PrefabDatabasePatcher
    {
        internal static void LoadPrefabDatabase_Postfix()
        {
            foreach(ModPrefab prefab in ModPrefab.Prefabs)
            {
                PrefabDatabase.prefabFiles[prefab.ClassID] = prefab.PrefabFileName;
            }
        }

        internal static bool GetPrefabForFilename_Prefix(string filename, ref GameObject __result)
        {
            if (ModPrefab.TryGetFromFileName(filename, out ModPrefab prefab))
            {
                GameObject go = prefab.GetGameObjectInternal();
                __result = go;

                return false;
            }

            return true;
        }

        internal static bool GetPrefabAsync_Prefix(ref IPrefabRequest __result, string classId)
        {
            if (ModPrefab.TryGetFromClassId(classId, out ModPrefab prefab))
            { 
                GameObject go = prefab.GetGameObjectInternal();
                __result = new LoadedPrefabRequest(go);

                return false;
            }

            return true;
        }

        internal static void Patch(HarmonyInstance harmony)
        {
            Type prefabDatabaseType = typeof(PrefabDatabase);
            MethodInfo loadPrefabDatabase = prefabDatabaseType.GetMethod("LoadPrefabDatabase", BindingFlags.Public | BindingFlags.Static);
            MethodInfo getPrefabForFilename = prefabDatabaseType.GetMethod("GetPrefabForFilename", BindingFlags.Public | BindingFlags.Static);
            MethodInfo getPrefabAsync = prefabDatabaseType.GetMethod("GetPrefabAsync", BindingFlags.Public | BindingFlags.Static);

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
