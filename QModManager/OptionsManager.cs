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
                    AddToggleOption.Invoke(__instance, new object[] { ModsTab, "Enable Mod List Menu", Config.EnableModListMenu, new UnityAction<bool>(value => Config.EnableModListMenu = value) });
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
                    AddToggleOption.Invoke(__instance, new object[] { ModsTab, "Enable Mod List Menu", Config.EnableModListMenu, new UnityAction<bool>(value => Config.EnableModListMenu = value), null });
                }
                #endregion Mod Config

                #region Mod List
                Logger.Log(Logger.Level.Debug, "OptionsMenu - Reset Mod List Menu");

                //Reset ModList and Pending Changes on Reload the List Menu - We do not want to save any Data or take over Data from previous time here
                //Part 1 things we should reset anyway
                ModListPendingChanges = new List<QMod>();

                if (Config.EnableModListMenu)
                {
                    //Part 2 things we only need to reset when activly using the Mod List Menu
                    List<QMod> mods = new List<QMod>();
                    List<QMod> activeMods = new List<QMod>();
                    List<QMod> inactiveMods = new List<QMod>();
                    foreach (var iQMod in QModServices.Main.GetAllMods().OrderBy(mod => mod.DisplayName))
                        if (iQMod is QMod qMod)
                            mods.Add(qMod);
                    List<QMod> erroredMods = mods.FindAll(m => !m.IsLoaded && m.Status > 0);
                    int erroredModscount = erroredMods.Count;

                    //Start creating Ingame Mod List
                    Logger.Log(Logger.Level.Debug, "OptionsMenu - Start creating Modlist");

                    //Create new Tab in the Menu
                    ModListTab = __instance.AddTab("QMods List");

                    //Add QMM Informations
#if SUBNAUTICA_STABLE
                    __instance.AddHeading(ModListTab, $"Running QModManager {Assembly.GetExecutingAssembly().GetName().Version.ToStringParsed()} for Subnautica");
#elif SUBNAUTICA_EXP
                    __instance.AddHeading(ModListTab, $"Running QModManager -Experimental- {Assembly.GetExecutingAssembly().GetName().Version.ToStringParsed()} for Subnautica");
#elif BELOWZERO_STABLE
                    __instance.AddHeading(ModListTab, $"Running QModManager {Assembly.GetExecutingAssembly().GetName().Version.ToStringParsed()} for Below Zero");
#elif BELOWZERO_EXP
                    __instance.AddHeading(ModListTab, $"Running QModManager -Experimental- {Assembly.GetExecutingAssembly().GetName().Version.ToStringParsed()} for Below Zero");
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

                    //Now lets create the enable and disable Mod Lists
                    foreach (var mod in mods)
                    {
                        if (mod.Id != "SMLHelper")
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
                    }

                    //Now show some Statistics ahead of the List.
                    __instance.AddHeading(ModListTab, $"- - Statistics - -");
                    __instance.AddHeading(ModListTab, $"{mods.Count()} Mods found");
                    if (erroredModscount != 0)
                    {
                        __instance.AddHeading(ModListTab, $"WARNING: {erroredModscount} Mods failed to load");
                    }
                    __instance.AddHeading(ModListTab, $"{activeMods.Count} Mods enabled");
                    __instance.AddHeading(ModListTab, $"{inactiveMods.Count} Mods disabled");


                    //At first show the Mods with errors. But show it only, when Mod errors exist
                    if (erroredModscount != 0)
                    {
                        __instance.AddHeading(ModListTab, $"- - List of Mods wit Loading Errors. You need to take Actions on them ! - -");
                        foreach (var mod in erroredMods)
                        {
                            //This is the Header Entry
                            //__instance.AddHeading(ModListTab, $"{mod.DisplayName} from {mod.Author}");

                            //This is the Collapse SubMenu of the Entry
                            if (Patcher.CurrentlyRunningGame == QModGame.Subnautica)
                            {
                                MethodInfo Modlist_AddToggleOption = null;
                                Modlist_AddToggleOption = typeof(uGUI_OptionsPanel).GetMethod(nameof(AddToggleOption), new System.Type[] { typeof(int), typeof(string), typeof(bool), typeof(UnityAction<bool>) });
#if SUBNAUTICA_STABLE
                                Modlist_AddToggleOption.Invoke(__instance, new object[] { ModListTab, $"{mod.DisplayName}", mod.Enable, new UnityAction<bool>(value => OnChangeModStatus(mod, value, __instance)) });
#else
                                Modlist_AddToggleOption.Invoke(__instance, new object[] { ModListTab, $"{mod.DisplayName} from {mod.Author}", mod.Enable, new UnityAction<bool>(value => OnChangeModStatus(mod, value, __instance)) });
#endif
                            }
                            else
                            {
                                MethodInfo Modlist_AddToggleOption = null;
                                Modlist_AddToggleOption = typeof(uGUI_OptionsPanel).GetMethod(nameof(AddToggleOption), new System.Type[] { typeof(int), typeof(string), typeof(bool), typeof(UnityAction<bool>), typeof(string) });
                                Modlist_AddToggleOption.Invoke(__instance, new object[] { ModListTab, $"{mod.DisplayName} from {mod.Author}", mod.Enable, new UnityAction<bool>(value => OnChangeModStatus(mod, value, __instance)), null });
                            }
                        }
                    }

                    //Now we write down the actull List with all Mods. Starting with the Active Mods
                    __instance.AddHeading(ModListTab, $"- - List of currently running Mods - -");
                    foreach (var mod in activeMods)
                    {
                        //This is the Header Entry
                        //__instance.AddHeading(ModListTab, $"{mod.DisplayName} v{mod.ParsedVersion.ToString()} from {mod.Author}");

                        //This is the Collapse SubMenu of the Entry
                        if (Patcher.CurrentlyRunningGame == QModGame.Subnautica)
                        {
                            MethodInfo Modlist_AddToggleOption = null;
                            Modlist_AddToggleOption = typeof(uGUI_OptionsPanel).GetMethod(nameof(AddToggleOption), new System.Type[] { typeof(int), typeof(string), typeof(bool), typeof(UnityAction<bool>) });
#if SUBNAUTICA_STABLE
                            Modlist_AddToggleOption.Invoke(__instance, new object[] { ModListTab, $"{mod.DisplayName}", mod.Enable, new UnityAction<bool>(value => OnChangeModStatus(mod, value, __instance)) });
#else
                            Modlist_AddToggleOption.Invoke(__instance, new object[] { ModListTab, $"{mod.DisplayName} v{mod.ParsedVersion.ToString()} from {mod.Author}", mod.Enable, new UnityAction<bool>(value => OnChangeModStatus(mod, value, __instance)) });
#endif
                        }
                        else
                        {
                            MethodInfo Modlist_AddToggleOption = null;
                            Modlist_AddToggleOption = typeof(uGUI_OptionsPanel).GetMethod(nameof(AddToggleOption), new System.Type[] { typeof(int), typeof(string), typeof(bool), typeof(UnityAction<bool>), typeof(string) });
                            Modlist_AddToggleOption.Invoke(__instance, new object[] { ModListTab, $"{mod.DisplayName} v{mod.ParsedVersion.ToString()} from {mod.Author}", mod.Enable, new UnityAction<bool>(value => OnChangeModStatus(mod, value, __instance)), null });
                        }
                    }

                    //Continue with Disabled Mods
                    __instance.AddHeading(ModListTab, $"- - List of Disabled Mods - -");
                    foreach (var mod in inactiveMods)
                    {
                        //This is the Header Entry
                        //__instance.AddHeading(ModListTab, $"{mod.DisplayName} from {mod.Author}");

                        //This is the Collapse SubMenu of the Entry
                        if (Patcher.CurrentlyRunningGame == QModGame.Subnautica)
                        {
                            MethodInfo Modlist_AddToggleOption = null;
                            Modlist_AddToggleOption = typeof(uGUI_OptionsPanel).GetMethod(nameof(AddToggleOption), new System.Type[] { typeof(int), typeof(string), typeof(bool), typeof(UnityAction<bool>) });
#if SUBNAUTICA_STABLE
                            Modlist_AddToggleOption.Invoke(__instance, new object[] { ModListTab, $"{mod.DisplayName}", mod.Enable, new UnityAction<bool>(value => OnChangeModStatus(mod, value, __instance)) });
#else
                            Modlist_AddToggleOption.Invoke(__instance, new object[] { ModListTab, $"{mod.DisplayName} from {mod.Author}", mod.Enable, new UnityAction<bool>(value => OnChangeModStatus(mod, value, __instance)) });
#endif
                        }
                        else
                        {
                            MethodInfo Modlist_AddToggleOption = null;
                            Modlist_AddToggleOption = typeof(uGUI_OptionsPanel).GetMethod(nameof(AddToggleOption), new System.Type[] { typeof(int), typeof(string), typeof(bool), typeof(UnityAction<bool>), typeof(string) });
                            Modlist_AddToggleOption.Invoke(__instance, new object[] { ModListTab, $"{mod.DisplayName} from {mod.Author}", mod.Enable, new UnityAction<bool>(value => OnChangeModStatus(mod, value, __instance)), null });
                        }
                    }

                    Logger.Log(Logger.Level.Debug, "OptionsMenu - ModList - Creating Modlist Ending");
                }
                else
                {
                    Logger.Log(Logger.Level.Info, "OptionsMenu - ModList - No Mod List Menu was created due to User wish.");
                }

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
                if (Config.EnableModListMenu)
                {
                    //Warning Save Button is shared over the hole Option Menu !
                    if (OptionsManager.ModListPendingChanges.Count > 0)
                    {
                        //if the List Contains Pending entries show Info Box
                        Dialog dialog = new Dialog();
                        dialog.message = "Important ! Changes on Mods will enforce a Game Reboot after all changes are saved to system.";
                        dialog.color = Dialog.DialogColor.Red;
                        dialog.rightButton = Dialog.Button.CancelModChanges;
                        dialog.leftButton = Dialog.Button.ApplyModChanges;
                        dialog.Show();
                    }

                    //In case User Canceled or the Quit was not executed properly
                    if (OptionsManager.ModListPendingChanges.Count != 0)
                    {
                        //As the Original Methode would be disable the Button anyway. We need to Enable it again.
                        __instance.applyButton.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}
