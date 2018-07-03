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

        /// <summary>
        /// The class identifier used for the <see cref="PrefabIdentifier" /> component whenever applicable.
        /// </summary>
        public readonly string ClassID;

        /// <summary>
        /// Name of the prefab file.
        /// </summary>
        public readonly string PrefabFileName;

        /// <summary>
        /// The techtype of the corresponding item.
        /// Used for the <see cref="TechTypeFixer" />, <see cref="TechTag" />, and <see cref="Constructable" /> components whenever applicable.
        /// </summary>
        public readonly TechType TechType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModPrefab" /> class.
        /// </summary>
        /// <param name="classId">The class identifier used for the <see cref="PrefabIdentifier" /> component whenever applicable.</param>
        /// <param name="prefabFileName">Name of the prefab file.</param>
        /// <param name="techType">The techtype of the corresponding item.
        /// Used for the <see cref="TechTypeFixer" />, <see cref="TechTag" />, and <see cref="Constructable" /> components whenever applicable.</param>
        protected ModPrefab(string classId, string prefabFileName, TechType techType)
        {
            ClassID = classId;
            PrefabFileName = prefabFileName;
            TechType = techType;
        }

        internal GameObject GetGameObjectInternal()
        {
            var go = GetGameObject();

            if (go == null)
            {
                return null;
            }

            go.name = ClassID;
            go.AddComponent<Fixer>().techType = TechType;

            if (go.GetComponent<PrefabIdentifier>() != null)
            {
                go.GetComponent<PrefabIdentifier>().ClassId = ClassID;
            }

            if (go.GetComponent<TechTag>() != null)
            {
                go.GetComponent<TechTag>().type = TechType;
            }

            if (go.GetComponent<Constructable>() != null)
            {
                go.GetComponent<Constructable>().techType = TechType;
            }

            go.SetActive(false);

            return go;
        }

        /// <summary>
        /// Gets the prefab game object. Set up your prefab components here.
        /// The <see cref="TechType"/> and ClassID are already handled.
        /// </summary>
        /// <returns>The game object to be instantiated into a new in-game entity.</returns>
        public abstract GameObject GetGameObject();
    }
}
