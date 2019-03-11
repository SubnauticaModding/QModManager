namespace SMLHelper.V2.Utility
{
    // Extension methods for ItemStorageHelper, could be made into a bigger class for other extendables
    public static class IStorageHelperExtensions
    {
        public static bool HasRoomCached(this ItemsContainer container, int width, int height)
        {
            return ItemStorageHelper.Helper.HasRoomCached(container, width, height);
        }

        public static bool HasRoomCached(this ItemsContainer container, Vector2int itemSize)
        {
            return ItemStorageHelper.Helper.HasRoomCached(container, itemSize);
        }

        public static bool IsEmpty(this ItemsContainer container)
        {
            return ItemStorageHelper.Helper.IsEmpty(container);
        }

        public static bool IsFull(this ItemsContainer container)
        {
            return ItemStorageHelper.Helper.IsFull(container);
        }
    }
}
