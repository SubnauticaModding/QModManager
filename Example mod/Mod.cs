using HarmonyLib;
using QModManager.API.ModLoading;
using SMLHelper.V2.Commands;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Interfaces;
using SMLHelper.V2.Json;
using SMLHelper.V2.Json.Attributes;
using SMLHelper.V2.Options;
using SMLHelper.V2.Options.Attributes;
using System;
using UnityEngine;
using UnityEngine.UI;
using Logger = QModManager.Utility.Logger;

namespace SMLHelper.V2.Examples
{
    [QModCore]
    public static class ExampleMod
    {
        /// <summary>
        /// A simple SaveDataCache implementation, intended to save the players current position to disk.
        /// </summary>
        [FileName("player_position")]
        internal class SaveData : SaveDataCache
        {
            public Vector3 PlayerPosition { get; set; }
        }

        [QModPatch]
        public static void Patch()
        {
            /// Here, we are setting up a instance of <see cref="Config"/>, which will automatically generate an 
            /// options menu using Attributes. The values in this instance will be updated whenever the user changes 
            /// the corresponding option in the menu.
            Config config = OptionsPanelHandler.Main.RegisterModOptions<Config>();

            /// In a similar manner to the above, here we set up an instance of <see cref="SaveData"/>, which will 
            /// automatically be saved and loaded to and from disk appropriately.
            /// The values in this instance will be updated automatically whenever the user switches between save slots.
            SaveData saveData = SaveDataHandler.Main.RegisterSaveDataCache<SaveData>();

            // Simply display the recorded player position whenever the save data is loaded
            saveData.OnFinishedLoading += (object sender, JsonFileEventArgs e) =>
            {
                SaveData data = e.Instance as SaveData; // e.Instance is the instance of your SaveData stored as a JsonFile.
                                                        // We can use polymorphism to convert it back into a SaveData
                                                        // instance, and access its members, such as PlayerPosition.

                Logger.Log(Logger.Level.Info,
                           $"loaded player position from save slot: {data.PlayerPosition}",
                           showOnScreen: true);
            };

            // Update the player position before saving it
            saveData.OnStartedSaving += (object sender, JsonFileEventArgs e) =>
            {
                SaveData data = e.Instance as SaveData;
                data.PlayerPosition = Player.main.transform.position;
            };

            // Simply display the position we recorded to the save file whenever the save data it is saved
            saveData.OnFinishedSaving += (object sender, JsonFileEventArgs e) =>
            {
                SaveData data = e.Instance as SaveData;
                Logger.Log(Logger.Level.Info,
                           $"saved player position to save slot: {data.PlayerPosition}",
                           showOnScreen: true);
            };

            /// Here we are registering a console command by use of a delegate. The delegate will respond to the "delegatecommand"
            /// command from the dev console, passing values following "delegatecommand" as the correct types, provided they can be
            /// parsed to that type. For example, "delegatecommand foo 3 true" would be a valid command for the
            /// <see cref="MyCommand"/> delegate signature. You can also use Func or Action to define your delegate signatures
            /// if you prefer, and you can also pass a reference to a method that matches this signature.
            /// 
            /// Registered commands must be unique. If another mod has already added the command, your command will be rejected.
            /// 
            /// If the user enters incorrect parameters for a command, they will be notified of the expected parameter types,
            /// both on-screen and in the log.
            /// 
            /// Note that a command can have a return type, but it is not necessary. If it does return any type, it will be printed
            /// both on-screen and in the log.
            ConsoleCommandsHandler.Main.RegisterConsoleCommand<MyCommand>("delegatecommand", (myString, myInt, myBool) =>
            {
                return $"Parameters: {myString} {myInt} {myBool}";
            });

            /// Here we are registering all console commands defined in <see cref="ExampleMod"/>, defined by decorating them
            /// with the <see cref="ConsoleCommandAttribute"/>. See <see cref="MyAttributedCommand(string, int, bool)"/> below
            /// for an example.
            ConsoleCommandsHandler.Main.RegisterConsoleCommands(typeof(ExampleMod));

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }

        private delegate string MyCommand(string myString, int myInt, bool myBool);

