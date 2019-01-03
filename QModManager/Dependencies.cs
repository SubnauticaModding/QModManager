using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QModManager
{
    internal partial class QModPatcher
    {
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
    }
}
