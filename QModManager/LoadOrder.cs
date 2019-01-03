using System;
using System.Collections.Generic;
using System.Linq;

namespace QModManager
{
    internal partial class QModPatcher
    {
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
    }
}
