namespace SMLHelper.V2.Options
{
    public class TestOptions : ModOptions
    {
        public static bool Toggle;
        public static float Slider;
        public static int index;

        public TestOptions() : base("Test")
        {
            ToggleChanged += OnToggleChange;
            ChoiceChanged += OnChoiceChange;
            SliderChanged += OnSliderChange;
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

        public void OnToggleChange(object sender, ToggleChangedEventArgs args)
        {
            ErrorMessage.AddDebug("Toggle Changed Id: " + args.Id + " Value: " + args.Value);

            if (args.Id == "ToggleId")
            {
                Toggle = args.Value;
            }
        }

        public void OnSliderChange(object sender, SliderChangedEventArgs args)
        {
            ErrorMessage.AddDebug("Slider Changed Id: " + args.Id + " Value: " + args.Value);

            if(args.Id == "SliderId")
            {
                Slider = args.Value;
            }
        }

        public void OnChoiceChange(object sender, ChoiceChangedEventArgs args)
        {
            ErrorMessage.AddDebug("Choice Changed Id: " + args.Id + " Index: " + args.Index);

            if(args.Id == "ChoiceId")
            {
                index = args.Index;
            }
        }
    }
}
