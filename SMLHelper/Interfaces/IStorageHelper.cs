namespace SMLHelper.V2.Utility
{
    using System.Collections.Generic;

    /// <summary>
    /// A utility class that offers additional info about <see cref="ItemsContainer"/> instances.
    /// </summary>
    /// <seealso cref="StorageHelperExtensions" />
    public interface IStorageHelper
    {
        /// <summary>
        /// Using the cached container info, determines whether the specified container has room for an item of the specified size.
        /// </summary>
        /// <param name="container">The container to check.</param>
        /// <param name="width">The item width.</param>
        /// <param name="height">The item height.</param>
        /// <returns>
        ///   <c>true</c> if there is room for the item in the container,; otherwise, <c>false</c>.
        /// </returns>
        bool HasRoomForCached(ItemsContainer container, int width, int height);

        /// <summary>
        /// Using the cached container info, determines whether the specified container has room for an item of the specified size.
        /// </summary>
        /// <param name="container">The container to check.</param>
        /// <param name="itemSize">Size of the item.</param>
        /// <returns>
        ///   <c>true</c> if there is room for the item in the container,; otherwise, <c>false</c>.
        /// </returns>
        bool HasRoomForCached(ItemsContainer container, Vector2int itemSize);

        /// <summary>
        /// Determines whether the specified container is empty.
        /// </summary>
        /// <param name="container">The items container to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified container is empty; otherwise, <c>false</c>.
        /// </returns>
        bool IsEmpty(ItemsContainer container);

        /// <summary>
        /// Determines whether the specified container is full.
        /// </summary>
        /// <param name="container">The items container to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified container is full; otherwise, <c>false</c>.
        /// </returns>
        bool IsFull(ItemsContainer container);

        /// <summary>
        /// Get the totals number of 1x1 slots in the container, as calculated by the container's width and height.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>The total number of slots in the container.</returns>
        int GetTotalSlots(ItemsContainer container);

        /// <summary>
        /// Get the inernal label for the storage container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>The label used and displayed in-game for the container.</returns>
        string GetStorageLabel(ItemsContainer container);

        /// <summary>
        /// Gets the set of techtypes allowed in this container. This set can be altered.
        /// If the set is null or empty, then all items can be added.
        /// </summary>
        /// <param name="container">The container to check.</param>
        /// <returns>The collection of techtypes allowed in the container.</returns>
        HashSet<TechType> GetAllowedTechTypes(ItemsContainer container);
    }
}
