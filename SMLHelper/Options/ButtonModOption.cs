namespace SMLHelper.V2.Options
{
    using Interfaces;
    using System;
    using UnityEngine.Events;
    using UnityEngine.UI;

    /// <summary>
    /// Contains all the information about a button click event.
    /// </summary>
    public class ButtonClickedEventArgs : EventArgs, IModOptionEventArgs
    {
        /// <summary>
        /// The ID of the <see cref="ModButtonOption"/> that was clicked.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Constructs a new <see cref="ButtonClickedEventArgs"/>.
        /// </summary>
        /// <param name="id">The ID of the <see cref="ModButtonOption"/> that was clicked.</param>
        public ButtonClickedEventArgs(string id) { this.Id = id; }
    }

    public abstract partial class ModOptions
    {
        /// <summary>
        /// The event that is called whenever a button is clicked. Subscribe to this in the constructor.
        /// </summary>
        protected event EventHandler<ButtonClickedEventArgs> ButtonClicked;

        /// <summary>
        /// Notifies button click to all subscribed event handlers.
        /// </summary>
        /// <param name="id">The internal ID for the button option.</param>
        internal void OnButtonClicked(string id) => ButtonClicked(this, new ButtonClickedEventArgs(id));

        /// <summary>
        /// Adds a new <see cref="ModButtonOption"/> to this instance.
        /// </summary>
        /// <param name="id">The internal ID for the button option.</param>
        /// <param name="label">The display text to use in the in-game menu.</param>
        protected void AddButtonOption(string id, string label) => AddOption(new ModButtonOption(id, label));
    }

    /// <summary>
    /// A mod option class for handling a button that can be clicked.
    /// </summary>
    public class ModButtonOption : ModOption
    {
        internal override void AddToPanel(uGUI_TabbedControlsPanel panel, int tabIndex)
        {
            // Add button to GUI
            Button componentInChildren = panel.AddItem(tabIndex, panel.buttonPrefab, Label).GetComponentInChildren<Button>();

            // Store a reference to parent object to simplify further modifications
            OptionGameObject = componentInChildren.transform.parent.gameObject;

            // Setup "on click" event
            componentInChildren.onClick.AddListener(new UnityAction(() =>
            {
                // Apply "deselected" style to button right after it is clicked
                componentInChildren.OnDeselect(null);
                // Propagate button click event to parent
                parentOptions.OnButtonClicked(Id);
            }));

            // Add button to panel
            base.AddToPanel(panel, tabIndex);
        }

        /// <summary>
        /// Instantiates a new <see cref="ModButtonOption"/> for handling a button that can be clicked.
        /// </summary>
        /// <param name="id">The internal ID of this option.</param>
        /// <param name="label">The display text to show on the in-game menus.</param>
        internal ModButtonOption(string id, string label) : base(label, id) { }

        internal override Type AdjusterComponent => null;
    }
}
