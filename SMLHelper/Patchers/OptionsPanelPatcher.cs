namespace SMLHelper.V2.Patchers
{
    using HarmonyLib;
    using Options;
    using System.IO;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using QModManager.API;

#if SUBNAUTICA_STABLE
    using Oculus.Newtonsoft.Json;
#else
    using Newtonsoft.Json;
#endif

#if SUBNAUTICA
    using Text = UnityEngine.UI.Text;
#elif BELOWZERO
    using Text = TMPro.TextMeshProUGUI;
    using static SMLHelper.V2.Options.ModKeybindOption;
#endif

    internal class OptionsPanelPatcher
    {
        internal static SortedList<string, ModOptions> modOptions = new SortedList<string, ModOptions>();

        private static int  modsTabIndex = -1;

        internal static void Patch(Harmony harmony)
        {
            PatchUtils.PatchClass(harmony);
            PatchUtils.PatchClass(harmony, typeof(ScrollPosKeeper));

            if (QModServices.Main.FindModById("ModsOptionsAdjusted")?.Enable == true)
                V2.Logger.Log("ModOptionsAdjuster is not inited (ModsOptionsAdjusted mod is active)", LogLevel.Warn);
            else
                PatchUtils.PatchClass(harmony, typeof(ModOptionsHeadingsToggle));
        }


        // 'Mods' tab also added in QModManager, so we can't rely on 'modsTab' in AddTabs_Postfix
        [PatchUtils.Postfix]
        [HarmonyPatch(typeof(uGUI_TabbedControlsPanel), nameof(uGUI_TabbedControlsPanel.AddTab))]
        internal static void AddTab_Postfix(string label, int __result)
        {
            if (label == "Mods")
                modsTabIndex = __result;
        }

#if BELOWZERO
        [PatchUtils.Prefix]
        [HarmonyPatch(typeof(uGUI_Binding), nameof(uGUI_Binding.RefreshValue))]
        internal static bool RefreshValue_Prefix(uGUI_Binding __instance)
        {
            if (__instance.gameObject.GetComponent<ModBindingTag>() is null)
                return true;

            __instance.currentText.text = (__instance.active || __instance.value == null) ? "" : __instance.value;
            __instance.UpdateState();
            return false;
        }
#endif

        [PatchUtils.Postfix]
        [HarmonyPatch(typeof(uGUI_OptionsPanel), nameof(uGUI_OptionsPanel.AddTabs))]
        internal static void AddTabs_Postfix(uGUI_OptionsPanel __instance)
        {
            uGUI_OptionsPanel optionsPanel = __instance;

            // Start the modsTab index at a value of -1
            int modsTab = -1;
            // Loop through all of the tabs
            for (int i = 0; i < optionsPanel.tabsContainer.childCount; i++)
            {
                // Check if they are named "Mods"
                var text = optionsPanel.tabsContainer.GetChild(i).GetComponentInChildren<Text>(true);

                if (text != null && text.text == "Mods")
                {
                    // Set the tab index to the found one and break
                    modsTab = i;
                    break;
                }
            }

            // If no tab was found, create one
            if (modsTab == -1)
            {
                modsTab = optionsPanel.AddTab("Mods");
            }

            // Maybe this could be split into its own file to handle smlhelper options, or maybe it could be removed alltogether
            optionsPanel.AddHeading(modsTab, "SMLHelper");
            optionsPanel.AddToggleOption(modsTab, "Enable debug logs", V2.Logger.EnableDebugging, V2.Logger.SetDebugging);
            optionsPanel.AddChoiceOption(modsTab, "Extra item info", new string[]
            {
                "Mod name (default)",
                "Mod name and item ID",
                "Nothing"
            }, (int)TooltipPatcher.ExtraItemInfoOption, (i) => TooltipPatcher.SetExtraItemInfo((TooltipPatcher.ExtraItemInfo)i));

            // adding all other options here
            modOptions.Values.ForEach(options => options.AddOptionsToPanel(optionsPanel, modsTab));
        }

#if SUBNAUTICA_STABLE
        // fix for slider, check for zero divider added (in that case just return value unchanged)
        // it happens when slider is in pre-awake state, so any given value snaps to default value
        [PatchUtils.Transpiler]
        [HarmonyPatch(typeof(uGUI_SnappingSlider), nameof(uGUI_SnappingSlider.SnapValue))]
        internal static IEnumerable<CodeInstruction> SnapValue_Fix(IEnumerable<CodeInstruction> cins)
        {
            var list = new List<CodeInstruction>(cins);

            int indexLabel = list.FindIndex(cin => cin.opcode == OpCodes.Starg_S && cin.operand.Equals((byte)1)) + 1;
            if (indexLabel == 0 || list[indexLabel].labels.Count == 0)
            {
                V2.Logger.Log("SnapValue_Fix: indexLabel not found", LogLevel.Warn);
                return cins;
            }

            int indexToInject = list.FindIndex(cin => cin.opcode == OpCodes.Stloc_1) + 1;
            if (indexToInject == 0)
            {
                V2.Logger.Log("SnapValue_Fix: indexToInject not found", LogLevel.Warn);
                return cins;
            }

            list.InsertRange(indexToInject, new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldc_R4, 0f),
                new CodeInstruction(OpCodes.Beq, list[indexLabel].labels[0])
            });

            return list;
        }
