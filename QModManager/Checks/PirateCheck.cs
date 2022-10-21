using System;
using System.Collections.Generic;
using System.IO;
using Logger = QModManager.Utility.Logger;

namespace QModManager.Checks
{
    internal static class PirateCheck
    {
        internal const string SteamApiName = "steam_api64.dll";
        internal const int SteamApiLength = 220000;
        internal static bool PirateDetected;

        private static readonly string _folder = Environment.CurrentDirectory;

        internal static readonly HashSet<string> CrackedFiles = new HashSet<string>()
        {
            "steam_api64.cdx",
            "steam_api64.ini",
            "steam_emu.ini",
            "valve.ini",
            "SmartSteamEmu.ini",
            "Subnautica_Data/Plugins/steam_api64.cdx",
            "Subnautica_Data/Plugins/steam_api64.ini",
            "Subnautica_Data/Plugins/steam_emu.ini",
            "Profile/SteamUserID.cfg",
            "Profile/Stats/Achievements.Bin",
            "Profile/VALVE/SteamUserID.cfg",
            "Profile/VALVE/Stats/Achievements.Bin",
            "launcher.bat",
            "chuj.cdx",
        };

        internal static void IsPirate()
        {
            string steamDll = Path.Combine(_folder, SteamApiName);
            bool steamStore = File.Exists(steamDll);
            if (steamStore)
            {
                FileInfo fileInfo = new FileInfo(steamDll);
                if (fileInfo.Length > SteamApiLength)
                {
                    PirateDetected = true;
                }
            }

            if (!PirateDetected)
            {
                foreach (string file in CrackedFiles)
                {
                    if (File.Exists(Path.Combine(_folder, file)))
                    {
                        PirateDetected = true;
                        break;
                    }
                }
            }

            Logger.Info(PirateDetected ? "Ahoy, matey! Ye be a pirate!" : "Seems legit.");
        }
    }
}
