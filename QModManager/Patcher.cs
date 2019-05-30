using Harmony;
using Oculus.Newtonsoft.Json;
using QModManager.API;
using QModManager.API.SMLHelper;
using QModManager.Checks;
using QModManager.Utility;
using SemVer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using Logger = QModManager.Utility.Logger;

namespace QModManager
{
    /// <summary>
    /// The main class which handles all of QModManager's patching
    /// </summary>
    public static class Patcher
    {
        internal static HarmonyInstance harmony;

        internal static readonly Regex IDRegex = new Regex("[^0-9a-z_]", RegexOptions.IgnoreCase);

        internal static string QModBaseDir = Environment.CurrentDirectory.Contains("system32") && Environment.CurrentDirectory.Contains("Windows") ? null : Path.Combine(Environment.CurrentDirectory, "QMods");
        internal static bool patched = false;

        internal static List<QMod> foundMods = new List<QMod>();
        internal static List<QMod> sortedMods = new List<QMod>();
        internal static List<QMod> loadedMods = new List<QMod>();
        internal static List<QMod> erroredMods = new List<QMod>();

        internal static void Patch()
        {
            try
            {
                if (patched)
                {
                    Logger.Warn("Patch method was called multiple times!");
                    return;
                }
                patched = true;

                Logger.Info($"Loading QModManager v{Assembly.GetExecutingAssembly().GetName().Version.ToStringParsed()}...");

                if (QModBaseDir == null)
                {
                    Logger.Fatal("A fatal error has occurred.");
                    Logger.Fatal("There was an error with the QMods directory");
                    Logger.Fatal("Please make sure that you ran Subnautica from Steam/Epic/Discord, and not from the executable file!");
                    return;
                }

                try
                {
                    IOUtilities.LogFolderStructureAsTree();
                }
                catch (Exception e)
                {
                    Logger.Error("There was an error while trying to display the folder structure.");
                    Logger.Exception(e);
                }

                QModHooks.Load();

                PirateCheck.CheckIfPirate(Environment.CurrentDirectory);

                if (!DetectGame()) return;

                PatchHarmony();

                if (NitroxCheck.IsInstalled)
                {
                    Logger.Fatal($"Nitrox was detected!");
                    Dialog.Show("Both QModManager and Nitrox detected. QModManager is not compatible with Nitrox. Please uninstall one of them.", Dialog.Button.disabled, Dialog.Button.disabled, false);
                    return;
                }

                StartLoadingMods();
                ShowErroredMods();

                VersionCheck.Check();

                //QModHooks.Start += PrefabDebugger.Main;

                UpdateSMLHelper();
                Initializer.PostPostInit();

                QModHooks.OnLoadEnd?.Invoke();

                Logger.Info($"Finished loading QModManager. Loaded {loadedMods.Count} mods.");
            }
            catch (Exception e)
            {
                Logger.Error("EXCEPTION CAUGHT!");
                Logger.Exception(e);
            }
        }

        internal static void PatchHarmony()
        {
            harmony = HarmonyInstance.Create("qmodmanager");
            harmony.PatchAll();
            Logger.Debug("Patched!");
        }

        #region Mod loading

