using System;
using System.Collections.Generic;
using System.IO;
using QModManager.Patching;

#if SUBNAUTICA_STABLE
using Oculus.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif

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
                    try
                    {
                        FileInfo fileinfo = new FileInfo(files[i - 1]);
                        if (i != files.Length)
                            Console.WriteLine($"{GenerateSpaces(0)}|---- {fileinfo.Name} ({ParseSize(fileinfo.Length)})");
                        else
                            Console.WriteLine($"{GenerateSpaces(0)}`---- {fileinfo.Name} ({ParseSize(fileinfo.Length)})");
                    }
                    catch (Exception e)
                    {
                        if (i != files.Length)
                            Console.WriteLine($"{GenerateSpaces(0)}|---- ERROR ON GETTING FILE INFORMATIONS - {e.Message}");
                        else
                            Console.WriteLine($"{GenerateSpaces(0)}`---- ERROR ON GETTING FILE INFORMATIONS - {e.Message}");
                    }
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
                    try
                    {
                        FileInfo fileinfo = new FileInfo(files[i - 1]);
                        if (fileinfo.Name == "mod.json")
                        {
                            var modjson = JsonConvert.DeserializeObject<QMod>(File.ReadAllText(fileinfo.FullName));
                            if (i != files.Length)
                            {

                                Console.WriteLine($"{GenerateSpaces(spaces + 4)}|---- {fileinfo.Name} [{modjson.Id} v{modjson.Version} by {modjson.Author} for {modjson.Game}] ({ParseSize(fileinfo.Length)})");
                            }
                            else
                            {
                                Console.WriteLine($"{GenerateSpaces(spaces + 4)}`---- {fileinfo.Name} [{modjson.Id} v{modjson.Version} by {modjson.Author} for {modjson.Game}] ({ParseSize(fileinfo.Length)})");
                            }
                        }
                        else
                        {
                            if (i != files.Length)
                                Console.WriteLine($"{GenerateSpaces(spaces + 4)}|---- {fileinfo.Name} ({ParseSize(fileinfo.Length)})");
                            else
                                Console.WriteLine($"{GenerateSpaces(spaces + 4)}`---- {fileinfo.Name} ({ParseSize(fileinfo.Length)})");
                        }
                    }
                    catch (Exception e)
                    {
                        if (i != files.Length)
                            Console.WriteLine($"{GenerateSpaces(spaces + 4)}|---- ERROR ON GETTING FILE INFORMATIONS - {e.Message}");
                        else
                            Console.WriteLine($"{GenerateSpaces(spaces + 4)}`---- ERROR ON GETTING FILE INFORMATIONS - {e.Message}");
                    }
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

        internal static void ChangeModStatustoFile(QMod qmod)
        {
            //Get the Configfile (The mod.json in the Mod Directory)
            string modconfigpath = Path.Combine(Path.GetFullPath(qmod.SubDirectory), "mod.json");
            if (File.Exists(modconfigpath))
            {
                //we doing it with a Dictionary instead of <IQMod> because we have Data in the File that is NOT handles by QMM we would loose this information and that is bad.
                var modconfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(modconfigpath));

                //Modify the Configfile
                foreach (var kvp in modconfig)
                {
                    if (kvp.Key.ToLower() == "enable")
                    {
                        modconfig[kvp.Key] = qmod.Enable;
                        break;
                    }
                }

                //Create a JSON Format so it will be Readable by User in a Text Editor
                Formatting myformat = new Formatting();
                myformat = Formatting.Indented;

                //Save it back to File
                string jsonstr = JsonConvert.SerializeObject(modconfig, myformat);
                try
                {
                    File.WriteAllText(modconfigpath, jsonstr);
                    Logger.Log(Logger.Level.Info, $"IOUtilities - ChangeModStatustoFile - Enabled Status Update for {qmod.Id} was succesful written to: {modconfigpath}");
                }
                catch (Exception ex)
                {
                    Logger.Log(Logger.Level.Error, $"ErrorID:5713/36B - Saving mod.json for Mod {qmod.Id} to {modconfigpath} failed. - Was the File open in a other Program ? Permission Error ? Original Message:\n {ex.Message}");
                }
            }
            else
            {
                Logger.Log(Logger.Level.Error, $"ErrorID:5713/17A - File {modconfigpath} does not exist! - Mod {qmod.Id}");
            }
        }
    }
}
