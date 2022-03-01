namespace SMLHelper.V2.Options.Attributes
{
    using Interfaces;
    using System;
    using QModManager.Utility;

    /// <summary>
    /// Attribute used to signify a method to call whenever the <see cref="UnityEngine.GameObject"/> for the 
    /// <see cref="ModOption"/> corresponding to the decorated member is created.
    /// </summary>
    /// <remarks>
    /// The method must be a member of the same class. Can be specified multiple times to call multiple methods.
    /// <para>
    /// The specified method can optionally take the following parameters in any order:<br/>
    /// - <see cref="object"/> sender: The sender of the event<br/>
    /// - <see cref="IModOptionEventArgs"/> eventArgs: The generalized event arguments of the event<br/>
    /// - <see cref="GameObjectCreatedEventArgs"/> gameObjectCreatedEventArgs: The <see cref="GameObjectCreatedEventArgs"/>
    ///   for the event
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// using SMLHelper.V2.Json;
    /// using SMLHelper.V2.Options;
    /// using QModManager.Utility;
    /// using UnityEngine;
    /// 
    /// [Menu("SMLHelper Example Mod")]
    /// public class Config : ConfigFile
    /// {
    ///     [Toggle("My checkbox"), OnGameObjectCreated(nameof(MyGameObjectCreatedEvent))]
    ///     public bool ToggleValue;
    /// 
    ///     private void MyGameObjectCreatedEvent(GameObjectCreatedEventArgs e)
    ///    {
    ///        Logger.Log(Logger.Level.Info, "GameObject was created");
    ///        Logger.Log(Logger.Level.Info, $"{e.Id}: {e.GameObject}");
    ///    }
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="MenuAttribute"/>
    /// <seealso cref="ToggleAttribute"/>
    /// <seealso cref="IModOptionEventArgs"/>
    /// <seealso cref="GameObjectCreatedEventArgs"/>
    /// <seealso cref="OnChangeAttribute"/>
    /// <seealso cref="Logger"/>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class OnGameObjectCreatedAttribute : ModOptionEventAttribute
    {
        /// <summary>
        /// Signifies a method to call whenever the <see cref="UnityEngine.GameObject"/> for the 
        /// <see cref="ModOption"/> corresponding to the decorated member is created.
        /// </summary>
        /// <remarks>
        /// The method must be a member of the same class.
        /// </remarks>
        /// <param name="methodName">The name of the method within the same class to invoke.</param>
        public OnGameObjectCreatedAttribute(string methodName) : base(methodName) { }
    }
}
