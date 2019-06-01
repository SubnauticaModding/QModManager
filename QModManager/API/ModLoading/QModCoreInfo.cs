namespace QModManager.API.ModLoading
{
    using System;

    /// <summary>
    /// Provides important custom meta data about your QMod.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class QModCoreInfo : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QModCoreInfo" /> class.
        /// </summary>
        /// <param name="id">The mod identifier.</param>
        /// <param name="displayName">The mod display name.</param>
        /// <param name="author">The mod author or team name.</param>
        /// <param name="developedFor">The game the mod was developed for.</param>
        /// <param name="patchMethod">The patch method.</param>
        public QModCoreInfo(string id, string displayName, string author, DevelopedFor developedFor, string patchMethod)
        {
            this.Id = id;
            this.DisplayName = displayName;
            this.Author = author;
            this.DevelopedFor = developedFor;
            this.Game = (Patcher.Game)this.DevelopedFor;
            this.PatchMethod = patchMethod;
        }

        /// <summary>
        /// The ID of the mod <para/>
        /// Can only contain alphanumeric characters and underscores: (<see langword="a-z"/>, <see langword="A-Z"/>, <see langword="0-9"/>, <see langword="_"/>)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The display name of the mod
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The author of the mod
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The game this mod was developed for.
        /// </summary>
        public DevelopedFor DevelopedFor { get; set; }

        /// <summary>
        /// Identifies the patch method. <c>WARNING</c>: This method must be defined in the class using this attribute.
        /// </summary>
        public string PatchMethod { get; set; }

        internal Patcher.Game Game { get; set; }
    }
}
