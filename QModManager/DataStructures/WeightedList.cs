namespace QModManager.DataStructures
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal class WeightedList<TKey> : ICollection<TKey>
    {
        private readonly Dictionary<TKey, int> weights = new Dictionary<TKey, int>();

        public int Count => weights.Count;

        public bool IsReadOnly => false;

        public IEnumerator<TKey> GetEnumerator()
        {
            var tempList = new List<TKey>(weights.Keys);

            while (tempList.Count > 0)
            {
                int minWeight = -1;
                TKey minKey = default;

                foreach (TKey key in tempList)
                {
                    int weight = weights[key];
                    if (minWeight == -1 || weight <= minWeight)
                    {
                        minWeight = weight;
                        minKey = key;
                    }
                }

                tempList.Remove(minKey);

                yield return minKey;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TKey item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "entries must not be null");

            if (weights.TryGetValue(item, out int weight))
                weights[item] = ++weight;
            else
                weights.Add(item, 1);
        }

        public void Clear()
        {
            weights.Clear();
        }

        public bool Contains(TKey item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "entries must not be null");

            return weights.ContainsKey(item);
        }

        public void CopyTo(TKey[] array, int arrayIndex)
        {
            List<TKey> sorted = ToSortedList();
            sorted.CopyTo(array, arrayIndex);
        }

        public bool Remove(TKey item)
        {
            return Remove(item, true);
        }

        public bool Remove(TKey item, bool removeAtZero)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "entries must not be null");

            if (weights.TryGetValue(item, out int weight))
            {
                weight = Math.Max(0, weight - 1);

                if (weight > 0)
                    weights[item] = weight;
                else if (removeAtZero)
                    weights.Remove(item);

                return true;
            }

            return false;
        }

        public List<TKey> ToSortedList()
        {
            var list = new List<TKey>(weights.Count);

            foreach (TKey key in this)
                list.Add(key);

            return list;
        }

        public TKey GetMinWeight()
        {
            if (weights.Count == 0)
                throw new ArgumentOutOfRangeException("", "There are no weights");

            return FindByWeight((targetWeight, weight) => weight <= targetWeight);
        }

        public TKey GetMaxWeight()
        {
            if (weights.Count == 0)
                throw new ArgumentOutOfRangeException("", "There are no weights");

            return FindByWeight((targetWeight, weight) => weight >= targetWeight);
        }

        public int GetWeight(TKey key, int forUnknown = 0)
        {
            return weights.TryGetValue(key, out int weight) ? weight : forUnknown;
        }

        private TKey FindByWeight(Func<int, int, bool> weightFunction)
        {
            int targetWeight = -1;
            TKey maxKey = default;

            foreach (KeyValuePair<TKey, int> weightEntry in weights)
            {
                int weight = weightEntry.Value;
                if (targetWeight == -1 || weightFunction(targetWeight, weight))
                {
                    targetWeight = weight;
                    maxKey = weightEntry.Key;
                }
            }

            return maxKey;
        }
    }
}