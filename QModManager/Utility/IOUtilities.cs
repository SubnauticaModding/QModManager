using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QModManager.Utility
{
    internal static class IOUtilities
    {
        internal static readonly HashSet<string> BannedFolders = new HashSet<string>()
        {
            ".git",
            ".svn",
            "OST",
            "AssetBundles",
            "MonoBleedingEdge",
            "SNAppData",
            "SNUnmanagedData",
            "Subnautica_Data",
            "SubnauticaZero_Data",
            "_CommonRedist",
            "steam_shader_cache",
        };

        internal static void LogFolderStructureAsTree(string directory = null)
        {
            try
            {
                directory ??= Environment.CurrentDirectory;

                GenerateFolderStructure(directory);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        internal static void GenerateFolderStructure(string directory)
        {
            var builder = new StringBuilder();

            try
            {
                builder.AppendLine();
                builder.AppendLine($"+ {new DirectoryInfo(directory).Name}");

                Logger.Info(builder.ToString());
                builder.Clear();
                foreach (string dir in Directory.GetDirectories(directory))
                {
                    GetFolderStructureRecursively(builder, dir, 0);
                    Logger.Info(builder.ToString());
                    builder.Clear();
                }

                string[] files = Directory.GetFiles(directory);
                for (int i = 1; i <= files.Length; i++)
                {
                    FileInfo fileinfo = new FileInfo(files[i - 1]);
                    if (i != files.Length)
                        builder.AppendLine($"{GenerateSpaces(0)}|---- {fileinfo.Name} ({ParseSize(fileinfo.Length)})");
                    else
                        builder.AppendLine($"{GenerateSpaces(0)}|---- {fileinfo.Name} ({ParseSize(fileinfo.Length)})");
                }

                builder.AppendLine();
                Logger.Info(builder.ToString());
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        internal static void GetFolderStructureRecursively(StringBuilder builder, string directory, int spaces = 0)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(directory);
                builder.AppendLine($"{GenerateSpaces(spaces)}|---+ {dirInfo.Name}");

                if (BannedFolders.Contains(dirInfo.Name) || BannedFolders.Contains($"{dirInfo.Parent.Name}/{dirInfo.Name}"))
                {
                    builder.AppendLine($"{GenerateSpaces(spaces + 4)}`---- (Folder content not shown)");
                    return;
                }

                foreach (string dir in Directory.GetDirectories(directory))
                {
                    GetFolderStructureRecursively(builder, dir, spaces + 4);
                }

                string[] files = Directory.GetFiles(directory);
                for (int i = 1; i <= files.Length; i++)
                {
                    FileInfo fileinfo = new FileInfo(files[i - 1]);
                    if (i != files.Length)
                        builder.AppendLine($"{GenerateSpaces(spaces + 4)}|---- {fileinfo.Name} ({ParseSize(fileinfo.Length)})");
                    else
                        builder.AppendLine($"{GenerateSpaces(spaces + 4)}`---- {fileinfo.Name} ({ParseSize(fileinfo.Length)})");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        internal static string ParseSize(long lsize)
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

        internal static string GenerateSpaces(int spaces)
        {
            string s = "";
            for (int i = 1; i <= spaces; i += 4)
                s += "|   ";
            return s;
        }
    }
}
