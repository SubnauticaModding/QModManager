namespace SMLHelper.V2.Patchers
{
    using System.Collections.Generic;
    using System.Reflection;
    using Harmony;
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
            
            var modsTab = -1;
            for (int i = 0; i < optionsPanel.tabsContainer.childCount; i++)
            {
                var text = optionsPanel.tabsContainer.GetChild(i).GetComponentInChildren<Text>(true);
                if (text != null && text.text == "Mods")
                {
                    modsTab = i;
                    break;
                }
            }
            if (modsTab == -1)
            {
                modsTab = optionsPanel.AddTab("Mods");
            }

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
