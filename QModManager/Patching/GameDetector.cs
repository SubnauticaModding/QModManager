namespace QModManager.Patching
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using QModManager.API;
    using QModManager.Utility;

    internal class GameDetector
    {
        internal readonly QModGame CurrentlyRunningGame;
        internal readonly int CurrentGameVersion = -1;

        /// <summary>
        /// Game -> game version.
        /// 0 = no minimum version required.
        /// </summary>
        private static readonly Dictionary<QModGame, int> SupportedGameVersions = new Dictionary<QModGame, int>
        {
            { QModGame.BelowZero, 32698 }, { QModGame.Subnautica, 66193 }
        };

        internal bool IsValidGameRunning => SupportedGameVersions.ContainsKey(CurrentlyRunningGame);
        internal int MinimumBuildVersion => IsValidGameRunning ? SupportedGameVersions[CurrentlyRunningGame] : -1;
        internal bool IsValidGameVersion => IsValidGameRunning && MinimumBuildVersion == 0 || (CurrentGameVersion > -1 && CurrentGameVersion >= MinimumBuildVersion);

        internal GameDetector()
        {
            bool isSubnautica = Directory.GetFiles(Environment.CurrentDirectory, "Subnautica.exe", SearchOption.TopDirectoryOnly).Length > 0
                || Directory.GetDirectories(Environment.CurrentDirectory, "Subnautica.app", SearchOption.TopDirectoryOnly).Length > 0;
            bool isBelowZero = Directory.GetFiles(Environment.CurrentDirectory, "SubnauticaZero.exe", SearchOption.TopDirectoryOnly).Length > 0
                || Directory.GetDirectories(Environment.CurrentDirectory, "SubnauticaZero.app", SearchOption.TopDirectoryOnly).Length > 0;

            if (isSubnautica && !isBelowZero)
            {
                Logger.Info("Detected game: Subnautica");
                CurrentlyRunningGame = QModGame.Subnautica;
            }
            else if (isBelowZero && !isSubnautica)
            {
                Logger.Info("Detected game: BelowZero");
                CurrentlyRunningGame = QModGame.BelowZero;
            }
            else if (isSubnautica && isBelowZero)
            {
                Logger.Fatal("A fatal error has occurred. Both Subnautica and Below Zero files detected!");
                throw new FatalPatchingException("Both Subnautica and Below Zero files detected!");
            }
            else
            {
                Logger.Fatal("A fatal error has occurred. No game executable was found!");
                throw new FatalPatchingException("No game executable was found!");
            }

            Logger.Info($"Game Version: {SNUtils.GetPlasticChangeSetOfBuild()} Build Date: {SNUtils.GetDateTimeOfBuild():dd-MMMM-yyyy}");
            Logger.Info($"Loading QModManager v{Assembly.GetExecutingAssembly().GetName().Version.ToStringParsed()}{(IsValidGameRunning && MinimumBuildVersion != 0 ? $" built for {CurrentlyRunningGame} v{MinimumBuildVersion}" : string.Empty)}...");
            Logger.Info($"Today is {DateTime.Today:dd-MMMM-yyyy}");

            CurrentGameVersion = SNUtils.GetPlasticChangeSetOfBuild(-1);
            if (!IsValidGameVersion)
            {
                Logger.Fatal("A fatal error has occurred. An invalid game version was detected!");
                throw new FatalPatchingException("An invalid game version was detected!");
            }
        }
    }
}
