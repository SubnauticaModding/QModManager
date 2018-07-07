namespace SMLHelper.V2.Utility
{
    using System.IO;
    using System.Collections.Generic;
    using System;

    internal static class ExtBannedIdManager
    {
        private static bool IsInitialized = false;

        private static readonly Dictionary<string, List<int>> BannedIdDictionary = new Dictionary<string, List<int>>();

        private const string BannedIdDirectory = @"./QMods/Modding Helper/RestrictedIDs/";

        internal static IEnumerable<int> GetBannedIdsFor(string keyName, List<int> combineWith)
        {
            if (!IsInitialized)
                LoadFromFiles();

            if (!BannedIdDictionary.ContainsKey(keyName))
                BannedIdDictionary.Add(keyName, new List<int>(combineWith));
            else
                BannedIdDictionary[keyName].AddRange(combineWith);

            return GetBannedIdsFor(keyName);
        }

        internal static IEnumerable<int> GetBannedIdsFor(string keyName)
        {
            if (!IsInitialized)
                LoadFromFiles();

            if (!BannedIdDictionary.ContainsKey(keyName))
                return new int[0]; // No entries

            return BannedIdDictionary[keyName].ToArray();
        }

        private static void LoadFromFiles()
        {
            if (!Directory.Exists(BannedIdDirectory))
            {
                Directory.CreateDirectory(BannedIdDirectory);                
                IsInitialized = true; // No folder. No entries.
                Logger.Log("RetrictedIDs folder was not found. Folder created.");
                return;
            }

            string[] files = Directory.GetFiles(BannedIdDirectory);
            
            foreach (string filePath in files) // An empty directory will skip over this
            {
                string[] entries = File.ReadAllLines(filePath);

                foreach (string line in entries) // A blank file will skip over this
                {
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
                Logger.Log($"{BannedIdDictionary[bannedIdType].Count} retricted IDs were register for {bannedIdType}.");
            }            

            IsInitialized = true;
        }

        private static void LogBadEntry(string filePath, string line)
        {
            Logger.Log($"Badly formatter entry for Retricted IDs{Environment.NewLine}" +
                       $"        File: '{filePath}{Environment.NewLine}'" +
                       $"        Line: '{line}'{Environment.NewLine}" +
                       $"        This entry has been skipped.");
        }

    }
}
