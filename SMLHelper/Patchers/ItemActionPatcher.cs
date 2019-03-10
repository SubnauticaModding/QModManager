using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SMLHelper.V2.Patchers
{
    public class ItemActionPatcher
    {
        #region Internal Fields

        internal const ItemAction MiddleClickItemAction = (ItemAction)1337;

        internal static IDictionary<TechType, Action<InventoryItem>> CustomItemActions = new SelfCheckingDictionary<TechType, Action<InventoryItem>>("CustomItemActions");

        internal static IDictionary<TechType, string> CustomItemActionTooltips = new SelfCheckingDictionary<TechType, string>("CustomItemActionTooltips");

        #endregion

        #region Patching

        internal static void Patch(HarmonyInstance harmony)
        {
            // Direct access to private fields made possible by https://github.com/CabbageCrow/AssemblyPublicizer/
            // See README.md for details.

            Type thisType = typeof(ItemActionPatcher);

            Type uGUI_InventoryTabType = typeof(uGUI_InventoryTab);
            MethodInfo OnPointerClickMethod = uGUI_InventoryTabType.GetMethod("OnPointerClick", BindingFlags.Public | BindingFlags.Instance);
            harmony.Patch(OnPointerClickMethod, new HarmonyMethod(thisType.GetMethod("OnPointerClick_Prefix", BindingFlags.NonPublic | BindingFlags.Static)));

            Type InventoryType = typeof(Inventory);
            MethodInfo ExecuteItemActionMethod = InventoryType.GetMethod("ExecuteItemAction", BindingFlags.NonPublic | BindingFlags.Instance);
            harmony.Patch(ExecuteItemActionMethod, new HarmonyMethod(thisType.GetMethod("ExecuteItemAction_Prefix", BindingFlags.NonPublic | BindingFlags.Static)));

            Type TooltipFactoryType = typeof(TooltipFactory);
            MethodInfo ItemActionsMethod = TooltipFactoryType.GetMethod("ItemActions", BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(ItemActionsMethod, null, new HarmonyMethod(thisType.GetMethod("ItemActions_Postfix", BindingFlags.NonPublic | BindingFlags.Static)));

            Logger.Log("ItemActionPatcher is done.", LogLevel.Debug);
        }

        internal static bool OnPointerClick_Prefix(InventoryItem item, int button)
        {
            if (ItemDragManager.isDragging)
            {
                return true;
            }
            if (button == 2)
            {
                ErrorMessage.AddDebug("Middle clicked an item!");
                Inventory.main.ExecuteItemAction(MiddleClickItemAction, item);
                return false;
            }
            else return true;
        }

        internal static bool ExecuteItemAction_Prefix(ItemAction action, InventoryItem item)
        {
            if (action != MiddleClickItemAction) return true;

            TechType itemTechType = item.item.GetTechType();
            if (CustomItemActions.TryGetValue(itemTechType, out Action<InventoryItem> method))
            {
                method?.Invoke(item);
                return false;
            }
            return true;
        }

        internal static void ItemActions_Postfix(StringBuilder sb, InventoryItem item)
        {
            TechType itemTechType = item.item.GetTechType();
            if (CustomItemActionTooltips.TryGetValue(itemTechType, out string tooltip))
            {
                sb.Append("\n");
                TooltipFactory.WriteAction(sb, "MMB", tooltip);
            }
        }

        #endregion
    }
}
