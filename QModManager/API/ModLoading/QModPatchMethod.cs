namespace QModManager.API.ModLoading
{
    using Internal;
    using System;

    /// <summary>
    /// Identifies a normal patch method for a QMod.<para/>
    /// ALERT: The class that defines this method must have a <seealso cref="QModCoreInfo"/> attribute.
    /// </summary>
    /// <seealso cref="Attribute" />
    public class QModPatchMethod : QModPatchAttributeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QModPatchMethod"/> class for normal patching.        
        /// </summary>
        public QModPatchMethod() : base(PatchingOrder.NormalInitialize)
        {
        }
    }
}