        internal static void StartLoadingMods()
        {
            Logger.Info("Started loading mods");

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

            Logger.Debug("Added AssemblyResolve event");

            if (!Directory.Exists(QModBaseDir))
            {
                Logger.Info("QMods directory was not found! Creating...");

                return;
            }

            string[] subDirs = Directory.GetDirectories(QModBaseDir);

            foreach (string subDir in subDirs)
            {
                if (Directory.GetFiles(subDir, "*.dll", SearchOption.TopDirectoryOnly).Length < 1) continue;

                string folderName = new DirectoryInfo(subDir).Name;
                string jsonFile = Path.Combine(subDir, "mod.json");

                if (!File.Exists(jsonFile))
                {
                    Logger.Error($"No \"mod.json\" file found for mod located in folder \"{subDir}\". A template file will be created");
                    File.WriteAllText(jsonFile, JsonConvert.SerializeObject(new QMod()));
                    erroredMods.Add(QMod.CreateFakeQMod(folderName));
                    continue;
                }

                QMod mod = QMod.FromJsonFile(Path.Combine(subDir, "mod.json"));

                if (!QMod.QModValid(mod, folderName))
                {
                    erroredMods.Add(QMod.CreateFakeQMod(folderName));

                    continue;
                }

                if (mod.Enable == false)
                {
                    Logger.Info($"Mod \"{mod.DisplayName}\" is disabled via config, skipping...");

                    continue;
                }

                string modAssemblyPath = Path.Combine(subDir, mod.AssemblyName);

                if (!File.Exists(modAssemblyPath))
                {
                    Logger.Error($"No matching dll found at \"{modAssemblyPath}\" for mod \"{mod.DisplayName}\"");
                    erroredMods.Add(mod);

                    continue;
                }

                mod.LoadedAssembly = Assembly.LoadFrom(modAssemblyPath);
                mod.ModAssemblyPath = modAssemblyPath;
                //mod.MessageReceivers = GetMessageRecievers(mod.LoadedAssembly);

                foundMods.Add(mod);
            }

            // Add the found mods into the sortedMods list
            sortedMods.AddRange(foundMods);

            // Disable mods that are not for the detected game
            // (Disable Subnautica mods if Below Zero is detected and disable Below Zero mods if Subnautica is detected)
            DisableNonApplicableMods();

            // Remove mods with duplicate mod ids if any are found
            RemoveDuplicateModIDs();

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
            List<string> toWrite = new List<string> { "Loaded mods:" };

            List<QMod> loadingErrorMods = new List<QMod>();

            foreach (QMod mod in sortedMods)
            {
                if (mod != null && !mod.Loaded)
                {
                    if (!LoadMod(mod))
                    {
                        if (!erroredMods.Contains(mod))
                            erroredMods.Add(mod);

                        if (!loadingErrorMods.Contains(mod))
                            loadingErrorMods.Add(mod);

                        continue;
                    }
                    else
                    {
                        toWrite.Add($"- {mod.DisplayName} ({mod.Id})");
                        loadedMods.Add(mod);
                    }
                }
            }

            if (loadingErrorMods.Count != 0)
            {
                List<string> write = new List<string> { "The following mods could not be loaded:" };

                foreach (QMod mod in loadingErrorMods)
                {
                    write.Add($"- {mod.DisplayName} ({mod.Id})");
                }

                Logger.Error(write.ToArray());
            }

            Logger.Info(toWrite.ToArray());

            CheckOldHarmony();
        }

        internal static bool LoadMod(QMod mod)
        {
            if (mod == null || mod.Loaded) return false;

            try
            {
                string[] entryMethodSig = mod.EntryMethod.Split('.');
                string entryType = string.Join(".", entryMethodSig.Take(entryMethodSig.Length - 1).ToArray());
                string entryMethod = entryMethodSig[entryMethodSig.Length - 1];

                MethodInfo patchMethod = mod.LoadedAssembly.GetType(entryType).GetMethod(entryMethod);
                patchMethod.Invoke(mod.LoadedAssembly, new object[] { });
            }
            catch (ArgumentNullException e)
            {
                Logger.Error($"Could not parse entry method \"{mod.AssemblyName}\" for mod \"{mod.Id}\"");
                Logger.Exception(e);
                erroredMods.Add(mod);

                return false;
            }
            catch (TargetInvocationException e)
            {
                Logger.Error($"Invoking the specified entry method \"{mod.EntryMethod}\" failed for mod \"{mod.Id}\"");
                Logger.Exception(e);
                return false;
            }
            catch (Exception e)
            {
                Logger.Error($"An unexpected error occurred whilst trying to load mod \"{mod.Id}\"");
                Logger.Exception(e);
                return false;
            }

            if (QModAPI.ErroredMods.Contains(mod?.LoadedAssembly))
            {
                Logger.Error($"Mod \"{mod.Id}\" could not be loaded.");
                QModAPI.ErroredMods.Remove(mod?.LoadedAssembly);
                return false;
            }
            mod.Loaded = true;
            Logger.Info($"Loaded mod \"{mod.Id}\"");

            return true;
        }

