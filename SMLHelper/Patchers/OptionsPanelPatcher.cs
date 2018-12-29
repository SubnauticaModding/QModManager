namespace SMLHelper.V2.Patchers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Harmony;
    using Options;
    using UnityEngine;
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

            bool found = false;

            var tabs = optionsPanel.GetInstanceField("tabs") as List<object>;
            foreach (object tab in tabs)
            {
                Type type = tab.GetType();
                FieldInfo field = type.GetField("tab", BindingFlags.NonPublic | BindingFlags.Instance);
                GameObject tabObject = field.GetValue(tab) as GameObject;
                string name = tabObject.GetComponentInChildren<TranslationLiveUpdate>().translationKey;
                if (name == "Mods")
                {
                    found = true;
                    break;
                }
            }

            int modsTab = optionsPanel.AddTab("Mods");

            if (found == false && modOptions.Count <= 0) optionsPanel.AddHeading(modsTab, "No options here...");

            foreach (ModOptions modOption in modOptions)
            {
                optionsPanel.AddHeading(modsTab, modOption.Name);

                foreach (ModOption option in modOption.Options)
                {
                    if (option.Type == ModOptionType.Slider)
                    {
                        var slider = (ModSliderOption)option;

                        optionsPanel.AddSliderOption(modsTab, slider.Label, slider.Value, slider.MinValue, slider.MaxValue, slider.Value,
                            callback: new UnityEngine.Events.UnityAction<float>((float sliderVal) =>
                                modOption.OnSliderChange(slider.Id, sliderVal)));
                    }
                    else if (option.Type == ModOptionType.Toggle)
                    {
                        var toggle = (ModToggleOption)option;

                        optionsPanel.AddToggleOption(modsTab, toggle.Label, toggle.Value,
                            callback: new UnityEngine.Events.UnityAction<bool>((bool toggleVal) =>
                                modOption.OnToggleChange(toggle.Id, toggleVal)));
                    }
                    else if (option.Type == ModOptionType.Choice)
                    {
                        var choice = (ModChoiceOption)option;

                        optionsPanel.AddChoiceOption(modsTab, choice.Label, choice.Options, choice.Index,
                            callback: new UnityEngine.Events.UnityAction<int>((int index) =>
                                modOption.OnChoiceChange(choice.Id, index)));
                    }
                    else
                    {
                        V2.Logger.Log($"Invalid ModOptionType detected for option: {option.Id}");
                    }
                }
            }
        }
    }
}
