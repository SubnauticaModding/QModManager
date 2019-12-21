namespace QModManager.API
{
    using System;
    using QModManager.Patching;

    /// <summary>
    /// Base class to all attributes that identify QMod patch methods.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public abstract class QModPatchAttributeBase : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QModPatchAttributeBase"/> class.
        /// </summary>
        /// <param name="patchOrder">The patch order.</param>
        internal QModPatchAttributeBase(PatchingOrder patchOrder)
        {
            this.PatchOrder = patchOrder;
        }

        internal PatchingOrder PatchOrder { get; }
    }
}