        internal static void RemoveDuplicateModIDs()
        {
            List<QMod> duplicateModIDs = new List<QMod>();

            foreach (QMod mod in sortedMods)
            {
                List<QMod> matchingMods = sortedMods.Where((QMod qmod) => qmod.Id == mod.Id).ToList();
                if (matchingMods.Count > 1)
                {
                    foreach (QMod duplicateMod in matchingMods)
                    {
                        if (!duplicateModIDs.Contains(duplicateMod)) duplicateModIDs.Add(duplicateMod);
                        if (!erroredMods.Contains(duplicateMod)) erroredMods.Add(duplicateMod);
                    }
                }
            }

            if (duplicateModIDs.Count > 0)
            {
                List<string> toWrite = new List<string> { $"Multiple mods with the same ID found:" };
                foreach (QMod mod in duplicateModIDs)
                {
                    if (sortedMods.Contains(mod)) sortedMods.Remove(mod);
                    toWrite.Add($"- {mod.DisplayName} ({mod.Id})");
                }

                Logger.Error(toWrite.ToArray());
            }
        }

        internal static void UpdateSMLHelper()
        {
            string oldPath = IOUtilities.Combine(".", "QMods", "Modding Helper");
            if (File.Exists(Path.Combine(oldPath, "SMLHelper.dll")))
                File.Delete(Path.Combine(oldPath, "SMLHelper.dll"));
            if (File.Exists(Path.Combine(oldPath, "mod.json")))
                File.Delete(Path.Combine(oldPath, "mod.json"));

            // To do in #81
        }

        /*
        internal static Dictionary<IQMod, List<MethodInfo>> GetMessageRecievers(Assembly assembly)
        {
            IEnumerable<Type> messageReceivers = assembly.GetTypes()
                           .Where(t => t.IsSubclassOf(typeof(MessageReceiver)));

            IEnumerable<KeyValuePair<IQMod, List<MethodInfo>>> groupedMessageReceivers = messageReceivers
                           .GroupBy(t =>
                           {
                               object instance;
                               try
                               {
                                   instance = t.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
                               }
                               catch
                               {
                                   instance = null;
                               }

                               IQMod From;
                               if (instance == null)
                               {
                                   Logger.Error($"Could not get the targeted QMod from a MessageReceiver in mod \"{QModAPI.GetMod(assembly, true, true).DisplayName}\". The constructor could not be invoked.");
                                   Logger.Warn($"That MessageReceiver has been turned into a GlobalMessageReceiver.");

                                   From = QMod.QModManagerQMod;
                               }
                               else
                               {
                                   try
                                   {
                                       From = t.GetField("From").GetValue(instance) as IQMod;
                                   }
                                   catch
                                   {
                                       From = null;
                                   }
                               }

                               if (From == null)
                               {
                                   Logger.Error($"Could not get the targeted QMod from a MessageReceiver in mod \"{QModAPI.GetMod(assembly, true, true).DisplayName}\". The value of the property \"From\" could not be obtained.");
                                   Logger.Warn($"That MessageReceiver has been turned into a GlobalMessageReceiver.");

                                   From = QMod.QModManagerQMod;
                               }

                               return From;
                           })
                           .Select(g => new KeyValuePair<IQMod, List<MethodInfo>>(g.Key, g.Select(t => t.GetMethod("OnMessageReceived"))
                               .ToList()));

            KeyValuePair<IQMod, List<MethodInfo>> globalMessageReceivers = new KeyValuePair<IQMod, List<MethodInfo>>(QMod.QModManagerQMod, assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(GlobalMessageReceiver)))
                .Select(t => t.GetMethod("OnMessageReceived"))
                .ToList());

            Dictionary<IQMod, List<MethodInfo>> finalReceivers = groupedMessageReceivers.Add(globalMessageReceivers).ToDictionary(k => k.Key, v => v.Value);

            return finalReceivers;
        }
        */

        #endregion

        #region Game detection

