namespace QModManager.Patching
{
    using System;
    using System.IO;
    using Checks;

    internal class StoreDetector
    {
        internal static string GetUsedGameStore()
        {
            string directory = null;

            try
            {
                directory ??= Environment.CurrentDirectory;
            }
            catch
            {
                return "Error on getting Store";
            }

            if (NonValidStore(directory))
            {
                return "free Store";
            }
            else if (IsSteam(directory))
            {
                return "Steam";
            }
            else if (IsEpic(directory))
            {
                return "Eic Games";
            }
            else if (IsMSStore(directory))
            {
                return "MSStore";
            }
            else
            {
                return "was not able to identify Store";
            }
        }

        internal static bool IsSteam(string directory)
        {
            string checkfile = Path.Combine(directory, "steam_api64.dll");
            if (File.Exists(checkfile))
            {
                return true;
            }
            return false;
        }

        internal static bool IsEpic(string directory)
        {
            string checkfolder = Path.Combine(directory, ".eggstore");
            if (Directory.Exists(checkfolder)
            {
                return true;
            }
            return false;
        }

        internal static bool IsMSStore(string directory)
        {
            string checkfile = Path.Combine(directory, "MicrosoftGame.config");
            if (File.Exists(checkfile))
            {
                return true;
            }
            return false;
        }

        internal static bool NonValidStore(string folder)
        {
            string steamDll = Path.Combine(folder, PirateCheck.Steamapi);
            if (File.Exists(steamDll))
            {
                FileInfo fileInfo = new FileInfo(steamDll);

                if (fileInfo.Length > PirateCheck.Steamapilengh)
                {
                    return true;
                }
            }

            foreach (string file in PirateCheck.CrackedFiles)
            {
                if (File.Exists(Path.Combine(folder, file)))
                {
                    return false;
                }
            }
            return false;
        }
    }
}
