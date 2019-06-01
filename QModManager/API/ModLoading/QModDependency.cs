namespace QModManager.API.ModLoading
{
    using System;

    /// <summary>
    /// Identifies other QMods that this mod requires.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class QModDependency : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QModDependency"/> class.
        /// </summary>
        /// <param name="requiredMod">The other mod required by this one to function.</param>
        public QModDependency(string requiredMod)
            : this(requiredMod, new Version())
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QModDependency" /> class.
        /// </summary>
        /// <param name="requiredMod">The other mod required by this one to function.</param>
        /// <param name="major">The minimum major version of the mod required.</param>
        public QModDependency(string requiredMod, int major)
            : this(requiredMod, new Version(major, 0))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QModDependency"/> class.
        /// </summary>
        /// <param name="requiredMod">The other mod required by this one to function.</param>
        /// <param name="major">The minimum major version of the mod required.</param>
        /// <param name="minor">The specific minimum minor version.</param>
        public QModDependency(string requiredMod, int major, int minor)
            : this(requiredMod, new Version(major, minor))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QModDependency"/> class.
        /// </summary>
        /// <param name="requiredMod">The other mod required by this one to function.</param>
        /// <param name="major">The minimum major version of the mod required.</param>
        /// <param name="minor">The specific minimum minor update of the major version.</param>
        /// <param name="build">The specific build update of the minor version.</param>
        public QModDependency(string requiredMod, int major, int minor, int build)
            : this(requiredMod, new Version(major, minor, build))
        {
        }

        internal QModDependency(string requiredMod, Version requiredVersion)
        {
            this.RequiredMod = requiredMod;
            this.MinimumVersion = requiredVersion;
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
