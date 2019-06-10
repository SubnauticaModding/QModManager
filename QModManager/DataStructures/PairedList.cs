namespace QModManager.DataStructures
{
    using System.Collections.Generic;

    internal class Pair<KeyType, ValueType>
    {
        public Pair(KeyType key, ValueType value)
        {
            this.Key = key;
            this.Value = value;
        }

        internal KeyType Key { get; set; }
        internal ValueType Value { get; set; }
    }

    internal class PairedList<KeyType, ValueType> : List<Pair<KeyType, ValueType>>
    {
        private readonly Dictionary<ValueType, int> valueCounts = new Dictionary<ValueType, int>();

        public PairedList()
        {
        }

        public PairedList(int capacity) : base(capacity)
        {
        }

        public void Add(KeyType key, ValueType value)
        {
            Add(new Pair<KeyType, ValueType>(key, value));

            if (valueCounts.TryGetValue(value, out int count))
                count++;
            else
                valueCounts.Add(value, 1);
        }

        public bool Contains(KeyType key)
        {
            foreach (Pair<KeyType, ValueType> item in this)
            {
                if (item.Key.Equals(key))
                    return true;
            }

            return false;
        }

        public IEnumerable<KeyType> Keys
        {
            get
            {
                foreach (Pair<KeyType, ValueType> item in this)
                {
                    yield return item.Key;
                }
            }
        }

        public int ValueCount(ValueType value)
        {
            if (valueCounts.TryGetValue(value, out int count))
                return count;
            else
                return 0;
        }
    }
}
