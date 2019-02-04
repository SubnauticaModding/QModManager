namespace SMLHelper.V2.Options
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

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
    }

    /// <summary>
    /// Abstract class that provides the framework for your mod's in-game configuration options.
    /// </summary>
    public abstract partial class ModOptions : IComparable
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
        /// <seealso cref="AddSliderOption"/> | <seealso cref="AddToggleOption"/> | <seealso cref="AddChoiceOption(string, string, string[], int)"/> | <seealso cref="AddKeybindOption(string, string, GameInput.Device, KeyCode)"/>.</para>
        /// <para>Make sure you have subscribed to the events in the constructor to handle what happens when the value is changed:
        /// <seealso cref="SliderChanged"/> | <seealso cref="ToggleChanged"/> | <seealso cref="ChoiceChanged"/> | <seealso cref="KeybindChanged"/>.</para>
        /// </summary>
        public abstract void BuildModOptions();

        /// <summary>
        /// Compares one <see cref="ModOptions"/> object to another, based on their <see cref="Name"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            return Name.CompareTo(obj);
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
}
