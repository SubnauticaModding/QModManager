using System;
using UnityEngine;

namespace QModManager.Utility
{
    internal static class ExtensionMethods
    {
        internal static string ToStringParsed(this Version version)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));

            string text = version.ToString();

            while (text.EndsWith(".0"))
                text = text.Substring(0, text.Length - 2);

            return text;
        }

        internal static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (!gameObject) throw new ArgumentNullException(nameof(gameObject));
            return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }
        internal static T GetOrAddComponent<T>(this MonoBehaviour behaviour) where T : Component
        {
            if (!behaviour) throw new ArgumentNullException(nameof(behaviour));
            if (!behaviour.gameObject) throw new ArgumentException($"The provided component is not attached to a GameObject!");
            return behaviour.gameObject.GetOrAddComponent<T>();
        }

        internal static string Repeat(this char c, int count)
        {
            string s = "";
            for (int i = 0; i < count; i++) s += c;
            return s;
        }
    }
}
