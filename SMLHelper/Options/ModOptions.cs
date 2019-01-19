namespace SMLHelper.V2.Options
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Contains all the information about a slider changed event.
    /// </summary>
    public class SliderChangedEventArgs : EventArgs
    {        
        /// <summary>
        /// The ID of the <see cref="ModSliderOption"/> that was changed.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The new value for the <see cref="ModSliderOption"/>.
        /// </summary>
        public float Value { get; }

        public SliderChangedEventArgs(string id, float value)
        {
            Id = id;
            Value = value;
        }
    }

    /// <summary>
    /// Contains all the information about a toggle changed event.
    /// </summary>
    public class ToggleChangedEventArgs : EventArgs
    {        
        /// <summary>
        /// The ID of the <see cref="ModToggleOption"/> that was changed.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The new value for the <see cref="ModToggleOption"/>.
        /// </summary>
        public bool Value { get; }

        public ToggleChangedEventArgs(string id, bool value)
        {
            Id = id;
            Value = value;
        }
    }

    /// <summary>
    /// Contains all the information about a choice changed event.
    /// </summary>
    public class ChoiceChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The ID of the <see cref="ModChoiceOption"/> that was changed.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The new index for the <see cref="ModChoiceOption"/>.
        /// </summary>
        public int Index { get; }

        public ChoiceChangedEventArgs(string id, int index)
        {
            Id = id;
            Index = index;
        }
    }

    /// <summary>
    /// Contains all the information about a keybind changed event.
    /// </summary>
    public class KeybindChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The ID of the <see cref="ModKeybindOption"/> that was changed.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The new value for the <see cref="ModKeybindOption"/>.
        /// </summary>
        public KeyCode Key { get; }

        public KeybindChangedEventArgs(string id, KeyCode key)
        {
            Id = id;
            Key = key;
        }
    }

    /// <summary>
    /// Contains all the information about a choice changed event.
    /// </summary>
    public class DropdownChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The ID of the <see cref="ModDropdownOption"/> that was changed.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The new index for the <see cref="ModDropdownOption"/>.
        /// </summary>
        public int Index { get; }

        public DropdownChangedEventArgs(string id, int index)
        {
            Id = id;
            Index = index;
        }
    }

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
        Choice,

        /// <summary>
        /// The option uses a keybind field that can be changed to a certain keyt
        /// </summary>
        Keybind,

        /// <summary>
        /// The option uses a selection of one of a discrete number of possible values.
        /// </summary>
        Dropdown,
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
        /// The event that is called whenever a slider is changed. Subscribe to this in the constructor.
        /// </summary>
        protected event EventHandler<SliderChangedEventArgs> SliderChanged;

        /// <summary>
        /// The event that is called whenever a toggle is changed. Subscribe to this in the constructor.
        /// </summary>
        protected event EventHandler<ToggleChangedEventArgs> ToggleChanged;

        /// <summary>
        /// The event that is called whenever a choice is changed. Subscribe to this in the constructor.
        /// </summary>
        protected event EventHandler<ChoiceChangedEventArgs> ChoiceChanged;

        /// <summary>
        /// The event that is called whenever a keybind is changed. Subscribe to this in the constructor.
        /// </summary>
        protected event EventHandler<KeybindChangedEventArgs> KeybindChanged;

        /// <summary>
        /// The event that is called whenever a dropdown is changed. Subscribe to this in the constructor.
        /// </summary>
        protected event EventHandler<DropdownChangedEventArgs> DropdownChanged;

        /// <summary>
        /// Builds and obtains the <see cref="ModOption"/>s that belong to this instance.
        /// </summary>
        public List<ModOption> Options
        {
            get
            {
                _options = new Dictionary<string, ModOption>();
                BuildModOptions();

                return _options.Values.ToList();
            }
        }

        // This is a dictionary now in case we want to get the ModOption quickly
        // based on the provided ID.
        private Dictionary<string, ModOption> _options;

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
        /// <seealso cref="AddSliderOption"/> | <seealso cref="AddToggleOption"/> | <seealso cref="AddChoiceOption"/> | <seealso cref="AddKeybindOption"/> | <seealso cref="AddDropdownOption"/>.</para>
        /// <para>Make sure you have subscribed to the events in the constructor to handle what happens when the value is changed:
        /// <seealso cref="SliderChanged"/> | <seealso cref="ToggleChanged"/> | <seealso cref="ChoiceChanged"/> | <seealso cref="KeybindChanged"/> | <seealso cref="DropdownChanged"/>.</para>
        /// </summary>
        public abstract void BuildModOptions();

        /// <summary>
        /// Notifies a slider change to all subsribed event handlers.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        internal void OnSliderChange(string id, float value)
        {
            SliderChanged(this, new SliderChangedEventArgs(id, value));
        }

        /// <summary>
        /// Notifies a toggle change to all subscribed event handlers.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        internal void OnToggleChange(string id, bool value)
        {
            ToggleChanged(this, new ToggleChangedEventArgs(id, value));
        }

        /// <summary>
        /// Notifies a choice change to all subscribed event handlers.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="indexValue"></param>
        internal void OnChoiceChange(string id, int indexValue)
        {
            ChoiceChanged(this, new ChoiceChangedEventArgs(id, indexValue));
        }

        /// <summary>
        /// Notifies a keybind change to all subscribed event handlers.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="key"></param>
        internal void OnKeybindChange(string id, KeyCode key)
        {
            KeybindChanged(this, new KeybindChangedEventArgs(id, key));
        }

        /// <summary>
        /// Notifies a choice change to all subscribed event handlers.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="indexValue"></param>
        internal void OnDropdownChange(string id, int indexValue)
        {
            DropdownChanged(this, new DropdownChangedEventArgs(id, indexValue));
        }

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
            _options.Add(id, new ModSliderOption(id, label, minValue, maxValue, value));
        }

        /// <summary>
        /// Adds a new <see cref="ModToggleOption"/> to this instance.
        /// </summary>
        /// <param name="id">The internal ID for the toggle option.</param>
        /// <param name="label">The display text to use in the in-game menu.</param>
        /// <param name="value">The starting value.</param>
        protected void AddToggleOption(string id, string label, bool value)
        {
            _options.Add(id, new ModToggleOption(id, label, value));
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
            _options.Add(id, new ModChoiceOption(id, label, options, index));
        }

        /// <summary>
        /// Adds a new <see cref="ModKeybindOption"/> to this instance.
        /// </summary>
        /// <param name="id">The internal ID for the toggle option.</param>
        /// <param name="label">The display text to use in the in-game menu.</param>
        /// <param name="device">The device name.</param>
        /// <param name="key">The starting keybind value.</param>
        protected void AddKeybindOption(string id, string label, GameInput.Device device, KeyCode key)
        {
            _options.Add(id, new ModKeybindOption(id, label, device, key));
        }

        /// <summary>
        /// Adds a new <see cref="ModDropdownOption"/> to this instance.
        /// </summary>
        /// <param name="id">The internal ID for the dropdown option.</param>
        /// <param name="label">The display text to use in the in-game menu.</param>
        /// <param name="options">The collection of available values.</param>
        /// <param name="index">The starting value.</param>
        protected void AddDropdownOption(string id, string label, string[] options, int index)
        {
            _options.Add(id, new ModDropdownOption(id, label, options, index));
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
        /// <param name="id">The internal ID of this option.</param>
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
        /// <param name="id">The internal ID of this option.</param>
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
        /// <param name="id">The internal ID of this option.</param>
        /// <param name="label">The display text to show on the in-game menus.</param>
        /// <param name="options">The collection of available values.</param>
        /// <param name="index">The starting value.</param>
        internal ModChoiceOption(string id, string label, string[] options, int index) : base(ModOptionType.Choice, label, id)
        {
            Options = options;
            Index = index;
        }
    }

    /// <summary>
    /// A mod option class for handling an option that is a keybind.
    /// </summary>
    public class ModKeybindOption : ModOption
    {
        public KeyCode Key { get; }
        public GameInput.Device Device { get; }

        /// <summary>
        /// Instantiates a new <see cref="ModKeybindOption"/> for handling an option that is a keybind.
        /// </summary>
        /// <param name="id">The internal ID of this option.</param>
        /// <param name="label">The display text to show on the in-game menus.</param>
        /// <param name="device">The device name.</param>
        /// <param name="key">The starting keybind value.</param>
        internal ModKeybindOption(string id, string label, GameInput.Device device, KeyCode key) : base(ModOptionType.Keybind, label, id)
        {
            Device = device;
            Key = key;
        }
    }

    /// <summary>
    /// A mod option class for handling an option that can select one item from a list of values.
    /// </summary>
    public class ModDropdownOption : ModOption
    {
        public string[] Options { get; }
        public int Index { get; }

        /// <summary>
        /// Instantiates a new <see cref="ModDropdownOption"/> for handling an option that can select one item from a list of values.
        /// </summary>
        /// <param name="id">The internal ID of this option.</param>
        /// <param name="label">The display text to show on the in-game menus.</param>
        /// <param name="options">The collection of available values.</param>
        /// <param name="index">The starting value.</param>
        internal ModDropdownOption(string id, string label, string[] options, int index) : base(ModOptionType.Dropdown, label, id)
        {
            Options = options;
            Index = index;
        }
    }
}
