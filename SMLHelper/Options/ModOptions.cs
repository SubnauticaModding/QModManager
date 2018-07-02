namespace SMLHelper.V2.Options
{
    using System.Collections.Generic;

    public enum ModOptionType
    {
        Slider,
        Toggle,
        Choice
    }

    public abstract class ModOptions
    {
        public string Name { get; }
        public List<ModOption> Options
        {
            get
            {
                _options = new List<ModOption>();
                BuildModOptions();

                return _options;
            }
        }

        private List<ModOption> _options;

        public ModOptions(string name)
        {
            Name = name;
        }

        public abstract void BuildModOptions();
        public abstract void OnSliderChange(string id, float value);
        public abstract void OnToggleChange(string id, bool value);
        public abstract void OnChoiceChange(string id, int indexValue);

        protected void AddSliderOption(string id, string label, float minValue, float maxValue, float value)
        {
            _options.Add(new ModSliderOption(id, label, minValue, maxValue, value));
        }

        protected void AddToggleOption(string id, string label, bool value)
        {
            _options.Add(new ModToggleOption(id, label, value));
        }

        protected void AddChoiceOption(string id, string label, string[] options, int index)
        {
            _options.Add(new ModChoiceOption(id, label, options, index));
        }
    }

    public abstract class ModOption
    {
        public string Id { get; }
        public string Label { get; }
        public ModOptionType Type { get; }

        public ModOption(ModOptionType type, string label, string id)
        {
            Type = type;
            Label = label;
            Id = id;
        }
    }

    public class ModSliderOption : ModOption
    {
        public float MinValue { get; }
        public float MaxValue { get; }
        public float Value { get; }

        public ModSliderOption(string id, string label, float minValue, float maxValue, float value) : base(ModOptionType.Slider, label, id)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Value = value;
        }
    }

    public class ModToggleOption : ModOption
    {
        public bool Value { get; }

        public ModToggleOption(string id, string label, bool value) : base(ModOptionType.Toggle, label, id)
        {
            Value = value;
        }
    }

    public class ModChoiceOption : ModOption
    {
        public string[] Options { get; }
        public int Index { get; }

        public ModChoiceOption(string id, string label, string[] options, int index) : base(ModOptionType.Choice, label, id)
        {
            Options = options;
            Index = index;
        }
    }
}