#endif

        // Class for collapsing/expanding options in 'Mods' tab
        // Options can be collapsed/expanded by clicking on mod's title or arrow button
        private static class ModOptionsHeadingsToggle
        {
            private enum HeadingState { Collapsed, Expanded };

            private static GameObject headingPrefab = null;

            private static class StoredHeadingStates
            {
                private static readonly string configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "headings_states.json");

                private class StatesConfig
                {
                    [JsonProperty]
                    private readonly Dictionary<string, HeadingState> states = new Dictionary<string, HeadingState>();

                    public HeadingState this[string name]
                    {
                        get => states.TryGetValue(name, out HeadingState state) ? state : HeadingState.Expanded;
                        
                        set
                        {
                            states[name] = value;
                            File.WriteAllText(configPath, JsonConvert.SerializeObject(this, Formatting.Indented));
                        }
                    }
                }
                private static readonly StatesConfig statesConfig = CreateConfig();

                private static StatesConfig CreateConfig()
                {
                    if (File.Exists(configPath))
                        return JsonConvert.DeserializeObject<StatesConfig>(File.ReadAllText(configPath));
                    else
                        return new StatesConfig();
                }

                public static HeadingState get(string name) => statesConfig[name];
                public static void store(string name, HeadingState state) => statesConfig[name] = state;
            }

            // we add arrow button from Choice ui element to the options headings for collapsing/expanding 
            private static void InitHeadingPrefab(uGUI_TabbedControlsPanel panel)
            {
                if (headingPrefab)
                    return;

                headingPrefab = Object.Instantiate(panel.headingPrefab);
                headingPrefab.name = "OptionHeadingToggleable";
                headingPrefab.AddComponent<HeadingToggle>();

                Transform captionTransform = headingPrefab.transform.Find("Caption");
                captionTransform.localPosition = new Vector3(45f, 0f, 0f);
                captionTransform.gameObject.AddComponent<HeadingClickHandler>();
                captionTransform.gameObject.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

                GameObject button = Object.Instantiate(panel.choiceOptionPrefab.transform.Find("Choice/Background/NextButton").gameObject);
                button.name = "HeadingToggleButton";
                button.AddComponent<ToggleButtonClickHandler>();

                RectTransform buttonTransform = button.transform as RectTransform;
                buttonTransform.SetParent(headingPrefab.transform);
                buttonTransform.SetAsFirstSibling();
                buttonTransform.localEulerAngles = new Vector3(0f, 0f, -90f);
                buttonTransform.localPosition = new Vector3(15f, -13f, 0f);
                buttonTransform.pivot = new Vector2(0.25f, 0.5f);
                buttonTransform.anchorMin = buttonTransform.anchorMax = new Vector2(0f, 0.5f);
            }

