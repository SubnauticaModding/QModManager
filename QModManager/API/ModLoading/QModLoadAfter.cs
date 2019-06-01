namespace QModManager.API.ModLoading
{
    using Internal;
    using System;

    /// <summary>
    /// Identifies another QMod that must be loaded before this mod.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class QModLoadAfter : Attribute, IModOrder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QModLoadBefore"/> class.
        /// </summary>
        /// <param name="otherMod">The other mod that must be loaded before this mod can be.</param>
        public QModLoadAfter(string otherMod)
        {
            this.OtherMod = otherMod;
        }

        /// <summary>
        /// Identifies another QMod, by Id. If this other mod is present, your mod will be loaded after it.
        /// </summary>
        public string OtherMod { get; set; }
    }
}
