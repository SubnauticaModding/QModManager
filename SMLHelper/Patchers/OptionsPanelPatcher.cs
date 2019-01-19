namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using Options;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    internal class OptionsPanelPatcher
    {
        internal static List<ModOptions> modOptions = new List<ModOptions>();

        internal static void Patch(HarmonyInstance harmony)
        {
            Type uGUI_OptionsPanelType = typeof(uGUI_OptionsPanel);
            Type thisType = typeof(OptionsPanelPatcher);
            MethodInfo startMethod = uGUI_OptionsPanelType.GetMethod("AddTabs", BindingFlags.NonPublic | BindingFlags.Instance);

            harmony.Patch(startMethod, null, new HarmonyMethod(thisType.GetMethod("AddTabs_Postfix", BindingFlags.NonPublic | BindingFlags.Static)));
        }

        internal static void AddTabs_Postfix(uGUI_OptionsPanel __instance)
        {
            uGUI_OptionsPanel optionsPanel = __instance;
            int modsTab = optionsPanel.AddTab("Mods");

            foreach (ModOptions modOption in modOptions)
            {
                optionsPanel.AddHeading(modsTab, modOption.Name);

                foreach (ModOption option in modOption.Options)
                {
                    switch (option.Type)
                    {
                        case ModOptionType.Slider:
                            var slider = (ModSliderOption)option;

                            optionsPanel.AddSliderOption(modsTab, slider.Label, slider.Value, slider.MinValue, slider.MaxValue, slider.Value,
                                new UnityAction<float>((float sliderVal) =>
                                    modOption.OnSliderChange(slider.Id, sliderVal)));
                            break;
                        case ModOptionType.Toggle:
                            var toggle = (ModToggleOption)option;

                            optionsPanel.AddToggleOption(modsTab, toggle.Label, toggle.Value,
                                new UnityAction<bool>((bool toggleVal) =>
                                    modOption.OnToggleChange(toggle.Id, toggleVal)));
                            break;
                        case ModOptionType.Choice:
                            var choice = (ModChoiceOption)option;

                            optionsPanel.AddChoiceOption(modsTab, choice.Label, choice.Options, choice.Index,
                                new UnityAction<int>((int index) =>
                                    modOption.OnChoiceChange(choice.Id, index)));
                            break;
                        case ModOptionType.Keybind:
                            var keybind = (ModKeybindOption)option;

                            AddBindingOptionWithCallback(optionsPanel, modsTab, keybind.Label, keybind.Key, keybind.Device,
                                new UnityAction<KeyCode>((KeyCode key) => 
                                    modOption.OnKeybindChange(keybind.Id, key)));
                            break;
                        case ModOptionType.Dropdown:
                            var dropdown = (ModDropdownOption)option;

                            optionsPanel.AddDropdownOption(modsTab, dropdown.Label, dropdown.Options, dropdown.Index,
                                new UnityAction<int>((int index) =>
                                    modOption.OnDropdownChange(dropdown.Id, index)));
                            break;
                        default:
                            V2.Logger.Log($"Invalid ModOptionType detected for option: {option.Id} ({option.Type.ToString()})");
                            break;
                    }
                }
            }
        }

        internal static string KeyCodeToString(KeyCode keyCode)
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
        internal static KeyCode StringToKeyCode(string s)
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
                        V2.Logger.Log($"Failed to parse {s} to a valid KeyCode.");
                        return 0;
                    }
            }
        }

        internal static GameObject AddBindingOptionWithCallback(uGUI_OptionsPanel panel, int tab, string label, KeyCode key, GameInput.Device device, UnityAction<KeyCode> callback)
        {
            GameObject gameObject = panel.AddItem(tab, panel.bindingOptionPrefab);
            Text text = gameObject.GetComponentInChildren<Text>();
            if (text != null)
            {
                //gameObject.GetComponentInChildren<TranslationLiveUpdate>().translationKey = _label;
                //text.text = Language.main.Get(_label);
                text.text = label;
            }
            uGUI_Bindings bindings = gameObject.GetComponentInChildren<uGUI_Bindings>();
            uGUI_Binding binding = bindings.bindings.First();
            UnityEngine.Object.Destroy(bindings.bindings.Last().gameObject);
            UnityEngine.Object.Destroy(bindings);
            binding.device = device;
            binding.value = KeyCodeToString(key);
            binding.onValueChanged.RemoveAllListeners();
            binding.onValueChanged.AddListener(new UnityAction<string>((string s) => callback?.Invoke(StringToKeyCode(s))));
            return gameObject;
        }
    }
}
