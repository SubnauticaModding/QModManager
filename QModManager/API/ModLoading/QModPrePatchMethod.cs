namespace QModManager.API.ModLoading
{
    using Internal;
    using System;

    /// <summary>
    /// Identifies a pre-patch method for a QMod.<para/>
    /// This method must be public, must take no parameters, and must return either <seealso cref="void"/> or <seealso cref="PatchResults"/>.<para/>
    /// ALERT: The class that defines this method must have a <seealso cref="QModCoreInfo"/> attribute.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class QModPrePatchMethod : QModPatchAttributeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QModPrePatchMethod"/> class for pre-patching.
        /// </summary>
        public QModPrePatchMethod() : base(PatchingOrder.PreInitialize)
        {
        }
    }
}
