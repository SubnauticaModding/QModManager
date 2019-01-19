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
    using Utility;

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
            binding.value = KeyCodeUtils.KeyCodeToString(key);
            binding.onValueChanged.RemoveAllListeners();
            binding.onValueChanged.AddListener(new UnityAction<string>((string s) => callback?.Invoke(KeyCodeUtils.StringToKeyCode(s))));
            return gameObject;
        }
    }
}
