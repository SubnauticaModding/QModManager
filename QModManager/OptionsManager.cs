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
    using System;

    internal static class OptionsManager
    {      
        internal static int ModsTab;
        internal static int ModListTab;
        
        [HarmonyPatch(typeof(uGUI_OptionsPanel), nameof(uGUI_OptionsPanel.AddTabs))]
        internal static class OptionsPatch
        {
            internal static List<ModDataTemplate> ModList;
            public static List<ModDataTemplate> ModListPendingChanges;
            //internal static bool testbool { get; set; }

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
                ModList = new List<ModDataTemplate>();
                ModListPendingChanges = new List<ModDataTemplate>();
                IEnumerable<IQMod> mods = QModServices.Main.GetAllMods().OrderBy(mod => mod.DisplayName);
                List<IQMod> activeMods = new List<IQMod>();
                List<IQMod> inactiveMods = new List<IQMod>();

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

                    //Write down a Temporary reduced Modlist that is interactable (The QMM Modlist is only Readonly and has many information we do not need for now)
                    ModDataTemplate _tmpmod = new ModDataTemplate();
                    try
                    {
                        _tmpmod.ID = mod.Id;
                        if (mod.LoadedAssembly != null)
                        {
                            Logger.Log(Logger.Level.Debug, $"NANANANANANA - Just checking {mod.DisplayName} ; {mod.AssemblyName} ; {mod.LoadedAssembly.Location}");
                            _tmpmod.PathToAssemblyFile = mod.LoadedAssembly.Location;
                        }
                        else
                        {
                            Logger.Log(Logger.Level.Debug, $"NANANANANANA - Just checking {mod.DisplayName} has no Loaded Assembly");
                        }   
                        _tmpmod.Enabled = mod.Enable;
                        ModList.Add(_tmpmod);
                    }
                    catch
                    {
                        Logger.Log(Logger.Level.Debug, "NANANANANANA - Error on adding Temp Mod to Modlist");
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
                    Modlist_AddToggleOption.Invoke(__instance, new object[] { ModListTab, "Enable Mod", ModList[ModList.FindIndex(mdt => mdt.ID == mod.Id)].Enabled , new UnityAction<bool>(value => OnChangeModStatus(ModList[ModList.FindIndex(mdt => mdt.ID == mod.Id)], value)) });
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
                    Modlist_AddToggleOption.Invoke(__instance, new object[] { ModListTab, "Enable Mod", ModList[ModList.FindIndex(mdt => mdt.ID == mod.Id)].Enabled, new UnityAction<bool>(value => OnChangeModStatus(ModList[ModList.FindIndex(mdt => mdt.ID == mod.Id)], value)) });
                }

                Logger.Log(Logger.Level.Debug, "OptionsMenu - ModList - Creating Modlist Ending");
                #endregion Mod List
            }

            static void OnChangeModStatus(ModDataTemplate ChangedMod,bool status)
            {
                Logger.Log(Logger.Level.Debug, $"WOLOLOLOLOLO - the submit id is {ChangedMod.ID} and the Status is {status}");
                ChangedMod.Enabled = status;

                if( string.IsNullOrEmpty(ModListPendingChanges.Find(mdt => mdt.ID == ChangedMod.ID).ID) )
                {
                    try
                    {
                        ModListPendingChanges.Add(ChangedMod);
                    }
                    catch
                    {
                        Logger.Log(Logger.Level.Debug, "OptionsMenu - ModList - Error on Adding Mod to Pending Status Change List");
                    }
                }
                else
                {
                    try
                    {
                        ModListPendingChanges.Remove(ChangedMod);
                    }
                    catch
                    {
                        Logger.Log(Logger.Level.Debug, "OptionsMenu - ModList - Error on Removing Mod to Pending Status Change List");
                    }
                }

                if (ModListPendingChanges.Count == 0)
                {
                    //disable Apply Button
                }
                else
                {
                    //enable Apply Button
                }
            }
        }

        internal class ModDataTemplate
        {
            public string ID { get; set; }
            public string PathToAssemblyFile { get; set; }
            public bool Enabled { get; set; }
        }
    }
}
