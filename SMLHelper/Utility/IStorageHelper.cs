namespace SMLHelper.V2.Utility
{
	interface IStorageHelper
	{
		void CacheNewContainer(IItemsContainer container);

		void RecacheContainer(IItemsContainer container);

		void CacheNewHasRoomData(IItemsContainer container, Vector2int itemSize, bool hasRoom);

		bool GetCachedHasRoomData(IItemsContainer container, Vector2int itemSize, ref bool __result);
	}
}
