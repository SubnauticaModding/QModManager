namespace QModManager.API.SMLHelper.Utility
{
    using System.Collections.Generic;

    /// <summary>
    /// A utility class that offers additional info about <see cref="ItemsContainer"/> instances.
    /// </summary>
    /// <seealso cref="IItemStorageHelper" />
    public class ItemStorageHelper : IItemStorageHelper
    {
        /// <summary>
        /// Main entry point for all calls to this handler.
        /// </summary>
        public static IItemStorageHelper Main => Singleton;
        internal static readonly ItemStorageHelper Singleton = new ItemStorageHelper();

        private readonly Dictionary<ItemsContainer, Dictionary<Vector2int, bool>> HasRoomCacheCollection = new Dictionary<ItemsContainer, Dictionary<Vector2int, bool>>();

        private ItemStorageHelper() { }

        #region Common Item Sizes

        private static readonly Vector2int Size1x1 = new Vector2int(1, 1);
        private static readonly Vector2int Size1x2 = new Vector2int(1, 2);
        private static readonly Vector2int Size2x1 = new Vector2int(2, 1);
        private static readonly Vector2int Size2x2 = new Vector2int(2, 2);
        private static readonly Vector2int Size2x3 = new Vector2int(2, 3);
        private static readonly Vector2int Size3x1 = new Vector2int(3, 1);
        private static readonly Vector2int Size3x2 = new Vector2int(3, 2);

        private static readonly IEnumerable<Vector2int> SmallerThan3x3 = new Vector2int[7]
        {
            Size3x2,
            Size3x1,
            Size2x3,
            Size2x2,
            Size2x1,
            Size1x2,
            Size1x1
        };

        private static readonly IEnumerable<Vector2int> SmallerThan2x3 = new Vector2int[4]
        {
            Size2x2,
            Size2x1,
            Size1x2,
            Size1x1
        };

        private static readonly IEnumerable<Vector2int> SmallerThan2x2 = new Vector2int[3]
        {
            Size2x1,
            Size1x2,
            Size1x1
        };

        private static readonly IEnumerable<Vector2int> Just1x1 = new Vector2int[1]
        {
            Size1x1
        };

        private IEnumerable<Vector2int> CommonSmallerSizes(Vector2int original)
        {
            if (original.x == 2)
            {
                if (original.y == 2)
                {
                    return SmallerThan2x2;
                }

                if (original.y > 2)
                {
                    return SmallerThan2x3;
                }
            }

            if (original.x >= 3 && original.y >= 3)
            {
                return SmallerThan3x3;
            }

            return Just1x1;
        }

        #endregion

        internal void ClearContainerCache(ItemsContainer container, bool missingCache = false)
        {
            // Items in storage have changed. Clear the related cache
            if (HasRoomCacheCollection.TryGetValue(container, out Dictionary<Vector2int, bool> cache))
            {
                cache.Clear();
            }
            else
            {
                // This is a new container we haven't seen before, save it to the cache collection
                HasRoomCacheCollection.Add(container, new Dictionary<Vector2int, bool>());
            }

            // Can technically be simplified (with a micro performance hit):
            // this.HasRoomCacheCollection[container] = new Dictionary<Vector2int, bool>();
        }

        internal void CacheNewHasRoomData(ItemsContainer container, Vector2int itemSize, bool hasRoom)
        {
            HasRoomCacheCollection[container][itemSize] = hasRoom;

            // If item fits and is larger than 1x1, cache common sizes as true
            if (hasRoom && (itemSize.x > 1 || itemSize.y > 1))
            {
                foreach (Vector2int size in CommonSmallerSizes(itemSize))
                {
                    HasRoomCacheCollection[container][size] = true;
                }
            }
        }

        // Called by ItemsContainer.HasRoom via Harmony
        internal bool TryGetCachedHasRoom(ItemsContainer container, Vector2int itemSize, ref bool cachedResult)
        {
            if (HasRoomCacheCollection.TryGetValue(container, out Dictionary<Vector2int, bool> cache))
            {
                if (cache.TryGetValue(itemSize, out cachedResult))
                {
                    return true;
                }

                // If no value is cached, the vanilla method will assign one
            }
            else
            {
                // This is a new container we haven't seen before, save it to the cache collection
                HasRoomCacheCollection.Add(container, new Dictionary<Vector2int, bool>());
            }

            return false;
        }

        #region Static Methods

        /// <summary>
        /// Using the cached container info, determines whether the specified container has room for an item of the specified size.
        /// </summary>
        /// <param name="container">The container to check.</param>
        /// <param name="width">The item width.</param>
        /// <param name="height">The item height.</param>
        /// <returns>
        ///   <c>true</c> if there is room for the item in the container,; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasRoomCached(ItemsContainer container, int width, int height)
        {
            return Main.HasRoomForCached(container, width, height);
        }

        /// <summary>
        /// Using the cached container info, determines whether the specified container has room for an item of the specified size.
        /// </summary>
        /// <param name="container">The container to check.</param>
        /// <param name="itemSize">Size of the item.</param>
        /// <returns>
        ///   <c>true</c> if there is room for the item in the container,; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasRoomCached(ItemsContainer container, Vector2int itemSize)
        {
            return Main.HasRoomForCached(container, itemSize);
        }

        /// <summary>
        /// Determines whether the specified container is empty.
        /// </summary>
        /// <param name="container">The items container to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified container is empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEmpty(ItemsContainer container)
        {
            return Main.IsEmpty(container);
        }

        /// <summary>
        /// Determines whether the specified container is full.
        /// </summary>
        /// <param name="container">The items container to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified container is full; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFull(ItemsContainer container)
        {
            return Main.IsFull(container);
        }

        /// <summary>
        /// The totals number of 1x1 slots in the container, as calculated by the container's width and height.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns></returns>
        public static int GetTotalSlots(ItemsContainer container)
        {
            return Main.GetTotalSlots(container);
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
            return Main.GetStorageLabel(container);
        }

        /// <summary>
        /// Gets the set of techtypes allowed in  container. This set can be altered.
        /// If the set is null or empty, then all items can be added.
        /// </summary>
        /// <param name="container">The container to check.</param>
        /// <returns>
        /// The collection of techtypes allowed in the container.
        /// </returns>
        public static HashSet<TechType> GetAllowedTechTypes(ItemsContainer container)
        {
            return Main.GetAllowedTechTypes(container);
        }

        #endregion

        #region Interface Methods

        /// <summary>
        /// Using the cached container info, determines whether the specified container has room for an item of the specified size.
        /// </summary>
        /// <param name="container">The container to check.</param>
        /// <param name="width">The item width.</param>
        /// <param name="height">The item height.</param>
        /// <returns>
        ///   <c>true</c> if there is room for the item in the container,; otherwise, <c>false</c>.
        /// </returns>
        bool IItemStorageHelper.HasRoomForCached(ItemsContainer container, int width, int height)
        {
            return Main.HasRoomForCached(container, new Vector2int(width, height));
        }

        /// <summary>
        /// Using the cached container info, determines whether the specified container has room for an item of the specified size.
        /// </summary>
        /// <param name="container">The container to check.</param>
        /// <param name="itemSize">Size of the item.</param>
        /// <returns>
        ///   <c>true</c> if there is room for the item in the container,; otherwise, <c>false</c>.
        /// </returns>
        bool IItemStorageHelper.HasRoomForCached(ItemsContainer container, Vector2int itemSize)
        {
            if (HasRoomCacheCollection.TryGetValue(container, out Dictionary<Vector2int, bool> cache)
                && cache.TryGetValue(itemSize, out bool hasRoom))
            {
                // Return the cached result
                return hasRoom;
            }
            else
            {
                // Return the normal result, it will be cached for next time
                return container.HasRoomFor(itemSize.x, itemSize.y);
            }
        }

        /// <summary>
        /// Determines whether the specified container is empty.
        /// </summary>
        /// <param name="container">The items container to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified container is empty; otherwise, <c>false</c>.
        /// </returns>
        bool IItemStorageHelper.IsEmpty(ItemsContainer container)
        {
            // This method exists for StorageContainer, but strangely not for ItemsContainer
            return container.count <= 0;
        }

        /// <summary>
        /// Determines whether the specified container is full.
        /// </summary>
        /// <param name="container">The items container to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified container is full; otherwise, <c>false</c>.
        /// </returns>
        bool IItemStorageHelper.IsFull(ItemsContainer container)
        {
            return !Main.HasRoomForCached(container, Size1x1);
        }

        /// <summary>
        /// The totals number of 1x1 slots in the container, as calculated by the container's width and height.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns></returns>
        int IItemStorageHelper.GetTotalSlots(ItemsContainer container)
        {
            return container.sizeX * container.sizeY;
        }

        /// <summary>
        /// Get the inernal label for the storage container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>
        /// The label used and displayed in-game for the container.
        /// </returns>
        string IItemStorageHelper.GetStorageLabel(ItemsContainer container)
        {
            return container._label;
        }

        /// <summary>
        /// Gets the set of techtypes allowed in this container. This set can be altered.
        /// If the set is null or empty, then all items can be added.
        /// </summary>
        /// <param name="container">The container to check.</param>
        /// <returns>
        /// The collection of techtypes allowed in the container.
        /// </returns>
        HashSet<TechType> IItemStorageHelper.GetAllowedTechTypes(ItemsContainer container)
        {
            return container.allowedTech;
        }

        #endregion
    }
}
