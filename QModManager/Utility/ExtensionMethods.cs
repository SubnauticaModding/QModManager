using System;
using UnityEngine;

namespace QModManager.Utility
{
    /// <summary>
    /// Miscellaneous extension methods to make your life easier
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Turns a <see cref="Version"/> into its <see cref="string"/> representation and removes all leading zeroes
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static string ToStringParsed(this Version version)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));

            string text = version.ToString();

            while (text.EndsWith(".0"))
                text = text.Substring(0, text.Length - 2);

            return text;
        }

        /// <summary>
        /// Gets a component from a <see cref="GameObject"/>, or adds it if it doesn't exist
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (!gameObject) throw new ArgumentNullException(nameof(gameObject));
            return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }

        /// <summary>
        /// Gets a component from a <see cref="GameObject"/>, or adds it if it doesn't exist
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(this MonoBehaviour behaviour) where T : Component
        {
            if (!behaviour) throw new ArgumentNullException(nameof(behaviour));
            if (!behaviour.gameObject) throw new ArgumentException($"The provided component is not attached to a GameObject!");
            return behaviour.gameObject.GetOrAddComponent<T>();
        }
    }
}
