namespace SMLHelper.V2.Options
{
    using SMLHelper.V2.Options.Utility;
    using System;
    using System.Linq;

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

    public abstract partial class ModOptions
    {
        /// <summary>
        /// The event that is called whenever a choice is changed. Subscribe to this in the constructor.
        /// </summary>
        protected event EventHandler<ChoiceChangedEventArgs> ChoiceChanged;

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
        /// Adds a new <see cref="ModChoiceOption"/> to this instance.
        /// </summary>
        /// <param name="id">The internal ID for the choice option.</param>
        /// <param name="label">The display text to use in the in-game menu.</param>
        /// <param name="options">The collection of available values.</param>
        /// <param name="index">The starting value.</param>
        protected void AddChoiceOption(string id, string label, string[] options, int index)
        {
            if (!Validator.ValidateChoiceOption(id, label, options, index)) return;
            _options.Add(id, new ModChoiceOption(id, label, options, index));
        }

        /// <summary>
        /// Adds a new <see cref="ModChoiceOption"/> to this instance.
        /// </summary>
        /// <param name="id">The internal ID for the choice option.</param>
        /// <param name="label">The display text to use in the in-game menu.</param>
        /// <param name="options">The collection of available values.</param>
        /// <param name="value">The starting value.</param>
        protected void AddChoiceOption(string id, string label, string[] options, string value)
        {
            int index = Array.IndexOf(options, value);
            AddChoiceOption(id, label, options, index);
        }

        /// <summary>
        /// Adds a new <see cref="ModChoiceOption"/> to this instance.
        /// </summary>
        /// <param name="id">The internal ID for the choice option.</param>
        /// <param name="label">The display text to use in the in-game menu.</param>
        /// <param name="options">The collection of available values.</param>
        /// <param name="index">The starting value.</param>
        protected void AddChoiceOption(string id, string label, object[] options, int index)
        {
            AddChoiceOption(id, label, options.Select(obj => obj.ToString()).ToArray(), index);
        }

        /// <summary>
        /// Adds a new <see cref="ModChoiceOption"/> to this instance.
        /// </summary>
        /// <param name="id">The internal ID for the choice option.</param>
        /// <param name="label">The display text to use in the in-game menu.</param>
        /// <param name="options">The collection of available values.</param>
        /// <param name="value">The starting value.</param>
        protected void AddChoiceOption(string id, string label, object[] options, object value)
        {
            int index = Array.IndexOf(options, value);
            AddChoiceOption(id, label, options, index);
        }

        /// <summary>
        /// Adds a new <see cref="ModChoiceOption"/> to this instance, automatically using the values of an enum
        /// </summary>
        /// <typeparam name="T">The enum which will be used to populate the options</typeparam>
        /// <param name="id">The internal ID for the choice option.</param>
        /// <param name="label">The display text to use in the in-game menu.</param>
        /// <param name="value">The starting value</param>
        protected void AddChoiceOption<T>(string id, string label, T value) where T : Enum
        {
            string[] options = Enum.GetNames(typeof(T));
            string valueString = value.ToString();

            AddChoiceOption(id, label, options, valueString);
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
}