#region components
            // main component for headings toggling
            private class HeadingToggle: MonoBehaviour
            {
                private HeadingState headingState = HeadingState.Expanded;
                private string headingName = null;
                private List<GameObject> childOptions = null;

                private void Init()
                {
                    if (childOptions != null)
                        return;

                    headingName = transform.Find("Caption")?.GetComponent<Text>()?.text ?? "";

                    childOptions = new List<GameObject>();

                    for (int i = transform.GetSiblingIndex() + 1; i < transform.parent.childCount; i++)
                    {
                        GameObject option = transform.parent.GetChild(i).gameObject;

                        if (option.GetComponent<HeadingToggle>())
                            break;

                        childOptions.Add(option);
                    }
                }

                public void EnsureState() // for setting previously saved state
                {
                    Init();

                    HeadingState storedState = StoredHeadingStates.get(headingName);

                    if (headingState != storedState)
                    {
                        SetState(storedState);
                        GetComponentInChildren<ToggleButtonClickHandler>()?.SetStateInstant(storedState);
                    }
                }

                public void SetState(HeadingState state)
                {
                    Init();

                    childOptions.ForEach(option => option.SetActive(state == HeadingState.Expanded));
                    headingState = state;

                    StoredHeadingStates.store(headingName, state);
                }
            }

            // click handler for arrow button
            private class ToggleButtonClickHandler: MonoBehaviour, IPointerClickHandler
            {
                private const float timeRotate = 0.1f;
                private HeadingState headingState = HeadingState.Expanded;
                private bool isRotating = false;

                public void SetStateInstant(HeadingState state)
                {
                    headingState = state;
                    transform.localEulerAngles = new Vector3(0, 0, headingState == HeadingState.Expanded? -90: 0);
                }

                public void OnPointerClick(PointerEventData _)
                {
                    if (isRotating)
                        return;

                    headingState = headingState == HeadingState.Expanded? HeadingState.Collapsed: HeadingState.Expanded;
                    StartCoroutine(SmoothRotate(headingState == HeadingState.Expanded? -90: 90));

                    GetComponentInParent<HeadingToggle>()?.SetState(headingState);
                }

                private IEnumerator SmoothRotate(float angles)
                {
                    isRotating = true;

                    Quaternion startRotation = transform.localRotation;
                    Quaternion endRotation = Quaternion.Euler(new Vector3(0f, 0f, angles)) * startRotation;

                    float timeStart = Time.realtimeSinceStartup; // Time.deltaTime works only in main menu

                    while (timeStart + timeRotate > Time.realtimeSinceStartup)
                    {
                        transform.localRotation = Quaternion.Lerp(startRotation, endRotation, (Time.realtimeSinceStartup - timeStart) / timeRotate);
                        yield return null;
                    }

                    transform.localRotation = endRotation;
                    isRotating = false;
                }
            }

            // click handler for title, just redirects clicks to button click handler
            private class HeadingClickHandler: MonoBehaviour, IPointerClickHandler
            {
                public void OnPointerClick(PointerEventData eventData) =>
                    transform.parent.GetComponentInChildren<ToggleButtonClickHandler>()?.OnPointerClick(eventData);
            }
#endregion

