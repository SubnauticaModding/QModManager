namespace QModManager.API.SMLHelper.Patchers
{
    using Harmony;
    using Options;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    internal static class OptionsPanelPatcher
    {
        internal static SortedList<string, ModOptions> modOptions = new SortedList<string, ModOptions>();

        internal static void Patch(HarmonyInstance harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(uGUI_OptionsPanel), "AddTabs"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(OptionsPanelPatcher), "AddTabs_Postfix")));
        }

        internal static void AddTabs_Postfix(uGUI_OptionsPanel __instance)
        {
            uGUI_OptionsPanel optionsPanel = __instance;

            // Start the modsTab index at a value of -1
            var modsTab = OptionsManager.ModsTab;

            optionsPanel.AddChoiceOption(modsTab, "Extra item info", new string[]
            {
                "Mod name (default)",
                "Mod name and item ID",
                "Nothing"
            }, (int)TooltipPatcher.ExtraItemInfoOption, (i) => TooltipPatcher.SetExtraItemInfo((TooltipPatcher.ExtraItemInfo)i));

            foreach (ModOptions modOption in modOptions.Values)
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
                                    modOption.OnChoiceChange(choice.Id, index, choice.Options[index])));
                            break;
                        case ModOptionType.Keybind:
                            var keybind = (ModKeybindOption)option;

                            ModKeybindOption.AddBindingOptionWithCallback(optionsPanel, modsTab, keybind.Label, keybind.Key, keybind.Device,
                                new UnityAction<KeyCode>((KeyCode key) => 
                                    modOption.OnKeybindChange(keybind.Id, key)));
                            break;
                        default:
                            API.SMLHelper.Logger.Log($"Invalid ModOptionType detected for option: {option.Id} ({option.Type.ToString()})", LogLevel.Error);
                            break;
                    }
                }
            }
        }
    }
}
