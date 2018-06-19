namespace SMLHelper.V2.Handlers
{
    using Assets;

    public class PrefabHandler
    {
        /// <summary>
        /// Registers a ModPrefab into the game.
        /// </summary>
        /// <param name="prefab"></param>
        /// <seealso cref="ModPrefab"/>
        public static void RegisterPrefab(ModPrefab prefab)
        {
            foreach(var modPrefab in ModPrefab.Prefabs)
            {
                if (modPrefab.TechType == prefab.TechType || modPrefab.ClassID == prefab.ClassID || modPrefab.PrefabFileName == prefab.PrefabFileName)
                    return;
            }

            ModPrefab.Prefabs.Add(prefab);
        }
    }
}
