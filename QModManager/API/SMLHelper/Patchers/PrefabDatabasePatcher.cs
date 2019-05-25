namespace QModManager.API.SMLHelper.Patchers
{
    using Assets;
    using Harmony;
    using UnityEngine;
    using UWE;
    using Logger = API.SMLHelper.Logger;

    internal static class PrefabDatabasePatcher
    {
        internal static void LoadPrefabDatabase_Postfix()
        {
            foreach (ModPrefab prefab in ModPrefab.Prefabs)
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
            harmony.Patch(AccessTools.Method(typeof(PrefabDatabase), "LoadPrefabDatabase"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(PrefabDatabasePatcher), "LoadPrefabDatabase_Postfix")));

            harmony.Patch(AccessTools.Method(typeof(PrefabDatabase), "GetPrefabForFilename"), 
                prefix: new HarmonyMethod(AccessTools.Method(typeof(PrefabDatabasePatcher), "GetPrefabForFilename_Prefix")));

            harmony.Patch(AccessTools.Method(typeof(PrefabDatabase), "GetPrefabAsync"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(PrefabDatabasePatcher), "GetPrefabAsync_Prefix")));

            Logger.Log("PrefabDatabasePatcher is done.", LogLevel.Debug);
        }
    }
}
