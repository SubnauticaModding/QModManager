namespace SMLHelper.V2.Utility
{
    // Extension methods for ItemStorageHelper, could be made into a bigger class for other extendables
    public static class IStorageHelperExtensions
    {
        public static bool HasRoomCached(this ItemsContainer container, int width, int height)
        {
            return ItemStorageHelper.singleton.HasRoomForCached(container, width, height);
        }

        public static bool HasRoomCached(this ItemsContainer container, Vector2int itemSize)
        {
            return ItemStorageHelper.singleton.HasRoomForCached(container, itemSize);
        }

        public static bool IsEmpty(this ItemsContainer container)
        {
            return ItemStorageHelper.singleton.IsEmpty(container);
        }

        public static bool IsFull(this ItemsContainer container)
        {
            return ItemStorageHelper.singleton.IsFull(container);
        }
    }
}
