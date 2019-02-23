using System;
using UnityEngine;

namespace QModManager
{
    public static class Utility
    {
        public static string ToStringParsed(this Version version)
        {
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

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }
        public static T GetOrAddComponent<T>(this MonoBehaviour behaviour) where T : Component
        {
            return behaviour.gameObject.GetOrAddComponent<T>();
        }
    }
}
