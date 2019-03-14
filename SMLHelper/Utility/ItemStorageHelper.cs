namespace SMLHelper.V2.Utility
{
    using System.Collections.Generic;

    public class ItemStorageHelper : IStorageHelper
    {
        private readonly Dictionary<ItemsContainer, Dictionary<Vector2int, bool>> HasRoomCacheCollection = new Dictionary<ItemsContainer, Dictionary<Vector2int, bool>>();

        internal static readonly ItemStorageHelper singleton = new ItemStorageHelper();

        public static IStorageHelper Main => singleton;

        #region Common Item Sizes

        private static readonly Vector2int size1x1 = new Vector2int(1, 1);
        private static readonly Vector2int size1x2 = new Vector2int(1, 2);
        private static readonly Vector2int size2x1 = new Vector2int(2, 1);
        private static readonly Vector2int size2x2 = new Vector2int(2, 2);

        private readonly IEnumerable<Vector2int> SmallerThan3x3 = new Vector2int[]
        {
            new Vector2int(3,2),
            new Vector2int(3,1),
            new Vector2int(2,3),
            size2x2,
            size2x1,
            size1x2,
            size1x1
        };

        private readonly IEnumerable<Vector2int> smallerThan2x3 = new Vector2int[]
        {
            size2x2,
            size2x1,
            size1x2,
            size1x1
        };

        private readonly IEnumerable<Vector2int> smallerThan2x2 = new Vector2int[]
        {
            size2x1,
            size1x2,
            size1x1
        };

        private readonly IEnumerable<Vector2int> just1x1 = new Vector2int[]
        {
            size1x1
        };

        #endregion

        // Used to quickly cache common sizes if a large item is found to fit a container
        private IEnumerable<Vector2int> CommonSmallerSizes(Vector2int original)
        {
            if (original.x == 3 && original.y == 3) // Size of Mobile Vehicle Bay
            {
                return SmallerThan3x3;
            }

            if (original.x == 2 && original.y == 3) // Size of Seaglide
            {
                return smallerThan2x3;
            }

            if (original.x == 2 && original.y == 2) // Size of common game items
            {
                return smallerThan2x2;
            }

            return just1x1;
        }

        private void CacheNewContainer(ItemsContainer container)
        {
            HasRoomCacheCollection.Add(container, new Dictionary<Vector2int, bool>());
        }

        internal void RecacheContainer(ItemsContainer container, bool missingCache = false)
        {
            // Items in storage have changed. Clear the related cache
            if (HasRoomCacheCollection.TryGetValue(container, out Dictionary<Vector2int, bool> cache))
            {
                cache.Clear();
            }

            else
            {
                // This is a new container we haven't seen before, save it to the cache collection
                CacheNewContainer(container);
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
                CacheNewContainer(container);
            }

            return false;
        }

        public bool HasRoomForCached(ItemsContainer container, int width, int height)
        {
            var size = new Vector2int(width, height);

            return HasRoomForCached(container, size);
        }

        public bool HasRoomForCached(ItemsContainer container, Vector2int itemSize)
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

        // This method exists for StorageContainer, but strangely not for ItemsContainer
        public bool IsEmpty(ItemsContainer container)
        {
            return container.count <= 0;
        }

        public bool IsFull(ItemsContainer container)
        {
            return !HasRoomForCached(container, size1x1);
        }
    }
}
