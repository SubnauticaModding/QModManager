namespace SMLHelper.V2.Assets
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class ModPrefab
    {
        internal static List<ModPrefab> Prefabs = new List<ModPrefab>();

        public string ClassID;
        public string PrefabFileName;

        public TechType TechType;

        public ModPrefab(string classId, string prefabFileName, TechType techType)
        {
            ClassID = classId;
            PrefabFileName = prefabFileName;
            TechType = techType;
        }

        public abstract GameObject GetGameObject();
    }
}
