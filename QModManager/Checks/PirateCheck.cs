using System.Collections.Generic;
using System.IO;
using Logger = QModManager.Utility.Logger;

namespace QModManager.Checks
{
    internal static class PirateCheck
    {
        private const int FileSize = 220000;
      
        private static void PirateDetected()
        {
            Logger.Warn("Ahoy, matey! Ye be a pirate!");
        }
        
        private static readonly HashSet<string> CrackedFiles = new HashSet<string>()
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

        internal static void IsPirate(string folder)
        {
            var steamDll = Path.Combine(folder, "steam_api64.dll");
            if (File.Exists(steamDll))
            {
                FileInfo dllInfo = new FileInfo(steamDll);
                if (dllInfo.Length > FileSize)
                {
                    FileChecking(folder);
                }
            }
        }
        
        private static void FileChecking(string folder)
        {
            foreach (var file in CrackedFiles) {
                if (File.Exists(Path.Combine(folder, file)))
                {
                    PirateDetected();
                    return;
                }
            }
        }
    }
}
