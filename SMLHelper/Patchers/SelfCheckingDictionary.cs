namespace SMLHelper.V2.Patchers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;

    internal class SelfCheckingDictionary<K, V> : IDictionary<K, V>
    {
        [NonSerialized] private object _syncRoot;

        internal readonly HashSet<K> DuplicatesFound;
        internal readonly Dictionary<K, V> UniqueEntries;
        internal readonly string CollectionName;

        public SelfCheckingDictionary(string collectionName)
        {
            CollectionName = collectionName;
            UniqueEntries = new Dictionary<K, V>();
            DuplicatesFound = new HashSet<K>();
        }

        public SelfCheckingDictionary(string collectionName, IEqualityComparer<K> equalityComparer)
        {
            CollectionName = collectionName;
            UniqueEntries = new Dictionary<K, V>(equalityComparer);
            DuplicatesFound = new HashSet<K>(equalityComparer);
        }

        public V this[K key]
        {
            get => UniqueEntries[key];
            set => UniqueEntries[key] = value;
        }

        public ICollection<K> Keys => UniqueEntries.Keys;
        public ICollection<V> Values => UniqueEntries.Values;
        public int Count => UniqueEntries.Count;
        public bool IsReadOnly { get; } = false;
        public bool IsFixedSize { get; } = false;

        public object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);
                }

                return _syncRoot;
            }
        }

        public bool IsSynchronized { get; } = false;

        public void Add(K key, V value)
        {
            if (DuplicatesFound.Contains(key))
            {
                DupFoundError(key);
                return;
            }

            if (UniqueEntries.ContainsKey(key))
            {
                UniqueEntries.Remove(key);
                DuplicatesFound.Add(key);

                DupFoundError(key);
                return;
            }

            UniqueEntries.Add(key, value);
        }

        private void DupFoundError(K key)
        {
            Logger.Log($"[ERROR] {CollectionName} already exists for '{key}'. {Environment.NewLine}" +
                                        "All entries will be removed so conflict can be noted and resolved.");
        }

        public void Add(KeyValuePair<K, V> item) => Add(item.Key, item.Value);

        public void Clear()
        {
            UniqueEntries.Clear();
            DuplicatesFound.Clear();
        }

        public bool Contains(KeyValuePair<K, V> item) => UniqueEntries.TryGetValue(item.Key, out V value) && value.Equals(item.Value);

        public bool ContainsKey(K key) => UniqueEntries.ContainsKey(key);

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) => throw new NotImplementedException();

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => UniqueEntries.GetEnumerator();

        public bool Remove(K key) => UniqueEntries.Remove(key) | DuplicatesFound.Remove(key);

        public bool Remove(KeyValuePair<K, V> item)
        {
            if (UniqueEntries.TryGetValue(item.Key, out V value) && value.Equals(item.Value))
            {
                return UniqueEntries.Remove(item.Key);
            }

            return DuplicatesFound.Remove(item.Key);
        }

        public bool TryGetValue(K key, out V value) => UniqueEntries.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => UniqueEntries.GetEnumerator();       
    }
}
