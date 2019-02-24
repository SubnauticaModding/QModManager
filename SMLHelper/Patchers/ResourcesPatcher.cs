namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using System.Reflection;
    using Assets;
    using System;

    internal class ResourcesPatcher
    {
        internal static bool Prefix(ref UnityEngine.Object __result, string path)
        {
            if (ModPrefab.TryGetFromFileName(path, out ModPrefab prefab))
            {
                __result = prefab.GetGameObject();
                return false;
            }

            return true;
        }

        private static readonly PropertyInfo AssetsInfo = 
            typeof(UnityEngine.ResourceRequest).GetProperty("asset");

        private static readonly FieldInfo MPathInfo =
            typeof(UnityEngine.ResourceRequest).GetField("m_Path", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo MTypeInfo =
            typeof(UnityEngine.ResourceRequest).GetField("m_Type", BindingFlags.NonPublic | BindingFlags.Instance);

        internal static bool Prefix_Async(ref UnityEngine.ResourceRequest __result, string path)
        {
            if (ModPrefab.TryGetFromFileName(path, out ModPrefab prefab))
            {
                __result = new UnityEngine.ResourceRequest();
                AssetsInfo.SetValue(__result, prefab.GetGameObject(), null);

                MPathInfo.SetValue(__result, path);
                MTypeInfo.SetValue(__result, prefab.GetGameObject().GetType());

                return false;
            }

            return true;
        }

        internal static void Patch(HarmonyInstance harmony)
        {
            Type resourcesType = typeof(UnityEngine.Resources);
            MethodInfo[] methods = resourcesType.GetMethods();

            foreach (MethodInfo method in methods)
            {
                if (method.Name == "Load")
                {
                    if (method.GetParameters().Length == 1)
                    {
                        if (method.IsGenericMethod)
                        {
                            MethodInfo genericMethod = method.MakeGenericMethod(typeof(UnityEngine.Object));

                            harmony.Patch(genericMethod,
                                new HarmonyMethod(typeof(ResourcesPatcher).GetMethod("Prefix", BindingFlags.Static | BindingFlags.NonPublic)), null);
                        }
                        else
                        {
                            harmony.Patch(method,
                                new HarmonyMethod(typeof(ResourcesPatcher).GetMethod("Prefix", BindingFlags.Static | BindingFlags.NonPublic)), null);
                        }
                    }
                }

                if (method.Name == "LoadAsync")
                {
                    if (method.GetParameters().Length == 1)
                    {
                        if (method.IsGenericMethod)
                        {
                            MethodInfo genericMethod = method.MakeGenericMethod(typeof(UnityEngine.Object));

                            harmony.Patch(genericMethod,
                                new HarmonyMethod(typeof(ResourcesPatcher).GetMethod("Prefix_Async", BindingFlags.Static | BindingFlags.NonPublic)), null);
                        }
                        else
                        {
                            harmony.Patch(method,
                                new HarmonyMethod(typeof(ResourcesPatcher).GetMethod("Prefix_Async", BindingFlags.Static | BindingFlags.NonPublic)), null);
                        }
                    }
                }
            }

            Logger.Log("ResourcesPatcher is done.", LogLevel.Debug);
        }
    }
}
