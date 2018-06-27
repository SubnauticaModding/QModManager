namespace SMLHelper.V2.Util
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    internal class EnumTypeCache
    {
        internal int Index;
        internal string Name;
    }

    internal class EnumCacheManager<T>
    {
        internal readonly string enumTypeName;
        internal readonly int startingIndex;
        internal readonly List<int> bannedIndices;
        internal bool cacheLoaded = false;

        internal List<EnumTypeCache> cacheList = new List<EnumTypeCache>();
        internal Dictionary<T, EnumTypeCache> customEnumTypes = new Dictionary<T, EnumTypeCache>();

        internal EnumCacheManager(string enumTypeName, int startingIndex)
        {
            this.enumTypeName = enumTypeName;
            this.startingIndex = startingIndex;            
        }

        internal EnumCacheManager(string enumTypeName, int startingIndex, IEnumerable<int> bannedIndices)
        {
            this.enumTypeName = enumTypeName;
            this.startingIndex = startingIndex;
            this.bannedIndices = bannedIndices.ToList();
        }

        #region Caching

        private string GetCachePath()
        {
            var saveDir = @"./QMods/Modding Helper/" + $"{enumTypeName}Cache";

            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);

            return Path.Combine(saveDir, $"{enumTypeName}Cache.txt");
        }

        internal void LoadCache()
        {
            if (cacheLoaded) return;

            var savePathDir = GetCachePath();

            if (!File.Exists(savePathDir))
            {
                SaveCache();
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

            Logger.Log($"Loaded ${enumTypeName} Cache!");

            cacheLoaded = true;
        }

        internal void SaveCache()
        {
            var savePathDir = GetCachePath();
            var stringBuilder = new StringBuilder();

            foreach (var entry in customEnumTypes)
            {
                cacheList.Add(entry.Value);

                stringBuilder.AppendLine(string.Format("{0}:{1}", entry.Value.Name, entry.Value.Index));
            }

            File.WriteAllText(savePathDir, stringBuilder.ToString());
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

            var index = startingIndex;

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

            var largestIndex = GetLargestIndexFromCache();
            var freeIndex = largestIndex + 1;

            if (bannedIndices != null && bannedIndices.Contains(freeIndex))
                freeIndex = bannedIndices[bannedIndices.Count - 1] + 1;

            return freeIndex;
        }

        internal bool IsIndexConflicting(int index)
        {
            LoadCache();

            var count = 0;

            foreach (var cache in cacheList)
            {
                if (cache.Index == index)
                    count++;
            }

            if (count >= 2)
                return true;

            return false;
        }

        internal bool IsIndexBanned(int index)
        {
            return bannedIndices?.Contains(index) ?? false;
        }

        #endregion
    }
}
