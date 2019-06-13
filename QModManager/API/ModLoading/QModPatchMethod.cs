namespace QModManager.API.ModLoading
{
    using Internal;
    using System;

    /// <summary>
    /// Identifies a normal patch method for a QMod.<para/>
    /// This method must be public, must take no parameters, and must return either <seealso cref="void"/> or <seealso cref="PatchResults"/>.<para/>
    /// ALERT: The class that defines this method must have a <seealso cref="QModCoreInfo"/> attribute.
    /// </summary>
    /// <seealso cref="Attribute" />
    public sealed class QModPatchMethod : QModPatchAttributeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QModPatchMethod"/> class for normal patching.        
        /// </summary>
        public QModPatchMethod() : base(PatchingOrder.NormalInitialize)
        {
        }
    }
}
