using System;
using UnityEngine;

namespace QModManager.Utility
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Turns a <see cref="Version"/> into a <see cref="string"/> with as few as possible digits <para/>
        /// Example: <see langword="new"/> <see cref="Version"/>(1, 2, 0, 0) will be displayed as "1.2" instead of "1.2.0.0"
        /// </summary>
        /// <param name="version">The <see cref="Version"/> which to turn into a <see cref="string"/></param>
        /// <returns>A <see cref="string"/> representing the provided <see cref="Version"/></returns>
        /// <exception cref="ArgumentNullException"/>
        public static string ToStringParsed(this Version version)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));
            if (version.Revision == 0)
                if (version.Build == 0)
                    if (version.Minor == 0)
                        return version.ToString(1);
                    else
                        return version.ToString(2);
                else
                    return version.ToString(3);
            else return version.ToString(4);
        }

        /// <summary>
        /// Gets a component from a <see cref="GameObject"/>, or adds it if it doesn't already exist
        /// </summary>
        /// <typeparam name="T">The component which to add to the <see cref="GameObject"/></typeparam>
        /// <param name="gameObject">The <see cref="GameObject"/> on which to add the component</param>
        /// <returns>The added component</returns>
        /// <exception cref="ArgumentNullException"/>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (!gameObject) throw new ArgumentNullException(nameof(gameObject));
            return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }
        /// <summary>
        /// Gets a component from the <see cref="GameObject"/> this <see cref="MonoBehaviour"/> is attached to, or adds it if it doesn't already exist
        /// </summary>
        /// <typeparam name="T">The component which to add to the <see cref="GameObject"/></typeparam>
        /// <param name="behaviour">The <see cref="MonoBehaviour"/> attached to the <see cref="GameObject"/></param>
        /// <returns>The added component</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="NullReferenceException"/>
        public static T GetOrAddComponent<T>(this MonoBehaviour behaviour) where T : Component
        {
            if (!behaviour) throw new ArgumentNullException(nameof(behaviour));
            if (!behaviour.gameObject) throw new NullReferenceException($"The provided component is not attached to a GameObject!");
            return behaviour.gameObject.GetOrAddComponent<T>();
        }
    }
}
