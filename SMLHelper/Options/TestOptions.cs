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

        public override void BuildModOptions()
        {
            AddSliderOption("SliderId", "Slider", 0f, 100f, Slider);
            AddToggleOption("ToggleId", "Toggle", Toggle);
            AddChoiceOption("ChoiceId", "Choice", new string[]
            {
                "Test1",
                "Test2",
                "Test3"
            }, index);
        }

        public override void OnToggleChange(string id, bool value)
        {
            ErrorMessage.AddDebug("Toggle Changed Id: " + id + " Value: " + value);

            if (id == "ToggleId")
            {
                Toggle = value;
            }
        }

        public override void OnSliderChange(string id, float value)
        {
            ErrorMessage.AddDebug("Slider Changed Id: " + id + " Value: " + value);

            if(id == "SliderId")
            {
                Slider = value;
            }
        }

        public override void OnChoiceChange(string id, int indexValue)
        {
            ErrorMessage.AddDebug("Choice Changed Id: " + id + " Index: " + indexValue);

            if(id == "ChoiceId")
            {
                index = indexValue;
            }
        }
    }
}
