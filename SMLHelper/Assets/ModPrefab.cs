namespace SMLHelper.V2.Assets
{
    using System.Collections.Generic;
    using UnityEngine;
    using MonoBehaviours;

    /// <summary>
    /// The abstract class to inherit when you want to add new PreFabs into the game.
    /// </summary>
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

        internal GameObject GetGameObjectInternal()
        {
            var go = GetGameObject();

            go.name = ClassID;

            if(go.GetComponent<PrefabIdentifier>() != null)
            {
                go.GetComponent<PrefabIdentifier>().ClassId = ClassID;
            }

            if(go.GetComponent<TechTag>() != null)
            {
                go.GetComponent<TechTag>().type = TechType;
            }

            if(go.GetComponent<Constructable>() != null)
            {
                go.GetComponent<Constructable>().techType = TechType;
            }

            go.AddComponent<TechTypeFixer>().techType = TechType;

            return go;
        }

        public abstract GameObject GetGameObject();
    }
}
