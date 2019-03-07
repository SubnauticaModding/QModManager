namespace SMLHelper.V2.Utility
{
	using System.Collections.Generic;

	public class ItemStorageHelper : IStorageHelper
	{
		private readonly Dictionary<IItemsContainer, Dictionary<Vector2int, bool>> HasRoomCacheCollection = new Dictionary<IItemsContainer, Dictionary<Vector2int, bool>>();

		public static readonly ItemStorageHelper Helper = new ItemStorageHelper();

		private readonly IEnumerable<Vector2int> SmallerThan3x3 = new Vector2int[]
		{
			new Vector2int(3,2),
			new Vector2int(3,1),
			new Vector2int(2,3),
			new Vector2int(2,2),
			new Vector2int(2,1),
			new Vector2int(1,2),
			new Vector2int(1,1)
		};

		private readonly IEnumerable<Vector2int> SmallerThan2x3 = new Vector2int[]
		{
			new Vector2int(2,2),
			new Vector2int(2,1),
			new Vector2int(1,2),
			new Vector2int(1,1)
		};

		private readonly IEnumerable<Vector2int> SmallerThan2x2 = new Vector2int[]
		{
			new Vector2int(2,1),
			new Vector2int(1,2),
			new Vector2int(1,1)
		};

		private readonly IEnumerable<Vector2int> Just1x1 = new Vector2int[]
		{
			new Vector2int(1,1)
		};

		// Used to quickly cache common sizes if a large item is found to fit a container
		private IEnumerable<Vector2int> CommonSmallerSizes(Vector2int original)
		{
			if (original.x == 3 && original.y == 3) // Size of Mobile Vehicle Bay
			{
				return this.SmallerThan3x3;
			}

			if (original.x == 2 && original.y == 3) // Size of Seaglide
			{
				return this.SmallerThan2x3;
			}

			if (original.x == 2 && original.y == 2) // Size of common game items
			{
				return this.SmallerThan2x2;
			}

			return this.Just1x1;
		}

		private void CacheNewContainer(IItemsContainer container)
		{
			this.HasRoomCacheCollection.Add(container, new Dictionary<Vector2int, bool>());
		}

		internal void RecacheContainer(IItemsContainer container)
		{
			// Items in storage have changed. Clear the related cache.
			if (this.HasRoomCacheCollection.TryGetValue(container, out Dictionary<Vector2int, bool> cache))
			{
				cache.Clear();
			}

			else
			{
				// This is a new container we haven't seen before, save it to the cache collection.
				this.CacheNewContainer(container);
			}
		}

		internal void CacheNewHasRoomData(IItemsContainer container, Vector2int itemSize, bool hasRoom)
		{
			this.HasRoomCacheCollection[container][itemSize] = hasRoom;

			// If item fits and is larger than 1x1, cache common sizes as true
			if (hasRoom && (itemSize.x > 1 || itemSize.y > 1))
			{
				foreach (Vector2int size in this.CommonSmallerSizes(itemSize))
				{
					this.HasRoomCacheCollection[container][size] = true;
				}
			}
		}

		public bool TryGetCachedHasRoom(IItemsContainer container, int width, int height, ref bool cachedResult)
		{
			Vector2int size = new Vector2int(width, height);

			return this.TryGetCachedHasRoom(container, size, ref cachedResult);
		}

		public bool TryGetCachedHasRoom(IItemsContainer container, Vector2int itemSize, ref bool cachedResult)
		{
			if (this.HasRoomCacheCollection.TryGetValue(container, out Dictionary<Vector2int, bool> cache))
			{
				// Container has existing cache
				if (cache.TryGetValue(itemSize, out cachedResult))
				{
					return false; // Detour original code to avoid waste
				}
			}

			else
			{
				// Container is not cached, save it to the cache collection
				this.CacheNewContainer(container);
			}

			return true; // Let vanilla code run and cache the result
		}
	}
}