        /// <summary>
        /// An enum which contains possible values for <see cref="IQMod.ParsedGame"/>
        /// </summary>
        [Flags]
        public enum Game
        {
            /// <summary>
            /// No game was detected <para/>
            /// In theory, this should never be the case
            /// </summary>
            None = 0b00,
            /// <summary>
            /// Subnautica was detected
            /// </summary>
            Subnautica = 0b01,
            /// <summary>
            /// Below Zero was detected
            /// </summary>
            BelowZero = 0b10,
            /// <summary>
            /// Both games were detected <para/>
            /// In theory, this should never be the case
            /// </summary>
            Both = Subnautica | BelowZero,
        }

        internal static Game game;

        internal static bool DetectGame()
        {
            bool sn = Directory.GetFiles(Environment.CurrentDirectory, "Subnautica.exe", SearchOption.TopDirectoryOnly).Length > 0
                || Directory.GetFiles(Environment.CurrentDirectory, "Subnautica.app", SearchOption.TopDirectoryOnly).Length > 0
                || Directory.GetDirectories(Environment.CurrentDirectory, "Subnautica.app", SearchOption.TopDirectoryOnly).Length > 0;
            bool bz = Directory.GetFiles(Environment.CurrentDirectory, "SubnauticaZero.exe", SearchOption.TopDirectoryOnly).Length > 0
                || Directory.GetFiles(Environment.CurrentDirectory, "SubnauticaZero.app", SearchOption.TopDirectoryOnly).Length > 0
                || Directory.GetDirectories(Environment.CurrentDirectory, "SubnauticaZero.app", SearchOption.TopDirectoryOnly).Length > 0;

            if (sn && !bz)
            {
                Logger.Info("Detected game: Subnautica");
                game = Game.Subnautica;

                return true;
            }
            else if (bz && !sn)
            {
                Logger.Info("Detected game: BelowZero");
                game = Game.BelowZero;

                return true;
            }
            else if (sn && bz)
            {
                Logger.Fatal("A fatal error has occurred.");
                Logger.Fatal("Both Subnautica and Below Zero files detected!");
            }
            else
            {
                Logger.Fatal("A fatal error has occurred.");
                Logger.Fatal("No game executable was found!");
            }
            return false;
        }

        internal static void DisableNonApplicableMods()
        {
            List<QMod> nonApplicableMods = new List<QMod>();
            sortedMods = sortedMods.Where(mod =>
            {
                if (mod.ParsedGame == Game.Both || mod.ParsedGame == game) return true;

                if (!nonApplicableMods.Contains(mod)) nonApplicableMods.Add(mod);
                if (!erroredMods.Contains(mod)) erroredMods.Add(mod);
                return false;
            }).ToList();

            if (nonApplicableMods.Count > 0)
            {
                List<string> toWrite = new List<string> { $"The following {GetOtherGame()} mods were not loaded because {game.ToString()} was detected:" };
                foreach (QMod mod in nonApplicableMods)
                {
                    toWrite.Add($"- {mod.DisplayName} ({mod.Id})");
                }

                Logger.Warn(toWrite.ToArray());
            }
        }

        internal static string GetOtherGame()
        {
            if (game == Game.Subnautica) return "BelowZero";
            else return "Subnautica";
        }

        #endregion

        #region Load order

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
                Logger.Error("There was en error while sorting some mods!", "Please check the 'LoadAfter' and 'LoadBefore' properties of these mods:");

                foreach (List<QMod> list in sortingErrorLoops)
                {
                    string outputStr = "- ";

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
            }
        }

