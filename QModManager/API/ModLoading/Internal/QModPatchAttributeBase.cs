namespace QModManager.API.ModLoading.Internal
{
    using System;

    /// <summary>
    /// Base class to all attributes that identify QMod patch methods.
    /// </summary>
    /// <seealso cref="System.Attribute" />
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
