namespace QModManager
{
    using HarmonyLib;
    using QModManager.API;
    using QModManager.Patching;
    using QModManager.Utility;
    using System.Reflection;
    using UnityEngine.Events;
    using System.Collections.Generic;
    using System.Linq;

    internal static class OptionsManager
    {
        internal static int ModsTab;
        internal static string ModsListTabName = "QMods List";
        internal static int ModListTab;
        public static List<QMod> ModListPendingChanges;

        [HarmonyPatch(typeof(uGUI_OptionsPanel), nameof(uGUI_OptionsPanel.AddTabs))]
        internal static class OptionsPatch
        {
            [HarmonyPostfix]
            internal static void Postfix(uGUI_OptionsPanel __instance)
            {
                #region Mod Config
                ModsTab = __instance.AddTab("Mods");
                __instance.AddHeading(ModsTab, "QModManager");

                MethodInfo AddToggleOption = null; 
                if(Patcher.CurrentlyRunningGame == QModGame.Subnautica)
                {
                    AddToggleOption = typeof(uGUI_OptionsPanel).GetMethod(nameof(AddToggleOption), new System.Type[] { typeof(int), typeof(string), typeof(bool), typeof(UnityAction<bool>) });
                    AddToggleOption.Invoke(__instance, new object[] { ModsTab, "Check for updates", Config.CheckForUpdates, new UnityAction<bool>(value => Config.CheckForUpdates = value) });

                    AddToggleOption.Invoke(__instance, new object[] { ModsTab, "Enable console", Config.EnableConsole, new UnityAction<bool>(value =>
                {
                    Config.EnableConsole = value;
#if SUBNAUTICA_STABLE
                    DevConsole.disableConsole = !value;
                    UnityEngine.PlayerPrefs.SetInt("UWE.DisableConsole", value ? 0 : 1);
#elif BELOWZERO || SUBNAUTICA_EXP
                    PlatformUtils.SetDevToolsEnabled(value);
#endif
                }) });

                    AddToggleOption.Invoke(__instance, new object[] { ModsTab, "Enable debug logs", Config.EnableDebugLogs, new UnityAction<bool>(value => Config.EnableDebugLogs = value) });
                    AddToggleOption.Invoke(__instance, new object[] { ModsTab, "Enable developer mode", Config.EnableDevMode, new UnityAction<bool>(value => Config.EnableDevMode = value) });
                }
                else
                {
                    AddToggleOption = typeof(uGUI_OptionsPanel).GetMethod(nameof(AddToggleOption), new System.Type[] { typeof(int), typeof(string), typeof(bool), typeof(UnityAction<bool>), typeof(string) });
                    AddToggleOption.Invoke(__instance, new object[] { ModsTab, "Check for updates", Config.CheckForUpdates, new UnityAction<bool>(value => Config.CheckForUpdates = value), null });

                    AddToggleOption.Invoke(__instance, new object[] { ModsTab, "Enable console", Config.EnableConsole, new UnityAction<bool>(value =>
                {
                    Config.EnableConsole = value;
#if SUBNAUTICA_STABLE
                    DevConsole.disableConsole = !value;
                    UnityEngine.PlayerPrefs.SetInt("UWE.DisableConsole", value ? 0 : 1);
#elif BELOWZERO || SUBNAUTICA_EXP
                    PlatformUtils.SetDevToolsEnabled(value);
#endif
                }), null });

                    AddToggleOption.Invoke(__instance, new object[] { ModsTab, "Enable debug logs", Config.EnableDebugLogs, new UnityAction<bool>(value => Config.EnableDebugLogs = value), null });
                    AddToggleOption.Invoke(__instance, new object[] { ModsTab, "Enable developer mode", Config.EnableDevMode, new UnityAction<bool>(value => Config.EnableDevMode = value), null });
                }
                #endregion Mod Config

                #region Mod List
                //Start creating Ingame Mod List
                Logger.Log(Logger.Level.Debug, "OptionsMenu - Start creating Modlist");

                //Reset ModList and Pending Changes on Relead the List Menu - We do not want to save any Data or take over Data from previous time here
                ModListPendingChanges = new List<QMod>();
                List<QMod> mods = new List<QMod>();
                foreach (var iQMod in QModServices.Main.GetAllMods().OrderBy(mod => mod.DisplayName))
                    if (iQMod is QMod qMod)
                        mods.Add(qMod);
                List<QMod> activeMods = new List<QMod>();
                List<QMod> inactiveMods = new List<QMod>();

                //Create new Tab in the Menu
                ModListTab = __instance.AddTab("QMods List");

                //Add QMM Informations
#if SUBNAUTICA_EXP || BELOWZERO_EXP
                __instance.AddHeading(ModListTab, $"QModManager -Experimental- running version {Assembly.GetExecutingAssembly().GetName().Version.ToStringParsed()}");
#else
                __instance.AddHeading(ModListTab, $"QModManager running version {Assembly.GetExecutingAssembly().GetName().Version.ToStringParsed()}");
#endif
                //Add SML Informations to the Top as its pretty Important
                var modprio_sml = QModServices.Main.GetMod("SMLHelper");
                if (modprio_sml != null)
                {
                    __instance.AddHeading(ModListTab, $"{modprio_sml.DisplayName} {(modprio_sml.Enable ? $"v{modprio_sml.ParsedVersion}" : string.Empty)} is {(modprio_sml.IsLoaded ? "enabled" : "disabled")}");
                }
                else
                {
                    __instance.AddHeading(ModListTab, $"SMLHelper is not installed");
                }

                //Now lets create the Mod List
                foreach (var mod in mods)
                {
                    if (mod.Enable)
                    {
                        activeMods.Add(mod);

                    }
                    else
                    {
                        inactiveMods.Add(mod);
                    }
                }

                //Now show some Statistics ahead of the List.
                __instance.AddHeading(ModListTab, $"- - Statistics - -");
                __instance.AddHeading(ModListTab, $"{mods.Count()} Mods found");
                __instance.AddHeading(ModListTab, $"{activeMods.Count} Mods enabled");
                __instance.AddHeading(ModListTab, $"{inactiveMods.Count} Mods disabled");

                //Now we write down the actull List with all Mods. Starting with the Active Mods
                __instance.AddHeading(ModListTab, $"- - List of currently running Mods - -");
                foreach (var mod in activeMods)
                {
                    //This is the Header Entry
                    __instance.AddHeading(ModListTab, $"{mod.DisplayName} v{mod.ParsedVersion.ToString()} from {mod.Author}");

                    //This is the Collapse SubMenu of the Entry
                    MethodInfo Modlist_AddToggleOption = null;
                    Modlist_AddToggleOption = typeof(uGUI_OptionsPanel).GetMethod(nameof(AddToggleOption), new System.Type[] { typeof(int), typeof(string), typeof(bool), typeof(UnityAction<bool>) });                    
                    Modlist_AddToggleOption.Invoke(__instance, new object[] { ModListTab, "Enable Mod", mod.Enable , new UnityAction<bool>(value => OnChangeModStatus(mod, value, __instance)) });
                }

                //Continue with Disabled Mods
                __instance.AddHeading(ModListTab, $"- - List of Disabled Mods - -");
                foreach (var mod in inactiveMods)
                {
                    //This is the Header Entry
                    __instance.AddHeading(ModListTab, $"{mod.DisplayName} from {mod.Author}");

                    //This is the Collapse SubMenu of the Entry
                    MethodInfo Modlist_AddToggleOption = null;
                    Modlist_AddToggleOption = typeof(uGUI_OptionsPanel).GetMethod(nameof(AddToggleOption), new System.Type[] { typeof(int), typeof(string), typeof(bool), typeof(UnityAction<bool>) });
                    Modlist_AddToggleOption.Invoke(__instance, new object[] { ModListTab, "Enable Mod", mod.Enable , new UnityAction<bool>(value => OnChangeModStatus(mod, value, __instance)) });
                }

                Logger.Log(Logger.Level.Debug, "OptionsMenu - ModList - Creating Modlist Ending");
                
                #endregion Mod List
            }

