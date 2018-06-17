using System.Collections.Generic;
using UnityEngine;
using CustomPrefabHandler2 = SMLHelper.V2.CustomPrefabHandler;
using CustomPrefab2 = SMLHelper.V2.CustomPrefab;

namespace SMLHelper
{
    public delegate GameObject GetResource();

    [System.Obsolete("SMLHelper.CustomPrefabHandler is obsolete. Please use SMLHelper.V2 instead.")]
    public class CustomPrefabHandler
    {
        [System.Obsolete("SMLHelper.CustomPrefabHandler.customPrefabs is obsolete. Please use SMLHelper.V2 instead.")]
        public static List<CustomPrefab> customPrefabs = new List<CustomPrefab>();

        internal static void Patch()
        {
            customPrefabs.ForEach(x => CustomPrefabHandler2.customPrefabs.Add(x.GetV2CustomPrefab()));
        }
    }

    [System.Obsolete("SMLHelper.CustomPrefab is obsolete. Please use SMLHelper.V2 instead.")]
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

        internal CustomPrefab2 GetV2CustomPrefab()
        {
            var customPrefab = new CustomPrefab2(ClassID, PrefabFileName, Object, TechType);
            customPrefab.GetResourceDelegate = new V2.GetResource(GetResourceDelegate);

            return customPrefab;
        }
    }
}
