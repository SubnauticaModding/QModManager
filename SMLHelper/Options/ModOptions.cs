namespace SMLHelper.V2.Options
{
    using System.Collections.Generic;

    /// <summary>
    /// Indicates how the option is interacted with by the user.
    /// </summary>
    public enum ModOptionType
    {
        /// <summary>
        /// The option uses a slider that allows for a choice within a continues range of values within a maximum and minimum.
        /// </summary>
        Slider,

        /// <summary>
        /// The option uses a checkbox that can be either enabled or disabled.
        /// </summary>
        Toggle,

        /// <summary>
        /// The option uses a selection of one of a discrete number of possible values.
        /// </summary>
        Choice
    }

    /// <summary>
    /// Abstract class that provides the framework for your mod's in-game configuration options.
    /// </summary>
    public abstract class ModOptions
    {
        /// <summary>
        /// The name of this set of configuration options.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Builds and obtains the <see cref="ModOption"/>s that belong to this instance.
        /// </summary>
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

        /// <summary>
        /// Creates a new insteance of <see cref="ModOptions"/>.
        /// </summary>
        /// <param name="name">The name that will display above this section of options in the in-game menu.</param>
        public ModOptions(string name)
        {
            Name = name;
        }

        /// <summary>
        /// <para>Builds up the configuration the options.</para>
        /// <para>This method should be composed of calls into the following methods: 
        /// <seealso cref="AddSliderOption"/> | <seealso cref="AddToggleOption"/> | <seealso cref="AddChoiceOption"/>.</para>
        /// <para>Make sure you override the corresponding method to handle what happens when the value is changed:
        /// <seealso cref="OnSliderChange"/> | <seealso cref="OnToggleChange"/> | <seealso cref="OnChoiceChange"/>.</para>
        /// </summary>
        public abstract void BuildModOptions();

        /// <summary>
        /// This method is executed whenever the slider value at the specified ID changes.
        /// </summary>
        /// <param name="id">The <see cref="ModSliderOption"/> id.</param>
        /// <param name="value">The new slider value.</param>
        /// <seealso cref="ModSliderOption"/>
        public virtual void OnSliderChange(string id, float value) { }

        /// <summary>
        /// This method is executed whenever the toggle value at the specified ID changes.
        /// </summary>
        /// <param name="id">The <see cref="ModToggleOption"/> id.</param>
        /// <param name="value">The new toggle value.</param>
        /// <seealso cref="ModToggleOption"/>
        public virtual void OnToggleChange(string id, bool value) { }

        /// <summary>
        /// This method is executed whenever the choice option at the specified ID changes.
        /// </summary>
        /// <param name="id">The <see cref="ModChoiceOption"/> id.</param>
        /// <param name="indexValue">The new index value.</param>
        /// <seealso cref="ModChoiceOption"/>
        public virtual void OnChoiceChange(string id, int indexValue) { }

        /// <summary>
        /// Adds a new <see cref="ModSliderOption"/> to this instance.
        /// </summary>
        /// <param name="id">The internal ID for the slider option.</param>
        /// <param name="label">The display text to use in the in-game menu.</param>
        /// <param name="minValue">The minimum value for the range.</param>
        /// <param name="maxValue">The maximum value for the range.</param>
        /// <param name="value">The starting value.</param>
        protected void AddSliderOption(string id, string label, float minValue, float maxValue, float value)
        {
            _options.Add(new ModSliderOption(id, label, minValue, maxValue, value));
        }

        /// <summary>
        /// Adds a new <see cref="ModToggleOption"/> to this instance.
        /// </summary>
        /// <param name="id">The internal ID for the toggle option.</param>
        /// <param name="label">The display text to use in the in-game menu.</param>
        /// <param name="value">The starting value.</param>
        protected void AddToggleOption(string id, string label, bool value)
        {
            _options.Add(new ModToggleOption(id, label, value));
        }

        /// <summary>
        /// Adds a new <see cref="ModChoiceOption"/> to this instance.
        /// </summary>
        /// <param name="id">The internal ID for the choice option.</param>
        /// <param name="label">The display text to use in the in-game menu.</param>
        /// <param name="options">The collection of available values.</param>
        /// <param name="index">The starting value.</param>
        protected void AddChoiceOption(string id, string label, string[] options, int index)
        {
            _options.Add(new ModChoiceOption(id, label, options, index));
        }
    }

    /// <summary>
    /// The common abstract class to all mod options.
    /// </summary>
    public abstract class ModOption
    {
        /// <summary>
        /// The internal ID that identifies this option.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The display text to be shown for this option in the in-game menus.
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// The type of option for this instance.
        /// </summary>
        public ModOptionType Type { get; }

        /// <summary>
        /// Base constructor for all mod options.
        /// </summary>
        /// <param name="type">The mod option type.</param>
        /// <param name="label">The display text to show on the in-game menus.</param>
        /// <param name="id">The internal ID if this option.</param>
        internal ModOption(ModOptionType type, string label, string id)
        {
            Type = type;
            Label = label;
            Id = id;
        }
    }

    /// <summary>
    /// A mod option class for handling an option that can have any floating point value between a minimum and maximum.
    /// </summary>
    public class ModSliderOption : ModOption
    {
        public float MinValue { get; }
        public float MaxValue { get; }
        public float Value { get; }

        /// <summary>
        /// Instantiates a new <see cref="ModSliderOption"/> for handling an option that can have any floating point value between a minimum and maximum.
        /// </summary>
        /// <param name="id">The internal ID if this option.</param>
        /// <param name="label">The display text to show on the in-game menus.</param>
        /// <param name="minValue">The minimum value for the range.</param>
        /// <param name="maxValue">The maximum value for the range.</param>
        /// <param name="value">The starting value.</param>
        internal ModSliderOption(string id, string label, float minValue, float maxValue, float value) : base(ModOptionType.Slider, label, id)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Value = value;
        }
    }

    /// <summary>
    /// A mod option class for handling an option that can be either ON or OFF.
    /// </summary>
    public class ModToggleOption : ModOption
    {
        public bool Value { get; }

        /// <summary>
        /// Instantiates a new <see cref="ModToggleOption"/> for handling an option that can be either ON or OFF.
        /// </summary>
        /// <param name="id">The internal ID if this option.</param>
        /// <param name="label">The display text to show on the in-game menus.</param>
        /// <param name="value">The starting value.</param>
        internal ModToggleOption(string id, string label, bool value) : base(ModOptionType.Toggle, label, id)
        {
            Value = value;
        }
    }

    /// <summary>
    /// A mod option class for handling an option that can select one item from a list of values.
    /// </summary>
    public class ModChoiceOption : ModOption
    {
        public string[] Options { get; }
        public int Index { get; }

        /// <summary>
        /// Instantiates a new <see cref="ModChoiceOption"/> for handling an option that can select one item from a list of values.
        /// </summary>
        /// <param name="id">The internal ID if this option.</param>
        /// <param name="label">The display text to show on the in-game menus.</param>
        /// <param name="options">The collection of available values.</param>
        /// <param name="index">The starting value.</param>
        internal ModChoiceOption(string id, string label, string[] options, int index) : base(ModOptionType.Choice, label, id)
        {
            Options = options;
            Index = index;
        }
    }
}
