namespace SMLHelper.V2.Options
{
    using System;

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

    public abstract partial class ModOptions
    {
        /// <summary>
        /// The event that is called whenever a dropdown is changed. Subscribe to this in the constructor.
        /// </summary>
        protected event EventHandler<DropdownChangedEventArgs> DropdownChanged;

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
