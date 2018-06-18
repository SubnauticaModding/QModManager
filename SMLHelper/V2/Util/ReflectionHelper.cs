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

        public static object GetStaticPrivateField<T>(string fieldName, BindingFlags bindingFlags = BindingFlags.Default)
        {
            FieldInfo fieldInfo = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static | bindingFlags);
            return fieldInfo.GetValue(null);
        }


        public static void SetStaticPrivateField<T>(string fieldName, object value, BindingFlags bindingFlags = BindingFlags.Default)
        {
            FieldInfo fieldInfo = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static | bindingFlags);
            fieldInfo.SetValue(null, value);
        }
    }
}
