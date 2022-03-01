namespace SMLHelper.V2.Options.Attributes
{
    using HarmonyLib;
    using Json;
    using System;
    using System.Linq;
    using System.Reflection;

    internal enum MemberType { Unknown, Field, Property, Method };

    internal class MemberInfoMetadata<T> where T : ConfigFile, new()
    {
        public MemberType MemberType = MemberType.Unknown;
        public string Name;
        public Type ValueType;
        public Type[] MethodParameterTypes;
        public bool MethodValid = false;

        /// <summary>
        /// Uses the stored metadata to get the current value of the member.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="config">The config to get the value from.</param>
        /// <returns>The value.</returns>
        public TValue GetValue<TValue>(T config)
        {
            switch (MemberType)
            {
                case MemberType.Field:
                    return Traverse.Create(config).Field(Name).GetValue<TValue>();
                case MemberType.Property:
                    return Traverse.Create(config).Property(Name).GetValue<TValue>();
                default:
                    throw new InvalidOperationException($"Member must be a Field or Property but is {MemberType}: " +
                        $"{typeof(T).Name}.{Name}");
            }
        }

        /// <summary>
        /// Uses the stored metadata to get the current value of the member.
        /// </summary>
        /// <param name="config">The config to get the value from.</param>
        /// <returns>The value.</returns>
        public object GetValue(T config) => GetValue<object>(config);

        /// <summary>
        /// Uses the stored metadata to set the current value of the member.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="config">The config the set the value in.</param>
        /// <param name="value">The value.</param>
        public void SetValue<TValue>(T config, TValue value)
        {
            switch (MemberType)
            {
                case MemberType.Field:
                    Traverse.Create(config).Field(Name).SetValue(value);
                    break;
                case MemberType.Property:
                    Traverse.Create(config).Property(Name).SetValue(value);
                    break;
                default:
                    throw new InvalidOperationException($"Member must be a Field or Property but is {MemberType}: " +
                        $"{typeof(T).Name}.{Name}");
            }
        }

        /// <summary>
        /// Stores the <see cref="Type"/> of each parameter of a method to the
        /// <see cref="MethodParameterTypes"/> array.
        /// </summary>
        /// <param name="methodInfo"><see cref="MethodInfo"/> of the method to parse.</param>
        public void ParseMethodParameterTypes(MethodInfo methodInfo = null)
        {
            if (MemberType != MemberType.Method)
                throw new InvalidOperationException($"Member must be a Method but is {MemberType}: {typeof(T).Name}.{Name}");

            if (methodInfo == null)
            {
                methodInfo = AccessTools.Method(typeof(T), Name);

                if (methodInfo == null)
                {
                    // Method not found, log error and skip.
                    Logger.Error($"[OptionsMenuBuilder] Could not find the specified method: {typeof(T)}.{Name}");
                    return;
                }
            }

            MethodValid = true;
            MethodParameterTypes = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray();
        }

        /// <summary>
        /// Use the stored metadata to invoke the method.
        /// </summary>
        /// <param name="config">The config in which the method.</param>
        /// <param name="arguments">An array of arguments to pass to the method.</param>
        public void InvokeMethod(T config, params object[] arguments)
        {
            if (MemberType != MemberType.Method)
                throw new InvalidOperationException($"Member must be a Method but is {MemberType}: {typeof(T).Name}.{Name}");

            if (!MethodValid)
            {
                // Method not found, log error and skip.
                Logger.Error($"[OptionsMenuBuilder] Could not find the specified method: {typeof(T)}.{Name}");
                return;
            }

            Traverse.Create(config).Method(Name, MethodParameterTypes).GetValue(arguments);
        }
    }
}
