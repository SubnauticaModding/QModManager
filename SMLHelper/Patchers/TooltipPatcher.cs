namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using System;
    using System.Reflection;

    internal class TooltipPatcher
    {
        internal static void Patch(HarmonyInstance harmony)
        {
            Type TooltipFactoryType = typeof(TooltipFactory);
            Type thisType = typeof(TooltipPatcher);
            MethodInfo InventoryItemMethod = TooltipFactoryType.GetMethod("InventoryItem", BindingFlags.Public | BindingFlags.Static);
            MethodInfo InventoryItemViewMethod = TooltipFactoryType.GetMethod("InventoryItemView", BindingFlags.Public | BindingFlags.Static);

            harmony.Patch(InventoryItemMethod, new HarmonyMethod(thisType.GetMethod("II_Prefix", BindingFlags.NonPublic | BindingFlags.Static)));
            harmony.Patch(InventoryItemViewMethod, new HarmonyMethod(thisType.GetMethod("IIV_Prefix", BindingFlags.NonPublic | BindingFlags.Static)));
        }

        internal static bool II_Prefix(InventoryItem item)
        {
            return false;
        }
    }
}
