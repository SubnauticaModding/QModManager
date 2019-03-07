namespace SMLHelper.V2.Utility
{
	// Ease-of-use static methods for ItemStorageHelper, could be made into a bigger class for other extendables
	public static class IStorageHelperExtensions
	{
		public static bool TryGetCachedHasRoom(this IItemsContainer container, int width, int height, ref bool cachedResult)
		{
			return ItemStorageHelper.Helper.TryGetCachedHasRoom(container, width, height, ref cachedResult);
		}

		public static bool TryGetCachedHasRoom(this IItemsContainer container, Vector2int itemSize, ref bool cachedResult)
		{
			return ItemStorageHelper.Helper.TryGetCachedHasRoom(container, itemSize, ref cachedResult);
		}
	}
}
