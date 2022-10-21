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
#if SUBNAUTICA_STABLE
            { QModGame.Subnautica, 65786 }
#else
            { QModGame.BelowZero, 49184 },
            { QModGame.Subnautica, 68186 }
#endif
        };

        internal bool IsValidGameRunning => SupportedGameVersions.ContainsKey(CurrentlyRunningGame);
        internal int MinimumBuildVersion => IsValidGameRunning ? SupportedGameVersions[CurrentlyRunningGame] : -1;
        internal bool IsValidGameVersion => IsValidGameRunning && (MinimumBuildVersion == 0 || (CurrentGameVersion > -1 && CurrentGameVersion >= MinimumBuildVersion));

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

            CurrentGameVersion = SNUtils.GetPlasticChangeSetOfBuild(-1);

            try
            {
                Logger.Info($"Game Version: {CurrentGameVersion} Build Date: {SNUtils.GetDateTimeOfBuild():dd-MMMM-yyyy} Store: {StoreDetector.GetUsedGameStore()}");
            }
            catch
            {
                Logger.Warn($"Game Version: {CurrentGameVersion} Build Date: [Error in displaying Time and Date] Store: {StoreDetector.GetUsedGameStore()}");
            }

#if SUBNAUTICA_STABLE
            Logger.Info($"Loading QModManager v{Assembly.GetExecutingAssembly().GetName().Version.ToStringParsed()} built for Subnautica v{SupportedGameVersions[QModGame.Subnautica]}...");
#elif SUBNAUTICA_EXP
            Logger.Info($"Loading QModManager -Experimental- v{Assembly.GetExecutingAssembly().GetName().Version.ToStringParsed()} built for Subnautica v{SupportedGameVersions[QModGame.Subnautica]}...");
#elif BELOWZERO_STABLE
            Logger.Info($"Loading QModManager v{Assembly.GetExecutingAssembly().GetName().Version.ToStringParsed()} built for Below Zero v{SupportedGameVersions[QModGame.BelowZero]}...");
#elif BELOWZERO_EXP
            Logger.Info($"Loading QModManager -Experimental- v{Assembly.GetExecutingAssembly().GetName().Version.ToStringParsed()} built for Below Zero v{SupportedGameVersions[QModGame.BelowZero]}...");
#endif
            try
            {
                Logger.Info($"Today is {DateTime.Now:dd-MMMM-yyyy_HH:mm:ss}");
            }
            catch
            {
                try
                {
                    Logger.Warn($"Today is: Unable to format Time here is the raw Version - {DateTime.Now}");
                }
                catch 
                { 
                    Logger.Error($"Today is: Unable to Read Time and Date. Possible due to unsupported Calender settings."); 
                }    
            }

            if (!IsValidGameVersion)
            {
                Logger.Fatal("A fatal error has occurred. An invalid game version was detected!");
                throw new FatalPatchingException("An invalid game version was detected!");
            }
        }
    }
}
