using System;
using System.IO;
using QModManager.Checks;

namespace QModManager.Patching
{
    internal class StoreDetector
    {
        internal static string GetUsedGameStore()
        {
            var directory = Environment.CurrentDirectory;;
            

            if (NonValidStore(directory))
            {
                return "free Store";
            }

            if (IsSteam(directory))
            {
                return "Steam";
            }

            if (IsEpic(directory))
            {
                return "Epic Games";
            }

            if (IsMSStore(directory))
            {
                return "MSStore";
            }

            return "was not able to identify Store";
        }

        private static bool IsSteam(string directory)
        {
            string checkfile = Path.Combine(directory, "steam_api64.dll");
            if (File.Exists(checkfile))
            {
                return true;
            }
            return false;
        }

        private static bool IsEpic(string directory)
        {
            string checkfolder = Path.Combine(directory, ".eggstore");
            if (Directory.Exists(checkfolder))
            {
                return true;
            }
            return false;
        }

        private static bool IsMSStore(string directory)
        {
            string checkfile = Path.Combine(directory, "MicrosoftGame.config");
            if (File.Exists(checkfile))
            {
                return true;
            }
            return false;
        }

        private static bool NonValidStore(string folder)
        {
            string steamDll = Path.Combine(folder, PirateCheck.SteamApiName);
            if (File.Exists(steamDll))
            {
                FileInfo fileInfo = new FileInfo(steamDll);

                if (fileInfo.Length > PirateCheck.SteamApiLength)
                {
                    return true;
                }
            }

            foreach (string file in PirateCheck.CrackedFiles)
            {
                if (File.Exists(Path.Combine(folder, file)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