        /// <summary>
        /// <para>Here, we are using the <see cref="ConsoleCommandAttribute"/> to define a custom console command, which is
        /// registered via our use of <see cref="IConsoleCommandHandler.RegisterConsoleCommands(Type)"/> above.</para>
        /// 
        /// <para>This method will respond to the "attributedcommand" command from the dev console. The command will respect the method
        /// signature of the decorated method, passing values following "attributedcommand" as the correct types, as long as they can be
        /// parsed to that type. For example, "attributedcommand foo 3 true" would be a valid command for this method signature.</para>
        /// 
        /// <para>The decorated method must be both <see langword="public"/> and <see langword="static"/>, or the attribute will
        /// be ignored. <see cref="IConsoleCommandHandler.RegisterConsoleCommand(string, Type, string, Type[])"/> allows for
        /// targeting non-<see langword="public"/> methods (must still be <see langword="static"/>), and uses a
        /// similar syntax to <see cref="HarmonyPatch"/> for defining the target method.</para>
        /// </summary>
        /// <param name="myString"></param>
        /// <param name="myInt"></param>
        /// <param name="myBool"></param>
        /// <returns></returns>
        [ConsoleCommand("attributedcommand")]
        public static string MyAttributedCommand(string myString, int myInt, bool myBool = false)
        {
            return $"Parameters: {myString} {myInt} {myBool}";
        }
    }

    public enum CustomChoice { One, Two, Three }

    /// <summary>
    /// <para>The <see cref="MenuAttribute"/> allows us to set the title of our options menu in the "Mods" tab.</para>
    /// 
    /// <para>Optionally, we can set the <see cref="MenuAttribute.SaveOn"/> or <see cref="MenuAttribute.LoadOn"/> properties to
    /// customise when the values are saved to or loaded from disk respectively. By default, the values will be saved whenever
    /// they change, and loaded from disk when the game is registered to the options menu, which in this example happens on game
    /// launch and is the recommended setting.</para>
    /// 
    /// <para>Both of these values allow for bitwise combinations of their options, so
    /// <c>[Menu("SMLHelper Example Mod", LoadOn = MenuAttribute.LoadEvents.MenuRegistered | MenuAttribute.LoadEvents.MenuOpened)]</c>
    /// is valid and will result in the values being loaded both on game start and also whenever the menu is opened.</para>
    /// 
    /// <para>We could also specify a <see cref="ConfigFileAttribute"/> here to customise the name of the config file
    /// (defaults to "config") and an optional subfolder for the config file to reside in.</para>
    /// </summary>
    [Menu("SMLHelper Example Mod")]
    public class Config : ConfigFile
    {
        /// <summary>
        /// <para>A <see cref="ChoiceAttribute"/> is represented by a group of options where only one can be selected at a time,
        /// similar in usage to a dropdown or radial button group.</para>
        /// 
        /// <para>Here, we are defining a <see cref="ChoiceAttribute"/> with the label "My index-based choice", where the underlying
        /// <see cref="int"/> field represents the index in an array of choices, where the values "One", "Two" and "Three" make
        /// up that array. As we are not specifiying a default value, the index will by 0 by default.</para>
        ///
        /// <para>The <see cref="OnChangeAttribute"/> is optional and allows us to specify the name of a method in the Config class to
        /// call when the value has been changed via the options menu. Note that in many cases, you won't need to specify an OnChange
        /// event, as the values are automatically saved to disk for you as specified by the <see cref="MenuAttribute"/>, and are
        /// updated in the instance of <see cref="Config"/> returned when registering it to the options menu.</para>
        /// 
        /// <para>Here, we are specifying the name of a method which can handle any OnChange event, for the purposes of demonstrating
        /// its usage. See <see cref="MyGenericValueChangedEvent(IModOptionEventArgs)"/> for an example usage.</para>
        /// </summary>
        [Choice("My index-based choice", "One", "Two", "Three"), OnChange(nameof(MyGenericValueChangedEvent))]
        public int ChoiceIndex;

        /// <summary>
        /// Here, we are defining a <see cref="ChoiceAttribute"/> which uses a <see cref="string"/> as its backing field,
        /// where the value represents which option is currently selected, and are specifying "Foo" as the default.
        /// </summary>
        [Choice("My string-based choice", "Foo", "Bar"), OnChange(nameof(MyGenericValueChangedEvent))]
        public string ChoiceValue = "Foo";

