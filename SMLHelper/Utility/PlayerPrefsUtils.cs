using UnityEngine;

namespace SMLHelper.V2.Utility
{
    // Probably need to come up with a better name, since there is already a class name like this in the global namespace
    public static class PlayerPrefsUtils
    {
        public static bool GetBool(string key, bool defaultValue)
        {
            return PlayerPrefs.GetInt(key, defaultValue == true ? 1 : 0) == 1 ? true : false;
        }
        public static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value == true ? 1 : 0);
        }
    }
}
