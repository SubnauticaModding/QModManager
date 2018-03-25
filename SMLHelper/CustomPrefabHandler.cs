using System.Collections.Generic;
using UnityEngine;

namespace SMLHelper
{
    public delegate GameObject GetResource();

    public class CustomPrefabHandler
    {
        public static List<CustomPrefab> customPrefabs = new List<CustomPrefab>();
    }

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
}
