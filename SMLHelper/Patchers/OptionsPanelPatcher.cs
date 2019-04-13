namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using Options;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    internal class OptionsPanelPatcher
    {
        internal static SortedList<string, ModOptions> modOptions = new SortedList<string, ModOptions>();

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
            
            // Start the modsTab index at a value of -1
            var modsTab = -1;
            // Loop through all of the tabs
            for (int i = 0; i < optionsPanel.tabsContainer.childCount; i++)
            {
                // Check if they are named "Mods"
                var text = optionsPanel.tabsContainer.GetChild(i).GetComponentInChildren<Text>(true);
                if (text != null && text.text == "Mods")
                {
                    // Set the tab index to the found one and break
                    modsTab = i;
                    break;
                }
            }
            // If no tab was found, create one
            if (modsTab == -1)
            {
                modsTab = optionsPanel.AddTab("Mods");
            }

            // Maybe this could be split into its own file to handle smlhelper options, or maybe it could be removed alltogether
            optionsPanel.AddHeading(modsTab, "SMLHelper");
            optionsPanel.AddToggleOption(modsTab, "Enable debug logs", V2.Logger.EnableDebugging, V2.Logger.SetDebugging);
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
                            V2.Logger.Log($"Invalid ModOptionType detected for option: {option.Id} ({option.Type.ToString()})", LogLevel.Error);
                            break;
                    }
                }
            }
        }
    }
}
