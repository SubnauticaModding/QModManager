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
            string[] files = Directory.GetFiles(directory);
            for (int i = 1; i <= files.Length; i++)
            {
                FileInfo fileinfo = new FileInfo(files[i - 1]);
                if (i != files.Length)
                {
                    if (fileinfo.Name == "steam_api64.dll")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool IsEpic(string directory)
        {
            foreach (string dir in Directory.GetDirectories(directory))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                if (dirInfo.Name == ".eggstore")
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsMSStore(string directory)
        {
            string[] files = Directory.GetFiles(directory);
            for (int i = 1; i <= files.Length; i++)
            {
                FileInfo fileinfo = new FileInfo(files[i - 1]);
                if (i != files.Length)
                {
                    if (fileinfo.Name == "MicrosoftGame.config")
                    {
                        return true;
                    }
                }
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
