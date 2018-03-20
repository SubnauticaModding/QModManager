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
                //As long as we have the type we wanna check, and the instance - "is" would do the trick
                if (entry.Value is IDictionary)
                {
                    //You have to check the KEYS for being equal. Not just their type, since TechType.Titanium and TechType.AcidMushroom has equal types.
                    foreach (DictionaryEntry origDictEntry in origDict)
                    {
                        if (origDictEntry.Key.Equals(entry.Key))
                        {
                            PatchDictionaryInternal((IDictionary)origDictEntry.Value, (IDictionary)entry.Value);
                        }
                    }
                }

                if (entry.Value is IList)
                {
                    foreach (DictionaryEntry origDictEntry in origDict)
                    {
                        //Ok, see - here is the problem. You were updating original list. Right?
                        //Adding a new value to it.
                        if (origDictEntry.Key.Equals(entry.Key))
                        {
                            //Now - we are modifying the entry list, so it would contain all the default data.
                            //There should be over way, but it would be way overcoded, and I didn't come to a better solution....
                            PatchListInternal((IList)entry.Value,(IList)origDictEntry.Value);
                        }
                    }
                }
                //But here - you were setting the value of the original dict to UNMODIFIED entry list.
                //While the original value already modified....
                origDict[entry.Key] = entry.Value;
            }
        }

        private static void PatchListInternal(IList orig, IList additions)
        {
            foreach (var addition in additions)
            {
                if (addition is IList)
                {
                    foreach (var origEntry in orig)
                    {
                        if (origEntry.GetType() == addition.GetType())
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
