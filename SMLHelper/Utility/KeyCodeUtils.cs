namespace SMLHelper.V2.Utility
{
    using System;
    using UnityEngine;

    /// <summary>
    /// A collection of <see cref="KeyCode"/> related utility methods.
    /// </summary>
    public static class KeyCodeUtils
    {
        /// <summary>
        /// Turn a <seealso cref="KeyCode"/> into a <seealso cref="string"/>
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public static string KeyCodeToString(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.Alpha0:
                    return "0";
                case KeyCode.Alpha1:
                    return "1";
                case KeyCode.Alpha2:
                    return "2";
                case KeyCode.Alpha3:
                    return "3";
                case KeyCode.Alpha4:
                    return "4";
                case KeyCode.Alpha5:
                    return "5";
                case KeyCode.Alpha6:
                    return "6";
                case KeyCode.Alpha7:
                    return "7";
                case KeyCode.Alpha8:
                    return "8";
                case KeyCode.Alpha9:
                    return "9";
                case KeyCode.Mouse0:
                    return "MouseButtonLeft";
                case KeyCode.Mouse1:
                    return "MouseButtonRight";
                case KeyCode.Mouse2:
                    return "MouseButtonMiddle";
                case KeyCode.JoystickButton0:
                    return "ControllerButtonA";
                case KeyCode.JoystickButton1:
                    return "ControllerButtonB";
                case KeyCode.JoystickButton2:
                    return "ControllerButtonX";
                case KeyCode.JoystickButton3:
                    return "ControllerButtonY";
                case KeyCode.JoystickButton4:
                    return "ControllerButtonLeftBumper";
                case KeyCode.JoystickButton5:
                    return "ControllerButtonRightBumper";
                case KeyCode.JoystickButton6:
                    return "ControllerButtonBack";
                case KeyCode.JoystickButton7:
                    return "ControllerButtonHome";
                case KeyCode.JoystickButton8:
                    return "ControllerButtonLeftStick";
                case KeyCode.JoystickButton9:
                    return "ControllerButtonRightStick";
                default:
                    return keyCode.ToString();
            }
        }

        /// <summary>
        /// Turn a <seealso cref="string"/> into a <seealso cref="KeyCode"/>
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static KeyCode StringToKeyCode(string s)
        {
            switch (s)
            {
                case "0":
                    return KeyCode.Alpha0;
                case "1":
                    return KeyCode.Alpha1;
                case "2":
                    return KeyCode.Alpha2;
                case "3":
                    return KeyCode.Alpha3;
                case "4":
                    return KeyCode.Alpha4;
                case "5":
                    return KeyCode.Alpha5;
                case "6":
                    return KeyCode.Alpha6;
                case "7":
                    return KeyCode.Alpha7;
                case "8":
                    return KeyCode.Alpha8;
                case "9":
                    return KeyCode.Alpha9;
                case "MouseButtonLeft":
                    return KeyCode.Mouse0;
                case "MouseButtonRight":
                    return KeyCode.Mouse1;
                case "MouseButtonMiddle":
                    return KeyCode.Mouse2;
                case "ControllerButtonA":
                    return KeyCode.JoystickButton0;
                case "ControllerButtonB":
                    return KeyCode.JoystickButton1;
                case "ControllerButtonX":
                    return KeyCode.JoystickButton2;
                case "ControllerButtonY":
                    return KeyCode.JoystickButton3;
                case "ControllerButtonLeftBumper":
                    return KeyCode.JoystickButton4;
                case "ControllerButtonRightBumper":
                    return KeyCode.JoystickButton5;
                case "ControllerButtonBack":
                    return KeyCode.JoystickButton6;
                case "ControllerButtonHome":
                    return KeyCode.JoystickButton7;
                case "ControllerButtonLeftStick":
                    return KeyCode.JoystickButton8;
                case "ControllerButtonRightStick":
                    return KeyCode.JoystickButton9;
                default:
                    try
                    {
                        return (KeyCode)Enum.Parse(typeof(KeyCode), s);
                    }
                    catch (Exception)
                    {
                        V2.Logger.Log($"Failed to parse {s} as a valid KeyCode!", LogLevel.Error);
                        return 0;
                    }
            }
        }

        internal static GameInput.InputState GetInputStateForKeyCode(KeyCode keyCode)
        {
            var inputState = default(GameInput.InputState);
            if (!GameInput.clearInput && !GameInput.scanningInput)
            {
                for (var i = 0; i < GameInput.inputs.Count; i++)
                {
                    if (GameInput.inputs[i].keyCode == keyCode)
                    {
                        inputState.flags |= GameInput.inputStates[i].flags;
                        inputState.timeDown = Mathf.Max(inputState.timeDown, GameInput.inputStates[i].timeDown);
                        break;
                    }
                }
            }
            return inputState;
        }

        /// <summary>
        /// Check this is the first frame a key has been pressed
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns>True during the first frame a key has been pressed, otherwise false</returns>
        /// <seealso cref="KeyCode"/>
        /// <seealso cref="GetKeyDown(string)"/>
        public static bool GetKeyDown(KeyCode keyCode)
            => (GetInputStateForKeyCode(keyCode).flags & GameInput.InputStateFlags.Down) > 0U;
        /// <summary>
        /// Check this is the first frame a key has been pressed.
        /// </summary>
        /// <param name="s"></param>
        /// <returns>True during the first frame a key has been pressed, otherwise false</returns>
        /// <seealso cref="GetKeyDown(KeyCode)"/>
        public static bool GetKeyDown(string s) => GetKeyDown(StringToKeyCode(s));

        /// <summary>
        /// Check a key is currently held down
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns>True every frame a key is held down, otherwise false</returns>
        /// <seealso cref="KeyCode"/>
        /// <seealso cref="GetKeyHeld(string)"/>
        public static bool GetKeyHeld(KeyCode keyCode)
            => (GetInputStateForKeyCode(keyCode).flags & GameInput.InputStateFlags.Held) > 0U;
        /// <summary>
        /// Check a key is currently held down
        /// </summary>
        /// <param name="s"></param>
        /// <returns>True every frame a key is held down, otherwise false</returns>
        /// <seealso cref="GetKeyHeld(KeyCode)"/>
        public static bool GetKeyHeld(string s) => GetKeyHeld(StringToKeyCode(s));

        /// <summary>
        /// Check how long a key has been held down
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        /// <seealso cref="KeyCode"/>
        /// <seealso cref="GetKeyHeldTime(string)"/>
        public static float GetKeyHeldTime(KeyCode keyCode)
        {
            var inputStateForKeyCode = GetInputStateForKeyCode(keyCode);
            if ((inputStateForKeyCode.flags & GameInput.InputStateFlags.Held) == 0U)
            {
                return 0f;
            }
            return Time.unscaledTime - inputStateForKeyCode.timeDown;
        }
        /// <summary>
        /// Check how long a key has been held down
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <seealso cref="GetKeyHeldTime(KeyCode)"/>
        public static float GetKeyHeldTime(string s) => GetKeyHeldTime(StringToKeyCode(s));

        /// <summary>
        /// Check this is the frame a key has been released
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns>True during the first frame a key has been released, otherwise false</returns>
        /// <seealso cref="KeyCode"/>
        /// <seealso cref="GetKeyUp(string)"/>
        public static bool GetKeyUp(KeyCode keyCode)
            => (GetInputStateForKeyCode(keyCode).flags & GameInput.InputStateFlags.Up) > 0U;
        /// <summary>
        /// Check this is the first frame a key has been released
        /// </summary>
        /// <param name="s"></param>
        /// <returns>True during the first frame a key has been released, otherwise false</returns>
        /// <seealso cref="GetKeyUp(KeyCode)"/>
        public static bool GetKeyUp(string s) => GetKeyUp(StringToKeyCode(s));

        /// <summary>
        /// Gets the analog value for a <seealso cref="KeyCode"/> following the same logic as
        /// <seealso cref="GameInput.GetAnalogValueForButton(GameInput.Button)"/>
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns>1f while a key is being held, otherwise 0f</returns>
        /// <seealso cref="KeyCode"/>
        /// <seealso cref="GetAnalogValueForKey(string)"/>
        public static float GetAnalogValueForKey(KeyCode keyCode)
        {
            var inputStateForKeyCode = GetInputStateForKeyCode(keyCode);
            if ((inputStateForKeyCode.flags & GameInput.InputStateFlags.Held) != 0U)
            {
                return 1f;
            }
            return 0f;
        }
        /// <summary>
        /// Gets the analog value for a key by <seealso cref="string"/> value, following the same logic as
        /// <seealso cref="GameInput.GetAnalogValueForButton(GameInput.Button)"/>
        /// </summary>
        /// <param name="s"></param>
        /// <returns>1f while a key is being held, otherwise 0f</returns>
        /// <seealso cref="GetAnalogValueForKey(KeyCode)"/>
        public static float GetAnalogValueForKey(string s) => GetAnalogValueForKey(StringToKeyCode(s));
    }
}
