namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using SMLHelper.V2.Handlers;
    using System;
    using System.Reflection;
    using System.Text;

    internal class TooltipPatcher
    {
        internal static bool DisableEnumIsDefinedPatch = false;

        internal static void Patch(HarmonyInstance harmony)
        {
            Type TooltipFactoryType = typeof(TooltipFactory);
            Type thisType = typeof(TooltipPatcher);
            MethodInfo InventoryItemMethod = TooltipFactoryType.GetMethod("InventoryItem", BindingFlags.Public | BindingFlags.Static);
            MethodInfo InventoryItemViewMethod = TooltipFactoryType.GetMethod("InventoryItemView", BindingFlags.Public | BindingFlags.Static);

            harmony.Patch(InventoryItemMethod, new HarmonyMethod(thisType.GetMethod("II_Prefix", BindingFlags.NonPublic | BindingFlags.Static)));
            harmony.Patch(InventoryItemViewMethod, new HarmonyMethod(thisType.GetMethod("IIV_Prefix", BindingFlags.NonPublic | BindingFlags.Static)));
        }

        internal static string GetTooltip(InventoryItem item, bool view = false)
        {
            TooltipFactory.Initialize();
            StringBuilder stringBuilder = new StringBuilder();
            Pickupable item2 = item.item;
            TooltipFactory.ItemCommons(stringBuilder, item2.GetTechType(), item2.gameObject);
            if (!view) TooltipFactory.ItemActions(stringBuilder, item);

            CustomTooltip(stringBuilder, item);

            return stringBuilder.ToString();
        }
        internal static void CustomTooltip(StringBuilder sb, InventoryItem item)
        {
            TechType techType = item.item.GetTechType();
            WriteTechType(sb, techType);

            if (IsVanillaTechType(techType))
                WriteModName(sb, "Subnautica");
            else if (TechTypePatcher.cacheManager.ContainsKey(techType))
                GetAndWriteModName(sb, techType);
            else
                WriteModNameError(sb, "Unknown Mod");
        }

        internal static void WriteTechType(StringBuilder sb, TechType techType)
        {
            sb.AppendFormat("\n\n<size=19><color=#808080FF>{0} ({1})</color></size>", techType.AsString(), (int)techType);
        }
        internal static void WriteModName(StringBuilder sb, string text)
        {
            sb.AppendFormat("\n<size=23><color=#00ffffff>{0}</color></size>", text);
        }
        internal static void WriteModNameError(StringBuilder sb, string text)
        {
            sb.AppendFormat("\n<size=23><color=#ff0000ff>{0}</color></size>", text);
        }

        internal static void GetAndWriteModName(StringBuilder sb, TechType type)
        {
            if (TechTypeHandler.TechTypesByAssemblies.TryGetValue(type, out Assembly assembly))
            {

            }
            else
            {
                WriteModNameError(sb, "Unknown Mod");
            }
        }

        internal static bool IsVanillaTechType(TechType type)
        {
            DisableEnumIsDefinedPatch = true;
            bool result = Enum.IsDefined(typeof(TechType), type);
            DisableEnumIsDefinedPatch = false;
            return result;
        }

        internal static bool II_Prefix(ref string __result, InventoryItem item)
        {
            __result = GetTooltip(item);
            return false;
        }
        internal static bool IIV_Prefix(ref string __result, InventoryItem item)
        {
            __result = GetTooltip(item, true);
            return false;
        }
    }
}
