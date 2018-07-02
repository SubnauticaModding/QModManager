namespace SMLHelper.V2.Options
{
    using System.Collections.Generic;

    public delegate void OnSliderChange(float sliderValue);
    public delegate void OnToggleChange(bool toggle);
    public delegate void OnChoiceChange(int choiceIndex);

    public enum ModOptionType
    {
        Slider,
        Toggle,
        Choice
    }

    public class ModOptionsData
    {
        public List<ModOption> Options { get; } = new List<ModOption>();

        public void AddSliderOption(string label, float minValue, float maxValue, float value, OnSliderChange sliderChange)
        {
            Options.Add(new ModSliderOption(label, minValue, maxValue, value, sliderChange));
        }

        public void AddToggleOption(string label, bool value, OnToggleChange onToggleChange)
        {
            Options.Add(new ModToggleOption(label, value, onToggleChange));
        }

        public void AddChoiceOption(string label, string[] options, int index, OnChoiceChange onChoiceChange)
        {
            Options.Add(new ModChoiceOption(label, options, index, onChoiceChange));
        }
    }

    public abstract class ModOptions
    {
        public string Name { get; }

        public ModOptions(string name)
        {
            Name = name;
        }

        public abstract ModOptionsData BuildModOptions();
    }

    public abstract class ModOption
    {
        public string Label { get; }
        public ModOptionType Type { get; }

        public ModOption(ModOptionType type, string label)
        {
            Type = type;
            Label = label;
        }
    }

    public class ModSliderOption : ModOption
    {
        public float MinValue { get; }
        public float MaxValue { get; }
        public float Value { get; }
        public OnSliderChange OnSliderChange { get; }

        public ModSliderOption(string label, float minValue, float maxValue, float value, OnSliderChange onSliderChange) : base(ModOptionType.Slider, label)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Value = value;
            OnSliderChange = onSliderChange;
        }
    }

    public class ModToggleOption : ModOption
    {
        public bool Value { get; }
        public OnToggleChange OnToggleChange { get; }

        public ModToggleOption(string label, bool value, OnToggleChange onToggleChange) : base(ModOptionType.Toggle, label)
        {
            Value = value;
            OnToggleChange = onToggleChange;
        }
    }

    public class ModChoiceOption : ModOption
    {
        public string[] Options { get; }
        public int Index { get; }
        public OnChoiceChange OnChoiceChange { get; }

        public ModChoiceOption(string label, string[] options, int index, OnChoiceChange onChoiceChange) : base(ModOptionType.Choice, label)
        {
            Options = options;
            Index = index;
            OnChoiceChange = onChoiceChange;
        }
    }
}
