using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using SMLHelper.V2.Utility;
using UnityEngine;

namespace SMLHelper.Examples
{
    public static class ExampleMod
    {
        public static void Patch()
        {
            Config.Load();
            OptionsPanelHandler.RegisterModOptions(new Options());
        }
    }

    public static class Config
    {
        public static int ChoiceIndex;
        public static int DropdownIndex;
        public static KeyCode KeybindKey;
        public static float SliderValue;
        public static bool ToggleValue;

        public static void Load()
        {
            ChoiceIndex = PlayerPrefs.GetInt("SMLHelperExampleModChoice", 0);
            DropdownIndex = PlayerPrefs.GetInt("SMLHelperExampleModDropdown", 0);
            KeybindKey = KeyCodeUtils.StringToKeyCode(PlayerPrefs.GetString("SMLHelperExampleModKeybind", "X"));
            SliderValue = PlayerPrefs.GetFloat("SMLHelperExampleModSlider", 50f);
            ToggleValue = PlayerPrefs.GetInt("SMLHelperExampleModToggle", 1) == 1 ? true : false;
        }
    }

    public class Options : ModOptions
    {
        public Options() : base("SMLHelper Example Mod")
        {
            ChoiceChanged += Options_ChoiceChanged;
            DropdownChanged += Options_DropdownChanged;
            KeybindChanged += Options_KeybindChanged;
            SliderChanged += Options_SliderChanged;
            ToggleChanged += Options_ToggleChanged;
        }

        public void Options_ChoiceChanged(object sender, ChoiceChangedEventArgs e)
        {
            if (e.Id != "exampleChoice") return;
            Config.ChoiceIndex = e.Index;
            PlayerPrefs.SetInt("SMLHelperExampleModChoice", e.Index);
        }
        public void Options_DropdownChanged(object sender, DropdownChangedEventArgs e)
        {
            if (e.Id != "exampleDropdown") return;
            Config.DropdownIndex = e.Index;
            PlayerPrefs.SetInt("SMLHelperExampleModDropdown", e.Index);
        }
        public void Options_KeybindChanged(object sender, KeybindChangedEventArgs e)
        {
            if (e.Id != "exampleKeybind") return;
            Config.KeybindKey = e.Key;
            PlayerPrefs.SetString("SMLHelperExampleModKeybind", KeyCodeUtils.KeyCodeToString(e.Key));
        }
        public void Options_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            if (e.Id != "exampleSlider") return;
            Config.SliderValue = e.Value;
            PlayerPrefs.SetFloat("SMLHelperExampleModSlider", e.Value);
        }
        public void Options_ToggleChanged(object sender, ToggleChangedEventArgs e)
        {
            if (e.Id != "exampleToggle") return;
            Config.ToggleValue = e.Value;
            PlayerPrefs.SetInt("SMLHelperExampleModToggle", e.Value == true ? 1 : 0);
        }

        public override void BuildModOptions()
        {
            AddChoiceOption("exampleChoice", "Choice", new string[] { "Choice 1", "Choice 2", "Choice 3" }, Config.ChoiceIndex);
            AddDropdownOption("exampleDropdown", "Dropdown", new string[] { "Dropdown 1", "Dropdown 2", "Dropdown 3" }, Config.DropdownIndex);
            AddKeybindOption("exampleKeybind", "Keybind", GameInput.Device.Keyboard, Config.KeybindKey);
            AddSliderOption("exampleSlider", "Slider", 0, 100, Config.SliderValue);
            AddToggleOption("exampleToggle", "Toggle", Config.ToggleValue);
        }
    }
}
