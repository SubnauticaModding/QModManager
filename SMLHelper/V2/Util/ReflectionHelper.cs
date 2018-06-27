namespace SMLHelper.V2.Util
{
    using System;
    using System.Reflection;

    /// <summary>
    /// An extension method class for simplifying reflection calls for improved readability.
    /// </summary>
    public static class ReflectionHelper
    {

        /// <summary>
        /// Gets the value of a field from an instance of a class using reflection
        /// <para/> Extension from <see langword="AlexejheroYTB.Utilities.Extensions"/>
        /// </summary>
        /// <typeparam name="classInstance">The class type</typeparam>
        /// <param name="instance">The instance of <typeparamref name="classInstance"/></param>
        /// <param name="field">The name of the field</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> to use with <see cref="Type.GetField(string, BindingFlags)"/></param>
        /// <returns>The value of field from the instance of the class</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="TargetException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="FieldAccessException"/>
        /// <exception cref="ArgumentException"/>
        public static object GetInstanceField<classInstance>(this classInstance instance, string field, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance) where classInstance : class
            => typeof(classInstance).GetField(field, bindingFlags).GetValue(instance);

        /// <summary>
        /// Sets the value of a field from an instance of a class using reflection
        /// <para/> Extension from <see langword="AlexejheroYTB.Utilities.Extensions"/>
        /// </summary>
        /// <typeparam name="classInstance">The class type</typeparam>
        /// <param name="instance">The instance of <typeparamref name="classInstance"/></param>
        /// <param name="field">The name of the field</param>
        /// <param name="newValue">The new value to set</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> to use with <see cref="Type.GetField(string, BindingFlags)"/></param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FieldAccessException"/>
        /// <exception cref="TargetException"/>
        /// <exception cref="ArgumentException"/>
        public static void SetInstanceField<classInstance>(this classInstance instance, string field, object newValue, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance) where classInstance : class
            => typeof(classInstance).GetField(field, bindingFlags).SetValue(instance, newValue);


        /// <summary>
        /// Gets the value of a field from a static class using reflection
        /// <para/> Extension from <see langword="AlexejheroYTB.Utilities.Extensions"/>
        /// </summary>
        /// <typeparam name="class">The class to get the value from</typeparam>
        /// <param name="_class">An instance of the class. Not actually used for anything, just to make it an extension method</param>
        /// <param name="field">The name of the field</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> to use with <see cref="Type.GetField(string, BindingFlags)"/></param>
        /// <returns>The value of field from the instance of the class</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="TargetException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="FieldAccessException"/>
        /// <exception cref="ArgumentException"/>
        public static object GetStaticField<@class>(this @class _class, string field, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Static) where @class : class
            => typeof(@class).GetField(field, bindingFlags).GetValue(null);
        
        /// <summary>
        /// Sets the value of a field from a static class using reflection
        /// <para/> Extension from <see langword="AlexejheroYTB.Utilities.Extensions"/>
        /// </summary>
        /// <typeparam name="class">The class to get the value from</typeparam>
        /// <param name="_class">An instance of the class. Not actually used for anything, just to make it an extension method</param>
        /// <param name="field">The name of the field</param>
        /// <param name="newValue">The new value to set</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> to use with <see cref="Type.GetField(string, BindingFlags)"/></param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FieldAccessException"/>
        /// <exception cref="TargetException"/>
        /// <exception cref="ArgumentException"/>
        public static void SetStaticField<@class>(this @class _class, string field, object newValue, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Static) where @class : class
            => typeof(@class).GetField(field, bindingFlags).SetValue(null, newValue);

        /// <summary>
        /// Gets the value of a field from a static class using reflection
        /// <para/> Extension from <see langword="AlexejheroYTB.Utilities.Extensions"/>
        /// </summary>
        /// <typeparam name="class">The class to get the value from</typeparam>
        /// <param name="field">The name of the field</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> to use with <see cref="Type.GetField(string, BindingFlags)"/></param>
        /// <returns>The value of field from the instance of the class</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="TargetException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="FieldAccessException"/>
        /// <exception cref="ArgumentException"/>
        public static object GetStaticField<@class>(string field, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Static) where @class : class
            => typeof(@class).GetField(field, bindingFlags).GetValue(null);

        /// <summary>
        /// Sets the value of a field from a static class using reflection
        /// <para/> Extension from <see langword="AlexejheroYTB.Utilities.Extensions"/>
        /// </summary>
        /// <typeparam name="class">The class to get the value from</typeparam>
        /// <param name="field">The name of the field</param>
        /// <param name="newValue">The new value to set</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> to use with <see cref="Type.GetField(string, BindingFlags)"/></param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FieldAccessException"/>
        /// <exception cref="TargetException"/>
        /// <exception cref="ArgumentException"/>
        public static void SetStaticField<@class>(string field, object newValue, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Static) where @class : class
            => typeof(@class).GetField(field, bindingFlags).SetValue(null, newValue);

    }
}
