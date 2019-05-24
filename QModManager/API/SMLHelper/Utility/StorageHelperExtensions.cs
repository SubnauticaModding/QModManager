namespace QModManager.API.SMLHelper.Utility
{
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods to provide static calls for <see cref="ItemsContainer"/> into <see cref="ItemStorageHelper"/> methods.
    /// </summary>
    public static class StorageHelperExtensions
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
        public static bool HasRoomCached(this ItemsContainer container, int width, int height)
        {
            return ItemStorageHelper.Main.HasRoomForCached(container, width, height);
        }

        /// <summary>
        /// Using the cached container info, determines whether the specified container has room for an item of the specified size.
        /// </summary>
        /// <param name="container">The container to check.</param>
        /// <param name="itemSize">Size of the item.</param>
        /// <returns>
        ///   <c>true</c> if there is room for the item in the container,; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasRoomCached(this ItemsContainer container, Vector2int itemSize)
        {
            return ItemStorageHelper.Main.HasRoomForCached(container, itemSize);
        }

        /// <summary>
        /// Determines whether the specified container is empty.
        /// </summary>
        /// <param name="container">The items container to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified container is empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEmpty(this ItemsContainer container)
        {
            return ItemStorageHelper.Main.IsEmpty(container);
        }

        /// <summary>
        /// Determines whether the specified container is full.
        /// </summary>
        /// <param name="container">The items container to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified container is full; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFull(this ItemsContainer container)
        {
            return ItemStorageHelper.Main.IsFull(container);
        }

        /// <summary>
        /// The totals number of 1x1 slots in the container, as calculated by the container's width and height.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns></returns>
        public static int GetTotalSlots(ItemsContainer container)
        {
            return ItemStorageHelper.Main.GetTotalSlots(container);
        }

        /// <summary>
        /// Get the inernal label for the storage container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>
        /// The label used and displayed in-game for the container.
        /// </returns>
        public static string GetStorageLabel(ItemsContainer container)
        {
            return ItemStorageHelper.Main.GetStorageLabel(container);
        }

        /// <summary>
        /// Gets the set of techtypes allowed in this container. This set can be altered.
        /// If the set is null or empty, then all items can be added.
        /// </summary>
        /// <param name="container">The container to check.</param>
        /// <returns>
        /// The collection of techtypes allowed in the container.
        /// </returns>
        public static HashSet<TechType> GetAllowedTechTypes(ItemsContainer container)
        {
            return ItemStorageHelper.Main.GetAllowedTechTypes(container);
        }
    }
}
