namespace SMLHelper.V2.Interfaces
{
    using Assets;

    /// <summary>
    /// A handler for registering Unity prefabs associated to a <see cref="TechType"/>.
    /// </summary>
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
