﻿namespace QModManager.Utility
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal static class IOUtilities
    {
        #region Folder Structure

        private static readonly HashSet<string> BannedFolders = new HashSet<string>()
        {
            ".git",
            "OST",
            "AssetBundles",
            "MonoBleedingEdge",
            "SNAppData",
            "SNUnmanagedData",
            "Subnautica_Data",
            "_CommonRedist",
        };

        internal static string GetFolderStructureAsTree(string directory = null)
        {
            try
            {
                directory = directory ?? Environment.CurrentDirectory;

                return GenerateFolderStructure(directory) + "\n";
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static string GenerateFolderStructure(string directory)
        {
            try
            {
                string toWrite = $"+ {new DirectoryInfo(directory).Name}\n";

                foreach (string dir in Directory.GetDirectories(directory))
                {
                    toWrite += GetFolderStructureRecursively(dir, 0);
                }

                string[] files = Directory.GetFiles(directory);
                for (int i = 1; i <= files.Length; i++)
                {
                    FileInfo fileinfo = new FileInfo(files[i - 1]);
                    if (i != files.Length)
                        toWrite += $"{GenerateSpaces(0)}|---- {fileinfo.Name} ({ParseSize(fileinfo.Length)})\n";
                    else 
                        toWrite += $"{GenerateSpaces(0)}`---- {fileinfo.Name} ({ParseSize(fileinfo.Length)})\n";
                }

                return toWrite;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static string GetFolderStructureRecursively(string directory, int spaces = 0)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(directory);
                string toWrite = $"{GenerateSpaces(spaces)}|---+ {dirInfo.Name}\n";

                if (BannedFolders.Contains(dirInfo.Name) || BannedFolders.Contains($"{dirInfo.Parent.Name}/{dirInfo.Name}"))
                {
                    toWrite += $"{GenerateSpaces(spaces + 4)}`---- ({GetFileCountRecursively(directory)} elements not shown...)\n";
                    return toWrite;
                }

                foreach (string dir in Directory.GetDirectories(directory))
                {
                    toWrite += GetFolderStructureRecursively(dir, spaces + 4);
                }

                string[] files = Directory.GetFiles(directory);
                for (int i = 1; i <= files.Length; i++)
                {
                    FileInfo fileinfo = new FileInfo(files[i - 1]);
                    if (i != files.Length)
                        toWrite += $"{GenerateSpaces(spaces + 4)}|---- {fileinfo.Name} ({ParseSize(fileinfo.Length)})\n";
                    else
                        toWrite += $"{GenerateSpaces(spaces + 4)}`---- {fileinfo.Name} ({ParseSize(fileinfo.Length)})\n";
                }

                return toWrite;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static string ParseSize(long lsize)
        {
            string[] units = new[] { "B", "KB", "MB", "GB" };
            float size = lsize;
            int unit = 0;

            while (size > 1024)
            {
                unit++;
                size /= 1024;
            }

            string number = size.ToString("F2");
            number.TrimEnd('0');
            number.TrimEnd('.');

            return number + units[unit];
        }

        private static string GenerateSpaces(int spaces)
        {
            string s = "";
            for (int i = 1; i <= spaces; i += 4)
                s += "|   ";
            return s;
        }

        private static int GetFileCountRecursively(string directory)
        {
            int c = 0;
            foreach (string file in Directory.GetFiles(directory)) c++;
            foreach (string dir in Directory.GetDirectories(directory)) c += GetFileCountRecursively(dir);
            return c;
        }

        #endregion

        internal static void NormalizeQMods(string qmodsDir)
        {
            string[] folders = Directory.GetDirectories(qmodsDir);
            foreach (string folder in folders)
            {
                try
                {
                    if (File.Exists(Path.Combine(folder, "mod.json"))) continue;

                    Logger.Debug("Found folder with no mod.json: \"" + folder + "\"");

                    string[] subfolders = Directory.GetDirectories(folder);

                    foreach (string subfolder in subfolders)
                    {
                        try
                        {
                            if (!File.Exists(Path.Combine(subfolder, "mod.json"))) continue;

                            string newPath = Path.Combine(qmodsDir, Path.GetFileName(subfolder));

                            Logger.Info("Moving \"" + subfolder + "\" to \"" + newPath + "\"");

                            Directory.Move(subfolder, newPath);
                        }
                        catch (Exception e)
                        {
                            Logger.Error($"There was an error while attempting to normalize folder structure for folder: \"{subfolder}\"");
                            Logger.Exception(e);
                        }
                    }

                    string[] newSubfolders = Directory.GetFiles(folder).Concat(Directory.GetDirectories(folder)).ToArray();
                    if (newSubfolders.Length == 0) Directory.Delete(folder);
                }
                catch (Exception e)
                {
                    Logger.Error($"There was an error while attempting to normalize folder structure for folder: \"{folder}\"");
                    Logger.Exception(e);
                }
            }
        }
    }
}
