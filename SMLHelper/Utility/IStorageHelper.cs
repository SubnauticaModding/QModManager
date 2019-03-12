namespace SMLHelper.V2.Utility
{
    public interface IStorageHelper
    {
        bool HasRoomForCached(ItemsContainer container, int width, int height);

        bool HasRoomForCached(ItemsContainer container, Vector2int itemSize);

        bool IsEmpty(ItemsContainer container);

        bool IsFull(ItemsContainer container);
    }
}