        /// <summary>
        /// Here, we are defining a <see cref="ChoiceAttribute"/> which uses an <see langword="enum"/>-type as its backing field,
        /// and the string values for each value defined in the <see langword="enum"/> will be used to represent each option,
        /// so we don't need to specify them. The options will be "One", "Two" and "Three".
        /// </summary>
        [Choice("My enum-based choice"), OnChange(nameof(MyGenericValueChangedEvent))]
        public CustomChoice ChoiceEnum;

        /// <summary>
        /// <para>Here, we are again defining a <see cref="ChoiceAttribute"/> with a <see langword="enum"/>-type as its backing field,
        /// however we are also specifying custom strings which will be used to represent each option in the menu.</para>
        /// 
        /// <para>An option of <see cref="CustomChoice.One"/> will be represented by the <see cref="string"/> "1",
        /// <see cref="CustomChoice.Two"/> will be represented by the <see cref="string"/> "2", and so on.</para>
        /// </summary>
        [Choice("My customised enum-based choice", "1", "2", "3"), OnChange(nameof(MyGenericValueChangedEvent))]
        public CustomChoice ChoiceCustomEnum;

        /// <summary>
        /// <para>A <see cref="KeybindAttribute"/> is represented in the mod options menu as a customistable keybind, where the
        /// user clicks the box to set the binding, stored as a <see cref="KeyCode"/>.</para>
        /// 
        /// <para>Here, we are not specifying a default, so by default this keybind will not be set.</para>
        /// </summary>
        [Keybind("My keybind"), OnChange(nameof(MyGenericValueChangedEvent))]
        public KeyCode KeybindKey;

        /// <summary>
        /// <para>A <see cref="SliderAttribute"/> is used to represent a numeric value as a slider in the options menu, with a
        /// minimum and maximum value. By default, the minimum value is 0 and maximum is 100.</para>
        /// 
        /// <para>Here, we are setting an initial value of 25 for the slider. We are also setting the
        /// <see cref="SliderAttribute.DefaultValue"/> property so that this default can be represented by a notch in the slider.
        /// </para>
        /// </summary>
        [Slider("My slider", 0, 50, DefaultValue = 25), OnChange(nameof(MyGenericValueChangedEvent))]
        public int SliderValue = 25;

        /// <summary>
        /// <para>Here, we are defining a <see cref="SliderAttribute"/> with a step of 10, meaning that the value will only 
        /// increment/decrement by 10, resulting in possible values of 0, 10, 20, 30, and so on up to 100.</para>
        /// 
        /// <para>By default, an <see cref="int"/> has a step of 1, and a <see cref="float"/> or <see cref="double"/>
        /// has a step of 0.05.</para>
        /// </summary>
        [Slider("My stepped slider", Step = 10), OnChange(nameof(MyGenericValueChangedEvent))]
        public int SteppedSliderValue;

        /// <summary>
        /// Here, we are defining a <see cref="SliderAttribute"/> with a format string which defines how the numeric value
        /// is displayed.
        /// See <see href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings"/> for more
        /// info on numeric format strings.
        /// </summary>
        [Slider("My float-based slider", Format = "{0:F2}"), OnChange(nameof(MyGenericValueChangedEvent))]
        public float FloatSliderValue;

        /// <summary>
        /// <para>A <see cref="ToggleAttribute"/> is used to represent a checkbox in the options menu
        /// and is backed by a <see cref="bool"/>.</para>
        /// 
        /// <para>Note that here we are defining two <see cref="OnChangeAttribute"/>s which correspond to different methods
        /// in the class. They will both be fired when the value changes, but one of them is specific to this value only. See
        /// <see cref="MyCheckboxToggleEvent(ToggleChangedEventArgs)"/> for an example usage.</para>
        /// </summary>
        [Toggle("My checkbox"), OnChange(nameof(MyCheckboxToggleEvent)), OnChange(nameof(MyGenericValueChangedEvent))]
        public bool ToggleValue;

