using Oculus.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace QModManager
{
    public class QModPatcher
    {
        internal static string QModBaseDir = Environment.CurrentDirectory.Contains("system32") && Environment.CurrentDirectory.Contains("Windows") ? "ERR" : Path.Combine(Environment.CurrentDirectory, "QMods");
        internal static List<QMod> loadedMods = new List<QMod>();
        internal static List<QMod> foundMods = new List<QMod>();
        internal static LinkedList<QMod> linkedMods = new LinkedList<QMod>();
        internal static bool patched = false;

        public static void Patch()
        {
            try
            {
                LoadMods();
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION CAUGHT!");
                Console.WriteLine(e.ToString());
            }
        }
        internal static void LoadMods()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                FileInfo[] allDlls = new DirectoryInfo(QModBaseDir).GetFiles("*.dll", SearchOption.AllDirectories);
                foreach (FileInfo dll in allDlls)
                {
                    if (args.Name.Contains(Path.GetFileNameWithoutExtension(dll.Name)))
                    {
                        return Assembly.LoadFrom(dll.FullName);
                    }
                }

                return null;
            };

            if (patched) return;
            patched = true;

            if (!Directory.Exists(QModBaseDir))
            {
                Console.WriteLine("QMods directory was not found! Creating...");
                if (QModBaseDir == "ERR")
                {
                    Console.WriteLine("There was an error creating the QMods directory");
                    Console.WriteLine("Please make sure that you ran Subnautica from Steam");
                }
                try
                {
                    Directory.CreateDirectory(QModBaseDir);
                    Console.WriteLine("QMods directory created successfully!");
                }
                catch (Exception e)
                {
                    Console.WriteLine("EXCEPTION CAUGHT!");
                    Console.WriteLine(e.ToString());
                }
                return;
            }

            string[] subDirs = Directory.GetDirectories(QModBaseDir);
            foundMods = new List<QMod>();

            foreach (string subDir in subDirs)
            {
                string jsonFile = Path.Combine(subDir, "mod.json");

                if (!File.Exists(jsonFile))
                {
                    Console.WriteLine($"ERROR! No \"mod.json\" file found in folder \"{subDir}\"");
                    File.WriteAllText(jsonFile, JsonConvert.SerializeObject(new QMod()));
                    Console.WriteLine("A template file was created");
                    continue;
                }

                QMod mod = QMod.FromJsonFile(Path.Combine(subDir, "mod.json"));

                if (mod == null) continue;

                if (mod.Enable == false)
                {
                    Console.WriteLine($"{mod.DisplayName} is disabled via config, skipping");
                    continue;
                }

                string modAssemblyPath = Path.Combine(subDir, mod.AssemblyName);

                if (!File.Exists(modAssemblyPath))
                {
                    Console.WriteLine($"ERROR! No matching dll found at \"{modAssemblyPath}\" for mod \"{mod.DisplayName}\"");
                    continue;
                }

                mod.LoadedAssembly = Assembly.LoadFrom(modAssemblyPath);
                mod.ModAssemblyPath = modAssemblyPath;

                // Backward compatibility for Priority
                if (mod.Priority == "Last" && mod.NewPriority == 0)
                {
                    mod.NewPriority = 1000;
                }
                else if (mod.Priority == "First" && mod.NewPriority == 0)
                {
                    mod.NewPriority = -1000;
                }

                foundMods.Add(mod);
            }

            foundMods.Sort(); // The mods are sorted in the order of their priorities

            linkedMods = new LinkedList<QMod>(foundMods);

            for(int i = 0; i < foundMods.Count; i++)
            {
                modSortingChain.Clear();

                QMod mod = foundMods[i];
                bool success = SortMod(mod);

                if (!success)
                {
                    Console.WriteLine("\nQMOD ERROR: There was an error while sorting mods!");
                    Console.WriteLine("Please check the 'LoadBefore' and 'LoadAfter' properties of the following mods:\n");

                    for (int y = 0; y < modSortingChain.Count - 1 ; y++)
                    {
                        Console.WriteLine(modSortingChain[y].DisplayName);
                    }

                    Console.WriteLine("");

                    return;
                }
            }

            string toWrite = "\nInstalled mods:\n";

            foreach (QMod mod in linkedMods)
            {
                if (mod != null && !mod.Loaded)
                {
                    toWrite += $"- {mod.DisplayName} ({mod.Id})\n";
                    loadedMods.Add(LoadMod(mod));
                }
            }

            Console.WriteLine(toWrite);
        }

        internal static List<QMod> modSortingChain = new List<QMod>();

        internal static bool SortMod(QMod mod)
        {
            modSortingChain.Add(mod);

            List<QMod> duplicateKeys = modSortingChain.GroupBy(x => x)
                            .Where(group => group.Count() > 1)
                            .Select(group => group.Key)
                            .ToList();

            if(duplicateKeys.Count > 0)
            {
                // There's an error!
                // BIG BIG ERROR!
                // DO SOMETHING AHHHHHHHHHHHH
                return false;
            }

            LinkedListNode<QMod> current = linkedMods.Find(mod);

            List<QMod> loadBefore = GetModsToLoadBefore(mod);

            foreach(QMod loadBeforeMod in loadBefore)
            {
                LinkedListNode<QMod> node = linkedMods.Find(loadBeforeMod);
                linkedMods.Remove(node);
                linkedMods.AddAfter(current, node);

                bool success = SortMod(loadBeforeMod);

                if (!success)
                    return false;
            }

            List<QMod> loadAfter = GetModsToLoadAfter(mod);

            foreach(QMod loadAfterMod in loadAfter)
            {
                LinkedListNode<QMod> node = linkedMods.Find(loadAfterMod);
                linkedMods.Remove(node);
                linkedMods.AddBefore(current, node);

                bool success = SortMod(loadAfterMod);

                if (!success)
                    return false;
            }

            return true;
        }

        internal static QMod LoadMod(QMod mod)
        {
            if (mod == null || mod.Loaded) return null;

            if (string.IsNullOrEmpty(mod.EntryMethod))
            {
                Console.WriteLine($"ERROR! No EntryMethod specified for mod {mod.DisplayName}");
            }
            else
            {
                try
                {
                    string[] entryMethodSig = mod.EntryMethod.Split('.');
                    string entryType = string.Join(".", entryMethodSig.Take(entryMethodSig.Length - 1).ToArray());
                    string entryMethod = entryMethodSig[entryMethodSig.Length - 1];

                    MethodInfo qPatchMethod = mod.LoadedAssembly.GetType(entryType).GetMethod(entryMethod);
                    qPatchMethod.Invoke(mod.LoadedAssembly, new object[] { });
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine($"ERROR! Could not parse entry method {mod.AssemblyName} for mod {mod.DisplayName}");
                    Console.WriteLine(e.ToString());
                    return null;
                }
                catch (TargetInvocationException e)
                {
                    Console.WriteLine($"ERROR! Invoking the specified entry method {mod.EntryMethod} failed for mod {mod.Id}");
                    Console.WriteLine(e.ToString());
                    return null;
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR! An unexpected error occurred!");
                    Console.WriteLine(e.ToString());
                    return null;
                }
            }

            mod.Loaded = true;

            return mod;
        }

        internal static List<QMod> GetModsToLoadBefore(QMod mod)
        {
            if (mod == null) return null;

            List<QMod> mods = new List<QMod>();

            foreach(string loadBeforeId in mod.LoadBefore)
            {
                foreach(QMod loadBeforeMod in foundMods)
                {
                    if (loadBeforeId == loadBeforeMod.Id)
                        mods.Add(loadBeforeMod);
                }
            }

            return mods;
        }

        internal static List<QMod> GetModsToLoadAfter(QMod mod)
        {
            if (mod == null) return null;

            List<QMod> mods = new List<QMod>();

            foreach (string loadAfterId in mod.LoadAfter)
            {
                foreach (QMod loadAfterMod in foundMods)
                {
                    if (loadAfterId == loadAfterMod.Id)
                        mods.Add(loadAfterMod);
                }
            }

            return mods;
        }
    }
}
