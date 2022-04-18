using System;
using System.Collections.Generic;
using System.IO;
using QModManager.DataStructures;
using Oculus.Newtonsoft.Json;

namespace QModManager.Utility
{
    internal static class IOUtilities
    {
        internal static readonly HashSet<string> BannedFolders = new HashSet<string>()
        {
            ".backups",
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
            "SavesDir",
            "SavesDir2",
        };

        internal static void LogFolderStructureAsTree(string directory = null)
        {
            try
            {
                directory ??= Environment.CurrentDirectory;

                WriteFolderStructure(directory);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        internal static void WriteFolderStructure(string directory)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine($"+ {new DirectoryInfo(directory).Name}");

                foreach (string dir in Directory.GetDirectories(directory))
                {
                    WriteFolderStructureRecursively(dir, 0);
                }

                string[] files = Directory.GetFiles(directory);
                for (int i = 1; i <= files.Length; i++)
                {
                    FileInfo fileinfo = new FileInfo(files[i - 1]);
                    if (i != files.Length)
                        Console.WriteLine($"{GenerateSpaces(0)}|---- {fileinfo.Name} ({ParseSize(fileinfo.Length)})");
                    else
                        Console.WriteLine($"{GenerateSpaces(0)}|---- {fileinfo.Name} ({ParseSize(fileinfo.Length)})");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        internal static void WriteFolderStructureRecursively(string directory, int spaces = 0)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(directory);
                Console.WriteLine($"{GenerateSpaces(spaces)}|---+ {dirInfo.Name}");

                if (BannedFolders.Contains(dirInfo.Name) || BannedFolders.Contains($"{dirInfo.Parent.Name}/{dirInfo.Name}"))
                {
                    Console.WriteLine($"{GenerateSpaces(spaces + 4)}`---- (Folder content not shown)");
                    return;
                }

                foreach (string dir in Directory.GetDirectories(directory))
                {
                    WriteFolderStructureRecursively(dir, spaces + 4);
                }

                string[] files = Directory.GetFiles(directory);
                for (int i = 1; i <= files.Length; i++)
                {
                    FileInfo fileinfo = new FileInfo(files[i - 1]);
                    if (i != files.Length)
                        Console.WriteLine($"{GenerateSpaces(spaces + 4)}|---- {fileinfo.Name} ({ParseSize(fileinfo.Length)})");
                    else
                        Console.WriteLine($"{GenerateSpaces(spaces + 4)}`---- {fileinfo.Name} ({ParseSize(fileinfo.Length)})");
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

        /*
        internal static void ChangeModStatustoFile(SimpleModDataTemplate smdt)
        {
            Logger.Log(Logger.Level.Debug, $"Welcome to the ChangeModStatustoFile Methode");

            //Get the Configfile
            string modconfigpath = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(smdt.PathToAssemblyFile)),"mod.json");
            Logger.Log(Logger.Level.Debug, $"Path {modconfigpath}");
            dynamic modconfigfile = JsonConvert.DeserializeObject(File.ReadAllText(modconfigpath));

            //Modify the Configfile
            Logger.Log(Logger.Level.Debug, $"TESTTESTTESTTESTTEST - {modconfigfile["Enable"]}");
            modconfigfile["Enable"] = smdt.Enabled.ToString();
            Logger.Log(Logger.Level.Debug, $"TESTTESTTESTTESTTEST - {modconfigfile["Enable"]}");

            
            //Save it back
            Formatting myformat = new Formatting();
            myformat = Formatting.Indented;
            string jsonstr= JsonConvert.SerializeObject(modconfigfile, myformat);
            try
            {
                File.WriteAllText(modconfigpath, jsonstr);
                Logger.Log(Logger.Level.Info, "Mod Compare List for Savegame was saved to Mod Folder");
            }
            catch
            {
                Logger.Log(Logger.Level.Error, "ErrorID:5713/31A - Saving Changed Mod Configfile failed");
            }
            
        }
        */
    }
}
