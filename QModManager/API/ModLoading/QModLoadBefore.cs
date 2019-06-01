namespace QModManager.API.ModLoading
{
    using System;

    /// <summary>
    /// Identifies another QMod that cannot be loaded before this mod.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class QModLoadBefore : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QModLoadBefore"/> class.
        /// </summary>
        /// <param name="otherMod">The other mod that must wait to load until after this mod does.</param>
        public QModLoadBefore(string otherMod)
        {
            this.OtherMod = otherMod;
        }

        /// <summary>
        /// Identifies another QMod, by Id. If this other mod is present, your mod will be loaded before it.
        /// </summary>
        public string OtherMod { get; set; }
    }
}
