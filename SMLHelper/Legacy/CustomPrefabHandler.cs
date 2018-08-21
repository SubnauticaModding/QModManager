#pragma warning disable CS0618 // Type or member is obsolete
using System.Collections.Generic;
using UnityEngine;
using SMLHelper.V2.Assets;

namespace SMLHelper
{
    public delegate GameObject GetResource();

    [System.Obsolete("Use SMLHelper.V2 instead.")]
    public class CustomPrefabHandler
    {
        [System.Obsolete("Use SMLHelper.V2 instead.")]
        public static List<CustomPrefab> customPrefabs = new List<CustomPrefab>();

        internal static void Patch()
        {
            customPrefabs.ForEach(x => ModPrefab.Add(new WrapperPrefab(x)));
        }
    }

    [System.Obsolete("Use SMLHelper.V2 instead.")]
    public class CustomPrefab
    {
        public GetResource GetResourceDelegate;

        public string ClassID;
        public string PrefabFileName;

        public TechType TechType;
        private UnityEngine.Object Object;

        public CustomPrefab(string classId, string prefabFileName, UnityEngine.Object _object, TechType techType)
        {
            ClassID = classId;
            PrefabFileName = prefabFileName;
            TechType = techType;
            Object = _object;
        }

        public CustomPrefab(string classId, string prefabFileName, TechType techType, GetResource getResourceDelegate)
        {
            ClassID = classId;
            PrefabFileName = prefabFileName;
            TechType = techType;
            Object = null;

            GetResourceDelegate = getResourceDelegate;
        }

        public UnityEngine.Object GetResource()
        {
            if (Object != null) return Object;
            else return GetResourceDelegate.Invoke();
        }
    }

    internal class WrapperPrefab : ModPrefab
    {
        internal CustomPrefab Prefab;

        internal WrapperPrefab(CustomPrefab prefab) : base(prefab.ClassID, prefab.PrefabFileName, prefab.TechType)
        {
            Prefab = prefab;
        }

        public override GameObject GetGameObject()
        {
            return Prefab.GetResource() as GameObject;
        }
    }
}
