namespace QModManager.API.SMLHelper.Patchers
{
    using QModManager.Utility;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// This dictionary strtucture automatically checks for duplicate keys as they are being added to the collection.
    /// Duplicate entires are logged and removed from the final collection.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal class SelfCheckingDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        /// <summary>
        /// Maintains a collection of the keys that have encountered duplicates and how many of them were discarded.
        /// </summary>
        internal readonly Dictionary<TKey, int> DuplicatesDiscarded;

        /// <summary>
        /// Maintains the final collection of only unique keys.
        /// </summary>
        internal readonly Dictionary<TKey, TValue> UniqueEntries;
        internal readonly string CollectionName;

        public SelfCheckingDictionary(string collectionName)
        {
            CollectionName = collectionName;
            UniqueEntries = new Dictionary<TKey, TValue>();
            DuplicatesDiscarded = new Dictionary<TKey, int>();
        }

        public SelfCheckingDictionary(string collectionName, IEqualityComparer<TKey> equalityComparer)
        {
            CollectionName = collectionName;
            UniqueEntries = new Dictionary<TKey, TValue>(equalityComparer);
            DuplicatesDiscarded = new Dictionary<TKey, int>(equalityComparer);
        }

        /// <summary>
        /// Gets a key value pair from the collection or sets a key value pair into the collection.
        /// When setting, if a key already exists, the previous entry will be discarded.
        /// </summary>
        /// <param name="key">The unique key.</param>
        /// <returns>The value corresponding to the key.</returns>
        public TValue this[TKey key]
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

        public ICollection<TKey> Keys => UniqueEntries.Keys;
        public ICollection<TValue> Values => UniqueEntries.Values;
        public int Count => UniqueEntries.Count;
        public bool IsReadOnly { get; } = false;

        /// <summary>
        /// Add a new entry the collection.
        /// If a duplicate key is found, all entries with that key will be excluded from the final collection.
        /// </summary>
        /// <param name="key">The unique key.</param>
        /// <param name="value">The value.</param>
        public void Add(TKey key, TValue value)
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
        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        public void Clear()
        {
            UniqueEntries.Clear();
            DuplicatesDiscarded.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) => UniqueEntries.TryGetValue(item.Key, out TValue value) && value.Equals(item.Value);

        public bool ContainsKey(TKey key) => UniqueEntries.ContainsKey(key) | DuplicatesDiscarded.ContainsKey(key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (KeyValuePair<TKey, TValue> pair in UniqueEntries)
            {
                array[arrayIndex++] = pair;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => UniqueEntries.GetEnumerator();

        public bool Remove(TKey key) => UniqueEntries.Remove(key) | DuplicatesDiscarded.Remove(key);

        public bool Remove(KeyValuePair<TKey, TValue> item) => UniqueEntries.Remove(item.Key) | DuplicatesDiscarded.Remove(item.Key);

        public bool TryGetValue(TKey key, out TValue value) => UniqueEntries.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => UniqueEntries.GetEnumerator();         

        /// <summary>
        /// Informs the user that all entries for the specified key have been discarded.
        /// </summary>
        /// <param name="key">The no longer unique key.</param>
        private void DupFoundAllDiscardedLog(TKey key)
        {
            Logger.Warn($"{CollectionName} already exists for '{key}'.{Environment.NewLine}" +
                        $"All entries will be removed so conflict can be noted and resolved.{Environment.NewLine}" +
                        $"So far we have discarded or overwritten {DuplicatesDiscarded[key]} entries for '{key}'.");
        }

        /// <summary>
        /// Informs the user that the previous entry for the specified key has been discarded.
        /// </summary>
        /// <param name="key">The no longer unique key.</param>
        private void DupFoundLastDiscardedLog(TKey key)
        {
            Logger.Warn($"{CollectionName} already exists for '{key}'.{Environment.NewLine}" +
                        $"Original value has been overwritten by later entry.{Environment.NewLine}" +
                        $"So far we have discarded or overwritten {DuplicatesDiscarded[key]} entries for '{key}'.");
        }
    }
}
