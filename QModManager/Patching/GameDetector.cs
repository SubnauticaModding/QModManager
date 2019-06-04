namespace QModManager.Patching
{
    using System;
    using System.IO;
    using QModManager.API;
    using QModManager.Utility;

    internal class GameDetector
    {
        internal readonly Game CurrentlyRunningGame;

        internal bool IsValidGameRunning => CurrentlyRunningGame == Game.Subnautica || CurrentlyRunningGame == Game.BelowZero;

        internal GameDetector()
        {
            bool isSubnautica = Directory.GetFiles(Environment.CurrentDirectory, "Subnautica.exe", SearchOption.TopDirectoryOnly).Length > 0
                || Directory.GetFiles(Environment.CurrentDirectory, "Subnautica.app", SearchOption.TopDirectoryOnly).Length > 0
                || Directory.GetDirectories(Environment.CurrentDirectory, "Subnautica.app", SearchOption.TopDirectoryOnly).Length > 0;
            bool isBelowZero = Directory.GetFiles(Environment.CurrentDirectory, "SubnauticaZero.exe", SearchOption.TopDirectoryOnly).Length > 0
                || Directory.GetFiles(Environment.CurrentDirectory, "SubnauticaZero.app", SearchOption.TopDirectoryOnly).Length > 0
                || Directory.GetDirectories(Environment.CurrentDirectory, "SubnauticaZero.app", SearchOption.TopDirectoryOnly).Length > 0;

            if (isSubnautica && !isBelowZero)
            {
                Logger.Info("Detected game: Subnautica");
                CurrentlyRunningGame = Game.Subnautica;
            }
            else if (isBelowZero && !isSubnautica)
            {
                Logger.Info("Detected game: BelowZero");
                CurrentlyRunningGame = Game.BelowZero;
            }
            else if (isSubnautica && isBelowZero)
            {
                Logger.Fatal("A fatal error has occurred.", "Both Subnautica and Below Zero files detected!");
                CurrentlyRunningGame = Game.Both;
            }
            else
            {
                Logger.Fatal("A fatal error has occurred.", "No game executable was found!");
                CurrentlyRunningGame = Game.None;
            }
        }
    }
}
