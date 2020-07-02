using System;
using System.Reflection;
using HarmonyLib;

namespace QMMLoader.QMMHarmonyShimmer.Patches
{
    [HarmonyPatch(typeof(AccessTools), nameof(AccessTools.DeclaredField), typeof(Type), typeof(string))]
    internal static class AccessToolsDeclaredFieldShim
    {
        private static void Postfix(ref FieldInfo __result, Type type, string name)
        {
            if (__result == null)
            {
                __result = AccessTools.Field(type, name);
            }
        }
    }
}
