namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using SMLHelper.V2.Handlers;
    using System;
    using System.Reflection;
    using System.Text;
    using QModManager;

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
                WriteModNameError(sb, "Unknown Mod", "Mod added item without using SMLHelper");
        }

        internal static void WriteTechType(StringBuilder sb, TechType techType)
        {
            sb.AppendFormat("\n\n<size=19><color=#808080FF>{0} ({1})</color></size>", techType.AsString(), (int)techType);
        }
        internal static void WriteModName(StringBuilder sb, string text)
        {
            sb.AppendFormat("\n<size=23><color=#00ffffff>{0}</color></size>", text);
        }
        internal static void WriteModNameError(StringBuilder sb, string text, string reason)
        {
            sb.AppendFormat("\n<size=23><color=#ff0000ff>{0}</color></size><size=17><color=#808080FF>({1})</color></size>", text, reason);
        }

        internal static void GetAndWriteModName(StringBuilder sb, TechType type)
        {
            // if (MissingTechTypes.Contains(type)) WriteModNameError(sb, "Mod Missing");
            // This is for something else I am going to do

            if (TechTypeHandler.TechTypesByAssemblies.TryGetValue(type, out Assembly assembly))
            {
                // SMLHelper can access QModManager internals because of this: 
                // https://github.com/QModManager/QModManager/blob/releases/2.0.1/QModManager/Properties/AssemblyInfo.cs#L21

                string modName = null;

                foreach (QMod mod in Patcher.loadedMods)
                {
                    if (mod == null || mod.LoadedAssembly == null) continue;
                    if (mod.LoadedAssembly == assembly)
                    {
                        modName = mod.DisplayName;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(modName))
                {
                    WriteModNameError(sb, "Unknown Mod", "Mod could not be determined");
                }
                else
                {
                    WriteModName(sb, modName);
                }
            }
            else
            {
                WriteModNameError(sb, "Unknown Mod", "Assembly could not be determined");
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
