using UnityEngine;

namespace SMLHelper.V2.Utility
{
    public static class PlayerPrefsExtra
    {
        /// <summary>
        /// Get a <see cref="bool"/> value using <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool GetBool(string key, bool defaultValue)
        {
            return PlayerPrefs.GetInt(key, defaultValue == true ? 1 : 0) == 1 ? true : false;
        }
        /// <summary>
        /// Set a <see cref="bool"/> value using <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value == true ? 1 : 0);
        }

        /// <summary>
        /// Get a <see cref="KeyCode"/> value using <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static KeyCode GetKeyCode(string key, KeyCode defaultValue)
        {
            return KeyCodeUtils.StringToKeyCode(PlayerPrefs.GetString(key, KeyCodeUtils.KeyCodeToString(defaultValue)));
        }
        /// <summary>
        /// Set a <see cref="KeyCode"/> value using <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetKeyCode(string key, KeyCode value)
        {
            PlayerPrefs.SetString(key, KeyCodeUtils.KeyCodeToString(value));
        }
    }
}
