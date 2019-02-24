namespace SMLHelper.V2.Patchers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// This dictionary strtucture automatically checks for duplicate keys as they are being added to the collection.
    /// Duplicate entires are logged and removed from the final collection.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    internal class SelfCheckingDictionary<K, V> : IDictionary<K, V>
    {
        internal readonly Dictionary<K, int> DuplicatesDiscarded;
        internal readonly Dictionary<K, V> UniqueEntries;
        internal readonly string CollectionName;

        public SelfCheckingDictionary(string collectionName)
        {
            CollectionName = collectionName;
            UniqueEntries = new Dictionary<K, V>();
            DuplicatesDiscarded = new Dictionary<K, int>();
        }

        public SelfCheckingDictionary(string collectionName, IEqualityComparer<K> equalityComparer)
        {
            CollectionName = collectionName;
            UniqueEntries = new Dictionary<K, V>(equalityComparer);
            DuplicatesDiscarded = new Dictionary<K, int>(equalityComparer);
        }

        public V this[K key]
        {
            get => UniqueEntries[key];
            set
            {
                if (UniqueEntries.ContainsKey(key))
                {
                    if (DuplicatesDiscarded.ContainsKey(key))
                        DuplicatesDiscarded[key]++;
                    else
                        DuplicatesDiscarded.Add(key, 1);

                    DupFoundLastDiscardedLog(key);
                }

                UniqueEntries[key] = value;
            }
        }

        public ICollection<K> Keys => UniqueEntries.Keys;
        public ICollection<V> Values => UniqueEntries.Values;
        public int Count => UniqueEntries.Count;
        public bool IsReadOnly { get; } = false;

        public void Add(K key, V value)
        {
            if (DuplicatesDiscarded.ContainsKey(key))
            {
                DuplicatesDiscarded[key]++;
                DupFoundAllDiscardedLog(key);
                return;
            }

            if (UniqueEntries.ContainsKey(key))
            {
                UniqueEntries.Remove(key);
                DuplicatesDiscarded.Add(key, 1);

                DupFoundAllDiscardedLog(key);
                return;
            }

            UniqueEntries.Add(key, value);
        }

        private void DupFoundAllDiscardedLog(K key)
        {
            Logger.Warn($"{CollectionName} already exists for '{key}'. {Environment.NewLine}" +
                        "All entries will be removed so conflict can be noted and resolved.");
        }

        private void DupFoundLastDiscardedLog(K key)
        {
            Logger.Warn($"{CollectionName} already exists for '{key}'. {Environment.NewLine}" +
                        " Original value has been overwritten by later entry.");
        }

        public void Add(KeyValuePair<K, V> item) => Add(item.Key, item.Value);

        public void Clear()
        {
            UniqueEntries.Clear();
            DuplicatesDiscarded.Clear();
        }

        public bool Contains(KeyValuePair<K, V> item) => UniqueEntries.TryGetValue(item.Key, out V value) && value.Equals(item.Value);

        public bool ContainsKey(K key) => UniqueEntries.ContainsKey(key);

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            foreach (KeyValuePair<K, V> pair in UniqueEntries)
            {
                array[arrayIndex++] = pair;
            }
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => UniqueEntries.GetEnumerator();

        public bool Remove(K key) => UniqueEntries.Remove(key) | DuplicatesDiscarded.Remove(key);

        public bool Remove(KeyValuePair<K, V> item) => UniqueEntries.Remove(item.Key) | DuplicatesDiscarded.Remove(item.Key);

        public bool TryGetValue(K key, out V value) => UniqueEntries.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => UniqueEntries.GetEnumerator();       
    }
}
