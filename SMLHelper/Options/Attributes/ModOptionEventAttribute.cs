namespace SMLHelper.V2.Options.Attributes
{
    using Interfaces;
    using System;

    /// <summary>
    /// Abstract base attribute used to signify a method to call whenever the derivative event is invoked for the decorated member.
    /// </summary>
    /// <remarks>
    /// The method must be a member of the same class.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public abstract class ModOptionEventAttribute : Attribute, IModOptionEventAttribute
    {
        /// <summary>
        /// The name of the method to invoke.
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Signifies a method to call whenever the derivative event is invoked for the decorated member.
        /// </summary>
        /// <remarks>
        /// The method must be a member of the same class.
        /// </remarks>
        /// <param name="methodName">The name of the method within the same class to invoke.</param>
        public ModOptionEventAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}
