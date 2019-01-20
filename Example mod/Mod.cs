using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using SMLHelper.V2.Utility;
using UnityEngine;

namespace SMLHelper.V2.Examples
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
        public static KeyCode KeybindKey;
        public static float SliderValue;
        public static bool ToggleValue;

        public static void Load()
        {
            ChoiceIndex = PlayerPrefs.GetInt("SMLHelperExampleModChoice", 0);
            KeybindKey = PlayerPrefsExtra.GetKeyCode("SMLHelperExampleModKeybind", KeyCode.X);
            SliderValue = PlayerPrefs.GetFloat("SMLHelperExampleModSlider", 50f);
            ToggleValue = PlayerPrefsExtra.GetBool("SMLHelperExampleModToggle", true);
        }
    }

    public class Options : ModOptions
    {
        public Options() : base("SMLHelper Example Mod")
        {
            ChoiceChanged += Options_ChoiceChanged;
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
        public void Options_KeybindChanged(object sender, KeybindChangedEventArgs e)
        {
            if (e.Id != "exampleKeybind") return;
            Config.KeybindKey = e.Key;
            PlayerPrefsExtra.SetKeyCode("SMLHelperExampleModKeybind", e.Key);
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
            PlayerPrefsExtra.SetBool("SMLHelperExampleModToggle", e.Value);
        }

        public override void BuildModOptions()
        {
            AddChoiceOption("exampleChoice", "Choice", new string[] { "Choice 1", "Choice 2", "Choice 3" }, Config.ChoiceIndex);
            AddKeybindOption("exampleKeybind", "Keybind", GameInput.Device.Keyboard, Config.KeybindKey);
            AddSliderOption("exampleSlider", "Slider", 0, 100, Config.SliderValue);
            AddToggleOption("exampleToggle", "Toggle", Config.ToggleValue);
        }
    }
}
