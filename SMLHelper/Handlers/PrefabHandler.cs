namespace SMLHelper.V2.Handlers
{
    using Assets;
    using Interfaces;

    /// <summary>
    /// A handler for registering Unity prefabs associated to a <see cref="TechType"/>.
    /// </summary>
    public class PrefabHandler : IPrefabHandler
    {
        /// <summary>
        /// Main entry point for all calls to this handler.
        /// </summary>
        public static IPrefabHandler Main { get; } = new PrefabHandler();

        private PrefabHandler()
        {
            // Hide constructor
        }

        /// <summary>
        /// Registers a ModPrefab into the game.
        /// </summary>
        /// <param name="prefab">The mod prefab to register. Create a child class inheriting off this one and configure as needed.</param>
        /// <seealso cref="ModPrefab"/>
        void IPrefabHandler.RegisterPrefab(ModPrefab prefab)
        {
            foreach (ModPrefab modPrefab in ModPrefab.Prefabs)
            {
                if (modPrefab.TechType == prefab.TechType || modPrefab.ClassID == prefab.ClassID || modPrefab.PrefabFileName == prefab.PrefabFileName)
                    return;
            }

            ModPrefab.Add(prefab);
        }

        /// <summary>
        /// Registers a ModPrefab into the game.
        /// </summary>
        /// <param name="prefab">The mod prefab to register. Create a child class inheriting off this one and configure as needed.</param>
        /// <seealso cref="ModPrefab"/>
        public static void RegisterPrefab(ModPrefab prefab)
        {
            Main.RegisterPrefab(prefab);
        }
    }
}
