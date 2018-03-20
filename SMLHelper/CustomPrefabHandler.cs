using System.Collections.Generic;

namespace SMLHelper
{
    public class CustomPrefabHandler
    {
        public static List<CustomPrefab> customPrefabs = new List<CustomPrefab>();
    }

    public class CustomPrefab
    {
        public string ClassID;
        public string PrefabFileName;

        public TechType TechType;
        public UnityEngine.Object Object;

        public CustomPrefab(string classId, string prefabFileName, UnityEngine.Object _object, TechType techType)
        {
            ClassID = classId;
            PrefabFileName = prefabFileName;
            TechType = techType;
            Object = _object;
        }
    }
}
