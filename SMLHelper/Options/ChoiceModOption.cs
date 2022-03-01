namespace SMLHelper.V2.Options
{
    using Interfaces;
    using System;
    using System.Collections;
    using SMLHelper.V2.Options.Utility;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// Contains all the information about a choice changed event.
    /// </summary>
    public class ChoiceChangedEventArgs : EventArgs, IModOptionEventArgs
    {
        /// <summary>
        /// The ID of the <see cref="ModChoiceOption"/> that was changed.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The new index for the <see cref="ModChoiceOption"/>.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// The value of the <see cref="ModChoiceOption"/> as a string.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Constructs a new <see cref="ChoiceChangedEventArgs"/>.
        /// </summary>
        /// <param name="id">The ID of the <see cref="ModChoiceOption"/> that was changed.</param>
        /// <param name="index">The new index for the <see cref="ModChoiceOption"/>.</param>
        public ChoiceChangedEventArgs(string id, int index)
        {
            this.Id = id;
            this.Index = index;
        }

        /// <summary>
        /// Constructs a new <see cref="ChoiceChangedEventArgs"/>.
        /// </summary>
        /// <param name="id">The ID of the <see cref="ModChoiceOption"/> that was changed.</param>
        /// <param name="index">The new index for the <see cref="ModChoiceOption"/>.</param>
        /// <param name="value">The value of the <see cref="ModChoiceOption"/> as a string.</param>
        public ChoiceChangedEventArgs(string id, int index, string value)
        {
            this.Id = id;
            this.Index = index;
            this.Value = value;
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
        /// Notifies a choice change to all subscribed event handlers.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="indexValue"></param>
        /// <param name="value"></param>
        internal void OnChoiceChange(string id, int indexValue, string value)
        {
            ChoiceChanged(this, new ChoiceChangedEventArgs(id, indexValue, value));
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
            if (Validator.ValidateChoiceOrDropdownOption(id, label, options, index))
                AddOption(new ModChoiceOption(id, label, options, index));
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
            if (index < 0)
                index = 0;

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
            string[] strOptions = new string[options.Length];

            for (int i = 0; i < options.Length; i++)
                strOptions[i] = options[i].ToString();

            AddChoiceOption(id, label, strOptions, index);
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
        /// <summary>
        /// The array of readable string options to choose between in the <see cref="ModChoiceOption"/>.
        /// </summary>
        public string[] Options { get; }

        /// <summary>
        /// The currently selected index among the options array.
        /// </summary>
        public int Index { get; }

        internal override void AddToPanel(uGUI_TabbedControlsPanel panel, int tabIndex)
        {
            var choice = panel.AddChoiceOption(tabIndex, Label, Options, Index,
                new UnityAction<int>((int index) => parentOptions.OnChoiceChange(Id, index, Options[index])));

            OptionGameObject = choice.transform.parent.transform.parent.gameObject; // :(

            base.AddToPanel(panel, tabIndex);
        }

        /// <summary>
        /// Instantiates a new <see cref="ModChoiceOption"/> for handling an option that can select one item from a list of values.
        /// </summary>
        /// <param name="id">The internal ID of this option.</param>
        /// <param name="label">The display text to show on the in-game menus.</param>
        /// <param name="options">The collection of available values.</param>
        /// <param name="index">The starting value.</param>
        internal ModChoiceOption(string id, string label, string[] options, int index) : base(label, id)
        {
            this.Options = options;
            this.Index = index;
        }

        private class ChoiceOptionAdjust: ModOptionAdjust
        {
            private const float spacing = 10f;

            public IEnumerator Start()
            {
                SetCaptionGameObject("Choice/Caption");
                yield return null; // skip one frame

                RectTransform rect = gameObject.transform.Find("Choice/Background") as RectTransform;

                float widthAll = gameObject.GetComponent<RectTransform>().rect.width;
                float widthChoice = rect.rect.width;
                float widthText = CaptionWidth + spacing;

                if (widthText + widthChoice > widthAll)
                    rect.sizeDelta = SetVec2x(rect.sizeDelta, widthAll - widthText - widthChoice);

                Destroy(this);
            }
        }
        internal override Type AdjusterComponent => typeof(ChoiceOptionAdjust);
    }
}
