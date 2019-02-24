namespace SMLHelper.V2.Utility
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    internal class EnumTypeCache
    {
        internal int Index;
        internal string Name;
    }

    internal class EnumCacheManager<T> where T : Enum
    {
        internal readonly string EnumTypeName;
        internal readonly int StartingIndex;
        internal bool cacheLoaded = false;

        private List<EnumTypeCache> cacheList = new List<EnumTypeCache>();
        private readonly HashSet<int> BannedIDs;
        private readonly int LargestBannedID;

        private readonly Dictionary<T, EnumTypeCache> customEnumTypes = new Dictionary<T, EnumTypeCache>();
        private readonly Dictionary<string, T> customEnumNames = new Dictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);
        private readonly HashSet<T> HashedKeys = new HashSet<T>();

        public IEnumerable<T> ModdedKeys => HashedKeys;

        public bool TryGetValue(T key, out EnumTypeCache value) => customEnumTypes.TryGetValue(key, out value);

        public bool TryParse(string value, out T type) => customEnumNames.TryGetValue(value, out type);

        public void Add(T value, EnumTypeCache entry)
        {
            HashedKeys.Add(value);
            customEnumTypes.Add(value, entry);
            customEnumNames.Add(entry.Name, value);
        }

        public void Add(KeyValuePair<T, EnumTypeCache> item) => Add(item.Key, item.Value);

        public bool ContainsKey(T key) => HashedKeys.Contains(key);

        internal EnumCacheManager(string enumTypeName, int startingIndex, IEnumerable<int> bannedIDs)
        {
            EnumTypeName = enumTypeName;
            StartingIndex = startingIndex;

            int largestID = 0;
            BannedIDs = new HashSet<int>();
            foreach (int id in bannedIDs)
            {
                BannedIDs.Add(id);
                largestID = Math.Max(largestID, id);
            }

            LargestBannedID = largestID;
        }

        #region Caching

        private string GetCacheDirectoryPath()
        {
            string saveDir = $"./QMods/Modding Helper/{EnumTypeName}Cache";

            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);

            return saveDir;
        }

        private string GetCachePath() => Path.Combine(GetCacheDirectoryPath(), $"{EnumTypeName}Cache.txt");

        internal void LoadCache()
        {
            if (cacheLoaded) return;

            try
            {
                string savePathDir = GetCachePath();

                if (!File.Exists(savePathDir))
                {
                    cacheLoaded = true; // Just so it wont keep calling this over and over again.
                    return;
                }

                string[] allText = File.ReadAllLines(savePathDir);

                foreach (string line in allText)
                {
                    string[] split = line.Split(':');
                    string name = split[0];
                    string index = split[1];

                    var cache = new EnumTypeCache()
                    {
                        Name = name,
                        Index = int.Parse(index)
                    };

                    cacheList.Add(cache);
                }
            }
            catch (Exception exception)
            {
                Logger.Log("Caught exception when reading cache!", LogLevel.Error);
                Logger.Log("Exception message: " + exception.Message, LogLevel.Error);
                Logger.Log("StackTrace: " + Environment.NewLine + exception.StackTrace, LogLevel.Error);
            }

            cacheLoaded = true;
        }

        internal void SaveCache()
        {
            try
            {
                string savePathDir = GetCachePath();
                var stringBuilder = new StringBuilder();

                foreach (KeyValuePair<T, EnumTypeCache> entry in customEnumTypes)
                {
                    cacheList.Add(entry.Value);

                    stringBuilder.AppendLine($"{entry.Value.Name}:{entry.Value.Index}");
                }

                File.WriteAllText(savePathDir, stringBuilder.ToString());
            }
            catch (Exception exception)
            {
                Logger.Log("Caught exception when saving cache!", LogLevel.Error);
                Logger.Log("Exception message: " + exception.Message, LogLevel.Error);
                Logger.Log("StackTrace: " + Environment.NewLine + exception.StackTrace, LogLevel.Error);
            }
        }

        internal EnumTypeCache GetCacheForTypeName(string name)
        {
            LoadCache();

            foreach (EnumTypeCache cache in cacheList)
            {
                if (cache.Name == name)
                    return cache;
            }

            return null;
        }

        internal EnumTypeCache GetCacheForIndex(int index)
        {
            LoadCache();

            foreach (EnumTypeCache cache in cacheList)
            {
                if (cache.Index == index)
                    return cache;
            }

            return null;
        }

        internal int GetLargestIndexFromCache()
        {
            LoadCache();

            int index = StartingIndex;

            foreach (EnumTypeCache cache in cacheList)
            {
                if (cache.Index > index)
                    index = cache.Index;
            }

            return index;
        }

        internal int GetNextFreeIndex()
        {
            LoadCache();

            int freeIndex = GetLargestIndexFromCache() + 1;

            if (BannedIDs != null && BannedIDs.Contains(freeIndex))
            {
                freeIndex = LargestBannedID + 1;
            }

            return freeIndex;
        }

        internal bool IsIndexValid(int index)
        {
            LoadCache();

            int count = 0;

            foreach (EnumTypeCache cache in cacheList)
            {
                if (cache.Index == index)
                    count++;
            }

            return count >= 2 && (!BannedIDs?.Contains(index) ?? false);
        }

        #endregion
    }
}
