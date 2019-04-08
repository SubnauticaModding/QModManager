namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using SMLHelper.V2.Utility;
    using System;
    using System.Reflection;

    internal static class ItemsContainerPatcher
    {
        internal static void Patch(HarmonyInstance harmony)
        {
            // Original methods
            Type itemsContainerType = typeof(ItemsContainer); // Vanilla Subnautica class for all item storage containers
            MethodInfo HasRoomFor_XY_Method = itemsContainerType.GetMethod(nameof(ItemsContainer.HasRoomFor), new Type[] { typeof(int), typeof(int) });
            MethodInfo NotifyAddItem_Method = itemsContainerType.GetMethod("NotifyAddItem", BindingFlags.NonPublic | BindingFlags.Instance); // Private void
            MethodInfo NotifyRemoveItem_Method = itemsContainerType.GetMethod("NotifyRemoveItem", BindingFlags.NonPublic | BindingFlags.Instance); // Private void

            // Harmony methods
            Type patcherType = typeof(ItemsContainerPatcher);
            MethodInfo HasRoomFor_XY_Prefix_Method = patcherType.GetMethod(nameof(ItemsContainerPatcher.HasRoomFor_XY_Prefix), BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo HasRoomFor_Postfix_Method = patcherType.GetMethod(nameof(ItemsContainerPatcher.HasRoomFor_Postfix), BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo NotifyChangeItem_Postfix_Method = patcherType.GetMethod(nameof(ItemsContainerPatcher.NotifyChangeItem_Postfix), BindingFlags.NonPublic | BindingFlags.Static);

            /* Patch with Harmony
			 * All active ItemsContainer.HasRoom methods cascade into HasRoomFor(int width, int height)
			 * NotifyAddItem and NotifyRemoveItem share the same HarmonyPostfix */
            harmony.Patch(
                original: HasRoomFor_XY_Method,
                prefix: new HarmonyMethod(HasRoomFor_XY_Prefix_Method),
                postfix: new HarmonyMethod(HasRoomFor_Postfix_Method),
                transpiler: null);

            harmony.Patch(
                original: NotifyAddItem_Method,
                prefix: null,
                postfix: new HarmonyMethod(NotifyChangeItem_Postfix_Method),
                transpiler: null);

            harmony.Patch(
                original: NotifyRemoveItem_Method,
                prefix: null,
                postfix: new HarmonyMethod(NotifyChangeItem_Postfix_Method),
                transpiler: null);

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
