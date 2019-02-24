namespace SMLHelper.V2.Patchers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Harmony;

    // This entire patcher is only here for performance reasons.
    // This is not intended to be exposed by the public API.
    internal static class ItemsContainerPatcher
    {
        private static Dictionary<IItemsContainer, Dictionary<Vector2int, bool>> HasRoomCacheCollection = new Dictionary<IItemsContainer, Dictionary<Vector2int, bool>>();

        internal static void Patch(HarmonyInstance harmony)
        {
            // Original methods
            Type itemsContainerType = typeof(ItemsContainer); // The class for all item storages
            MethodInfo HasRoomFor_XY_Method = itemsContainerType.GetMethod("HasRoomFor", new Type[] { typeof(int), typeof(int) });
            MethodInfo NotifyAddItem_Method = itemsContainerType.GetMethod("NotifyAddItem", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo NotifyRemoveItem_Method = itemsContainerType.GetMethod("NotifyRemoveItem", BindingFlags.NonPublic | BindingFlags.Instance);

            // Patcher methods
            Type patcherType = typeof(ItemsContainerPatcher); // This class
            MethodInfo HasRoomFor_XY_Prefix_Method = patcherType.GetMethod("HasRoomFor_XY_Prefix", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo HasRoomFor_Postfix_Method = patcherType.GetMethod("HasRoomFor_Postfix", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo NotifyChangeItem_Postfix_Method = patcherType.GetMethod("NotifyChangeItem_Postfix", BindingFlags.NonPublic | BindingFlags.Static);

            // Harmony methods
            harmony.Patch(
                original: HasRoomFor_XY_Method, // The other HasRoomFor method just cascades into this one
                prefix: new HarmonyMethod(HasRoomFor_XY_Prefix_Method),
                postfix: new HarmonyMethod(HasRoomFor_Postfix_Method));

            harmony.Patch(
                original: NotifyAddItem_Method,
                prefix: null, // No prefix call
                postfix: new HarmonyMethod(NotifyChangeItem_Postfix_Method)); // NotifyAddItem and NotifyRemoveItem share the same Postfix method

            harmony.Patch(
                original: NotifyRemoveItem_Method,
                prefix: null, // No prefix call
                postfix: new HarmonyMethod(NotifyChangeItem_Postfix_Method)); // NotifyAddItem and NotifyRemoveItem share the same Postfix method

            Logger.Log($"ItemsContainerPatcher is done.", LogLevel.Debug);
        }

        private static bool HasRoomFor_XY_Prefix(ItemsContainer __instance, int width, int height, ref bool __result, ref Vector2int __state)
        {
            var itemSize = new Vector2int(width, height);

            return CheckInventoryCache(__instance, itemSize, ref __result, ref __state);
        }

        private static bool CheckInventoryCache(IItemsContainer container, Vector2int itemSize, ref bool __result, ref Vector2int __state)
        {
            // Minimum size should always be 1,1
            itemSize.x = itemSize.x == 0 ? 1 : itemSize.x;
            itemSize.y = itemSize.y == 0 ? 1 : itemSize.y;

            if (HasRoomCacheCollection.TryGetValue(container, out Dictionary<Vector2int, bool> cache))
            {
                if (cache.TryGetValue(itemSize, out bool cachedResult))
                {
                    // We've seen this size before.
                    __result = cachedResult;
                    return false; // Override the result and avoid the heavy calculation.
                }
            }
            else
            {
                // This is a new container we haven't seen before, save it to the cache collection.
                HasRoomCacheCollection.Add(container, new Dictionary<Vector2int, bool>());
            }

            // The result wasn't in the cache. Let the original code run to make the calculation.
            // We'll catch the result on the Postfix for the next frame.
            __state = itemSize;
            return true;
        }

        private static void HasRoomFor_Postfix(ItemsContainer __instance, bool __result, ref Vector2int __state)
        {
            // We should only enter this method if the Prefix didn't have a cached value to use.
            // Catch the result and map it to the size provided by the Prefix.
            if (__state.x == 0 || __state.y == 0)
                return; // Somehow, this can still happen. Don't store 0,0 in the cache. It breaks the game.

            SaveSizeCalculationToCache(__instance, __result, __state);
        }

        private static void SaveSizeCalculationToCache(IItemsContainer container, bool hasRoom, Vector2int itemSize)
        {
            // The large calculation won't have to be done again until something is added or removed from the inventory.
            // How does the base game get away with calculating this mess on every frame???
            HasRoomCacheCollection[container][itemSize] = hasRoom;

            if (hasRoom && // True means there is space for an item of this size
                (itemSize.x > 1 || itemSize.y > 1)) // If there is room for an item of a size larger than a 1x1
            {
                // Then we can preemptively add all item sizes that are smaller.
                foreach (Vector2int smallerItem in AllSmallerThan(itemSize))
                    HasRoomCacheCollection[container][smallerItem] = true;
            }
        }

        private static void NotifyChangeItem_Postfix(ItemsContainer __instance, InventoryItem item)
        {
            // Items in the inventory have changed. Clear out the cache.
            if (HasRoomCacheCollection.TryGetValue(__instance as IItemsContainer, out Dictionary<Vector2int, bool> cache))
            {
                cache.Clear();
            }
            else
            {
                // This is a new container we haven't seen before, save it to the cache collection.
                HasRoomCacheCollection.Add(__instance as IItemsContainer, new Dictionary<Vector2int, bool>());
            }
        }

        private static readonly IEnumerable<Vector2int> SmallerThan3x3 = new Vector2int[]
        {
            new Vector2int(2,3),
            new Vector2int(3,2),
            new Vector2int(2,2),
            new Vector2int(1,2),
            new Vector2int(2,1),
            new Vector2int(1,1)
        };

        private static readonly IEnumerable<Vector2int> SmallerThan2x3 = new Vector2int[]
        {
            new Vector2int(2,2),
            new Vector2int(1,2),
            new Vector2int(2,1),
            new Vector2int(1,1)
        };

        private static readonly IEnumerable<Vector2int> SmallerThan2x2 = new Vector2int[]
        {
            new Vector2int(1,2),
            new Vector2int(2,1),
            new Vector2int(1,1)
        };

        private static readonly IEnumerable<Vector2int> Just1x1 = new Vector2int[]
        {
            new Vector2int(1,1)
        };

        private static IEnumerable<Vector2int> AllSmallerThan(Vector2int original)
        {
            // This might not be pretty but it's simple enough to keep fast

            if (original.x == 3 && original.y == 3) // Size of Mobile Vehicle Bay
            {
                return SmallerThan3x3;
            }

            if (original.x == 2 && original.y == 3) // Size of the Seaglide
            {
                return SmallerThan2x3;
            }

            if (original.x == 2 && original.y == 2) // Size of many things bigger than 1x1
            {
                return SmallerThan2x2;
            }

            // 1x2 is the size of a Creature Decoy

            return Just1x1;
        }
    }
}
