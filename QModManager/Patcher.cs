using Oculus.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using QModManager.Debugger;

namespace QModManager
{
    internal static class QModPatcher
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

                StartLoadingMods();

                Hooks.Update += ShowErroredMods;

                Hooks.Start += PrefabDebugger.Main;

                Hooks.OnLoadEnd();
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION CAUGHT!");
                Console.WriteLine(e.ToString());
            }
        }

        #region Mod loading

        internal static void StartLoadingMods()
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
                else try
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

        #region LoadOrder

        internal static List<QMod> modSortingChain = new List<QMod>();

        internal static void SortMods()
        {
            // Contains all the mods that errored out during the sorting process.
            List<List<QMod>> sortingErrorLoops = new List<List<QMod>>();

            for (int i = 0; i < foundMods.Count; i++)
            {
                // Clear the chain from our main loop.
                modSortingChain.Clear();

                // Sort the current loop mod, recursively.
                QMod mod = foundMods[i];
                bool success = SortMod(mod);

                // If it wasn't a success (that is, something messed up with the order, print it out)
                if (!success)
                {
                    QMod duplicateMod = modSortingChain[modSortingChain.Count - 1];
                    int firstIndex = modSortingChain.IndexOf(duplicateMod);

                    List<QMod> loop = new List<QMod>();

                    for (int j = 0; j < modSortingChain.Count; j++)
                    {
                        if (j >= firstIndex)
                        {
                            loop.Add(modSortingChain[j]);

                            // Add this mod to the list of errored mods
                            // Reason we check if its already in the list is because there is going to be at least 1 duplicate
                            if (!erroredMods.Contains(modSortingChain[j]))
                                erroredMods.Add(modSortingChain[j]);
                        }
                    }

                    sortingErrorLoops.Add(loop);
                }
            }

            if (sortingErrorLoops.Count != 0)
            {
                Console.WriteLine("\nQMOD ERROR: There was en error while sorting the following mods!");
                Console.WriteLine("Please check the 'LoadAfter' and 'LoadBefore' properties of these mods!\n");

                foreach (List<QMod> list in sortingErrorLoops)
                {
                    string outputStr = "";

                    foreach (QMod mod in list)
                    {
                        // Remove it from list to prevent it from being loaded
                        if (sortedMods.Contains(mod))
                            sortedMods.Remove(mod);

                        outputStr += mod.Id + " -> ";
                    }

                    outputStr = outputStr.Substring(0, outputStr.Length - 4);
                    Console.WriteLine(outputStr);
                }

                Console.WriteLine("");
            }
        }

        internal static bool SortMod(QMod mod)
        {
            // Add the mod passed into this method to the chain
            modSortingChain.Add(mod);

            // Get all the duplicates present in the chain
            var duplicates = modSortingChain.GroupBy(s => s)
                .SelectMany(grp => grp.Skip(1))
                .ToList();

            // If there is any duplicate, that means something is going wrong, so exit out
            if (duplicates.Count > 0)
            {
                return false;
            }

            // The index of the mod passed into this method
            int currentIndex = sortedMods.IndexOf(mod);

            // This is a list of mods that need to be loaded after the mod that is passed into this method
            // I say it like this: load this (where this is the mod that is passed in) after these
            // Thus the variable name.
            // If anyone else can come up with better variable names, I'll be all for it
            List<QMod> loadThisAfterThese = GetLoadAfter(mod);

            // Loop through this list
            foreach (QMod loadModAfterThis in loadThisAfterThese)
            {
                // Get the index of the current mod we're looping through.
                int index = sortedMods.IndexOf(loadModAfterThis);

                // If our current mod index (that is, the index of the mod that is passed into this function)
                // is greater than the index of the current mod which we're looping through's index, skip it
                // This means that the mod that is passed in is already positioned after this mod that we're looping through.
                if (currentIndex > index) continue;

                // If we reached this point, that means that the index of the mod that is passed into this function is smaller than the one we're looping through currently
                // This means that the mod that is passed in is positioned before this mod

                // Remove the mod at the index to be able to move it
                sortedMods.RemoveAt(index);

                // Position the new index right at our current index.
                // Why, you ask?
                // This is because of 2 things: 
                // Number 1: Its because of how the Insert method works.
                // If I Insert something at index 2, that something would now take the place of index 2 
                // And what was previously at index 2 would be shifted down to index 3.
                // Number 2: When we remove the element above, everything below it shifts up. 
                // The mod that was passed in (which is currentIndex in this case) is below the element that was removed.
                int newIndex = currentIndex;

                // Insert the mod we're looping through at the new index
                sortedMods.Insert(newIndex, loadModAfterThis);

                // Update the index as it may have updated
                currentIndex = sortedMods.IndexOf(mod);

                // As a safety measure, sort the mod that we just updated, so that it's loadafters and loadbefores dont get messed up.
                bool success = SortMod(loadModAfterThis);

                if (!success)
                    return false;
            }

            // This is a list of mods that need to be loaded before the mod that is passed into this method
            // I say it like this: load this (where this is the mod that is passed in) before these
            // Thus the variable name.
            // If anyone else can come up with better variable names, I'll be all for it
            List<QMod> loadThisBeforeThese = GetLoadBefore(mod);

            // Loop through this list
            foreach (QMod loadAfter in loadThisBeforeThese)
            {
                // Get the current index
                int index = sortedMods.IndexOf(loadAfter);

                // If the index of the mod that is passed in is smaller than the current loop mod, skip it.
                // This means that the mod that is passed in is already positioned before the current loop mod.
                if (currentIndex < index) continue;

                // Remove the mod from the list at this index
                sortedMods.RemoveAt(index);

                // Position the new index right at the current index
                // "Why aren't you adding 1 to currentIndex to put it after currentIndex?", you ask
                // This is because of the RemoveAt method call right above. It shifts the entire list back by 1, for every element after the index we assigned it to
                // So if we remove the element at 2 (0-based), element 3 would become element 2, element 4 would become element 3, so on
                // So when we remove the element at the index, everything shifts up (at least, for the context of currentIndex, since currentIndex > index we're removing)
                // That is why we don't add 1.
                int newIndex = currentIndex;

                // Insert the current loop mod into the new index
                sortedMods.Insert(newIndex, loadAfter);

                // Update the index of the mod that is passed in, since it may have shifted.
                currentIndex = sortedMods.IndexOf(mod);

                // As a safety measure, sort the mod that we just updated, so that it's loadafters and loadbefores dont get messed up.
                bool success = SortMod(loadAfter);

                if (!success)
                    return false;
            }

            return true;
        }

        internal static List<QMod> GetLoadBefore(QMod mod)
        {
            if (mod == null) return null;

            List<QMod> mods = new List<QMod>();

            foreach (string loadBeforeId in mod.LoadBefore)
            {
                foreach (QMod loadBeforeMod in foundMods)
                {
                    if (loadBeforeId == loadBeforeMod.Id)
                        mods.Add(loadBeforeMod);
                }
            }

            return mods;
        }

        internal static List<QMod> GetLoadAfter(QMod mod)
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

        #endregion

        #region Dependencies

        internal static void CheckForDependencies()
        {
            // Check if all mods have dependencies present
            Dictionary<QMod, List<string>> missingDependenciesByMod = new Dictionary<QMod, List<string>>();

            foreach (QMod mod in foundMods)
            {
                List<QMod> presentDependencies = GetPresentDependencies(mod);
                List<string> missingDependencies = GetMissingDependencies(mod, presentDependencies);

                if (missingDependencies.Count != 0)
                {
                    missingDependenciesByMod.Add(mod, missingDependencies);

                    // Add this mod to the list of errored mods
                    if (!erroredMods.Contains(mod))
                        erroredMods.Add(mod);
                }
            }

            // There are missing dependencies! Output them!
            if (missingDependenciesByMod.Count != 0)
            {
                Console.WriteLine("\nQMOD ERROR: The following mods were not loaded due to missing dependencies!\n");

                foreach (var entry in missingDependenciesByMod)
                {
                    // Remove the mod from the sortedMods list to stop it from loading
                    if (sortedMods.Contains(entry.Key))
                        sortedMods.Remove(entry.Key);

                    // Build the string to be displayed for this mod
                    string str = entry.Key.DisplayName + " (missing: ";

                    foreach (string missingDependencyId in entry.Value)
                    {
                        str += missingDependencyId + ", ";
                    }

                    // Remove the ", " characters at the end of the string
                    str = str.Substring(0, str.Length - 2);
                    str += ")";

                    Console.WriteLine(str);
                }

                Console.WriteLine("");
            }
        }

        internal static List<QMod> GetPresentDependencies(QMod mod)
        {
            if (mod == null) return null;

            List<QMod> dependencies = new List<QMod>();

            foreach (string dependencyId in mod.Dependencies)
            {
                foreach (QMod dependencyMod in foundMods)
                {
                    if (dependencyId == dependencyMod.Id)
                        dependencies.Add(dependencyMod);
                }
            }

            return dependencies;
        }

        internal static List<string> GetMissingDependencies(QMod mod, IEnumerable<QMod> presentDependencies)
        {
            if (mod == null) return null;
            if (presentDependencies == null || presentDependencies.Count() == 0) return mod.Dependencies.ToList();

            List<string> dependenciesMissing = new List<string>(mod.Dependencies);

            foreach (string dependencyId in mod.Dependencies)
            {
                foreach (QMod presentDependency in presentDependencies)
                {
                    if (dependencyId == presentDependency.Id)
                        dependenciesMissing.Remove(dependencyId);
                }
            }

            return dependenciesMissing;
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
            Dialog.Show(display);
            Hooks.Update -= ShowErroredMods;
        }

        #endregion

        #region NUnit tests

        internal static void ClearModLists()
        {
            loadedMods.Clear();
            foundMods.Clear();
            sortedMods.Clear();
            erroredMods.Clear();
        }

        #endregion
    }
}
