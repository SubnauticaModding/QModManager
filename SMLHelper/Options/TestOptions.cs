using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLHelper.V2.Options
{
    public class TestOptions : ModOptions
    {
        public static bool Toggle;
        public static float Slider;
        public static int index;

        public TestOptions() : base("Test")
        {
        }

        public override ModOptionsData BuildModOptions()
        {
            var modOptionsData = new ModOptionsData();
            modOptionsData.AddSliderOption("Slider", 0f, 100f, Slider, OnSliderChange);
            modOptionsData.AddToggleOption("Toggle", Toggle, OnToggleChange);
            modOptionsData.AddChoiceOption("Choice", new string[]
            {
                "Test1",
                "Test2",
                "Test3"
            }, index, OnChoiceChange);

            return modOptionsData;
        }

        public static void OnSliderChange(float thing)
        {
            ErrorMessage.AddDebug("Slider: " + thing);
            Slider = thing;
        }

        public static void OnToggleChange(bool toggle)
        {
            ErrorMessage.AddDebug("Toggle: " + toggle);
            Toggle = toggle;
        }

        public static void OnChoiceChange(int choice)
        {
            ErrorMessage.AddDebug("Choice: " + choice);
            index = choice;
        }
    }
}
