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

    internal class EnumCacheManager<T>
    {
        internal readonly string EnumTypeName;
        internal readonly int StartingIndex;
        internal bool cacheLoaded = false;

        private List<EnumTypeCache> cacheList = new List<EnumTypeCache>();
        private readonly HashSet<int> BannedIDs;
        private readonly int LargestBannedID;

        internal Dictionary<T, EnumTypeCache> customEnumTypes = new Dictionary<T, EnumTypeCache>();

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
            var saveDir = $"./QMods/Modding Helper/{EnumTypeName}Cache";

            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);

            return saveDir;
        }

        private string GetCachePath()
        {
            return Path.Combine(GetCacheDirectoryPath(), $"{EnumTypeName}Cache.txt");
        }

        internal void LoadCache()
        {
            if (cacheLoaded) return;

            try
            {
                var savePathDir = GetCachePath();

                if (!File.Exists(savePathDir))
                {
                    cacheLoaded = true; // Just so it wont keep calling this over and over again.
                    return;
                }

                var allText = File.ReadAllLines(savePathDir);

                foreach (var line in allText)
                {
                    string[] split = line.Split(':');
                    var name = split[0];
                    var index = split[1];

                    var cache = new EnumTypeCache()
                    {
                        Name = name,
                        Index = int.Parse(index)
                    };

                    cacheList.Add(cache);
                }
            }
            catch(Exception exception)
            {
                Logger.Log("Caught exception when reading cache!");
                Logger.Log("Exception message: " + exception.Message);
                Logger.Log("StackTrace: " + Environment.NewLine + exception.StackTrace);
            }

            cacheLoaded = true;
        }

        internal void SaveCache()
        {
            try
            {
                var savePathDir = GetCachePath();
                var stringBuilder = new StringBuilder();

                foreach (var entry in customEnumTypes)
                {
                    cacheList.Add(entry.Value);

                    stringBuilder.AppendLine($"{entry.Value.Name}:{entry.Value.Index}");
                }

                File.WriteAllText(savePathDir, stringBuilder.ToString());
            }
            catch(Exception exception)
            {
                Logger.Log("Caught exception when saving cache!");
                Logger.Log("Exception message: " + exception.Message);
                Logger.Log("StackTrace: " + Environment.NewLine + exception.StackTrace);
            }
        }

        internal EnumTypeCache GetCacheForTypeName(string name)
        {
            LoadCache();

            foreach (var cache in cacheList)
            {
                if (cache.Name == name)
                    return cache;
            }

            return null;
        }

        internal EnumTypeCache GetCacheForIndex(int index)
        {
            LoadCache();

            foreach (var cache in cacheList)
            {
                if (cache.Index == index)
                    return cache;
            }

            return null;
        }

        internal int GetLargestIndexFromCache()
        {
            LoadCache();

            var index = StartingIndex;

            foreach (var cache in cacheList)
            {
                if (cache.Index > index)
                    index = cache.Index;
            }

            return index;
        }

        internal int GetNextFreeIndex()
        {
            LoadCache();

            var freeIndex = GetLargestIndexFromCache() + 1;

            if (BannedIDs != null && BannedIDs.Contains(freeIndex))
            {
                freeIndex = LargestBannedID + 1;
            }

            return freeIndex;
        }

        internal bool IsIndexValid(int index)
        {
            LoadCache();

            var count = 0;

            foreach (var cache in cacheList)
            {
                if (cache.Index == index)
                    count++;
            }

            return count >= 2 && (!BannedIDs?.Contains(index) ?? false);
        }

        #endregion
    }
}
