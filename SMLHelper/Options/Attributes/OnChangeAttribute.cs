namespace SMLHelper.V2.Options.Attributes
{
    using Interfaces;
    using Json;
    using QModManager.Utility;
    using System;

    /// <summary>
    /// Attribute used to signify a method to call whenever the decorated member's value changes.
    /// </summary>
    /// <remarks>
    /// The method must be a member of the same class. Can be specified mutliple times to call multiple methods.
    /// <para>
    /// The specified method can take the following parameters in any order:<br/>
    /// - <see cref="object"/> sender: The sender of the event<br/>
    /// - <see cref="IModOptionEventArgs"/> eventArgs: The generalized event arguments of the event<br/>
    /// - <see cref="ChoiceChangedEventArgs"/> choiceChangedEventArgs: Only when the member corresponds to a
    ///   <see cref="ModChoiceOption"/><br/>
    /// - <see cref="KeybindChangedEventArgs"/> keybindChangedEventArgs: Only when the member correspends to a
    ///   <see cref="ModKeybindOption"/><br/>
    /// - <see cref="SliderChangedEventArgs"/> sliderChangedEventArgs: Only when the member corresponds to a
    ///   <see cref="ModSliderOption"/><br/>
    /// - <see cref="ToggleChangedEventArgs"/> toggleChangedEventArgs: Only when the member corresponds to a
    ///   <see cref="ModToggleOption"/>
    /// </para>
    /// </remarks>
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
    ///     [Toggle("My checkbox"), OnChange(nameof(MyCheckboxToggleEvent)), OnChange(nameof(MyGenericValueChangedEvent))]
    ///     public bool ToggleValue;
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
    /// </code>
    /// </example>
    /// <seealso cref="MenuAttribute"/>
    /// <seealso cref="ToggleAttribute"/>
    /// <seealso cref="IModOptionEventArgs"/>
    /// <seealso cref="ChoiceChangedEventArgs"/>
    /// <seealso cref="KeybindChangedEventArgs"/>
    /// <seealso cref="SliderChangedEventArgs"/>
    /// <seealso cref="ToggleChangedEventArgs"/>
    /// <seealso cref="ConfigFile"/>
    /// <seealso cref="OnGameObjectCreatedAttribute"/>
    /// <seealso cref="Logger"/>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public sealed class OnChangeAttribute : ModOptionEventAttribute
    {
        /// <summary>
        /// Signifies a method to call whenever the decorated member's value changes.
        /// </summary>
        /// <remarks>
        /// The method must be a member of the same class.
        /// </remarks>
        /// <param name="methodName">The name of the method within the same class to invoke.</param>
        public OnChangeAttribute(string methodName) : base(methodName) { }
    }
}
