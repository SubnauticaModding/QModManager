namespace QModManager.API.ModLoading.Internal
{
    using System.Reflection;

    internal class QModPlaceholder : QMod, IQMod
    {
        private readonly ModStatus status;
        /// <summary>
        /// The dummy <see cref="QMod"/> which is used to represent QModManager
        /// </summary>
        internal static QModPlaceholder QModManager { get; } = new QModPlaceholder
        {
            Id = "QModManager",
            DisplayName = "QModManager",
            Author = "The QModManager Dev Team",
            LoadedAssembly = Assembly.GetExecutingAssembly(),
            SupportedGame = QModGame.Both,
        };

        private QModPlaceholder()
        {
            // Hide empty constructor. Only to be used for QModManager singlton.
            status = ModStatus.Success;
        }

        internal QModPlaceholder(string name)
        {
            this.Id = Patcher.IDRegex.Replace(name, "");
            this.DisplayName = name;
            this.Author = "Unknown";
            this.SupportedGame = QModGame.None;
            this.Enable = false;
            status = ModStatus.UnidentifiedMod;
        }

        protected override ModStatus Validate(string subDirectory)
        {
            return status;
        }
    }
}
