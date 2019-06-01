namespace QModManager.API.ModLoading
{
    using System;

    /// <summary>
    /// Identifies other QMods that this mod requires.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class QModDependency : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QModDependency"/> class.
        /// </summary>
        /// <param name="requiredMod">The other mod required by this one to function.</param>
        public QModDependency(string requiredMod)
            : this(requiredMod, null)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QModDependency"/> class.
        /// </summary>
        /// <param name="requiredMod">The other mod required by this one to function.</param>
        /// <param name="requiredVersion">The minimum version the other mod should be at.</param>
        public QModDependency(string requiredMod, string requiredVersion)
        {
            this.RequiredMod = requiredMod;
            this.MinimumVersion = requiredVersion == null ? new Version() : new Version(requiredVersion);
        }

        /// <summary>
        /// Identifies another QMod, by Id, that this mod requires to function.
        /// </summary>
        public string RequiredMod { get; set; }

        /// <summary>
        /// Identifies what minimum version the other mod should be at.
        /// </summary>
        public Version MinimumVersion { get; set; }
    }
}
