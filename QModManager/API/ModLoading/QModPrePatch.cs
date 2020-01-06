namespace QModManager.API.ModLoading
{
    using System;
    using QModManager.Patching;

    /// <summary>
    /// Identifies a pre-patch method for a QMod.<para/>
    /// This method must be public, must take no parameters, and must return <seealso cref="void"/><para/>
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
        /// Initializes a new instance of the <see cref="QModPrePatch"/> class.<para/>
        /// Should only be used for mods that need to load first, before all other mods. Read the documentation for instructions.
        /// </summary>
        /// <param name="secretPassword">Should only be used for mods that need to load first, before all other mods. Read the documentation for instructions.</param>
        public QModPrePatch(string secretPassword) : base(PatchingOrder.MetaPreInitialize, secretPassword)
        {
        }
    }
}
