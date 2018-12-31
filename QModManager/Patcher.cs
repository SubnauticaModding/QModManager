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
        internal static List<QMod> loadedMods;
        internal static List<QMod> foundMods;
        internal static List<QMod> sortedMods;
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

                foundMods.Add(mod);
            }

            sortedMods = new List<QMod>(foundMods);

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
                    Console.WriteLine("\nQMOD ERROR: There was an error while sorting mods!");
                    Console.WriteLine("Please check the 'LoadBefore' and 'LoadAfter' properties of the following mods:\n");

                    // Last element of the list will always be a duplicate, so no point to include it
                    for (int y = 0; y < modSortingChain.Count - 1; y++)
                    {
                        Console.WriteLine(modSortingChain[y].DisplayName);
                    }

                    Console.WriteLine("");

                    return;
                }
            }

            string toWrite = "\nInstalled mods:\n";
            loadedMods = new List<QMod>();

            QMod smlHelper = null;

            foreach (QMod mod in sortedMods)
            {
                if (mod != null && !mod.Loaded && mod.Id != "SMLHelper")
                {
                    toWrite += $"- {mod.DisplayName} ({mod.Id})\n";
                    loadedMods.Add(LoadMod(mod));
                }
                else if(mod.Id == "SMLHelper")
                {
                    smlHelper = mod;
                }
            }

            toWrite += $"- {smlHelper.DisplayName} ({smlHelper.Id})\n";
            loadedMods.Add(LoadMod(smlHelper));

            Console.WriteLine(toWrite);
        }

        internal static List<QMod> modSortingChain = new List<QMod>();

        internal static bool SortMod(QMod mod, bool force = false)
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

            //Console.WriteLine("Mod id: " + mod.Id + " Index: " + currentIndex);

            // This is a list of mods that need to be loaded after the mod that is passed into this method
            // I say it like this: load this (where this is the mod that is passed in) after these
            // Thus the variable name.
            // If anyone else can come up with better variable names, I'll be all for it
            List<QMod> loadThisAfterThese = GetModsToLoadAfter(mod);

            // Loop through this list
            foreach (QMod loadModAfterThis in loadThisAfterThese)
            {
                // Get the index of the current mod we're looping through.
                int index = sortedMods.IndexOf(loadModAfterThis);

                //Console.WriteLine("Load This After This Mod: " + loadModAfterThis.Id + " Index: " + index);

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

                //Console.WriteLine("Load This After This Mod: " + loadModAfterThis.Id + " NewIndex: " + newIndex);

                // Insert the mod we're looping through at the new index
                sortedMods.Insert(newIndex, loadModAfterThis);

                // Update the index as it may have updated
                currentIndex = sortedMods.IndexOf(mod);

                //int indexNow = sortedMods.IndexOf(loadModAfterThis);
                //Console.WriteLine("Load This After This Mod: " + loadModAfterThis.Id + " IndexNow: " + indexNow);

                // As a safety measure, sort the mod that we just updated, so that it's loadafters and loadbefores dont get messed up.
                bool success = SortMod(loadModAfterThis, true);

                if (!success)
                    return false;
            }

            // This is a list of mods that need to be loaded before the mod that is passed into this method
            // I say it like this: load this (where this is the mod that is passed in) before these
            // Thus the variable name.
            // If anyone else can come up with better variable names, I'll be all for it
            List<QMod> loadThisBeforeThese = GetModsToLoadBefore(mod);

            // Loop through this list
            foreach (QMod loadAfter in loadThisBeforeThese)
            {
                // Get the current index
                int index = sortedMods.IndexOf(loadAfter);
                //Console.WriteLine("Load This Before This Mod: " + loadAfter.Id + " Index: " + index);

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

                //Console.WriteLine("Load This Before This Mod: " + loadAfter.Id + " NewIndex: " + newIndex);

                // Insert the current loop mod into the new index
                sortedMods.Insert(newIndex, loadAfter);

                // Update the index of the mod that is passed in, since it may have shifted.
                currentIndex = sortedMods.IndexOf(mod);

                //int indexNow = sortedMods.IndexOf(loadAfter);
                //Console.WriteLine("Load This Before This Mod: " + loadAfter.Id + " IndexNow: " + indexNow);
                
                // As a safety measure, sort the mod that we just updated, so that it's loadafters and loadbefores dont get messed up.
                bool success = SortMod(loadAfter, true);

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
