namespace SMLHelper.V2.Utility
{
	public interface IStorageHelper
	{
		bool TryGetCachedHasRoom(IItemsContainer container, int width, int height, ref bool hasRoom);

		bool TryGetCachedHasRoom(IItemsContainer container, Vector2int itemSize, ref bool hasRoom);
	}
}
