using System;
using UnityEngine;

namespace QModManager.Utility
{
    internal static class ExtensionMethods
    {
        internal static string ToStringParsed(this Version version)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));
            if (version.Revision == 0)
                if (version.Build == 0)
                    if (version.Minor == 0)
                        return version.ToString(1);
                    else
                        return version.ToString(2);
                else
                    return version.ToString(3);
            else
                return version.ToString(4);
        }

        internal static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (!gameObject)
                throw new ArgumentNullException(nameof(gameObject));
            return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }
        internal static T GetOrAddComponent<T>(this MonoBehaviour behaviour) where T : Component
        {
            if (!behaviour)
                throw new ArgumentNullException(nameof(behaviour));
            if (!behaviour.gameObject)
                throw new NullReferenceException($"The provided component is not attached to a GameObject!");
            return behaviour.gameObject.GetOrAddComponent<T>();
        }
    }
}
