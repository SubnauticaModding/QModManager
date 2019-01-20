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

        /// <summary>
        /// Get a <see cref="Color"/> value using <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="key"></param>
        public static Color GetColor(string key)
        {
            return GetColor(key, new Color(0, 0, 0, 0));
        }
        /// <summary>
        /// Get a <see cref="Color"/> value using <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        public static Color GetColor(string key, Color defaultValue)
        {
            float r = PlayerPrefs.GetFloat($"{key}_x", defaultValue.r);
            float g = PlayerPrefs.GetFloat($"{key}_y", defaultValue.g);
            float b = PlayerPrefs.GetFloat($"{key}_z", defaultValue.b);
            float a = PlayerPrefs.GetFloat($"{key}_w", defaultValue.a);

            return new Color(r, g, b, a);
        }
        /// <summary>
        /// Set a <see cref="Color"/> value using <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetColor(string key, Color value)
        {
            PlayerPrefs.SetFloat($"{key}_x", value.r);
            PlayerPrefs.SetFloat($"{key}_y", value.g);
            PlayerPrefs.SetFloat($"{key}_z", value.b);
            PlayerPrefs.SetFloat($"{key}_w", value.a);
        }

        /// <summary>
        /// Get a <see cref="Vector2"/> value using <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Vector2 GetVector2(string key)
        {
            return GetVector2(key, Vector2.zero);
        }
        /// <summary>
        /// Get a <see cref="Vector2"/> value using <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Vector2 GetVector2(string key, Vector2 defaultValue)
        {
            float x = PlayerPrefs.GetFloat($"{key}_x", defaultValue.x);
            float y = PlayerPrefs.GetFloat($"{key}_y", defaultValue.y);

            return new Vector2(x, y);
        }
        /// <summary>
        /// Set a <see cref="Vector2"/> value using <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetVector2(string key, Vector2 value)
        {
            PlayerPrefs.SetFloat($"{key}_x", value.x);
            PlayerPrefs.SetFloat($"{key}_y", value.y);
        }

        /// <summary>
        /// Get a <see cref="Vector2int"/> value using <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Vector2int GetVector2int(string key)
        {
            return GetVector2int(key, new Vector2int(0, 0));
        }
        /// <summary>
        /// Get a <see cref="Vector2int"/> value using <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Vector2int GetVector2int(string key, Vector2int defaultValue)
        {
            int x = PlayerPrefs.GetInt($"{key}_vector2int_x", defaultValue.x);
            int y = PlayerPrefs.GetInt($"{key}_vector2int_y", defaultValue.y);

            return new Vector2int(x, y);
        }
        /// <summary>
        /// Set a <see cref="Vector2int"/> value using <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetVector2int(string key, Vector2int value)
        {
            PlayerPrefs.SetInt($"{key}_vector2int_x", value.x);
            PlayerPrefs.SetInt($"{key}_vector2int_y", value.y);
        }

        /// <summary>
        /// Get a <see cref="Vector3"/> value using <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Vector3 GetVector3(string key)
        {
            return GetVector3(key, Vector3.zero);
        }
        /// <summary>
        /// Get a <see cref="Vector3"/> value using <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Vector3 GetVector3(string key, Vector3 defaultValue)
        {
            float x = PlayerPrefs.GetFloat($"{key}_x", defaultValue.x);
            float y = PlayerPrefs.GetFloat($"{key}_y", defaultValue.y);
            float z = PlayerPrefs.GetFloat($"{key}_z", defaultValue.z);

            return new Vector3(x, y, z);
        }
        /// <summary>
        /// Set a <see cref="Vector3"/> value using <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetVector3(string key, Vector3 value)
        {
            PlayerPrefs.SetFloat($"{key}_x", value.x);
            PlayerPrefs.SetFloat($"{key}_y", value.y);
            PlayerPrefs.SetFloat($"{key}_z", value.z);
        }

        /// <summary>
        /// Get a <see cref="Vector4"/> value using <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Vector4 GetVector4(string key)
        {
            return GetVector4(key, Vector4.zero);
        }
        /// <summary>
        /// Get a <see cref="Vector4"/> value using <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Vector4 GetVector4(string key, Vector4 defaultValue)
        {
            float x = PlayerPrefs.GetFloat($"{key}_x", defaultValue.x);
            float y = PlayerPrefs.GetFloat($"{key}_y", defaultValue.y);
            float z = PlayerPrefs.GetFloat($"{key}_z", defaultValue.z);
            float w = PlayerPrefs.GetFloat($"{key}_w", defaultValue.w);

            return new Vector4(x, y, z, w);
        }
        /// <summary>
        /// Set a <see cref="Vector4"/> value using <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetVector4(string key, Vector4 value)
        {
            PlayerPrefs.SetFloat($"{key}_x", value.x);
            PlayerPrefs.SetFloat($"{key}_y", value.y);
            PlayerPrefs.SetFloat($"{key}_z", value.z);
            PlayerPrefs.SetFloat($"{key}_w", value.w);
        }

        /// <summary>
        /// Get a <see cref="Quaternion"/> value using <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Quaternion GetQuaternion(string key)
        {
            return GetQuaternion(key, Quaternion.identity);
        }
        /// <summary>
        /// Get a <see cref="Quaternion"/> value using <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Quaternion GetQuaternion(string key, Quaternion defaultValue)
        {
            float x = PlayerPrefs.GetFloat($"{key}_x", defaultValue.x);
            float y = PlayerPrefs.GetFloat($"{key}_y", defaultValue.y);
            float z = PlayerPrefs.GetFloat($"{key}_z", defaultValue.z);
            float w = PlayerPrefs.GetFloat($"{key}_w", defaultValue.w);

            return new Quaternion(x, y, z, w);
        }
        /// <summary>
        /// Set a <see cref="Quaternion"/> value using <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetQuaternion(string key, Quaternion value)
        {
            PlayerPrefs.SetFloat($"{key}_x", value.x);
            PlayerPrefs.SetFloat($"{key}_y", value.y);
            PlayerPrefs.SetFloat($"{key}_z", value.z);
            PlayerPrefs.SetFloat($"{key}_w", value.w);
        }
    }
}
