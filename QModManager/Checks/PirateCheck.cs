using System;
using System.Collections.Generic;
using System.IO;
using Logger = QModManager.Utility.Logger;

namespace QModManager.Checks
{
    internal static class PirateCheck
    {
        internal static string Steamapi => "steam_api64.dll";
        internal static int Steamapilengh => 220000;


        internal static string folder = Environment.CurrentDirectory;
        internal static bool PirateDetected;

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
            "launcher.bat",
            "chuj.cdx",
        };

        internal static void IsPirate()
        {
            string steamDll = Path.Combine(folder, Steamapi);
            bool steamStore = File.Exists(steamDll);
            if (steamStore)
            {
                FileInfo fileInfo = new FileInfo(steamDll);
                if (fileInfo.Length > Steamapilengh)
                {
                    PirateDetected = true;
                }
            }

            if (!PirateDetected)
            {
                foreach (string file in CrackedFiles)
                {
                    if (File.Exists(Path.Combine(folder, file)))
                    {
                        PirateDetected = true;
                        break;
                    }
                }
            }

            Logger.Info(PirateDetected? "Ahoy, matey! Ye be a pirate!":"Seems Legit.");
        }
    }
}