        /// <summary>
        /// <para>A <see cref="ButtonAttribute"/> is used to represent a button in the options menu, and is different to the other
        /// attributes above in that it is backed by a public method rather than a field or property, and the method that is
        /// decorated with the <see cref="ButtonAttribute"/> will be called when the corresponding button is clicked, allowing you
        /// to perform some custom action on demand from the options menu.</para>
        /// 
        /// <para>Here, we are also defining an <see cref="OnGameObjectCreatedAttribute"/> to demonstrate its usage. In most cases,
        /// this attribute will not be needed, but in some cases if you would like to perform some special action with the
        /// <see cref="GameObject"/> it can be useful. See <see cref="MyGameObjectCreatedEvent(GameObjectCreatedEventArgs)"/> for an
        /// example usage.</para>
        /// </summary>
        /// <param name="e">The <see cref="ButtonClickedEventArgs"/> passed from the button click event, containing the id of the
        /// button as a string. It is worth mentioning that it is not necessary to define the <see cref="ButtonClickedEventArgs"/>
        /// if you don't need it.</param>
        [Button("My button"), OnGameObjectCreated(nameof(MyGameObjectCreatedEvent))]
        public void MyButtonClickEvent(ButtonClickedEventArgs e)
        {
            Logger.Log(Logger.Level.Info, "Button was clicked!");
            Logger.Log(Logger.Level.Info, $"{e.Id}");
        }

        /// <summary>
        /// This method will be called whenever a value with an <see cref="OnChangeAttribute"/> referencing it by name is changed.
        /// In this example, only the field <see cref="ToggleValue"/> references it, so it will only be called whenever this value
        /// is changed by the user via the options menu.
        /// </summary>
        /// <param name="e">The <see cref="ToggleChangedEventArgs"/> passed from the onchange event, containing the id of the field
        /// as a string as well as the new value. As with the other events in this example, it is not necessary to define the
        /// parameter if you do not need the data it contains.</param>
        private void MyCheckboxToggleEvent(ToggleChangedEventArgs e)
        {
            Logger.Log(Logger.Level.Info, "Checkbox value was changed!");
            Logger.Log(Logger.Level.Info, $"{e.Value}");
        }

        /// <summary>
        /// This method will be called whenever a value with an <see cref="OnChangeAttribute"/> referencing it by name is changed.
        /// In this example, every field above references it, so it will be called whenever any value in this class is changed by the
        /// user via the options menu.
        /// </summary>
        /// <param name="e"><para>The data from the onchange event, passed as the interface <see cref="IModOptionEventArgs"/>.</para>
        /// 
        /// <para>As this particular method is being used as an onchange event for various field types, the usage of the
        /// <see cref="IModOptionEventArgs"/> interface here enables coercion to its original data type for correct handling, as
        /// demonstrated by the <see langword="switch"/> statement below.</para>
        /// 
        /// <para>As with the other events in this example, it is not necessary to define the parameter if you do not need the data
        /// it contains.</para></param>
        private void MyGenericValueChangedEvent(IModOptionEventArgs e)
        {
            Logger.Log(Logger.Level.Info, "Generic value changed!");
            Logger.Log(Logger.Level.Info, $"{e.Id}: {e.GetType()}");

            switch (e)
            {
                case KeybindChangedEventArgs keybindChangedEventArgs:
                    Logger.Log(Logger.Level.Info, keybindChangedEventArgs.KeyName);
                    break;
                case ChoiceChangedEventArgs choiceChangedEventArgs:
                    Logger.Log(Logger.Level.Info, $"{choiceChangedEventArgs.Index}: {choiceChangedEventArgs.Value}");
                    break;
                case SliderChangedEventArgs sliderChangedEventArgs:
                    Logger.Log(Logger.Level.Info, sliderChangedEventArgs.Value.ToString());
                    break;
                case ToggleChangedEventArgs toggleChangedEventArgs:
                    Logger.Log(Logger.Level.Info, toggleChangedEventArgs.Value.ToString());
                    break;
            }
        }

        /// <summary>
        /// The method will be called whenever the <see cref="GameObject"/> for a member with a
        /// <see cref="OnGameObjectCreatedAttribute"/> referencing it by name is created. In this example, only the
        /// <see cref="MyButtonClickEvent(ButtonClickedEventArgs)"/> button is referencing it, so it will only be called whenever
        /// this button is created.
        /// </summary>
        /// <param name="e">The <see cref="GameObjectCreatedEventArgs"/> passed from the event, containing the id of the field
        /// as a string as well as the newly created <see cref="GameObject"/>.</param>
        private void MyGameObjectCreatedEvent(GameObjectCreatedEventArgs e)
        {
            Logger.Log(Logger.Level.Info, "GameObject was created");
            Logger.Log(Logger.Level.Info, $"{e.Id}: {e.GameObject}");
        }
    }
}
