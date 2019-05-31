namespace QModManager.API.SMLHelper.Patchers
{
    using Assets;
    using Harmony;
    using System.Reflection;
    using UnityEngine;
    using Logger = QModManager.Utility.Logger;

    internal static class ResourcesPatcher
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

        private static readonly PropertyInfo AssetsInfo = AccessTools.Property(typeof(ResourceRequest), "asset");

        private static readonly FieldInfo MPathInfo = AccessTools.Field(typeof(ResourceRequest), "m_Path");

        private static readonly FieldInfo MTypeInfo = AccessTools.Field(typeof(ResourceRequest), "m_Type");

        internal static bool Prefix_Async(ref ResourceRequest __result, string path)
        {
            if (ModPrefab.TryGetFromFileName(path, out ModPrefab prefab))
            {
                __result = new ResourceRequest();
                AssetsInfo.SetValue(__result, prefab.GetGameObject(), null);

                MPathInfo.SetValue(__result, path);
                MTypeInfo.SetValue(__result, prefab.GetGameObject().GetType());

                return false;
            }

            return true;
        }

        internal static void Patch(HarmonyInstance harmony)
        {
            MethodInfo[] methods = typeof(Resources).GetMethods();

            foreach (MethodInfo method in methods)
            {
                if (method.GetParameters().Length == 1)
                {
                    if (method.Name == "Load")
                    {
                        MethodInfo patchMethod = method;

                        if (method.IsGenericMethod)
                        {
                            patchMethod = method.MakeGenericMethod(typeof(UnityEngine.Object));
                        }

                        harmony.Patch(patchMethod,
                            prefix: new HarmonyMethod(AccessTools.Method(typeof(ResourcesPatcher), "Prefix")));
                    }
                    else if (method.Name == "LoadAsync")
                    {
                        MethodInfo patchMethod = method;

                        if (method.IsGenericMethod)
                        {
                            patchMethod = method.MakeGenericMethod(typeof(UnityEngine.Object));
                        }

                        harmony.Patch(patchMethod,
                            prefix: new HarmonyMethod(AccessTools.Method(typeof(ResourcesPatcher), "Prefix_Async")));
                    }
                }
            }

            Logger.Debug("ResourcesPatcher is done.");
        }
    }
}
