namespace SMLHelper.V2.Util
{
    using System.Reflection;

    public static class ReflectionHelper
    {
        public static object GetPrivateField<T>(this T instance, string fieldName, BindingFlags bindingFlags = BindingFlags.Default)
        {
            FieldInfo fieldInfo = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | bindingFlags);
            return fieldInfo.GetValue(instance);
        }

        public static void SetPrivateField<T>(this T instance, string fieldName, object value, BindingFlags bindingFlags = BindingFlags.Default)
        {
            FieldInfo fieldInfo = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | bindingFlags);
            fieldInfo.SetValue(instance, value);
        }

        public static object GetPrivateStatic<T>(string fieldName, BindingFlags bindingFlags = BindingFlags.Default)
        {
            FieldInfo fieldInfo = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static | bindingFlags);
            return fieldInfo.GetValue(null);
        }


        public static void SetPrivateStaticField<T>(string fieldName, object value, BindingFlags bindingFlags = BindingFlags.Default)
        {
            FieldInfo fieldInfo = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static | bindingFlags);
            fieldInfo.SetValue(null, value);
        }

        public static MethodInfo GetPrivateMethod<T>(string methodName, BindingFlags flags = BindingFlags.Default)
        {
            MethodInfo methodInfo = typeof(T).GetMethod(methodName, BindingFlags.NonPublic | flags);
            return methodInfo;
        } 

        public static MethodInfo GetPrivateStaticMethod<T>(string methodName, BindingFlags flags = BindingFlags.Default)
        {
            MethodInfo methodInfo = typeof(T).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static | flags);
            return methodInfo;
        }
    }
}
