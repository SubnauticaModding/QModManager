using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ShimHelpers
{
    internal static class ShimUtil
    {
        public static T MakeDelegate<T>(Type t, string method) where T : class
        {

            var m = t.GetMethod(method, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return Delegate.CreateDelegate(typeof(T), m) as T;
        }

        public static T MakeGetter<T>(FieldInfo f) where T : class
        {
            DynamicMethod dm = new DynamicMethod($"shimutil_field_{f.DeclaringType.Name}_{f.Name}_get", f.FieldType, f.IsStatic ? null : new []{f.DeclaringType}, typeof(ShimUtil), true);

            var il = dm.GetILGenerator();

            if (f.IsStatic)
                il.Emit(OpCodes.Ldsfld, f);
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, f);
            }
            
            il.Emit(OpCodes.Ret);

            return dm.CreateDelegate(typeof(T)) as T;
        }

        public static T MakeSetter<T>(FieldInfo f) where T : class
        {
            var l = new List<Type>();
            if(f.IsStatic)
                l.Add(f.DeclaringType);
            l.Add(f.FieldType);

            DynamicMethod dm = new DynamicMethod($"shimutil_field_{f.DeclaringType.Name}_{f.Name}_set", f.FieldType, l.ToArray(), typeof(ShimUtil), true);

            var il = dm.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            if (f.IsStatic)
                il.Emit(OpCodes.Ldsfld, f);
            else
            {
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldfld, f);
            }

            il.Emit(OpCodes.Ret);

            return dm.CreateDelegate(typeof(T)) as T;
        }
    }
}
