namespace QModManager.API.SMLHelper.Patchers
{
    using Harmony;
    using QModManager;
    using QModManager.API.SMLHelper.Handlers;
    using QModManager.Utility;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;

    internal static class TooltipPatcher
    {
        internal static bool DisableEnumIsDefinedPatch = false;

        internal static void Patch(HarmonyInstance harmony)
        {
            Initialize();

            harmony.Patch(AccessTools.Method(typeof(TooltipFactory), "InventoryItem"),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(TooltipPatcher), "Transpiler")));

            harmony.Patch(AccessTools.Method(typeof(TooltipFactory), "InventoryItemView"),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(TooltipPatcher), "Transpiler")));

            harmony.Patch(AccessTools.Method(typeof(TooltipFactory), "BuildTech"),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(TooltipPatcher), "Transpiler")));

            harmony.Patch(AccessTools.Method(typeof(TooltipFactory), "Recipe"),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(TooltipPatcher), "Transpiler")));

            Logger.Debug("TooltipPatcher is done.");
        }

        internal static void CustomTooltip(StringBuilder sb, TechType techType)
        {
            if (ExtraItemInfoOption == ExtraItemInfo.Nothing) return;

            if (ExtraItemInfoOption == ExtraItemInfo.ModNameAndItemID) WriteTechType(sb, techType);
            else WriteSpace(sb);

            if (IsVanillaTechType(techType))
                WriteModName(sb, "Subnautica");
            else if (TechTypePatcher.cacheManager.ContainsKey(techType))
                WriteModNameFromTechType(sb, techType);
            else
                WriteModNameError(sb, "Unknown Mod", "Item added without SMLHelper");
        }
        internal static void CustomTooltip(StringBuilder sb, InventoryItem item)
        {
            CustomTooltip(sb, item.item.GetTechType());
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
            sb.AppendFormat("\n<size=23><color=#ff0000ff>{0}</color></size>\n<size=17><color=#808080FF>({1})</color></size>", text, reason);
        }
        internal static void WriteModNameFromTechType(StringBuilder sb, TechType type)
        {
            try
            {
                if (TechTypeHandler.TechTypesAddedBy.TryGetValue(type, out Assembly assembly))
                {
                    string modName = assembly.GetName().Name; // QModAPI.GetMod(assembly).DisplayName;

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
            catch (Exception e)
            {
                WriteModNameError(sb, "Unknown Mod", "An error has occurred");
                Logger.Error($"There was an error displaying extra item information for TechType '{type.AsString()}'");
                Logger.Exception(e);
            }
        }
        internal static void WriteSpace(StringBuilder sb)
        {
            sb.AppendFormat("\n<size=19></size>");
        }

        internal static bool IsVanillaTechType(TechType type)
        {
            DisableEnumIsDefinedPatch = true;
            bool result = Enum.IsDefined(typeof(TechType), type);
            DisableEnumIsDefinedPatch = false;
            return result;
        }

        #region Options

        internal enum ExtraItemInfo
        {
            ModName,
            ModNameAndItemID,
            Nothing
        }

        /// <summary>
        /// Should be one of: '<see langword="Mod name (default)">'</see>, '<see langword="Mod name and item ID"></see>', or '<see langword="Nothing"></see>'
        /// </summary>
        internal static ExtraItemInfo ExtraItemInfoOption { get; private set; }

        internal static void SetExtraItemInfo(ExtraItemInfo value)
        {
            string configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ExtraItemInfo.txt");

            string text;
            switch (value)
            {
                case ExtraItemInfo.ModName:
                    text = "Mod name (default)";
                    break;
                case ExtraItemInfo.ModNameAndItemID:
                    text = "Mod name and item ID";
                    break;
                case ExtraItemInfo.Nothing:
                    text = "Nothing";
                    break;
                default:
                    return;
            }

            File.WriteAllText(configPath, text);
            ExtraItemInfoOption = value;
        }

        internal static bool Initialized = false;

        internal static void Initialize()
        {
            if (Initialized) return;
            Initialized = true;

            string configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ExtraItemInfo.txt");

            if (!File.Exists(configPath))
            {
                File.WriteAllText(configPath, "Mod name (default)");
                ExtraItemInfoOption = ExtraItemInfo.ModName;

                return;
            }

            string fileContents = File.ReadAllText(configPath);

            switch (fileContents)
            {
                case "Mod name (default)":
                    ExtraItemInfoOption = ExtraItemInfo.ModName;
                    Logger.Info($"Extra item info set to: {fileContents}");
                    break;
                case "Mod name and item ID":
                    ExtraItemInfoOption = ExtraItemInfo.ModNameAndItemID;
                    Logger.Info($"Extra item info set to: {fileContents}");
                    break;
                case "Nothing":
                    ExtraItemInfoOption = ExtraItemInfo.Nothing;
                    Logger.Info($"Extra item info set to: {fileContents}");
                    break;
                default:
                    File.WriteAllText(configPath, "Mod name (default)");
                    ExtraItemInfoOption = ExtraItemInfo.ModName;
                    Logger.Warn("Error reading ExtraItemInfo.txt configuration file. Defaulted to mod name.");
                    break;
            }
        }

        #endregion

        #region Patches

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase method)
        {
            // Turn the IEnumerable into a List
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            // Get the target op code based on what method is being patched
            // Ldloc_0 for InventoryItem and InventoryItemView, Ldarg_2 for BuildTech and Recipe
            OpCode targetCode = method.Name.Contains("InventoryItem") ? OpCodes.Ldloc_0 : OpCodes.Ldarg_2;
            // Get the last occurance of the code
            int entryIndex = codes.FindLastIndex(c => c.opcode == targetCode);

            // Get the fnction that will be injected based on what method is currently being patched
            // CustomTooltip(StringBuilder, InventoryItem) for InventoryItem and InventoryItemView, CustomTooltip(StringBuilder, TechType) for BuildTech and Recipe
            Expression<Action> methodInfo;
            if (method.Name.Contains("InventoryItem")) methodInfo = () => CustomTooltip(null, null);
            else methodInfo = () => CustomTooltip(null, TechType.None);

            // Remove labels at the target op code
            List<Label> labelsToMove = codes[entryIndex].labels.ToArray().ToList();
            codes[entryIndex].labels.Clear();
            
            // Insert custom IL codes
            CodeInstruction[] codesToInsert = new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldloc_0)
                {
                    // Move labels correctly
                    labels = labelsToMove
                },
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(methodInfo))
            };
            codes.InsertRange(entryIndex, codesToInsert);

            // Return modified IL
            return codes.AsEnumerable();
        }

        #endregion
    }
}
