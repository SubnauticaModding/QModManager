namespace SMLHelper.V2
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal class PatchUtils
    {
        internal static void PatchDictionary<K, V>(Type type, string name, IDictionary<K, V> dictionary)
        {
            PatchDictionary(type, name, dictionary, BindingFlags.NonPublic | BindingFlags.Static);
        }

        internal static void PatchDictionary<K, V>(Type type, string name, IDictionary<K, V> dictionary, BindingFlags flags)
        {
            FieldInfo dictionaryField = type.GetField(name, flags);
            var dict = dictionaryField.GetValue(null) as IDictionary<K, V>;

            foreach (KeyValuePair<K, V> entry in dictionary)
            {
                dict[entry.Key] = entry.Value;
            }
        }

        internal static void PatchList<T>(Type type, string name, IList<T> list)
        {
            PatchList(type, name, list, BindingFlags.NonPublic | BindingFlags.Static);
        }

        internal static void PatchList<T>(Type type, string name, IList<T> list, BindingFlags flags)
        {
            FieldInfo listField = type.GetField(name, flags);
            var craftDataList = listField.GetValue(null) as IList<T>;

            foreach (T obj in list)
            {
                craftDataList.Add(obj);
            }
        }
    }
}
