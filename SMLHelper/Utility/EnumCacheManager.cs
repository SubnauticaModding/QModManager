namespace SMLHelper.V2.Utility
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;

    internal class EnumTypeCache
    {
        internal int Index;
        internal string Name;

        public EnumTypeCache()
        {
        }

        public EnumTypeCache(int index, string name)
        {
            Index = index;
            Name = name;
        }
    }

    internal class EnumCacheManager<T> where T : Enum
    {
        private class DoubleKeyDictionary : IEnumerable<KeyValuePair<int, string>>
        {
            private readonly SortedDictionary<int, string> MapIntString = new SortedDictionary<int, string>();
            private readonly SortedDictionary<T, string> MapEnumString = new SortedDictionary<T, string>();
            private readonly SortedDictionary<string, T> MapStringEnum = new SortedDictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);
            private readonly SortedDictionary<string, int> MapStringInt = new SortedDictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

            public bool TryGetValue(T enumValue, out string name)
            {
                return MapEnumString.TryGetValue(enumValue, out name);
            }

            public bool TryGetValue(string name, out T enumValue)
            {
                return MapStringEnum.TryGetValue(name, out enumValue);
            }

            public bool TryGetValue(string name, out int backingValue)
            {
                return MapStringInt.TryGetValue(name, out backingValue);
            }

            public void Add(int backingValue, string name)
            {
                Add((T)(object)backingValue, backingValue, name);
            }

            public void Add(T enumValue, int backingValue, string name)
            {
                MapIntString.Add(backingValue, name);
                MapEnumString.Add(enumValue, name);
                MapStringEnum.Add(name, enumValue);
                MapStringInt.Add(name, backingValue);

                if (backingValue > this.LargestIntValue)
                    this.LargestIntValue = backingValue;
            }

            public void Remove(int backingValue, string name)
            {
                Remove((T)(object)backingValue, backingValue, name);
            }

            public void Remove(T enumValue, int backingValue, string name)
            {
                MapIntString.Remove(backingValue);
                MapEnumString.Remove(enumValue);
                MapStringEnum.Remove(name);
                MapStringInt.Remove(name);
            }

            public int LargestIntValue { get; private set; }

            public IEnumerable<T> KnownsEnumKeys => MapEnumString.Keys;

            public int KnownsEnumCount => MapEnumString.Count;

            public bool IsKnownKey(T key)
            {
                return MapEnumString.ContainsKey(key);
            }

            public bool IsKnownKey(int key)
            {
                return MapIntString.ContainsKey(key);
            }

            public IEnumerator<KeyValuePair<int, string>> GetEnumerator()
            {
                return MapIntString.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return MapIntString.GetEnumerator();
            }

            public void Clear()
            {
                MapIntString.Clear();
                MapEnumString.Clear();
                MapStringEnum.Clear();
                MapStringInt.Clear();
            }
        }

        internal readonly string EnumTypeName;
        internal readonly int StartingIndex;
        internal bool cacheLoaded = false;

        private readonly HashSet<int> BannedIDs;
        private readonly int LargestBannedID;

        private readonly DoubleKeyDictionary entriesFromFile = new DoubleKeyDictionary();
        private readonly DoubleKeyDictionary entriesFromDeactivatedFile = new DoubleKeyDictionary();
        private readonly DoubleKeyDictionary entriesFromRequests = new DoubleKeyDictionary();

        public IEnumerable<T> ModdedKeys => entriesFromRequests.KnownsEnumKeys;

        public int ModdedKeysCount => entriesFromRequests.KnownsEnumCount;

        public bool TryGetValue(T key, out string value)
        {
            return entriesFromRequests.TryGetValue(key, out value);
        }

        public bool TryParse(string value, out T type)
        {
            return entriesFromRequests.TryGetValue(value, out type);
        }

        public void Add(T value, int backingValue, string name)
        {
            if (!entriesFromRequests.IsKnownKey(backingValue))
                entriesFromRequests.Add(value, backingValue, name);
        }

        public bool ContainsKey(T key)
        {
            return entriesFromRequests.IsKnownKey(key);
        }

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
            string saveDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{EnumTypeName}Cache");

            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);

            return saveDir;
        }

        private string GetCachePath()
        {
            return Path.Combine(GetCacheDirectoryPath(), $"{EnumTypeName}Cache.txt");
        }

        private string GetDeactivatedCachePath()
        {
            return Path.Combine(GetCacheDirectoryPath(), $"{EnumTypeName}DeactivatedCache.txt");
        }

        internal void LoadCache()
        {
            if (cacheLoaded)
                return;

            ReadCacheFile(GetCachePath(), (index, name) =>
            {
                entriesFromFile.Add(index, name);
            });

            ReadCacheFile(GetDeactivatedCachePath(), (index, name) =>
            {
                entriesFromDeactivatedFile.Add(index, name);
                entriesFromFile.Add(index, name);
            });

            cacheLoaded = true;
        }

        private void ReadCacheFile(string savePathDir, Action<int, string> loadParsedEntry)
        {
            try
            {
                if (!File.Exists(savePathDir))
                {
                    cacheLoaded = true;
                    return;
                }

                string[] allText = File.ReadAllLines(savePathDir);
                foreach (string line in allText)
                {
                    string[] split = line.Split(':');
                    string name = split[0];
                    string index = split[1];

                    loadParsedEntry.Invoke(Convert.ToInt32(index), name);
                }
            }
            catch (Exception exception)
            {
                Logger.Error($"Caught exception while reading {savePathDir}{Environment.NewLine}{exception}");
            }
        }

        internal void SaveCache()
        {
            LoadCache();
            try
            {
                string savePathDir = GetCachePath();
                var stringBuilder = new StringBuilder();

                foreach (KeyValuePair<int, string> entry in entriesFromRequests)
                {
                    stringBuilder.AppendLine($"{entry.Value}:{entry.Key}");
                }

                File.WriteAllText(savePathDir, stringBuilder.ToString());


                savePathDir = GetDeactivatedCachePath();
                stringBuilder = new StringBuilder();

                foreach (KeyValuePair<int, string> entry in entriesFromFile)
                {
                    if (!entriesFromRequests.TryGetValue(entry.Value, out int v))
                    {
                        stringBuilder.AppendLine($"{entry.Value}:{entry.Key}");
                    }
                }

                File.WriteAllText(savePathDir, stringBuilder.ToString());
                entriesFromFile.Clear();
            }
            catch (Exception exception)
            {
                Logger.Error($"Caught exception while saving cache!{Environment.NewLine}{exception}");
            }
        }

        internal EnumTypeCache RequestCacheForTypeName(string name, bool checkDeactivated = true)
        {
            LoadCache();

            if (entriesFromRequests.TryGetValue(name, out int value))
            {
                return new EnumTypeCache(value, name);
            }
            else if (entriesFromFile.TryGetValue(name, out value))
            {
                entriesFromRequests.Add(value, name);
                return new EnumTypeCache(value, name);
            }
            else if (checkDeactivated && entriesFromDeactivatedFile.TryGetValue(name, out value))
            {
                entriesFromRequests.Add(value, name);
                entriesFromDeactivatedFile.Remove(value, name);
                return new EnumTypeCache(value, name);
            }

            return null;
        }

        internal int GetNextAvailableIndex()
        {
            LoadCache();

            int index = StartingIndex + 1;

            while (entriesFromFile.IsKnownKey(index) ||
                   entriesFromRequests.IsKnownKey(index) ||
                   entriesFromDeactivatedFile.IsKnownKey(index) ||
                   BannedIDs.Contains(index))
            {
                index++;
            }

            return index;
        }


        #endregion
    }
}