        internal static bool SortMod(QMod mod)
        {
            // Add the mod passed into this method to the chain
            modSortingChain.Add(mod);

            // Get all the duplicates present in the chain
            List<QMod> duplicates = modSortingChain.GroupBy(s => s)
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
                if (currentIndex > index || index == -1) continue;

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
                if (currentIndex < index || index == -1) continue;

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
            Dictionary<QMod, List<string>> missingDependenciesByMod = new Dictionary<QMod, List<string>>();
            Dictionary<QMod, List<KeyValuePair<string, string>>> missingVersionDependenciesByMod = new Dictionary<QMod, List<KeyValuePair<string, string>>>();

            foreach (QMod mod in foundMods)
            {
                List<QMod> presentDependencies = GetPresentDependencies(mod);
                List<string> missingDependencies = GetMissingDependencies(mod, presentDependencies);

                if (missingDependencies.Count > 0)
                {
                    missingDependenciesByMod.Add(mod, missingDependencies);

                    if (!erroredMods.Contains(mod))
                        erroredMods.Add(mod);
                }

                List<QMod> presentVersionDependencies = GetPresentVersionDependencies(mod);
                List<KeyValuePair<string, string>> missingVersionDependencies = GetMissingVersionDependencies(mod, presentVersionDependencies);

                if (missingVersionDependencies.Count > 0)
                {
                    missingVersionDependenciesByMod.Add(mod, missingVersionDependencies);

                    if (!erroredMods.Contains(mod))
                        erroredMods.Add(mod);
                }
            }

            Dictionary<QMod, List<string>> finalDependencies = MergeDependencyDictionaries(missingDependenciesByMod, missingVersionDependenciesByMod);

            if (finalDependencies.Count > 0)
            {
                Logger.Error("The following mods were not loaded due to missing dependencies:");

                foreach (var entry in finalDependencies)
                {
                    if (sortedMods.Contains(entry.Key))
                        sortedMods.Remove(entry.Key);

                    string str = $"- {entry.Key.DisplayName}  (missing: ";

                    foreach (string missingDependencyId in entry.Value)
                    {
                        str += missingDependencyId + ", ";
                    }

                    str = str.Substring(0, str.Length - 2);
                    str += ")";

                    Console.WriteLine(str);
                }
            }
        }

        internal static List<QMod> GetPresentDependencies(QMod mod)
        {
            if (mod == null) return null;

            List<QMod> dependencies = new List<QMod>();

            foreach (string dependencyId in mod.Dependencies)
            {
                foreach (QMod dependencyMod in sortedMods)
                {
                    if (dependencyId == dependencyMod.Id)
                        dependencies.Add(dependencyMod);
                }
            }

            return dependencies;
        }
        internal static List<QMod> GetPresentVersionDependencies(QMod mod)
        {
            if (mod == null) return null;

            List<QMod> dependencies = new List<QMod>();

            foreach (KeyValuePair<string, string> dependency in mod.VersionDependencies)
            {
                if (dependency.Key.Trim() == "QModManager")
                {
                    try
                    {
                        if (dependency.Value.Trim() == QMod.QModManagerQMod.Version.Trim() || dependency.Value.Trim(' ', '=') == QMod.QModManagerQMod.Version.Trim() || Range.IsSatisfied(dependency.Value.Trim(), QMod.QModManagerQMod.Version.Trim(), true))
                            dependencies.Add(QMod.QModManagerQMod);
                    }
                    catch (ArgumentException)
                    {
                        Logger.Warn($"Caught an ArgumentException while trying to parse dependency version range \"{dependency.Value}\" of dependency \"QModManager\" for mod \"{mod.DisplayName}\"");
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"An error occurred while trying to parse version range \"{dependency.Value}\" of dependency \"QModManager\" for mod \"{mod.DisplayName}\"");
                        Logger.Exception(e);
                    }
                    continue;
                }

                foreach (QMod dependencyMod in sortedMods)
                {
                    if (dependency.Key.Trim() == dependencyMod.Id.Trim())
                    {
                        try
                        {
                            if (dependency.Value.Trim() == dependencyMod.Version.Trim() || dependency.Value.Trim(' ', '=') == dependencyMod.Version.Trim() || Range.IsSatisfied(dependency.Value.Trim(), dependencyMod.Version.Trim(), true))
                                dependencies.Add(dependencyMod);
                        }
                        catch (ArgumentException)
                        {
                            Logger.Warn($"Caught an ArgumentException while trying to parse dependency version range \"{dependency.Value}\" of dependency \"{dependencyMod.DisplayName}\" for mod \"{mod.DisplayName}\"");
                        }
                        catch (Exception e)
                        {
                            Logger.Error($"An error occurred while trying to parse version range \"{dependency.Value}\" of dependency \"{dependency.Key}\" for mod \"{mod.DisplayName}\"");
                            Logger.Exception(e);
                        }
                    }
                }
            }

