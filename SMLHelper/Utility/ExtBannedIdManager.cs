namespace SMLHelper.V2.Utility
{
    using System.IO;
    using System.Collections.Generic;
    using System;
    using System.Collections;

    /// <summary>
    /// This class is tasked with checking external txt files for banned IDs that are not to be isssued when patching in new enum entries.
    /// </summary>
    internal static class ExtBannedIdManager
    {
        private static bool IsInitialized = false;

        private static readonly Dictionary<string, List<int>> BannedIdDictionary = new Dictionary<string, List<int>>();

        private const string BannedIdDirectory = @"./QMods/Modding Helper/RestrictedIDs";

        /// <summary>
        /// Gets the banned ids, reported by the external files, for the specified enum.
        /// </summary>
        /// <param name="enumName">Name of the enum.</param>
        /// <param name="combineWith">Any previously known banned IDs for this enum can be combined into the final list.</param>
        /// <returns>An <see cref="IEnumerable"/> of banned indexes not to be issued for new entries of the specified enum.</returns>
        internal static IEnumerable<int> GetBannedIdsFor(string enumName, IList<int> combineWith)
        {
            if (!IsInitialized)
                LoadFromFiles();

            if (!BannedIdDictionary.ContainsKey(enumName))
                BannedIdDictionary.Add(enumName, new List<int>(combineWith));
            else
                BannedIdDictionary[enumName].AddRange(combineWith);

            return GetBannedIdsFor(enumName);
        }

        /// <summary>
        /// Gets the banned ids, reported by the external files, for the specified enum.
        /// </summary>
        /// <param name="enumName">Name of the enum.</param>
        /// <returns>An <see cref="IEnumerable"/> of banned indexes not to be issued for new entries of the specified enum.</returns>
        internal static IEnumerable<int> GetBannedIdsFor(string enumName)
        {
            if (!IsInitialized)
                LoadFromFiles();

            if (!BannedIdDictionary.ContainsKey(enumName))
                return new int[0]; // No entries

            return BannedIdDictionary[enumName].ToArray();
        }

        private static void LoadFromFiles()
        {
            if (!Directory.Exists(BannedIdDirectory))
            {
                CreateBannedIdDirectory();
                IsInitialized = true; // No folder. No entries.
                return;
            }

            // Check every individual file in the BannedIdDirectory
            string[] files = Directory.GetFiles(BannedIdDirectory);

            foreach (string filePath in files) // An empty directory will skip over this
            {
                // Each file in this directory represents a list of enum IDs that have been patched from outside of SMLHelper.
                // Normally, this means that each file will represent one specific mod.
                // This would also be user configurable so warning must be issued to the user not to alter these files.

                string[] entries = File.ReadAllLines(filePath);

                foreach (string line in entries) // A blank file will skip over this
                {
                    // Each line in the file must define a numeric ID and the name of the enum the entry belongs to.
                    // The format should look like this <numeric_id>:<enum_name>, with the number preceding the name and separated by a colon.                
                    // For example "11110:TechType" would be a valid entry.
                    // For ease of use, whitespace is ignored.

                    string[] components = line.Split(':');

                    int id = -1;

                    if (components.Length != 2 || // Improperly formatter line
                        !int.TryParse(components[0].Trim(), out id)) // Not a numeric ID
                    {
                        LogBadEntry(filePath, line);
                        continue;
                    }

                    string key = components[1].Trim();

                    if (string.IsNullOrEmpty(key) || // Missing key name
                        id < 0) // Not a valid ID
                    {
                        LogBadEntry(filePath, line);
                        continue;
                    }

                    if (!BannedIdDictionary.ContainsKey(key))
                        BannedIdDictionary.Add(key, new List<int>());

                    BannedIdDictionary[key].Add(id);
                }
            }

            foreach (string bannedIdType in BannedIdDictionary.Keys)
            {
                Logger.Log($"{BannedIdDictionary[bannedIdType].Count} retricted IDs were registered for {bannedIdType}.");
            }

            IsInitialized = true;
        }

        private static void CreateBannedIdDirectory()
        {
            try
            {
                Directory.CreateDirectory(BannedIdDirectory);
                Logger.Log("RetrictedIDs folder was not found. Folder created.");
            }
            catch (Exception ex)
            {
                Logger.Log($"RetrictedIDs folder was not found. Failed to create folder.{Environment.NewLine}" +
                           $"        Exception: {ex}");

            }
        }

        private static void LogBadEntry(string filePath, string line)
        {
            Logger.Log($"Badly formatted entry for Retricted IDs{Environment.NewLine}" +
                       $"        File: '{filePath}{Environment.NewLine}'" +
                       $"        Line: '{line}'{Environment.NewLine}" +
                       $"        This entry has been skipped.");
        }

    }
}
