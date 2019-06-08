namespace QModManager.API.ModLoading
{
    using System;

    /// <summary>
    /// Provides important custom meta data about your QMod.
    /// ALERT: This class must contain a method with a <seealso cref="QModPatchMethod"/> attribute.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class QModCoreInfo : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QModCoreInfo" /> class.
        /// </summary>
        /// <param name="id">The mod identifier.</param>
        /// <param name="displayName">The mod display name.</param>
        /// <param name="author">The mod author or team name.</param>
        /// <param name="supportedGame">The game the mod was developed for.</param>
        public QModCoreInfo(string id, string displayName, string author, QModGame supportedGame)
        {
            this.Id = id;
            this.DisplayName = displayName;
            this.Author = author;
            this.SupportedGame = supportedGame;
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
        public QModGame SupportedGame { get; set; }
    }
}
