namespace SMLHelper.V2.Patchers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using Harmony;
    using UnityEngine;
    using UnityEngine.UI;
    using Options;

    internal class OptionsPanelPatcher
    {
        internal static List<ModOptions> modOptions = new List<ModOptions>();

        internal static void Patch(HarmonyInstance harmony)
        {
            var uGUI_OptionsPanelType = typeof(uGUI_OptionsPanel);
            var thisType = typeof(OptionsPanelPatcher);
            var startMethod = uGUI_OptionsPanelType.GetMethod("AddTabs", BindingFlags.NonPublic | BindingFlags.Instance);

            harmony.Patch(startMethod, null, new HarmonyMethod(thisType.GetMethod("AddTabs_Postfix", BindingFlags.NonPublic | BindingFlags.Static)));
        }

        internal static void AddTabs_Postfix(uGUI_OptionsPanel __instance)
        {
            var optionsPanel = __instance;

            // Make the Mods tab
            var modsTab = optionsPanel.AddTab("Mods");

            foreach(var modOption in modOptions)
            {
                var data = modOption.BuildModOptions();

                optionsPanel.AddHeading(modsTab, modOption.Name);

                foreach(var option in data.Options)
                {
                    if(option.Type == ModOptionType.Slider)
                    {
                        var slider = (ModSliderOption)option;

                        optionsPanel.AddSliderOption(modsTab, slider.Label, slider.Value, slider.MinValue, slider.MaxValue, slider.Value,
                        new UnityEngine.Events.UnityAction<float>((float sliderVal) => slider.OnSliderChange.Invoke(sliderVal)));
                    }
                    else if(option.Type == ModOptionType.Toggle)
                    {
                        var toggle = (ModToggleOption)option;

                        optionsPanel.AddToggleOption(modsTab, toggle.Label, toggle.Value,
                        new UnityEngine.Events.UnityAction<bool>((bool toggleVal) => toggle.OnToggleChange.Invoke(toggleVal)));
                    }
                    else
                    {
                        var choice = (ModChoiceOption)option;

                        optionsPanel.AddChoiceOption(modsTab, choice.Label, choice.Options, choice.Index,
                        new UnityEngine.Events.UnityAction<int>((int index) => choice.OnChoiceChange.Invoke(index)));
                    }
                }
            }
        }
    }
}
