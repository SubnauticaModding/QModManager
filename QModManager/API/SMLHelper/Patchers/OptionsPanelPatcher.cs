namespace QModManager.API.SMLHelper.Patchers
{
    using Harmony;
    using Options;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using Logger = QModManager.Utility.Logger;

    internal static class OptionsPanelPatcher
    {
        internal static int ModsTab { get => OptionsManager.ModsTab; }

        internal static SortedList<string, ModOptions> modOptions = new SortedList<string, ModOptions>();

        internal static void Patch(HarmonyInstance harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(uGUI_OptionsPanel), "AddTabs"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(OptionsPanelPatcher), "AddTabs_Postfix")));

            Logger.Debug("OptionsPanelPatcher is done.");
        }

        internal static void AddTabs_Postfix(uGUI_OptionsPanel __instance)
        {
            uGUI_OptionsPanel optionsPanel = __instance;

            optionsPanel.AddChoiceOption(ModsTab, "Extra item info", new string[]
            {
                "Mod name (default)",
                "Mod name and item ID",
                "Nothing"
            }, (int)TooltipPatcher.ExtraItemInfoOption, (i) => TooltipPatcher.SetExtraItemInfo((TooltipPatcher.ExtraItemInfo)i));

            foreach (ModOptions modOptions in modOptions.Values)
            {
                optionsPanel.AddHeading(ModsTab, modOptions.Name);

                foreach (ModOption option in modOptions.Options)
                {
                    switch (option.Type)
                    {
                        case ModOptionType.Slider:
                            AddSliderOption(option, modOptions, optionsPanel);
                            break;
                        case ModOptionType.Toggle:
                            AddToggleOption(option, modOptions, optionsPanel);
                            break;
                        case ModOptionType.Choice:
                            AddChoiceOption(option, modOptions, optionsPanel);
                            break;
                        case ModOptionType.Keybind:
                            AddKeybindOption(option, modOptions, optionsPanel);
                            break;
                        default:
                            Logger.Error($"Invalid ModOptionType detected for option: {option.Id} ({option.Type.ToString()})");
                            break;
                    }
                }
            }
        }

        internal static void AddSliderOption(ModOption option, ModOptions modOptions, uGUI_OptionsPanel optionsPanel)
        {
            ModSliderOption slider = (ModSliderOption)option;

            optionsPanel.AddSliderOption(ModsTab, slider.Label, slider.Value, slider.MinValue, slider.MaxValue, slider.Value,
                new UnityAction<float>((float sliderVal) =>
                    modOptions.OnSliderChange(slider.Id, sliderVal)));
        }

        internal static void AddToggleOption(ModOption option, ModOptions modOptions, uGUI_OptionsPanel optionsPanel)
        {
            ModToggleOption toggle = (ModToggleOption)option;

            optionsPanel.AddToggleOption(ModsTab, toggle.Label, toggle.Value,
                new UnityAction<bool>((bool toggleVal) =>
                    modOptions.OnToggleChange(toggle.Id, toggleVal)));
        }

        internal static void AddChoiceOption(ModOption option, ModOptions modOptions, uGUI_OptionsPanel optionsPanel)
        {
            ModChoiceOption choice = (ModChoiceOption)option;

            optionsPanel.AddChoiceOption(ModsTab, choice.Label, choice.Options, choice.Index,
                new UnityAction<int>((int index) =>
                    modOptions.OnChoiceChange(choice.Id, index, choice.Options[index])));
        }

        internal static void AddKeybindOption(ModOption option, ModOptions modOptions, uGUI_OptionsPanel optionsPanel)
        {
            ModKeybindOption keybind = (ModKeybindOption)option;

            ModKeybindOption.AddBindingOptionWithCallback(optionsPanel, ModsTab, keybind.Label, keybind.Key, keybind.Device,
                new UnityAction<KeyCode>((KeyCode key) =>
                    modOptions.OnKeybindChange(keybind.Id, key)));
        }
    }
}
