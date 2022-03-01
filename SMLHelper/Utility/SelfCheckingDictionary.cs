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
        /// <summary>
        /// Maintains a collection of the keys that have encountered duplicates and how many of them were discarded.
        /// </summary>
        internal readonly Dictionary<K, int> DuplicatesDiscarded;

        /// <summary>
        /// Maintains the final collection of only unique keys.
        /// </summary>
        internal readonly Dictionary<K, V> UniqueEntries;
        internal readonly string CollectionName;

        internal readonly Func<K, string> ToLogString;

        private SelfCheckingDictionary(Func<K, string> toLog)
        {
            ToLogString = toLog ?? ((k) => k.ToString());
        }

        public SelfCheckingDictionary(string collectionName, Func<K, string> toLog = null)
            : this(toLog)
        {
            CollectionName = collectionName;
            UniqueEntries = new Dictionary<K, V>();
            DuplicatesDiscarded = new Dictionary<K, int>();
        }

        public SelfCheckingDictionary(string collectionName, IEqualityComparer<K> equalityComparer, Func<K, string> toLog = null)
            : this(toLog)
        {
            CollectionName = collectionName;
            UniqueEntries = new Dictionary<K, V>(equalityComparer);
            DuplicatesDiscarded = new Dictionary<K, int>(equalityComparer);
        }

        /// <summary>
        /// Gets a key value pair from the collection or sets a key value pair into the collection.
        /// When setting, if a key already exists, the previous entry will be discarded.
        /// </summary>
        /// <param name="key">The unique key.</param>
        /// <returns>The value corresponding to the key.</returns>
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
                        DuplicatesDiscarded.Add(key, 1); // Original is overwritten.

                    DupFoundLastDiscardedLog(key);
                }

                UniqueEntries[key] = value;
            }
        }

        public ICollection<K> Keys => UniqueEntries.Keys;
        public ICollection<V> Values => UniqueEntries.Values;
        public int Count => UniqueEntries.Count;
        public bool IsReadOnly { get; } = false;

        /// <summary>
        /// Add a new entry the collection.
        /// If a duplicate key is found, all entries with that key will be excluded from the final collection.
        /// </summary>
        /// <param name="key">The unique key.</param>
        /// <param name="value">The value.</param>
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
                DuplicatesDiscarded.Add(key, 2); // Original is discarded and new entry is not added.

                DupFoundAllDiscardedLog(key);
                return;
            }

            UniqueEntries.Add(key, value);
        }

        /// <summary>
        /// Add a new entry the collection.
        /// If a duplicate key is found, all entries with that key will be excluded from the final collection.
        /// </summary>
        /// <param name="item">The key value pair.</param>
        public void Add(KeyValuePair<K, V> item) => Add(item.Key, item.Value);

        public void Clear()
        {
            UniqueEntries.Clear();
            DuplicatesDiscarded.Clear();
        }

        public bool Contains(KeyValuePair<K, V> item) => UniqueEntries.TryGetValue(item.Key, out V value) && value.Equals(item.Value);

        public bool ContainsKey(K key) => UniqueEntries.ContainsKey(key) | DuplicatesDiscarded.ContainsKey(key);

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

        /// <summary>
        /// Informs the user that all entries for the specified key have been discarded.
        /// </summary>
        /// <param name="key">The no longer unique key.</param>
        private void DupFoundAllDiscardedLog(K key)
        {
            string keyLogString = ToLogString(key);
            Logger.Warn($"{CollectionName} already exists for '{keyLogString}'.{Environment.NewLine}" +
                        $"All entries will be removed so conflict can be noted and resolved.{Environment.NewLine}" +
                        $"So far we have discarded or overwritten {DuplicatesDiscarded[key]} entries for '{keyLogString}'.");
        }

        /// <summary>
        /// Informs the user that the previous entry for the specified key has been discarded.
        /// </summary>
        /// <param name="key">The no longer unique key.</param>
        private void DupFoundLastDiscardedLog(K key)
        {
            string keyLogString = ToLogString(key);
            Logger.Warn($"{CollectionName} already exists for '{keyLogString}'.{Environment.NewLine}" +
                        $"Original value has been overwritten by later entry.{Environment.NewLine}" +
                        $"So far we have discarded or overwritten {DuplicatesDiscarded[key]} entries for '{keyLogString}'.");
        }
    }
}
