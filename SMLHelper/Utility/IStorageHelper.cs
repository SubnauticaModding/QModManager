namespace SMLHelper.V2.Utility
{
    public interface IStorageHelper
    {
        // Expose singleton here

        bool HasRoomCached(ItemsContainer container, int width, int height);

        bool HasRoomCached(ItemsContainer container, Vector2int itemSize);

        bool IsEmpty(ItemsContainer container);

        bool IsFull(ItemsContainer container);
    }
}
