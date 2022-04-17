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
            public static List<ModDataTemplate> modlist;
            public static List<ModDataTemplate> modchanges { get; set; }
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
                Logger.Log(Logger.Level.Debug, "OptionsMenu - Start creating Modlist");
                modlist = null;
                modchanges = null;

                //Create new Tab in the Menu
                ModListTab = __instance.AddTab("QMods List");

                //Add QMM Informations
#if SUBNAUTICA_EXP || BELOWZERO_EXP
                __instance.AddHeading(ModListTab, $"QModManager -Experimental- running version {Assembly.GetExecutingAssembly().GetName().Version.ToStringParsed()}");
#else
                __instance.AddHeading(ModListTab, $"QModManager running version {Assembly.GetExecutingAssembly().GetName().Version.ToStringParsed()}");
#endif
                //Add SML Informations
                var modprio_sml = QModServices.Main.GetMod("SMLHelper");
                if (modprio_sml != null)
                {
                    __instance.AddHeading(ModListTab, $"{modprio_sml.DisplayName} {(modprio_sml.Enable ? $"v{modprio_sml.ParsedVersion}" : string.Empty)} is {(modprio_sml.IsLoaded ? "enabled" : "disabled")}");


                    //*** Test
                    //testbool = modprio_sml.Enable;
                    //MethodInfo SML_AddToggleOption = null;
                    //SML_AddToggleOption = typeof(uGUI_OptionsPanel).GetMethod(nameof(AddToggleOption), new System.Type[] { typeof(int), typeof(string), typeof(bool), typeof(UnityAction<bool>) });
                    //SML_AddToggleOption.Invoke(__instance, new object[] { ModListTab, "Enable Mod", testbool, new UnityAction<bool>(value => testbool = value ) });
                    //SML_AddToggleOption.Invoke(__instance, new object[] { ModListTab, "Enable Mod", modprio_sml.Enable , new UnityAction<bool>(value => MyOnchangeMethode(modprio_sml.Id, value) ) });

                }
                else
                {
                    __instance.AddHeading(ModListTab, $"SMLHelper is not installed");
                }

                IEnumerable<IQMod> mods = QModServices.Main.GetAllMods().OrderBy(mod => mod.DisplayName);
                List<IQMod> activeMods = new List<IQMod>();
                List<IQMod> inactiveMods = new List<IQMod>();

                ModDataTemplate _tmpmod = new ModDataTemplate();
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

                    try
                    {
                        //_tmpmod.ID = mod.Id;
                        /*
                        if (mod.LoadedAssembly != null)
                        {
                            _tmpmod.AssemblyName = mod.AssemblyName;
                            _tmpmod.LoadedAssembly = mod.LoadedAssembly;
                        }
                        */
                        //_tmpmod.Enabled = mod.Enable;
                        //modlist.Add(_tmpmod);
                    }
                    catch
                    {
                        Logger.Log(Logger.Level.Debug, "123456789 - Error on adding Temp Mod to Modlist");
                    }

                }

                __instance.AddHeading(ModListTab, $"- - Statistics - -");
                __instance.AddHeading(ModListTab, $"{mods.Count()} Mods found");
                __instance.AddHeading(ModListTab, $"{activeMods.Count} Mods enabled");
                __instance.AddHeading(ModListTab, $"{inactiveMods.Count} Mods disabled");

                __instance.AddHeading(ModListTab, $"- - List of currently running Mods - -");
                foreach (var mod in activeMods)
                {
                    __instance.AddHeading(ModListTab, $"{mod.DisplayName} v{mod.ParsedVersion.ToString()} from {mod.Author}");

                    MethodInfo Modlist_AddToggleOption = null;
                    Modlist_AddToggleOption = typeof(uGUI_OptionsPanel).GetMethod(nameof(AddToggleOption), new System.Type[] { typeof(int), typeof(string), typeof(bool), typeof(UnityAction<bool>) });


                    //int index = modlist.IndexOf(modlist.Where(mdt => mdt.ID.ToString() == mod.Id.ToString()).FirstOrDefault());
                    //Modlist_AddToggleOption.Invoke(__instance, new object[] { ModListTab, "Enable Mod", modlist[index].Enabled , new UnityAction<bool>(value => modlist[index].Enabled = value) });
                    
                    //Modlist_AddToggleOption.Invoke(__instance, new object[] { ModListTab, "Enable Mod", modlist[index].Enabled, MyOnchangeMethode(Mod.id; value) });
                    //Modlist_AddToggleOption.Invoke(__instance, new object[] { ModListTab, "Enable Mod", modlist[index].Enabled, new UnityAction<bool>(value => MyOnchangeMethode(modlist[index].ID, value)) });

                    Modlist_AddToggleOption.Invoke(__instance, new object[] { ModListTab, "Enable Mod", mod.Enable, new UnityAction<bool>(value => MyOnChangeMethode(__instance, mod.Id, value)) });
                }

                __instance.AddHeading(ModListTab, $"- - List of Disabled Mods - -");

                foreach (var mod in inactiveMods)
                {
                    __instance.AddHeading(ModListTab, $"{mod.DisplayName} from {mod.Author}");
                }

                Logger.Log(Logger.Level.Debug, "OptionsMenu - Creating Modlist Ending");
                #endregion Mod List
            }

            static void MyOnChangeMethode(uGUI_OptionsPanel __instance, string id, bool status)
            {
                
                Logger.Log(Logger.Level.Debug, "WOLOLOLOLOLO - enter MyOnChangeMethode");

                Logger.Log(Logger.Level.Debug, $"WOLOLOLOLOLO - the submit id is {id} and the Status is {status}");

                ModDataTemplate _tmpmod = new ModDataTemplate();
                IEnumerable<ModDataTemplate> _tmpmodlist;

                //try
                //{
                Logger.Log(Logger.Level.Debug, "WOLOLOLOLOLO - searched Mod");
                try
                {
                    //_tmpmod = modlist.Where(mdt => mdt.ID.ToString() == id).FirstOrDefault();
                        
                    if(modlist.Count == 0)
                    {
                        Logger.Log(Logger.Level.Debug, $"modlistcount is Zero");
                        _tmpmodlist = null;
                    }
                    else
                    {
                        Logger.Log(Logger.Level.Debug, $"modlistcount not Zero");
                        _tmpmodlist = modlist.Where(mdt => mdt.ID == id);

                        foreach (ModDataTemplate test in _tmpmodlist)
                        {
                            Logger.Log(Logger.Level.Debug, $"WOLOLOLOLOLO - found mod {test.ID}");
                        }
                    }

                }
                catch (Exception e)
                {
                    throw e;
                }
                /*
                catch
                {
                    Logger.Log(Logger.Level.Debug, "WOLOLOLOLOLO - ERROR - searched Mod");
                }
                */
                                        
                if (_tmpmod == null)
                {
                    Logger.Log(Logger.Level.Debug, "WOLOLOLOLOLO - Mod is not null");
                    try
                    {
                        Logger.Log(Logger.Level.Debug, "WOLOLOLOLOLO - Adding mod to change list");
                        _tmpmod.Enabled = status;
                        modchanges.Add(_tmpmod);
                    }
                    catch
                    {
                        Logger.Log(Logger.Level.Debug, "WOLOLOLOLOLO - ERROR - Adding mod to change list");
                    }
                }
                else
                {
                    //theoretical i don't need that if statement because if the Mod is already in the List there is only the possibility of removing it anyway when only 2 status exist ???
                    if (_tmpmod.Enabled != status)
                    {
                        try
                        {
                            Logger.Log(Logger.Level.Debug, "WOLOLOLOLOLO - remove mod from list again.");
                            modchanges.Remove(_tmpmod);
                        }
                        catch
                        {
                            Logger.Log(Logger.Level.Debug, "WOLOLOLOLOLO - remove mod from list again.");
                        }

                    }
                }

                if (modchanges.Count == 0)
                {
                    //disable Apply Button
                }
                else
                {
                    //enable Apply Button
                }
                //}
                //catch
                //{
                    //Logger.Log(Logger.Level.Debug, "123456789 - Error on MyOnChangeMethode");
                //}

            }
        }

        internal class ModDataTemplate
        {
            public string ID { get; set; }
            //public Assembly LoadedAssembly { get; set; }
            //public string AssemblyName { get; set; }
            public bool Enabled { get; set; }
        }
    }
}
