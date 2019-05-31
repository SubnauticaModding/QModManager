namespace QModManager.API.SMLHelper.Interfaces
{
    using Assets;
    using Handlers;

    /// <summary>
    /// Interface for <see cref="PrefabHandler"/> <para/>
    /// Can be used for dependency injection
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
