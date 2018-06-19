namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using System.Reflection;
    using Assets;

    public class ResourcesPatcher
    {
        public static bool Prefix(ref UnityEngine.Object __result, string path)
        {
            foreach (var prefab in ModPrefab.Prefabs)
            {
                if (prefab.PrefabFileName.ToLowerInvariant() == path.ToLowerInvariant())
                {
                    __result = prefab.GetGameObject();
                    return false;
                }
            }

            return true;
        }
        private static readonly PropertyInfo AssetsInfo = 
            typeof(UnityEngine.ResourceRequest).GetProperty("asset");

        private static readonly FieldInfo MPathInfo =
            typeof(UnityEngine.ResourceRequest).GetField("m_Path", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo MTypeInfo =
            typeof(UnityEngine.ResourceRequest).GetField("m_Type", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool Prefix_Async(ref UnityEngine.ResourceRequest __result, string path)
        {
            foreach (var prefab in CustomPrefabHandler.customPrefabs)
            {
                if (prefab.PrefabFileName.ToLowerInvariant() == path.ToLowerInvariant())
                {
                    //__result = prefab.Object;

                    __result = new UnityEngine.ResourceRequest();
                    AssetsInfo.SetValue(__result, prefab.GetResource(), null);

                    MPathInfo.SetValue(__result, path);
                    MTypeInfo.SetValue(__result, prefab.GetResource().GetType());

                    return false;
                }
            }

            return true;
        }

        public static void Patch(HarmonyInstance harmony)
        {
            var resourcesType = typeof(UnityEngine.Resources);
            var methods = resourcesType.GetMethods();

            foreach (var method in methods)
            {
                if (method.Name == "Load")
                {
                    if (method.GetParameters().Length == 1)
                    {
                        if (method.IsGenericMethod)
                        {
                            var genericMethod = method.MakeGenericMethod(typeof(UnityEngine.Object));

                            harmony.Patch(genericMethod,
                                new HarmonyMethod(typeof(ResourcesPatcher).GetMethod("Prefix")), null);
                        }
                        else
                        {
                            harmony.Patch(method,
                                new HarmonyMethod(typeof(ResourcesPatcher).GetMethod("Prefix")), null);
                        }
                    }
                }

                if (method.Name == "LoadAsync")
                {
                    if (method.GetParameters().Length == 1)
                    {
                        if (method.IsGenericMethod)
                        {
                            var genericMethod = method.MakeGenericMethod(typeof(UnityEngine.Object));

                            harmony.Patch(genericMethod,
                                new HarmonyMethod(typeof(ResourcesPatcher).GetMethod("Prefix_Async")), null);
                        }
                        else
                        {
                            harmony.Patch(method,
                                new HarmonyMethod(typeof(ResourcesPatcher).GetMethod("Prefix_Async")), null);
                        }
                    }
                }
            }
            Logger.Log("ResourcesPatcher is done.");
        }
    }
}