            static void OnChangeModStatus(QMod ChangedMod,bool status, uGUI_OptionsPanel __instance)
            {              
                //write the new Status to the Mod Variable
                ChangedMod.Enable = status;

                QMod _modexist = new QMod();
                //Now check if the Mod is already in the Pending list
                try
                {
                    //Is there a better way than try/catch using Find while determind a null or Empty when no Mod is found ?
                    _modexist = ModListPendingChanges.Find(mdt => mdt.Id == ChangedMod.Id);
                }
                catch
                {
                    //if not it will result in an exception so i can set it to null
                    _modexist = null;
                }

                if (_modexist == null)
                {
                    //if the Mod is not in list add it to pending
                    try
                    {
                        ModListPendingChanges.Add(ChangedMod);
                    }
                    catch
                    {
                        Logger.Log(Logger.Level.Warn, "OptionsMenu - ModList - Error on Adding Mod to Pending Status Change List");
                    }
                }
                else if(_modexist.Id == ChangedMod.Id)
                {
                    //if the Mod is already in the List the user revert the change. Remove it
                    try
                    {
                        ModListPendingChanges.Remove(ChangedMod);
                    }
                    catch
                    {
                        Logger.Log(Logger.Level.Warn, "OptionsMenu - ModList - Error on Removing Mod to Pending Status Change List");
                    }
                }
                else
                {
                    Logger.Log(Logger.Level.Warn, "OptionsMenu - ModList - Error on Adding AND Removing Mod to Pending Status Change List.");
                    //mhh that should not happen....
                }

                if (ModListPendingChanges.Count != 0)
                {
                    //enable Apply Button
                    __instance.applyButton.gameObject.SetActive(true);
                }
            }
        }

        [HarmonyPatch(typeof(uGUI_OptionsPanel), nameof(uGUI_OptionsPanel.OnApplyButton))]
        internal static class OptionsPatch_OnApplyButton
        {
            [HarmonyPostfix]
            internal static void Postfix(uGUI_OptionsPanel __instance)
            {
                //Warning Save Button is shared over the hole Option Menu !
                if (OptionsManager.ModListPendingChanges.Count > 0)
                {
                    //if the List Contains Pending entries send every change to the File Writer
                    foreach (QMod _mod in OptionsManager.ModListPendingChanges)
                    {
                        IOUtilities.ChangeModStatustoFile(_mod);
                    }
                }
                //just in case disable to button again
                __instance.applyButton.gameObject.SetActive(false);
            }
        }
    }
}
