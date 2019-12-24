namespace QModManager.API.ModLoading
{
    using System;

    /// <summary>
    /// Identifies the patching class for your QMod.
    /// </summary>
    /// <seealso cref="QModPatch"/>
    /// <seealso cref="QModPrePatch"/>
    /// <seealso cref="QModPostPatch"/>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class QModCoreAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QModCoreAttribute" /> class.
        /// </summary>
        public QModCoreAttribute()
        {
        }
    }
}
