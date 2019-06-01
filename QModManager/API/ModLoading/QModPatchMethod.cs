namespace QModManager.API.ModLoading
{
    using System;
    using Advanced;

    /// <summary>
    /// Identifies a patch method for a QMod.<para/>
    /// ALERT: The class that defines this method must have a <seealso cref="QModCoreInfo"/> attribute.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class QModPatchMethod : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QModPatchMethod"/> class for normal patching.        
        /// </summary>
        public QModPatchMethod()
            : this(MetaPatchOrder.Normal)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QModPatchMethod"/> class for advanced patching.<para/>
        /// WARNING: Do not use this unless you know exactly why you are attempting to change the patch order.
        /// </summary>
        /// <param name="patchOrder">The patch order.</param>
        public QModPatchMethod(MetaPatchOrder patchOrder)
        {
            this.PatchOrder = patchOrder;
        }

        /// <summary>
        /// Gets the patch method's meta patch order.
        /// </summary>
        public MetaPatchOrder PatchOrder { get; }
    }
}
