using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SMLHelper.Util
{
    internal class EnumTypeCache
    {
        internal int Index;
        internal string Name;
    }

    internal class EnumCacheManager<T>
    {
        internal readonly string enumTypeName;
        internal readonly int startingIndex;
        internal bool cacheLoaded = false;

        internal List<EnumTypeCache> cacheList = new List<EnumTypeCache>();
        internal Dictionary<T, EnumTypeCache> customEnumTypes = new Dictionary<T, EnumTypeCache>();

        internal EnumCacheManager(string enumTypeName, int startingIndex)
        {
            this.enumTypeName = enumTypeName;
            this.startingIndex = startingIndex;            
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
                var techTypeName = line.Split(':')[0];
                var techTypeIndex = line.Split(':')[1];

                var cache = new EnumTypeCache()
                {
                    Name = techTypeName,
                    Index = int.Parse(techTypeIndex)
                };

                cacheList.Add(cache);
            }

            Logger.Log("Loaded EnumTypeCache!");

            cacheLoaded = true;
        }

        internal void SaveCache()
        {
            var savePathDir = GetCachePath();
            var stringBuilder = new StringBuilder();

            foreach (var techTypeEntry in customEnumTypes)
            {
                cacheList.Add(techTypeEntry.Value);

                stringBuilder.AppendLine(string.Format("{0}:{1}", techTypeEntry.Value.Name, techTypeEntry.Value.Index));
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
            return largestIndex + 1;
        }

        internal bool MultipleCachesUsingSameIndex(int index)
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

        #endregion
    }
}
