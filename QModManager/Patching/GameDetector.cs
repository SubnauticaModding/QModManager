namespace QModManager.Patching
{
    using System;
    using System.IO;
    using QModManager.API;
    using QModManager.Utility;

    internal class GameDetector
    {
        internal readonly QModGame CurrentlyRunningGame;

        internal bool IsValidGameRunning => CurrentlyRunningGame == QModGame.Subnautica || CurrentlyRunningGame == QModGame.BelowZero;

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
        }
    }
}
