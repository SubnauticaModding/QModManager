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
        private static Dictionary<string, GameObject> prefabsByFileName = new Dictionary<string, GameObject>();
        private static Dictionary<string, GameObject> prefabsByClassId = new Dictionary<string, GameObject>();

        internal static bool GetPrefabForFilename_Prefix(string filename, ref GameObject __result)
        {
            foreach(var prefab in ModPrefab.Prefabs)
            {
                if(prefab.PrefabFileName.ToLowerInvariant() == filename.ToLowerInvariant())
                {
                    var prefabGO = prefab.GetGameObject();
                    UpdateGameObject(prefab.TechType, ref prefabGO);
                    __result = prefabGO;

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
                    var prefabGO = prefab.GetGameObject();
                    UpdateGameObject(prefab.TechType, ref prefabGO);
                    __result = new LoadedPrefabRequest(prefabGO);

                    return false;
                }
            }

            return true;
        }

        internal static void UpdateGameObject(TechType techType, ref GameObject gameObject)
        {
            // Maybe there's more stuff we can put here?
            gameObject.AddComponent<TechTypeFixer>().techType = techType;
        }

        internal static void Patch(HarmonyInstance harmony)
        {
            var prefabDatabaseType = typeof(PrefabDatabase);
            var getPrefabForFilename = prefabDatabaseType.GetMethod("GetPrefabForFilename", BindingFlags.Public | BindingFlags.Static);
            var getPrefabAsync = prefabDatabaseType.GetMethod("GetPrefabAsync", BindingFlags.Public | BindingFlags.Static);

            harmony.Patch(getPrefabForFilename, 
                new HarmonyMethod(typeof(PrefabDatabasePatcher).GetMethod("GetPrefabForFilename_Prefix", BindingFlags.Static | BindingFlags.NonPublic)), null);

            harmony.Patch(getPrefabAsync,
                new HarmonyMethod(typeof(PrefabDatabasePatcher).GetMethod("GetPrefabAsync_Prefix", BindingFlags.Static | BindingFlags.NonPublic)), null);

            Logger.Log("PrefabDatabasePatcher is done.");
        }
    }
}
