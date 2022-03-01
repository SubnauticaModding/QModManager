namespace SMLHelper.V2.Options.Attributes
{
    using Json;
    using System;

    /// <summary>
    /// Attribute used to signify the given property, field or method should be ignored when generating your mod options menu.
    /// </summary>
    /// <remarks>
    /// By default, all members are ignored unless either they are decorated with a <see cref="ModOptionAttribute"/> derivative,
    /// or the <see cref="MenuAttribute.MemberProcessing"/> property is set to <see cref="MenuAttribute.Members.Implicit"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// using SMLHelper.V2.Json;
    /// using SMLHelper.V2.Options;
    /// 
    /// [Menu("My Options Menu")]
    /// public class Config : ConfigFile
    /// {
    ///     [Button("My Cool Button)]
    ///     public static void MyCoolButton(object sender, ButtonClickedEventArgs e)
    ///     {
    ///         Logger.Log(Logger.Level.Info, "Button was clicked!");
    ///         Logger.Log(Logger.Level.Info, e.Id.ToString());
    ///     }
    ///     
    ///     [IgnoreMember]
    ///     public int FieldNotDisplayedInMenu;
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="MenuAttribute"/>
    /// <seealso cref="ButtonAttribute"/>
    /// <seealso cref="ConfigFile"/>
    /// <seealso cref="MenuAttribute.MemberProcessing"/>
    /// <seealso cref="MenuAttribute.Members"/>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class IgnoreMemberAttribute : Attribute { }
}
