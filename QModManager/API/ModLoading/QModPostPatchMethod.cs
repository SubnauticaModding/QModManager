namespace QModManager.API.ModLoading
{
    using System;
    using QModManager.Patching;

    /// <summary>
    /// Identifies a post-patch method for a QMod.<para/>
    /// This method must be public, must take no parameters, and must return either <seealso cref="void"/> or <seealso cref="PatchResults"/>.<para/>
    /// ALERT: The class that defines this method must have a <seealso cref="QModCoreInfo"/> attribute.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class QModPostPatchMethod : QModPatchAttributeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QModPostPatchMethod"/> class for post-patching.
        /// </summary>
        public QModPostPatchMethod() : base(PatchingOrder.PostInitialize)
        {
        }
    }
}