#region patches for uGUI_TabbedControlsPanel
            [PatchUtils.Prefix]
            [HarmonyPatch(typeof(uGUI_TabbedControlsPanel), nameof(uGUI_TabbedControlsPanel.AddHeading))]
            private static bool AddHeading_Prefix(uGUI_TabbedControlsPanel __instance, int tabIndex, string label)
            {
                if (tabIndex != modsTabIndex)
                    return true;

                __instance.AddItem(tabIndex, headingPrefab, label);
                return false;
            }

            [PatchUtils.Postfix]
            [HarmonyPatch(typeof(uGUI_TabbedControlsPanel), nameof(uGUI_TabbedControlsPanel.OnEnable))]
            private static void Awake_Postfix(uGUI_TabbedControlsPanel __instance)
            {
                InitHeadingPrefab(__instance);
            }

            [PatchUtils.Prefix]
            [HarmonyPatch(typeof(uGUI_TabbedControlsPanel), nameof(uGUI_TabbedControlsPanel.SetVisibleTab))]
            private static void SetVisibleTab_Prefix(uGUI_TabbedControlsPanel __instance, int tabIndex)
            {
                if (tabIndex != modsTabIndex)
                    return;

                // just in case, for changing vertical spacing between ui elements
                //__instance.tabs[tabIndex].container.GetComponent<VerticalLayoutGroup>().spacing = 15f; // default is 15f

                Transform options = __instance.tabs[tabIndex].container.transform;

                for (int i = 0; i < options.childCount; i++)
                    options.GetChild(i).GetComponent<HeadingToggle>()?.EnsureState();
            }
#endregion
        }


        // Patch class for saving scroll positions for tabs in options menu
        // Restores positions after switching between tabs and after reopening menu
        private static class ScrollPosKeeper
        {
            // key - tab index, value - scroll position
            private static readonly Dictionary<int, float> devMenuScrollPos = new Dictionary<int, float>();
            private static readonly Dictionary<int, float> optionsScrollPos = new Dictionary<int, float>();

            private static void StorePos(uGUI_TabbedControlsPanel panel, int tabIndex)
            {
                var scrollPos = panel is uGUI_DeveloperPanel? devMenuScrollPos: optionsScrollPos;
                if (tabIndex >= 0 && tabIndex < panel.tabs.Count)
                    scrollPos[tabIndex] = panel.tabs[tabIndex].pane.GetComponent<ScrollRect>().verticalNormalizedPosition;
            }

            private static void RestorePos(uGUI_TabbedControlsPanel panel, int tabIndex)
            {
                var scrollPos = panel is uGUI_DeveloperPanel? devMenuScrollPos: optionsScrollPos;
                if (tabIndex >= 0 && tabIndex < panel.tabs.Count && scrollPos.TryGetValue(tabIndex, out float pos))
                    panel.tabs[tabIndex].pane.GetComponent<ScrollRect>().verticalNormalizedPosition = pos;
            }

            [PatchUtils.Prefix]
            [HarmonyPatch(typeof(uGUI_TabbedControlsPanel), nameof(uGUI_TabbedControlsPanel.RemoveTabs))]
            private static void RemoveTabs_Prefix(uGUI_TabbedControlsPanel __instance)
            {
                StorePos(__instance, __instance.currentTab);
            }

            [PatchUtils.Postfix]
            [HarmonyPatch(typeof(uGUI_TabbedControlsPanel), nameof(uGUI_TabbedControlsPanel.HighlightCurrentTab))]
            private static void HighlightCurrentTab_Postfix(uGUI_TabbedControlsPanel __instance)
            {
                __instance.StartCoroutine(_restorePos());

                IEnumerator _restorePos()
                {
                    yield return null;
                    RestorePos(__instance, __instance.currentTab);
                }
            }

            [PatchUtils.Prefix]
            [HarmonyPatch(typeof(uGUI_TabbedControlsPanel), nameof(uGUI_TabbedControlsPanel.SetVisibleTab))]
            private static void SetVisibleTab_Prefix(uGUI_TabbedControlsPanel __instance, int tabIndex)
            {
                if (tabIndex != __instance.currentTab)
                    StorePos(__instance, __instance.currentTab);
            }

            [PatchUtils.Postfix]
            [HarmonyPatch(typeof(uGUI_TabbedControlsPanel), nameof(uGUI_TabbedControlsPanel.SetVisibleTab))]
            private static void SetVisibleTab_Postfix(uGUI_TabbedControlsPanel __instance, int tabIndex)
            {
                RestorePos(__instance, tabIndex);
            }
        }
    }
}