            return dependencies;
        }

        internal static List<string> GetMissingDependencies(QMod mod, List<QMod> presentDependencies)
        {
            if (mod == null) return null;
            if (presentDependencies == null || presentDependencies.Count() == 0) return mod.Dependencies.ToList();

            List<string> dependenciesMissing = new List<string>(mod.Dependencies);

            dependenciesMissing = dependenciesMissing.Where(dep => dep != "QModManager").ToList();
            dependenciesMissing = dependenciesMissing.Where(dep => dep != "SMLHelper").ToList();

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
        internal static List<KeyValuePair<string, string>> GetMissingVersionDependencies(QMod mod, List<QMod> presentDependencies)
        {
            if (mod == null) return null;
            if (presentDependencies == null || presentDependencies.Count() == 0) return mod.VersionDependencies.ToList();

            List<KeyValuePair<string, string>> dependenciesMissing = new List<KeyValuePair<string, string>>(mod.VersionDependencies);

            dependenciesMissing = dependenciesMissing.Where(kvp => kvp.Key != "SMLHelper").ToList();

            foreach (KeyValuePair<string, string> dependency in mod.VersionDependencies)
            {
                foreach (QMod presentDependency in presentDependencies)
                {
                    if (dependency.Key == presentDependency.Id)
                        dependenciesMissing.Remove(dependency);
                }
            }

            return dependenciesMissing;
        }

        internal static Dictionary<QMod, List<string>> MergeDependencyDictionaries(Dictionary<QMod, List<string>> normalDependencies, Dictionary<QMod, List<KeyValuePair<string, string>>> versionDependencies)
        {
            Dictionary<QMod, List<string>> dependencies = new Dictionary<QMod, List<string>>(normalDependencies);

            foreach (KeyValuePair<QMod, List<KeyValuePair<string, string>>> dependency in versionDependencies)
            {
                List<string> parsed = dependency.Value.Select(kvp => kvp.Key + "@" + kvp.Value).ToList();
                if (dependencies.ContainsKey(dependency.Key))
                    dependencies[dependency.Key].AddRange(parsed);
                else
                    dependencies.Add(dependency.Key, parsed);
            }

            return dependencies;
        }

        #endregion

        #region Old harmony detection

        internal static void CheckOldHarmony()
        {
            List<QMod> modsThatUseOldHarmony = new List<QMod>();

            foreach (QMod mod in loadedMods)
            {
                AssemblyName[] references = mod.LoadedAssembly.GetReferencedAssemblies();
                foreach (AssemblyName reference in references)
                {
                    if (reference.FullName == "0Harmony, Version=1.0.9.1, Culture=neutral, PublicKeyToken=null")
                    {
                        modsThatUseOldHarmony.Add(mod);
                        break;
                    }
                }
            }

            if (modsThatUseOldHarmony.Count > 0)
            {
                Logger.Warn($"Some mods are using an old version of harmony! This will NOT cause any problems, but it's not recommended:");
                foreach (QMod mod in modsThatUseOldHarmony)
                {
                    Console.WriteLine($"- {mod.DisplayName} ({mod.Id})");
                }
            }
        }

        #endregion

        #region Errored mods

        internal static void ShowErroredMods()
        {
            if (erroredMods.Count <= 0) return;

            string display = "The following mods could not be loaded: ";

            string[] modsToDisplay = erroredMods.Take(5).Select(mod => mod.DisplayName).ToArray();

            display += string.Join(", ", modsToDisplay);

            if (erroredMods.Count > 5)
                display += $", and {erroredMods.Count - 5} others";

            display += ". Check the log for details.";

            Dialog.Show(display, Dialog.Button.seeLog, Dialog.Button.close, false);
        }

        #endregion
    }
}
