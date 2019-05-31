using System.Collections.Generic;
using System.IO;
using Logger = QModManager.Utility.Logger;

namespace QModManager.Checks
{
    /// <summary>
    /// Utility class which detects whether or not the game is pirated
    /// </summary>
    public static class PirateCheck
    {
        /// <summary>
        /// Whether or not the game is pirated
        /// </summary>
        public static bool IsPirate { get; internal set; } = false;

        private static void PirateDetected()
        {
            Logger.Warn("Ahoy, matey! Ye be a pirate!");

            IsPirate = true;
        }

        private static readonly HashSet<string> CrackedFiles = new HashSet<string>()
        {
            "steam_api64.cdx",
            "steam_api64.ini",
            "steam_emu.ini",
            "valve.ini",
            "Subnautica_Data/Plugins/steam_api64.cdx",
            "Subnautica_Data/Plugins/steam_api64.ini",
            "Subnautica_Data/Plugins/steam_emu.ini",
        };

        internal static void CheckIfPirate(string folder)
        {
            string steamDll = Path.Combine(folder, "steam_api64.dll");
            if (File.Exists(steamDll))
            {
                FileInfo fileInfo = new FileInfo(steamDll);

                if (fileInfo.Length > 220000)
                {
                    PirateDetected();
                    return;
                }
            }

            foreach (string file in CrackedFiles)
            {
                if (File.Exists(Path.Combine(folder, file)))
                {
                    PirateDetected();
                    return;
                }
            }
        }
    }
}
