using Oculus.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace QModManager
{
    internal partial class QModPatcher
    {
        internal static string QModBaseDir = Environment.CurrentDirectory.Contains("system32") && Environment.CurrentDirectory.Contains("Windows") ? "ERR" : Path.Combine(Environment.CurrentDirectory, "QMods");
        internal static bool patched = false;

        internal static List<QMod> loadedMods = new List<QMod>();
        internal static List<QMod> foundMods = new List<QMod>();
        internal static List<QMod> sortedMods = new List<QMod>();
        internal static List<QMod> erroredMods = new List<QMod>();

        internal static void Patch()
        {
            try
            {
                if (patched)
                {
                    Console.WriteLine("\nQMOD WARN: Patch method was called multiple times!");
                    return;
                }
                patched = true;

                Hooks.Patch();
                LoadMods();
                Hooks.Update += ShowErroredMods;

                Hooks.OnLoadEnd();
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION CAUGHT!");
                Console.WriteLine(e.ToString());
            }
        }

        #region Mod loading

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

                foundMods.Add(mod); 
            }

            // Add the found mods into the sortedMods list
            sortedMods.AddRange(foundMods);

            // Sort the mods based on their LoadBefore and LoadAfter properties
            // If any mods break (i.e., a loop is found), they are removed from the list so that they aren't loaded
            // And are outputted into the log.
            SortMods();

            // Check if all the mods' dependencies are present
            // If a mod's dependecies aren't present, that mods isn't loaded and it is outputted in the log.
            CheckForDependencies();

            // Finally, load all the mods after sorting and checking for dependencies. 
            // If anything goes wrong during loading, it is outputted in the log.
            LoadAllMods();
        }

        internal static void LoadAllMods()
        {
            string toWrite = "\nLoaded mods:\n";

            List<QMod> loadingErrorMods = new List<QMod>();

            QMod smlHelper = null;

            foreach (QMod mod in sortedMods)
            {
                if (mod != null && !mod.Loaded)
                {
                    if (mod.Id != "SMLHelper")
                    {
                        toWrite += $"- {mod.DisplayName} ({mod.Id})\n";

                        if (!LoadMod(mod))
                        {
                            if (!erroredMods.Contains(mod))
                                erroredMods.Add(mod);

                            if (!loadingErrorMods.Contains(mod))
                                loadingErrorMods.Add(mod);

                            continue;
                        }

                        loadedMods.Add(mod);
                    }
                    else
                    {
                        smlHelper = mod;
                    }
                }
            }

            if (smlHelper != null)
            {
                toWrite += $"- {smlHelper.DisplayName} ({smlHelper.Id})\n";

                if (!LoadMod(smlHelper))
                {
                    if (!erroredMods.Contains(smlHelper))
                        erroredMods.Add(smlHelper);

                    if (!loadingErrorMods.Contains(smlHelper))
                        loadingErrorMods.Add(smlHelper);
                }
                else
                {
                    loadedMods.Add(smlHelper);
                }
            }

            if (loadingErrorMods.Count != 0)
            {
                Console.WriteLine("\nQMOD ERROR: The following mods could not be loaded:\n");

                foreach (QMod mod in loadingErrorMods)
                {
                    Console.WriteLine(mod.Id);
                }
            }

            Console.WriteLine(toWrite);
        }

        internal static bool LoadMod(QMod mod)
        {
            if (mod == null || mod.Loaded) return false;

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
                    return false;
                }
                catch (TargetInvocationException e)
                {
                    Console.WriteLine($"ERROR! Invoking the specified entry method {mod.EntryMethod} failed for mod {mod.Id}");
                    Console.WriteLine(e.ToString());
                    return false;
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR! An unexpected error occurred!");
                    Console.WriteLine(e.ToString());
                    return false;
                }
            }

            mod.Loaded = true;

            return true;
        }

        #endregion

        #region Errored mods

        internal static string erroredModsPrefix = "The following mods could not be loaded: ";

        internal static float timer = 0f;

        internal static void ShowErroredMods()
        {
            timer += Time.deltaTime;
            if (timer < 1) return;
            if (erroredMods.Count <= 0) return;
            string display = erroredModsPrefix;
            for (int i = 0; i < erroredMods.Count; i++)
            {
                display += erroredMods[i].DisplayName;
                if (i + 1 != erroredMods.Count) display += ", ";
            }
            display += ". Check the log for details.";
            ErrorDialog.Show(display);
            Hooks.Update -= ShowErroredMods;
        }

        #endregion
    }
}
