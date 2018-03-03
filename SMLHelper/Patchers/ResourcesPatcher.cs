using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SMLHelper.Patchers
{
    public class ResourcesPatcher
    {
        public static bool Prefix(ref UnityEngine.Object __result, string path)
        {
            foreach(var prefab in CustomPrefabHandler.customPrefabs)
            {
                if(prefab.PrefabFileName.ToLowerInvariant() == path.ToLowerInvariant())
                {
                    __result = prefab.Object;
                    return false;
                }
            }

            return true;
        }

        public static bool Prefix_Async(ref UnityEngine.ResourceRequest __result, string path)
        {
            foreach (var prefab in CustomPrefabHandler.customPrefabs)
            {
                if (prefab.PrefabFileName.ToLowerInvariant() == path.ToLowerInvariant())
                {
                    //__result = prefab.Object;

                    var request = new UnityEngine.ResourceRequest();
                    typeof(UnityEngine.ResourceRequest).GetProperty("asset").SetValue(request, prefab.Object, null);
            
                    typeof(UnityEngine.ResourceRequest).GetField("m_Path", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(request, path);
                    typeof(UnityEngine.ResourceRequest).GetField("m_Type", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(request, prefab.Object.GetType());

                    __result = request;

                    return false;
                }
            }

            return true;
        }

        public static void Patch(HarmonyInstance harmony)
        {
            var resourcesType = typeof(UnityEngine.Resources);
            var methods = resourcesType.GetMethods();

            foreach(var method in methods)
            {
                if(method.Name == "Load")
                {
                    if(method.GetParameters().Length == 1)
                    {
                        if(method.IsGenericMethod)
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

                if(method.Name == "LoadAsync")
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
        }
    }
}
