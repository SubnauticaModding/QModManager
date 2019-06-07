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
        public PairedList()
        {
        }

        public PairedList(int capacity) : base(capacity)
        {
        }

        public void Add(KeyType key, ValueType value)
        {
            Add(new Pair<KeyType, ValueType>(key, value));
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
    }
}
