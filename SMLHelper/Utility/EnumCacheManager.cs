namespace SMLHelper.V2.Utility
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
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

        /*
        internal EnumCacheManager(string enumTypeName, int startingIndex)
        {
            this.enumTypeName = enumTypeName;
            this.startingIndex = startingIndex;            
        }
        */

        internal EnumCacheManager(string enumTypeName, int startingIndex, IEnumerable<int> bannedIndices)
        {
            this.enumTypeName = enumTypeName;
            this.startingIndex = startingIndex;
            this.bannedIndices = bannedIndices.ToList();

            // Make sure to exclude already registered TechTypes
            if (enumTypeName.CompareTo("TechType") == 0)
            {
#if DEBUG
                Logger.Log("DEBUG: Now loading known TechTypes...");
#endif
                int addedToBanList = 0;
                FieldInfo keyTechTypesField = typeof(TechTypeExtensions).GetField("keyTechTypes", BindingFlags.NonPublic | BindingFlags.Static);
                Dictionary<string, TechType> knownTechTypes = keyTechTypesField.GetValue(null) as Dictionary<string, TechType>;
                foreach (KeyValuePair<string, TechType> knownTechType in knownTechTypes)
                {
#if DEBUG
                    Logger.Log("DEBUG: Found known TechType: ID=[" + knownTechType.Key + "] Name=[" + knownTechType.Value.AsString(false) + "]");
#endif
                    int currentTechTypeKey = Convert.ToInt32(knownTechType.Key);
                    if (currentTechTypeKey > startingIndex)
                    {
                        if (!this.bannedIndices.Contains(currentTechTypeKey))
                        {
                            this.bannedIndices.Add(currentTechTypeKey);
                            ++addedToBanList;
                        }
#if DEBUG
                        else
                            Logger.Log("DEBUG: TechType ID=[" + knownTechType.Key + "] Name=[" + knownTechType.Value.AsString(false) + "] already present in ban list.");
                    }
                    else
                        Logger.Log("DEBUG: TechType ID=[" + knownTechType.Key + "] Name=[" + knownTechType.Value.AsString(false) + "] is one of the game TechTypes.");
                }
                Logger.Log("DEBUG: Finished known TechTypes exclusion. " + addedToBanList + " ID were added in ban list.");
#else
                    }
                }
#endif
            }
            else if (enumTypeName.CompareTo("CraftTreeType") == 0)
            {
#if DEBUG
                Logger.Log("DEBUG: Now loading known CraftTreeTypes...");
#endif
                int addedToBanList = 0;
                var enumValues = Enum.GetValues(typeof(CraftTree.Type));
                foreach (var enumValue in enumValues)
                {
                    if (enumValue != null)
                    {
                        int realEnumValue = (int)enumValue;
#if DEBUG
                        Logger.Log("DEBUG: Found known CraftTreeType: ID=[" + realEnumValue + "]");
#endif
                        if (realEnumValue > startingIndex)
                        {
                            if (!this.bannedIndices.Contains(realEnumValue))
                            {
                                this.bannedIndices.Add(realEnumValue);
                                ++addedToBanList;
                            }
#if DEBUG
                            else
                                Logger.Log("DEBUG: CraftTreeType ID=[" + realEnumValue + "] already present in ban list.");
                        }
                        else
                            Logger.Log("DEBUG: CraftTreeType ID=[" + realEnumValue + "] is one of the game CraftTreeType.");
#else
                        }
#endif
                    }
                }
                Logger.Log("DEBUG: Finished known CraftTreeType exclusion. " + addedToBanList + " ID were added in ban list.");
            }
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
            
            var freeIndex = GetLargestIndexFromCache() + 1;

            if (bannedIndices != null)
                while (bannedIndices.Contains(freeIndex))
                    ++freeIndex;

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
