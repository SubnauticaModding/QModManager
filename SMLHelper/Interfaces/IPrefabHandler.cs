namespace SMLHelper.V2.Interfaces
{
    using Assets;

    public interface IPrefabHandler
    {
        /// <summary>
        /// Registers a ModPrefab into the game.
        /// </summary>
        /// <param name="prefab">The mod prefab to register. Create a child class inheriting off this one and configure as needed.</param>
        /// <seealso cref="ModPrefab"/>
        void RegisterPrefab(ModPrefab prefab);
    }
}
