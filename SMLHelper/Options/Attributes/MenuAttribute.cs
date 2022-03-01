namespace SMLHelper.V2.Options.Attributes
{
    using Json;
    using System;
    using QModManager.Utility;

    /// <summary>
    /// Attribute used to signify a <see cref="ModOptions"/> menu should be automatically generated from a
    /// <see cref="ConfigFile"/>, as well as specifying options for handling the <see cref="ConfigFile"/>
    /// and <see cref="ModOptions"/> menu.
    /// </summary>
    /// <example>
    /// <code>
    /// using SMLHelper.V2.Interfaces;
    /// using SMLHelper.V2.Json;
    /// using SMLHelper.V2.Options;
    /// using QModManager.Utility;
    /// using UnityEngine;
    /// 
    /// [Menu("SMLHelper Example Mod")]
    /// public class Config : ConfigFile
    /// {
    ///     [Choice("My index-based choice", "Choice 1", "Choice 2", "Choice 3", Tooltip = "A simple tooltip")]
    ///     [OnChange(nameof(MyGenericValueChangedEvent))]
    ///     public int ChoiceIndex;
    ///
    ///     [Choice("My enum-based choice"), OnChange(nameof(MyGenericValueChangedEvent))]
    ///     public CustomChoice ChoiceEnum;
    /// 
    ///     [Keybind("My keybind"), OnChange(nameof(MyGenericValueChangedEvent))]
    ///     public KeyCode KeybindKey;
    /// 
    ///     [Slider("My slider", 0, 50, DefaultValue = 25, Format = "{0:F2}"), OnChange(nameof(MyGenericValueChangedEvent))]
    ///     public float SliderValue;
    /// 
    ///     [Toggle("My checkbox"), OnChange(nameof(MyCheckboxToggleEvent)), OnChange(nameof(MyGenericValueChangedEvent))]
    ///     public bool ToggleValue;
    /// 
    ///     [Button("My button")]
    ///     public void MyButtonClickEvent(ButtonClickedEventArgs e)
    ///     {
    ///         Logger.Log(Logger.Level.Info, "Button was clicked!");
    ///         Logger.Log(Logger.Level.Info, $"{e.Id}");
    ///     }
    /// 
    ///     public void MyCheckboxToggleEvent(ToggleChangedEventArgs e)
    ///     {
    ///         Logger.Log(Logger.Level.Info, "Checkbox value was changed!");
    ///         Logger.Log(Logger.Level.Info, $"{e.Value}");
    ///     }
    /// 
    ///     private void MyGenericValueChangedEvent(IModOptionEventArgs e)
    ///     {
    ///         Logger.Log(Logger.Level.Info, "Generic value changed!");
    ///         Logger.Log(Logger.Level.Info, $"{e.Id}: {e.GetType()}");
    /// 
    ///         switch (e)
    ///         {
    ///             case KeybindChangedEventArgs keybindChangedEventArgs:
    ///                 Logger.Log(Logger.Level.Info, keybindChangedEventArgs.KeyName);
    ///                 break;
    ///             case ChoiceChangedEventArgs choiceChangedEventArgs:
    ///                 Logger.Log(Logger.Level.Info, choiceChangedEventArgs.Value);
    ///                 break;
    ///             case SliderChangedEventArgs sliderChangedEventArgs:
    ///                 Logger.Log(Logger.Level.Info, sliderChangedEventArgs.Value.ToString());
    ///                 break;
    ///             case ToggleChangedEventArgs toggleChangedEventArgs:
    ///                 Logger.Log(Logger.Level.Info, toggleChangedEventArgs.Value.ToString());
    ///                 break;
    ///         }
    ///      }
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="ChoiceAttribute"/>
    /// <seealso cref="OnChangeAttribute"/>
    /// <seealso cref="KeybindAttribute"/>
    /// <seealso cref="SliderAttribute"/>
    /// <seealso cref="ToggleAttribute"/>
    /// <seealso cref="ButtonAttribute"/>
    /// <seealso cref="ModOptions"/>
    /// <seealso cref="ConfigFile"/>
    /// <seealso cref="Logger"/>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class MenuAttribute : Attribute
    {
        /// <summary>
        /// Specifies after which events the config file should be saved to disk automatically.
        /// </summary>
        /// <remarks>
        /// This enumeration has a <see cref="FlagsAttribute"/> that allows a bitwise combination of its member values.
        /// </remarks>
        [Flags]
        public enum SaveEvents : byte
        {
            /// <summary>
            /// Never automatically save.
            /// </summary>
            None = 0,
            /// <summary>
            /// Save whenever any value is changed.
            /// </summary>
            ChangeValue = 1,
            /// <summary>
            /// Save when the player saves the game.
            /// </summary>
            SaveGame = 2,
            /// <summary>
            /// Save when the player quits the game.
            /// </summary>
            QuitGame = 4
        }

        /// <summary>
        /// Specifies after which events the config file should be loaded from disk automatically.
        /// </summary>
        /// <remarks>
        /// This enumeration has a <see cref="FlagsAttribute"/> that allows a bitwise combination of its member values.
        /// </remarks>
        [Flags]
        public enum LoadEvents : byte
        {
            /// <summary>
            /// Never automatically load.
            /// </summary>
            None = 0,
            /// <summary>
            /// Load when the menu is registered to SMLHelper via <see cref="Handlers.OptionsPanelHandler.RegisterModOptions{T}"/>.
            /// </summary>
            /// <remarks>
            /// In normal usage, this option is equivalent to loading when the game is launched.
            /// </remarks>
            MenuRegistered = 1,
            /// <summary>
            /// Load when the menu is opened by the player.
            /// </summary>
            /// <remarks>
            /// Useful for allowing players to edit their config files manually without restarting the game for their changes to take effect.
            /// </remarks>
            MenuOpened = 2
        }

        /// <summary>
        /// Specifies which members of the <see cref="ConfigFile"/> will be parsed and added to the menu.
        /// </summary>
        public enum Members
        {
            /// <summary>
            /// Only <see langword="public"/> members decorated with a <see cref="ModOptionAttribute"/> derivative such as 
            /// <see cref="SliderAttribute"/>, <see cref="ChoiceAttribute"/> etc. will be processed.
            /// </summary>
            Explicit,
            /// <summary>
            /// All <see langword="public"/> members will be processed, and where they are not decorated, a default attribute
            /// will be generated for them.
            /// </summary>
            Implicit
        }

        /// <summary>
        /// The display name for the generated options menu.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The events after which the config file will be saved to disk automatically.
        /// Defaults to <see cref="SaveEvents.ChangeValue"/>.
        /// </summary>
        /// <seealso cref="SaveEvents"/>
        /// <seealso cref="LoadOn"/>
        public SaveEvents SaveOn { get; set; } = SaveEvents.ChangeValue;

        /// <summary>
        /// The events after which the config file will be loaded from disk automatically.
        /// Defaults to <see cref="LoadEvents.MenuRegistered"/>.
        /// </summary>
        /// <seealso cref="LoadEvents"/>
        /// <seealso cref="SaveOn"/>
        public LoadEvents LoadOn { get; set; } = LoadEvents.MenuRegistered;

        /// <summary>
        /// How members of the <see cref="ConfigFile"/> will be processed.
        /// Defaults to <see cref="Members.Explicit"/>, so that only <see langword="public"/> decorated members will be processed.
        /// </summary>
        /// <seealso cref="Members"/>
        public Members MemberProcessing { get; set; } = Members.Explicit;

        /// <summary>
        /// Signifies a <see cref="ModOptions"/> menu should be automatically generated from a <see cref="ConfigFile"/>.
        /// </summary>
        /// <param name="name">The display name for the generated options menu.</param>
        public MenuAttribute(string name)
        {
            Name = name;
        }

        internal MenuAttribute() { }
    }
}
