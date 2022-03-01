namespace SMLHelper.V2.Patchers
{
    using HarmonyLib;
    using SMLHelper.V2.Utility;
    using System;

    internal static class ItemsContainerPatcher
    {
        internal static void Patch(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(ItemsContainer), nameof(ItemsContainer.HasRoomFor), new Type[] { typeof(int), typeof(int) }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(ItemsContainerPatcher), nameof(ItemsContainerPatcher.HasRoomFor_XY_Prefix))),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(ItemsContainerPatcher), nameof(ItemsContainerPatcher.HasRoomFor_Postfix))));

            harmony.Patch(AccessTools.Method(typeof(ItemsContainer), nameof(ItemsContainer.NotifyAddItem)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(ItemsContainerPatcher), nameof(ItemsContainerPatcher.NotifyChangeItem_Postfix))));

            harmony.Patch(AccessTools.Method(typeof(ItemsContainer), nameof(ItemsContainer.NotifyRemoveItem)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(ItemsContainerPatcher), nameof(ItemsContainerPatcher.NotifyChangeItem_Postfix))));

            harmony.Patch(AccessTools.Method(typeof(ItemsContainer), nameof(ItemsContainer.NotifyResize)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(ItemsContainerPatcher), nameof(ItemsContainerPatcher.NotifyChangeItem_Postfix))));

            Logger.Log($"ItemsContainerPatcher is done.", LogLevel.Debug);
        }

        private static bool HasRoomFor_XY_Prefix(ItemsContainer __instance, int width, int height, ref bool __result, ref Vector2int __state)
        {
            // Completely avoid non-natural sizes due to game-breaking bugs
            if (width <= 0 || height <= 0)
            {
                __result = false;

                return false;
            }

            __state = new Vector2int(width, height); // Internal Harmony parameter which is passed to the Postfix

            // If no result exists (false), return true and continue with code execution
            // If a result exists (true), detour the method and return the cached result
            return !ItemStorageHelper.Singleton.TryGetCachedHasRoom(__instance, __state, ref __result);
        }

        private static void HasRoomFor_Postfix(ItemsContainer __instance, bool __result, Vector2int __state)
        {
            // We should only enter this method if the Prefix didn't have a cached value to use
            // Catch the result and map it to the size provided by the Prefix

            ItemStorageHelper.Singleton.CacheNewHasRoomData(__instance, __state, __result);
        }

        private static void NotifyChangeItem_Postfix(ItemsContainer __instance)
        {
            ItemStorageHelper.Singleton.ClearContainerCache(__instance);
        }
    }
}
