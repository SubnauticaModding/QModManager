using System;
using System.Collections.Generic;
using System.IO;

namespace QModManager.Utility
{
    /// <summary>
    /// Utilities for files and paths
    /// </summary>
    public static class IOUtilities
    {
        /// <summary>
        /// Works like <see cref="Path.Combine(string, string)"/>, but can have more than 2 paths
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="rest"></param>
        /// <returns></returns>
        public static string Combine(string one, string two, params string[] rest)
        {
            string path = Path.Combine(one, two);

            foreach (string str in rest)
            {
                path = Path.Combine(path, str);
            }

            return path;
        }

        #region Folder structure

        internal static void LogFolderStructureAsTree(string directory = null)
        {
            Logger.Info($"Folder structure:");
            Console.WriteLine(GetFolderStructureAsTree(directory));
            Console.WriteLine();
        }

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
            "Subnautica_Data/Mono",
            "Subnautica_Data/Plugins",
            "Subnautica_Data/Resources",
            "Subnautica_Data/StreamingAssets",
        };

        private static string GetFolderStructureAsTree(string directory = null)
        {
            try
            {
                directory = directory ?? Environment.CurrentDirectory;

                return GenerateFolderStructure(directory) + Environment.NewLine;
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
                string toWrite = $"+ {new DirectoryInfo(directory).Name}{Environment.NewLine}";

                foreach (string dir in Directory.GetDirectories(directory))
                {
                    toWrite += GetFolderStructureRecursively(dir, 0);
                }

                string[] files = Directory.GetFiles(directory);
                for (int i = 1; i <= files.Length; i++)
                {
                    FileInfo fileinfo = new FileInfo(files[i - 1]);
                    if (i != files.Length)
                        toWrite += $"{GenerateSpaces(0)}|---- {fileinfo.Name} ({ParseSize(fileinfo.Length)}){Environment.NewLine}";
                    else 
                        toWrite += $"{GenerateSpaces(0)}`---- {fileinfo.Name} ({ParseSize(fileinfo.Length)}){Environment.NewLine}";
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
                string toWrite = $"{GenerateSpaces(spaces)}|---+ {dirInfo.Name}{Environment.NewLine}";

                if (BannedFolders.Contains(dirInfo.Name) || BannedFolders.Contains($"{dirInfo.Parent.Name}/{dirInfo.Name}"))
                {
                    toWrite += $"{GenerateSpaces(spaces + 4)}`---- ({GetFileCountRecursively(directory)} elements not shown...){Environment.NewLine}";
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
                        toWrite += $"{GenerateSpaces(spaces + 4)}|---- {fileinfo.Name} ({ParseSize(fileinfo.Length)}){Environment.NewLine}";
                    else
                        toWrite += $"{GenerateSpaces(spaces + 4)}`---- {fileinfo.Name} ({ParseSize(fileinfo.Length)}){Environment.NewLine}";
                }

                return toWrite;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static int GetFileCountRecursively(string directory)
        {
            int c = 0;
            foreach (string file in Directory.GetFiles(directory)) c++;
            foreach (string dir in Directory.GetDirectories(directory)) c += GetFileCountRecursively(dir);
            return c;
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

        #endregion
    }
}
