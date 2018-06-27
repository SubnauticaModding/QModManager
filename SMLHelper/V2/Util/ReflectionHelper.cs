namespace SMLHelper.V2.Util
{
    using System.Reflection;

    /// <summary>
    /// An extension method class for simplifying reflection calls for improved readability.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Gets the value of the requested private field, using reflection, from the instance object.
        /// </summary>
        /// <typeparam name="T">The instance class type.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="fieldName">Name of the private field.</param>
        /// <param name="bindingFlags">The additional binding flags you wish to set.
        /// <see cref="BindingFlags.NonPublic" /> and <see cref="BindingFlags.Instance" /> are already included.</param>
        /// <returns>
        /// The value of the requested field as an <see cref="object" />.
        /// </returns>
        public static object GetPrivateField<T>(this T instance, string fieldName, BindingFlags bindingFlags = BindingFlags.Default)
        {
            FieldInfo fieldInfo = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | bindingFlags);
            return fieldInfo.GetValue(instance);
        }

        /// <summary>
        /// Sets the value of the requested private field, using reflection, on the instance object.
        /// </summary>
        /// <typeparam name="T">The instance class type.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="fieldName">Name of the private field.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="bindingFlags">The additional binding flags you wish to set.
        /// <see cref="BindingFlags.NonPublic" /> and <see cref="BindingFlags.Instance" /> are already included.</param>
        public static void SetPrivateField<T>(this T instance, string fieldName, object value, BindingFlags bindingFlags = BindingFlags.Default)
        {
            FieldInfo fieldInfo = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | bindingFlags);
            fieldInfo.SetValue(instance, value);
        }

        /// <summary>
        /// Gets the value of the requested private static field, using reflection, from the static object.
        /// </summary>
        /// <typeparam name="T">The static class type.</typeparam>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="bindingFlags">The additional binding flags you wish to set.
        /// <see cref="BindingFlags.NonPublic" /> and <see cref="BindingFlags.Static" /> are already included.</param>
        /// <returns>
        /// The value of the requested static field as an <see cref="object" />.
        /// </returns>
        public static object GetPrivateStaticField<T>(string fieldName, BindingFlags bindingFlags = BindingFlags.Default)
        {
            FieldInfo fieldInfo = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static | bindingFlags);
            return fieldInfo.GetValue(null);
        }

        /// <summary>
        /// Gets the value of the requested private static field, using reflection, from the instance object.
        /// </summary>
        /// <typeparam name="T">The static class type.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="bindingFlags">The additional binding flags you wish to set.
        /// <see cref="BindingFlags.NonPublic" /> and <see cref="BindingFlags.Static" /> are already included.</param>
        /// <returns>
        /// The value of the requested static field as an <see cref="object" />.
        /// </returns>
        public static object GetPrivateStaticField<T>(this T instance, string fieldName, BindingFlags bindingFlags = BindingFlags.Default) => GetPrivateStaticField<T>(fieldName, bindingFlags);

        /// <summary>
        /// Sets the value of the requested private static field, using reflection, on the static object.
        /// </summary>
        /// <typeparam name="T">The static class type.</typeparam>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="bindingFlags">The additional binding flags you wish to set.
        /// <see cref="BindingFlags.NonPublic" /> and <see cref="BindingFlags.Static" /> are already included.</param>
        public static void SetPrivateStaticField<T>(string fieldName, object value, BindingFlags bindingFlags = BindingFlags.Default)
        {
            FieldInfo fieldInfo = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static | bindingFlags);
            fieldInfo.SetValue(null, value);
        }

        /// <summary>
        /// Sets the value of the requested private static field, using reflection, on the instance object.
        /// </summary>
        /// <typeparam name="T">The static class type.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="bindingFlags">The additional binding flags you wish to set.
        /// <see cref="BindingFlags.NonPublic" /> and <see cref="BindingFlags.Static" /> are already included.</param>
        public static void SetPrivateStaticField<T>(this T instance, string fieldName, object value, BindingFlags bindingFlags = BindingFlags.Default) => SetPrivateStaticField<T>(fieldName, value, bindingFlags);

        /// <summary>
        /// Gets the <see cref="MethodInfo" /> of a private instance method, using refelction, from the specified class.
        /// </summary>
        /// <typeparam name="T">The instance object type.</typeparam>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="bindingFlags">The additional binding flags you wish to set.
        /// <see cref="BindingFlags.NonPublic" /> and <see cref="BindingFlags.Instance" /> are already included.</param>
        /// <returns>
        /// The <see cref="MethodInfo" /> of the requested private method.
        /// </returns>
        public static MethodInfo GetPrivateMethod<T>(string methodName, BindingFlags bindingFlags = BindingFlags.Default)
        {
            MethodInfo methodInfo = typeof(T).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance | bindingFlags);
            return methodInfo;
        }

        /// <summary>
        /// Gets the <see cref="MethodInfo" /> of a private instance method, using refelction, from the instance object.
        /// </summary>
        /// <typeparam name="T">The instance object type.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="bindingFlags">The additional binding flags you wish to set.
        /// <see cref="BindingFlags.NonPublic" /> and <see cref="BindingFlags.Instance" /> are already included.</param>
        /// <returns>
        /// The <see cref="MethodInfo" /> of the requested private method.
        /// </returns>
        public static MethodInfo GetPrivateMethod<T>(this T instance, string methodName, BindingFlags bindingFlags = BindingFlags.Default) => GetPrivateMethod<T>(methodName, bindingFlags);

        /// <summary>
        /// Gets the <see cref="MethodInfo" /> of a private static method, using refelction, from the specified class.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="bindingFlags">The additional binding flags you wish to set.
        /// <see cref="BindingFlags.NonPublic" /> and <see cref="BindingFlags.Static" /> are already included.</param>
        /// <returns>
        /// The <see cref="MethodInfo" /> of the requested private method.
        /// </returns>
        public static MethodInfo GetPrivateStaticMethod<T>(string methodName, BindingFlags bindingFlags = BindingFlags.Default)
        {
            MethodInfo methodInfo = typeof(T).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static | bindingFlags);
            return methodInfo;
        }

        /// <summary>
        /// Gets the <see cref="MethodInfo" /> of a private static method, using refelction, from the instance object.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="bindingFlags">The additional binding flags you wish to set.
        /// <see cref="BindingFlags.NonPublic" /> and <see cref="BindingFlags.Static" /> are already included.</param>
        /// <returns>
        /// The <see cref="MethodInfo" /> of the requested private method.
        /// </returns>
        public static MethodInfo GetPrivateStaticMethod<T>(this T instance, string methodName, BindingFlags bindingFlags = BindingFlags.Default) => GetPrivateStaticMethod<T>(methodName, bindingFlags);

        /// <summary>
        /// Does a deep copy of all field values from the original instance onto the copied instance.
        /// </summary>
        /// <typeparam name="T">The class type of both objects.</typeparam>
        /// <param name="original">The original instance.</param>
        /// <param name="copy">The instance receiving the copied values.</param>
        /// <param name="bindingFlags">The additional binding flags you wish to set.
        /// <see cref="BindingFlags.Instance" /> is already included.</param>
        public static void CopyFields<T>(this T original, T copy, BindingFlags bindingFlags = BindingFlags.Default)
        {
            FieldInfo[] fieldsInfo = typeof(T).GetFields(BindingFlags.Instance | bindingFlags);

            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                if (fieldInfo.GetType().IsClass)
                {
                    var origValue = fieldInfo.GetValue(original);
                    var copyValue = fieldInfo.GetValue(copy);

                    origValue.CopyFields(copyValue);
                }
                else
                {
                    var value = fieldInfo.GetValue(original);
                    fieldInfo.SetValue(copy, value);
                }
            }
        }
    }
}
