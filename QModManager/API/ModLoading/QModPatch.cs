namespace QModManager.API.ModLoading
{
    using System;
    using QModManager.Patching;

    /// <summary>
    /// Identifies a normal patch method for a QMod.<para/>
    /// This method must be public, must take no parameters, and must return <seealso cref="void"/>.<para/>
    /// ALERT: The class that defines this method must have a <seealso cref="QModCoreAttribute"/> attribute.
    /// </summary>
    /// <seealso cref="Attribute" />
    public sealed class QModPatch : QModPatchAttributeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QModPatch"/> class for normal patching.        
        /// </summary>
        public QModPatch() : base(PatchingOrder.NormalInitialize)
        {
        }
    }
}
