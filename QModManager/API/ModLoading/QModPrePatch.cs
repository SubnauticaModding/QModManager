namespace QModManager.API.ModLoading
{
    using System;
    using QModManager.Patching;

    /// <summary>
    /// Identifies a pre-patch method for a QMod.<para/>
    /// This method must be public, must take no parameters, and must return either <seealso cref="void"/> or <seealso cref="PatchResults"/>.<para/>
    /// ALERT: The class that defines this method must have a <seealso cref="QModCoreAttribute"/> attribute.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class QModPrePatch : QModPatchAttributeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QModPrePatch"/> class.
        /// </summary>
        public QModPrePatch() : base(PatchingOrder.PreInitialize)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QModPrePatch"/> class.
        /// </summary>
        /// <param name="secretPassword">The secret password that proves you read the documentation.</param>
        public QModPrePatch(string secretPassword) : base(PatchingOrder.MetaPreInitialize, secretPassword)
        {
        }
    }
}
