namespace SMLHelper.V2.Options.Attributes
{
    using Json;
    using System;
    using QModManager.Utility;

    /// <summary>
    /// Attribute used to signify the decorated method should be represented in the mod's options menu
    /// as a <see cref="ModButtonOption"/>.
    /// When the button is clicked, the given method will run.
    /// </summary>
    /// <example>
    /// <code>
    /// using SMLHelper.V2.Json;
    /// using SMLHelper.V2.Options;
    /// using QModManager.Utility;
    /// 
    /// [Menu("My Options Menu")]
    /// public class Config : ConfigFile
    /// {
    ///     [Button("My Cool Button")]
    ///     public static void MyCoolButton(object sender, ButtonClickedEventArgs e)
    ///     {
    ///         Logger.Log(Logger.Level.Info, "Button was clicked!");
    ///         Logger.Log(Logger.Level.Info, e.Id.ToString());
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="MenuAttribute"/>
    /// <seealso cref="ButtonClickedEventArgs"/>
    /// <seealso cref="Logger"/>
    /// <seealso cref="ConfigFile"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ButtonAttribute : ModOptionAttribute
    {
        /// <summary>
        /// Signifies the decorated method should be represented in the mod's options menu as a <see cref="ModButtonOption"/>
        /// and sets its label.
        /// </summary>
        /// <param name="label">The label for the button. If none is set, the name of the method will be used.</param>
        public ButtonAttribute(string label = null) : base(label) { }

        /// <summary>
        /// Signifies the decorated method should be represented in the mod's options menu as a <see cref="ModButtonOption"/>.
        /// </summary>
        public ButtonAttribute() { }
    }
}
