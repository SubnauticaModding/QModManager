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
        internal static int ModListTab;
        internal static List<ModDataTemplate> modlist;
        internal static List<ModDataTemplate> modchanges;


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
                }
                else
                {
                    __instance.AddHeading(ModListTab, $"SMLHelper is not installed");
                }

                IEnumerable<IQMod> mods = QModServices.Main.GetAllMods().OrderBy(mod => mod.DisplayName);
                List<IQMod> activeMods = new List<IQMod>();
                List<IQMod> inactiveMods = new List<IQMod>();

                foreach (var mod in mods)
                {
                    ModDataTemplate _tmpmod = new ModDataTemplate();
                    _tmpmod.ID = mod.Id;
                    _tmpmod.AssemblyName = mod.AssemblyName;
                    _tmpmod.LoadedAssembly = mod.LoadedAssembly;
                    _tmpmod.Enabled = mod.Enable;
                    modlist.Add(_tmpmod);

                    if (mod.Enable)
                    {
                        activeMods.Add(mod);
                    }
                    else
                    {
                        inactiveMods.Add(mod);
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
                    Modlist_AddToggleOption = typeof(uGUI_OptionsPanel).GetMethod(nameof(Modlist_AddToggleOption), new System.Type[] { typeof(int), typeof(string), typeof(bool), typeof(UnityAction<bool>) });
                    //Modlist_AddToggleOption.Invoke(__instance, new object[] { ModListTab, "Enable Mod", mod.Enable, new UnityAction<bool>(value => activeMods[(activeMods.IndexOf(mod))].Enable = value) });

                    //int index = modlist.IndexOf(modlist.Where(mdt => mdt.ID.ToString() == mod.Id.ToString()).FirstOrDefault());
                    //Modlist_AddToggleOption.Invoke(__instance, new object[] { ModListTab, "Enable Mod", modlist[index].Enabled , new UnityAction<bool>(value => modlist[index].Enabled = value) });
                    //Modlist_AddToggleOption.Invoke(__instance, new object[] { ModListTab, "Enable Mod", modlist[index].Enabled, MyOnchangeMethode(Mod.id; value) });
                    //Modlist_AddToggleOption.Invoke(__instance, new object[] { ModListTab, "Enable Mod", modlist[index].Enabled, new UnityAction<bool>(value => MyOnchangeMethode(modlist[index].ID, value)) });

                    Modlist_AddToggleOption.Invoke(__instance, new object[] { ModListTab, "Enable Mod", mod.Enable, new UnityAction<bool>(value => MyOnchangeMethode(mod.Id, value)) });
                }

                __instance.AddHeading(ModListTab, $"- - List of Disabled Mods - -");

                foreach (var mod in inactiveMods)
                {
                    __instance.AddHeading(ModListTab, $"{mod.DisplayName} from {mod.Author}");
                }
                #endregion Mod List
            }

            static void MyOnchangeMethode(string id, bool status)
            {
                ModDataTemplate _tmpmod = modlist.Where(mdt => mdt.ID.ToString() == id).FirstOrDefault();
                if (_tmpmod == null )
                {
                    _tmpmod.Enabled = status;
                    modchanges.Add(_tmpmod);
                }
                else
                {
                    //theoretical i don't need that if statement because if the Mod is already in the List there is only the possibility of removing it anyway when only 2 status exist ???
                    if(_tmpmod.Enabled != status)
                    {
                        modchanges.Remove(_tmpmod);
                    }
                }

                if(modchanges.Count == 0)
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
            public Assembly LoadedAssembly { get; set; }
            public string AssemblyName { get; set; }
            public bool Enabled { get; set; }
        }
    }
}
