namespace SMLHelper.V2
{
    using System.Collections.Generic;

    internal class PatchUtils
    {
        internal static void PatchDictionary<KeyType, ValueType>(Dictionary<KeyType, ValueType> original, IDictionary<KeyType, ValueType> patches)
        {
            foreach (KeyValuePair<KeyType, ValueType> entry in patches)
            {
                original[entry.Key] = entry.Value;
            }
        }

        internal static void PatchList<ValueType>(List<ValueType> original, IList<ValueType> patches)
        {
            foreach (ValueType entry in patches)
            {
                original.Add(entry);
            }
        }
    }
}
