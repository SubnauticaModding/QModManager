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
        /// Registers a custom left click action for a <see cref="TechType"/>
        /// </summary>
        /// <param name="targetTechType">The <see cref="TechType"/> to which the left click action will be assigned</param>
        /// <param name="callback">The method which will be called when a matching <see cref="InventoryItem"/> with the specified <see cref="TechType"/> was left-clicked</param>
        /// <param name="tooltip">The secondary tooltip which will appear in the description of the item</param>
        /// <param name="condition">The condition which must return <see langword="true"/> for the action to be called when the item is clicked<para/>If ommited, the action will always be called</param>
        public static void RegisterLeftClickAction(TechType targetTechType, Action<InventoryItem> callback, string tooltip, Predicate<InventoryItem> condition = null)
        {
            condition = condition ?? ((item) => true);
            ItemActionPatcher.LeftClickActions.Add(targetTechType, new ItemActionPatcher.CustomItemAction(callback, tooltip, condition));
        }
        /// <summary>
        /// Registers a custom middle click action for a <see cref="TechType"/>
        /// </summary>
        /// <param name="targetTechType">The <see cref="TechType"/> which the middle click action will be assigned</param>
        /// <param name="callback">The method which will be called when a matching <see cref="InventoryItem"/> with the specified <see cref="TechType"/> was middle-clicked</param>
        /// <param name="tooltip">The secondary tooltip which will appear in the description of the item</param>
        /// <param name="condition">The condition which must return <see langword="true"/> for the action to be called when the item is clicked<para/>If ommited, the action will always be called</param>
        public static void RegisterMiddleClickAction(TechType targetTechType, Action<InventoryItem> callback, string tooltip, Predicate<InventoryItem> condition = null)
        {
            condition = condition ?? ((item) => true);
            ItemActionPatcher.MiddleClickActions.Add(targetTechType, new ItemActionPatcher.CustomItemAction(callback, tooltip, condition));
        }
    }
}
