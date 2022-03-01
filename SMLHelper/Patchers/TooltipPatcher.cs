namespace SMLHelper.V2.Patchers
{
    using HarmonyLib;
    using QModManager.API;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Patchers.EnumPatching;
    using System.IO;
    using System.Reflection;
    using System.Text;

    internal class TooltipPatcher
    {
        internal static bool DisableEnumIsDefinedPatch = false;

        internal static void Patch(Harmony harmony)
        {
            Initialize();

            MethodInfo buildTech = AccessTools.Method(typeof(TooltipFactory), nameof(TooltipFactory.BuildTech));
            MethodInfo itemCommons = AccessTools.Method(typeof(TooltipFactory), nameof(TooltipFactory.ItemCommons));
#if BELOWZERO
            MethodInfo recipe = AccessTools.Method(typeof(TooltipFactory), nameof(TooltipFactory.CraftRecipe));
#else
            MethodInfo recipe = AccessTools.Method(typeof(TooltipFactory), nameof(TooltipFactory.Recipe));
#endif
            HarmonyMethod customTooltip = new HarmonyMethod(AccessTools.Method(typeof(TooltipPatcher), nameof(TooltipPatcher.CustomTooltip)));
            HarmonyMethod techTypePostfix = new HarmonyMethod(AccessTools.Method(typeof(TooltipPatcher), nameof(TooltipPatcher.TechTypePostfix)));

            harmony.Patch(itemCommons, postfix: customTooltip);
            harmony.Patch(recipe, postfix: techTypePostfix);
            harmony.Patch(buildTech, postfix: techTypePostfix);

            Logger.Log("TooltipPatcher is done.", LogLevel.Debug);
        }

        internal static void CustomTooltip(StringBuilder sb, TechType techType)
        {
            if (ExtraItemInfoOption == ExtraItemInfo.Nothing) return;

            if (ExtraItemInfoOption == ExtraItemInfo.ModNameAndItemID) WriteTechType(sb, techType);
            else WriteSpace(sb);

            if (IsVanillaTechType(techType))
#if SUBNAUTICA
                WriteModName(sb, "Subnautica");
#elif BELOWZERO
                WriteModName(sb, "BelowZero");
#endif
            else if (TechTypePatcher.cacheManager.ContainsKey(techType))
                WriteModNameFromTechType(sb, techType);
            else
                WriteModNameError(sb, "Unknown Mod", "Item added without SMLHelper");
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
            // if (MissingTechTypes.Contains(type)) WriteModNameError(sb, "Mod Missing");
            // This is for something else I am going to do

            if (TechTypeHandler.TechTypesAddedBy.TryGetValue(type, out Assembly assembly))
            {
                string modName = null;
                
                foreach (IQMod mod in QModServices.Main.GetAllMods())
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
        internal static void WriteSpace(StringBuilder sb)
        {
            sb.AppendFormat("\n<size=19></size>");
        }

        internal static bool IsVanillaTechType(TechType type)
        {
            return type <= TechType.Databox;
        }

#region Options

        internal enum ExtraItemInfo
        {
            ModName,
            ModNameAndItemID,
            Nothing
        }

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
                    Logger.Log($"Extra item info set to: {fileContents}", LogLevel.Info);
                    break;
                case "Mod name and item ID":
                    ExtraItemInfoOption = ExtraItemInfo.ModNameAndItemID;
                    Logger.Log($"Extra item info set to: {fileContents}", LogLevel.Info);
                    break;
                case "Nothing":
                    ExtraItemInfoOption = ExtraItemInfo.Nothing;
                    Logger.Log($"Extra item info set to: {fileContents}", LogLevel.Info);
                    break;
                default:
                    File.WriteAllText(configPath, "Mod name (default)");
                    ExtraItemInfoOption = ExtraItemInfo.ModName;
                    Logger.Log("Error reading ExtraItemInfo.txt configuration file. Defaulted to mod name.", LogLevel.Warn);
                    break;
            }
        }

#endregion

#region Patches


#if SUBNAUTICA
        internal static void TechTypePostfix(TechType techType, ref string tooltipText)
        {
            StringBuilder stringBuilder = new StringBuilder(tooltipText);

            CustomTooltip(stringBuilder, techType);
            tooltipText = stringBuilder.ToString();
        }
#elif BELOWZERO
        internal static void TechTypePostfix(TechType techType, TooltipData data)
        {
            CustomTooltip(data.prefix, techType);
        }
#endif
#endregion
    }
}
