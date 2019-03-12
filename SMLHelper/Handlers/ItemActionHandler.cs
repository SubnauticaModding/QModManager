namespace SMLHelper.V2.Handlers
{
    using SMLHelper.V2.Patchers;
    using System;

    /// <summary>
    /// A handler class for registering your custom middle click actions for items
    /// </summary>
    public class ItemActionHandler
    {
        /// <summary>
        /// Registers a middle click action for an <see cref="InventoryItem"/> based off of its <see cref="TechType"/>
        /// </summary>
        /// <param name="targetTechType">The <see cref="TechType"/> which the middle click action will be assigned to</param>
        /// <param name="callback">The method which is called when the <see cref="InventoryItem"/> was middle-clicked</param>
        /// <param name="tooltip">The secondary tooltip which appears in the description of the item</param>
        public static void RegisterMiddleClickAction(TechType targetTechType, Action<InventoryItem> callback, string tooltip)
        {
            ItemActionPatcher.CustomItemActions.Add(targetTechType, callback);
            if (!string.IsNullOrEmpty(tooltip)) ItemActionPatcher.CustomItemActionTooltips.Add(targetTechType, tooltip);
        }
    }
}
