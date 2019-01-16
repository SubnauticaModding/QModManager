namespace SMLHelper.V2.Handlers
{
    using Assets;

    public static class PrefabHandler
    {
        /// <summary>
        /// Registers a ModPrefab into the game.
        /// </summary>
        /// <param name="prefab">The mod prefab to register. Create a child class inheriting off this one and configure as needed.</param>
        /// <seealso cref="ModPrefab"/>
        public static void RegisterPrefab(ModPrefab prefab)
        {
            foreach(ModPrefab modPrefab in ModPrefab.Prefabs)
            {
                if (modPrefab.TechType == prefab.TechType || modPrefab.ClassID == prefab.ClassID || modPrefab.PrefabFileName == prefab.PrefabFileName)
                    return;
            }

            ModPrefab.Add(prefab);
        }
    }
}
