namespace SMLHelper.V2
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.IO;
    using UnityEngine;

    public class Utility
    {
        public static void AddBasicComponents(ref GameObject _object, string classId)
        {
            var rb = _object.AddComponent<Rigidbody>();
            _object.AddComponent<PrefabIdentifier>().ClassId = classId;
            _object.AddComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Near;
            var rend = _object.GetComponentInChildren<Renderer>();
            rend.material.shader = Shader.Find("MarmosetUBER");
            var applier = _object.AddComponent<SkyApplier>();
            applier.renderers = new Renderer[] { rend };
            applier.anchorSky = Skies.Auto;
            var forces = _object.AddComponent<WorldForces>();
            forces.useRigidbody = rb;
        }

        public static string GetCurrentSaveDataDir()
        {
            return Path.Combine(@"./SNAppData/SavedGames/", Utils.GetSavegameDir());
        }

        public static void PatchDictionary(Type type, string name, IDictionary dictionary)
        {
            PatchDictionary(type, name, dictionary, BindingFlags.NonPublic | BindingFlags.Static);
        }

        public static void PatchDictionary(Type type, string name, IDictionary dictionary, BindingFlags flags)
        {
            var dictionaryField = type.GetField(name, flags);
            var dict = dictionaryField.GetValue(null) as IDictionary;

            foreach(DictionaryEntry entry in dictionary)
            {
                dict.Add(entry.Key, entry.Value);
            }
        }

        public static void PatchList(Type type, string name, IList list)
        {
            PatchList(type, name, list, BindingFlags.NonPublic | BindingFlags.Static);
        }

        public static void PatchList(Type type, string name, IList list, BindingFlags flags)
        {
            var listField = type.GetField(name, flags);
            var craftDataList = listField.GetValue(null) as IList;

            foreach (var obj in list)
            {
                craftDataList.Add(obj);
            }
        }
    }
}
