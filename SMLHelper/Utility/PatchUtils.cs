namespace SMLHelper.V2
{
    using System;
    using System.Collections;
    using System.Reflection;

    internal class PatchUtils
    {
        internal static void PatchDictionary(Type type, string name, IDictionary dictionary)
        {
            PatchDictionary(type, name, dictionary, BindingFlags.NonPublic | BindingFlags.Static);
        }

        internal static void PatchDictionary(Type type, string name, IDictionary dictionary, BindingFlags flags)
        {
            FieldInfo dictionaryField = type.GetField(name, flags);
            var dict = dictionaryField.GetValue(null) as IDictionary;

            foreach(DictionaryEntry entry in dictionary)
            {
                dict[entry.Key] = entry.Value;
            }
        }

        internal static void PatchList(Type type, string name, IList list)
        {
            PatchList(type, name, list, BindingFlags.NonPublic | BindingFlags.Static);
        }

        internal static void PatchList(Type type, string name, IList list, BindingFlags flags)
        {
            FieldInfo listField = type.GetField(name, flags);
            var craftDataList = listField.GetValue(null) as IList;

            foreach (object obj in list)
            {
                craftDataList.Add(obj);
            }
        }
    }
}
