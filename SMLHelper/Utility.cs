using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace SMLHelper
{
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

        public static void PatchDictionary(Type type, string name, IDictionary dictionary)
        {
            PatchDictionary(type, name, dictionary, BindingFlags.NonPublic | BindingFlags.Static);
        }

        public static void PatchDictionary(Type type, string name, IDictionary dictionary, BindingFlags flags)
        {
            var dictionaryField = type.GetField(name, flags);
            var craftDataDict = dictionaryField.GetValue(null) as IDictionary;

            PatchDictionaryInternal(craftDataDict, dictionary);
        }

        private static void PatchDictionaryInternal(IDictionary origDict, IDictionary additions)
        {
            foreach(DictionaryEntry entry in additions)
            {
                if(typeof(IDictionary).IsAssignableFrom(entry.Value.GetType()))
                {
                    foreach(DictionaryEntry origDictEntry in origDict)
                    {
                        if (origDictEntry.Key.GetType().Equals(entry.GetType()))
                            PatchDictionaryInternal((IDictionary)origDictEntry.Value, (IDictionary)entry.Value);
                    }
                }

                if(typeof(IList).IsAssignableFrom(entry.Value.GetType()))
                {
                    foreach(DictionaryEntry origDictEntry in origDict)
                    {
                        if (origDictEntry.Key.GetType().Equals(entry.Value.GetType()))
                            PatchListInternal((IList)origDictEntry.Value, (IList)entry.Value);
                    }
                }

                origDict[entry.Key] = entry.Value;
            }
        }

        private static void PatchListInternal(IList orig, IList additions)
        {
            foreach(var addition in additions)
            {
                if (typeof(IList).IsAssignableFrom(addition.GetType()))
                {
                    foreach(var origEntry in orig)
                    {
                        if (origEntry.GetType().Equals(addition.GetType()))
                            PatchListInternal((IList)origEntry, (IList)addition);
                    }
                }

                orig.Add(addition);
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
