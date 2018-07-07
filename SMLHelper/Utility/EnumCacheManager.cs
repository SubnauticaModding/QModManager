namespace SMLHelper.V2.Utility
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
        internal bool cacheLoaded = false;
        internal bool banlistLoaded = false;

        internal List<EnumTypeCache> cacheList = new List<EnumTypeCache>();
        internal readonly List<int> bannedIndices = new List<int>();
        internal Dictionary<T, EnumTypeCache> customEnumTypes = new Dictionary<T, EnumTypeCache>();

        internal EnumCacheManager(string enumTypeName, int startingIndex)
        {
            this.enumTypeName = enumTypeName;
            this.startingIndex = startingIndex;
            this.bannedIndices = new List<int>();
        }

        internal EnumCacheManager(string enumTypeName, int startingIndex, IEnumerable<int> bannedIndices)
        {
            this.enumTypeName = enumTypeName;
            this.startingIndex = startingIndex;
            this.bannedIndices = bannedIndices.ToList();
        }

        #region Caching

        private string GetCacheDirectoryPath()
        {
            var saveDir = @"./QMods/Modding Helper/" + $"{enumTypeName}Cache";

            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);

            return saveDir;
        }

        private string GetCachePath()
        {
            return Path.Combine(GetCacheDirectoryPath(), $"{enumTypeName}Cache.txt");
        }

        private string GetBanListPath()
        {
            return Path.Combine(GetCacheDirectoryPath(), "Banlist.txt");
        }

        internal void LoadBanlist()
        {
            if (banlistLoaded) return;

            var banlistDir = GetBanListPath();

            if(!File.Exists(banlistDir))
            {
                File.Create(banlistDir);
                return;
            }

            var allText = File.ReadAllLines(banlistDir);

            foreach(var line in allText)
            {
                if (int.TryParse(line, out int id))
                {
                    bannedIndices.Add(id);
                }
                else
                {
                    Logger.Log("Invalid id: " + line + " in " + enumTypeName + " ban list!");
                }
            }

            banlistLoaded = true;

            Logger.Log($"{enumTypeName} ban list loaded!");
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

            Logger.Log($"Loaded {enumTypeName} Cache!");

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
            LoadBanlist();

            var largestIndex = GetLargestIndexFromCache();
            var freeIndex = largestIndex + 1;

            //if (bannedIndices != null && bannedIndices.Contains(freeIndex))
            //    freeIndex = bannedIndices[bannedIndices.Count - 1] + 1;

            if(bannedIndices != null && bannedIndices.Contains(freeIndex))
            {
                var largestBannIndex = bannedIndices.Max();
                freeIndex = largestBannIndex + 1;
            }

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
            LoadBanlist();

            return bannedIndices?.Contains(index) ?? false;
        }

        #endregion
    }
}
