namespace SMLHelper.V2
{
    using System;
    using System.Reflection;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using HarmonyLib;

    internal static class PatchUtils
    {
        internal static void PatchDictionary<KeyType, ValueType>(IDictionary<KeyType, ValueType> original, IDictionary<KeyType, ValueType> patches)
        {
            foreach (KeyValuePair<KeyType, ValueType> entry in patches)
            {
                original[entry.Key] = entry.Value;
            }
        }

        internal static void PatchList<ValueType>(IList<ValueType> original, IList<ValueType> patches)
        {
            foreach (ValueType entry in patches)
            {
                original.Add(entry);
            }
        }

        // for use with iterator methods, returns MoveNext method from the iterator type
        internal static MethodInfo GetIteratorMethod(MethodInfo method)
        {
            Type stateMachineType = method.GetCustomAttribute<StateMachineAttribute>()?.StateMachineType;
            return AccessTools.Method(stateMachineType, "MoveNext");
        }

        // attributes for use with PatchClass method
        // we using them instead of Harmony attributes to avoid confusion and show that these patches are not processed by Harmony.PatchAll
        [AttributeUsage(AttributeTargets.Method)]
        internal class Prefix: Attribute {}

        [AttributeUsage(AttributeTargets.Method)]
        internal class Postfix: Attribute {}

        [AttributeUsage(AttributeTargets.Method)]
        internal class Transpiler: Attribute {}

        // use methods from 'typeWithPatchMethods' class as harmony patches
        // valid method need to have HarmonyPatch and PatchUtils.[Prefix/Postfix/Transpiler] attributes
        // if typeWithPatchMethods is null, we use type from which this method is called
        internal static void PatchClass(Harmony harmony, Type typeWithPatchMethods = null)
        {
            static MethodInfo _getTargetMethod(HarmonyMethod hm) => AccessTools.Method(hm.declaringType, hm.methodName, hm.argumentTypes);

            typeWithPatchMethods ??= new StackTrace().GetFrame(1).GetMethod().ReflectedType;

            foreach (var method in typeWithPatchMethods.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                HarmonyMethod _method_if<H>() => method.IsDefined(typeof(H))? new HarmonyMethod(method): null;

                if (method.GetCustomAttribute<HarmonyPatch>() is HarmonyPatch harmonyPatch && _getTargetMethod(harmonyPatch.info) is MethodInfo targetMethod)
                    harmony.Patch(targetMethod, _method_if<Prefix>(), _method_if<Postfix>(), _method_if<Transpiler>());
            }
        }
    }
}
