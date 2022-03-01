namespace SMLHelper.V2.Options.Attributes
{
    using Json;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Logger = Logger;

    /// <summary>
    /// An internal derivative of <see cref="ModOptions"/> for use in auto-generating a menu based on attributes
    /// declared in a <see cref="ConfigFile"/>.
    /// </summary>
    /// <typeparam name="T">The type of the class derived from <see cref="ConfigFile"/> to use for
    /// loading to/saving from disk.</typeparam>
    internal class OptionsMenuBuilder<T> : ModOptions where T : ConfigFile, new()
    {
        public ConfigFileMetadata<T> ConfigFileMetadata { get; } = new ConfigFileMetadata<T>();

        /// <summary>
        /// Instantiates a new <see cref="OptionsMenuBuilder{T}"/>, generating <see cref="ModOption"/>s by parsing the fields,
        /// properties and methods declared in the class.
        /// </summary>
        public OptionsMenuBuilder() : base(null)
        {
            BindEvents();

            ConfigFileMetadata.ProcessMetadata();

            Name = ConfigFileMetadata.MenuAttribute.Name;

            // Conditionally load the config
            if (ConfigFileMetadata.MenuAttribute.LoadOn.HasFlag(MenuAttribute.LoadEvents.MenuRegistered))
                ConfigFileMetadata.Config.Load();
        }

        private void BindEvents()
        {
            ConfigFileMetadata.BindEvents();

            ButtonClicked += EventHandler;
            ButtonClicked += EventHandler;
            ChoiceChanged += EventHandler;
            KeybindChanged += EventHandler;
            SliderChanged += EventHandler;
            ToggleChanged += EventHandler;
            GameObjectCreated += EventHandler;
        }

        private void EventHandler(object sender, EventArgs e)
        {
            if (!ConfigFileMetadata.Registered)
            {
                // if we haven't marked the options menu as being registered yet, its too soon to fire the events,
                // so run a coroutine that waits until the first frame where Registered == true
                // before routing the event
                UWE.CoroutineHost.StartCoroutine(DeferredEventHandlerRoutine(sender, e));
            }
            else
            {
                // otherwise, route the event immediately
                RouteEventHandler(sender, e);
            }
        }

        private IEnumerator DeferredEventHandlerRoutine(object sender, EventArgs e)
        {
            yield return new WaitUntil(() => ConfigFileMetadata.Registered);
            RouteEventHandler(sender, e);
        }

        private void RouteEventHandler(object sender, EventArgs e)
        {
            switch (e)
            {
                case ButtonClickedEventArgs buttonClickedEventArgs:
                    ConfigFileMetadata.HandleButtonClick(sender, buttonClickedEventArgs);
                    break;
                case ChoiceChangedEventArgs choiceChangedEventArgs:
                    ConfigFileMetadata.HandleChoiceChanged(sender, choiceChangedEventArgs);
                    break;
                case KeybindChangedEventArgs keybindChangedEventArgs:
                    ConfigFileMetadata.HandleKeybindChanged(sender, keybindChangedEventArgs);
                    break;
                case SliderChangedEventArgs sliderChangedEventArgs:
                    ConfigFileMetadata.HandleSliderChanged(sender, sliderChangedEventArgs);
                    break;
                case ToggleChangedEventArgs toggleChangedEventArgs:
                    ConfigFileMetadata.HandleToggleChanged(sender, toggleChangedEventArgs);
                    break;
                case GameObjectCreatedEventArgs gameObjectCreatedEventArgs:
                    ConfigFileMetadata.HandleGameObjectCreated(sender, gameObjectCreatedEventArgs);
                    break;
            }
        }

        #region Build ModOptions Menu
        /// <summary>
        /// Adds options to the menu based on the <see cref="ConfigFileMetadata"/>.
        /// </summary>
        public override void BuildModOptions()
        {
            // Conditionally load the config
            if (ConfigFileMetadata.MenuAttribute.LoadOn.HasFlag(MenuAttribute.LoadEvents.MenuOpened))
                ConfigFileMetadata.Config.Load();

            foreach (KeyValuePair<string, ModOptionAttributeMetadata<T>> entry in ConfigFileMetadata.ModOptionAttributesMetadata
                .OrderBy(x => x.Value.ModOptionAttribute.Order)
                .ThenBy(x => x.Value.MemberInfoMetadata.Name))
            {
                string id = entry.Key;
                ModOptionAttributeMetadata<T> modOptionMetadata = entry.Value;

                string label = modOptionMetadata.ModOptionAttribute.Label;
                if (Language.main.TryGet(modOptionMetadata.ModOptionAttribute.LabelLanguageId, out string languageLabel))
                    label = languageLabel;

                Logger.Debug($"[{ConfigFileMetadata.QMod.DisplayName}] [{typeof(T).Name}] {modOptionMetadata.MemberInfoMetadata.Name}: " +
                    $"{modOptionMetadata.ModOptionAttribute.GetType().Name}");
                Logger.Debug($"[{ConfigFileMetadata.QMod.DisplayName}] [{typeof(T).Name}] Label: {label}");


                switch (modOptionMetadata.ModOptionAttribute)
                {
                    case ButtonAttribute _:
                        BuildModButtonOption(id, label);
                        break;
                    case ChoiceAttribute choiceAttribute:
                        BuildModChoiceOption(id, label, modOptionMetadata.MemberInfoMetadata, choiceAttribute);
                        break;
                    case KeybindAttribute _:
                        BuildModKeybindOption(id, label, modOptionMetadata.MemberInfoMetadata);
                        break;
                    case SliderAttribute sliderAttribute:
                        BuildModSliderOption(id, label, modOptionMetadata.MemberInfoMetadata, sliderAttribute);
                        break;
                    case ToggleAttribute _:
                        BuildModToggleOption(id, label, modOptionMetadata.MemberInfoMetadata);
                        break;
                }
            }
        }

        /// <summary>
        /// Adds a <see cref="ModButtonOption"/> to the <see cref="ModOptions"/> menu.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="label"></param>
        private void BuildModButtonOption(string id, string label)
        {
            AddButtonOption(id, label);
        }

        /// <summary>
        /// Adds a <see cref="ModChoiceOption"/> to the <see cref="ModOptions"/> menu.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="label"></param>
        /// <param name="memberInfoMetadata">The metadata of the corresponding member.</param>
        /// <param name="choiceAttribute">The defined or generated <see cref="ChoiceAttribute"/> of the member.</param>
        private void BuildModChoiceOption(string id, string label,
            MemberInfoMetadata<T> memberInfoMetadata, ChoiceAttribute choiceAttribute)
        {
            if (memberInfoMetadata.ValueType.IsEnum && (choiceAttribute.Options == null || !choiceAttribute.Options.Any()))
            {
                // Enum-based choice where the values are parsed from the enum type
                string[] options = Enum.GetNames(memberInfoMetadata.ValueType);
                string value = memberInfoMetadata.GetValue(ConfigFileMetadata.Config).ToString();
                AddChoiceOption(id, label, options, value);
            }
            else if (memberInfoMetadata.ValueType.IsEnum)
            {
                // Enum-based choice where the values are defined as custom strings
                string[] options = choiceAttribute.Options;
                string name = memberInfoMetadata.GetValue(ConfigFileMetadata.Config).ToString();
                int index = Math.Max(Array.IndexOf(Enum.GetNames(memberInfoMetadata.ValueType), name), 0);
                AddChoiceOption(id, label, options, index);
            }
            else if (memberInfoMetadata.ValueType == typeof(string))
            {
                // string-based choice value
                string[] options = choiceAttribute.Options;
                string value = memberInfoMetadata.GetValue<string>(ConfigFileMetadata.Config);
                AddChoiceOption(id, label, options, value);
            }
            else if (memberInfoMetadata.ValueType == typeof(int))
            {
                // index-based choice value
                string[] options = choiceAttribute.Options;
                int index = memberInfoMetadata.GetValue<int>(ConfigFileMetadata.Config);
                AddChoiceOption(id, label, options, index);
            }
        }

        /// <summary>
        /// Adds a <see cref="ModKeybindOption"/> to the <see cref="ModOptions"/> menu.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="label"></param>
        /// <param name="memberInfoMetadata">The metadata of the corresponding member.</param>
        private void BuildModKeybindOption(string id, string label, MemberInfoMetadata<T> memberInfoMetadata)
        {
            KeyCode value = memberInfoMetadata.GetValue<KeyCode>(ConfigFileMetadata.Config);
            AddKeybindOption(id, label, GameInput.Device.Keyboard, value);
        }

        /// <summary>
        /// Adds a <see cref="ModSliderOption"/> to the <see cref="ModOptions"/> menu.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="label"></param>
        /// <param name="memberInfoMetadata">The metadata of the corresponding member.</param>
        /// <param name="sliderAttribute">The defined or generated <see cref="SliderAttribute"/> of the member.</param>
        private void BuildModSliderOption(string id, string label,
            MemberInfoMetadata<T> memberInfoMetadata, SliderAttribute sliderAttribute)
        {
            float value = Convert.ToSingle(memberInfoMetadata.GetValue(ConfigFileMetadata.Config));

            float step = sliderAttribute.Step;
            if (memberInfoMetadata.ValueType == typeof(int))
                step = Mathf.CeilToInt(step);

            AddSliderOption(id, label, sliderAttribute.Min, sliderAttribute.Max,
                Convert.ToSingle(value), sliderAttribute.DefaultValue,
                sliderAttribute.Format, step);
        }

        /// <summary>
        /// Adds a <see cref="ModToggleOption"/> to the <see cref="ModOptions"/> menu.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="label"></param>
        /// <param name="memberInfoMetadata">The metadata of the corresponding member.</param>
        private void BuildModToggleOption(string id, string label, MemberInfoMetadata<T> memberInfoMetadata)
        {
            bool value = memberInfoMetadata.GetValue<bool>(ConfigFileMetadata.Config);
            AddToggleOption(id, label, value);
        }
        #endregion
    }
}
