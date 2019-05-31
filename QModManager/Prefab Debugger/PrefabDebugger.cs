using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Logger = QModManager.Utility.Logger;

namespace QModManager.Debugger
{
    internal class PrefabDebugger : MonoBehaviour
    {
        internal HierarchyItem selectedGameObject = null;
        internal HierarchyItem draggedGameObject = null;
        internal PropertyInfo colorProperty;
        internal object component;
        internal Color selectedColor;

        internal bool addComponent = false;
        internal readonly Dictionary<string, bool> componentTabs = new Dictionary<string, bool>
        {
            { "Subnautica", true },
            { "UnityEngine", true },
            { "UnityEngine.UI", true },
        };
        internal readonly Dictionary<string, bool> prefabTabs = new Dictionary<string, bool>
        {
            { "Assets", true },
            { "Scene", true },
        };

        internal static AssetBundle guiBundle;
        internal static GUISkin skinUWE;
        internal TreeNode<HierarchyItem> sceneTree;
        internal Vector2 compScrollPos, hierarchyScrollPos, consoleScrollPos, addComponentScrollPos, sceneScrollPos;
        internal int numGameObjects = 0;

        /* This is a list of class properties that are either identical for each component or hidden in the Inspector
         * These can be shown/hidden in the Component view but should almost always be hidden because they make
         * Navigating and debugging each component more difficult and lack any actual usage in debugging scenarios
         */
        internal readonly string[] propertyBlacklist = { "enabled", "transform", "gameObject", "tag", "name",
            "hideFlags", "sortingLayerName", "sortingLayerID", "sharedMaterial",
            "sharedMaterials", "additionalVertexStreams","sharedMesh", "lightmapScaleOffset", "useGUILayout",
            "runInEditMode", "alphaHitTestMinimumThreshold", "onCullStateChanged", "maskable", "overrideSprite",
            "worldCamera", "planeDistance", "overridePixelPerfect", "normalizedSortingGridSize", "lightmapBakeType",
            "alreadyLightmapped", "realtimeLightmapScaleOffset", "attachedRigidbody"};

        /* This is a list of components that are either default or impossible to add to components normally, 
         * so they are listed here to be removed from the Add Component window.
         */
        internal readonly string[] componentBlacklist = { "Behaviour", "Component", "Transform", "MonoBehaviour", "GameObject" };

        internal Rect dragRect = new Rect(0, 0, 200, 50);
        internal Rect colorRect = new Rect(0, 0, 200, 400);
        internal Rect addComponentRect = new Rect(0, 0, 350, 400);
        internal Rect debuggerRect;
        internal static RectTransform debuggerRectTransform;
        internal static uGUI_InputGroup inputGroup;
        internal int selectedTab;
        internal Stack<LogMessage> debugMessages = new Stack<LogMessage>();
        internal bool showDebugger = false;
        internal bool showErrors, showLogs, showWarnings;
        internal Vector2 screenResolution;

        internal readonly string[] tabs = { "Scenes", "Hierarchy", "Options" };

        internal const string right_arrow = "▶";
        internal const string down_arrow = "▼";
        internal static Texture2D stop_symbol, warning_symbol, log_symbol;

        //User options that are saved and loaded
        internal bool showReadonlyProperties = false;
        internal bool showBlacklistedProperties = false;
        internal bool pauseWhenWindowOpen = false;
        internal int windowMargin = 50;
        internal int viewMargin = 10;

        internal static void Main()
        {
            GameObject debugger = new GameObject("PrefabDebugger");
            debugger.AddComponent<PrefabDebugger>();
            debuggerRectTransform = debugger.AddComponent<RectTransform>();
            inputGroup = debugger.AddComponent<uGUI_InputGroup>();
            Logger.Debug("Debugger initialized");
        }

        internal void Start()
        {
            Scene scene = SceneManager.GetActiveScene();
            numGameObjects = scene.rootCount;

            Application.logMessageReceived += HandleLog;
            DontDestroyOnLoad(this);

            //Load Player Settings
            windowMargin = PlayerPrefs.GetInt("QModManager_PrefabDebugger_WindowMargin", 50);
            viewMargin = PlayerPrefs.GetInt("QModManager_PrefabDebugger_ViewMargin", 10);
            showReadonlyProperties = PlayerPrefs.GetInt("QModManager_PrefabDebugger_ShowReadonlyProperties", 0) == 1 ? true : false;
            showBlacklistedProperties = PlayerPrefs.GetInt("QModManager_PrefabDebugger_ShowBlacklistedProperties", 0) == 1 ? true : false;
            pauseWhenWindowOpen = PlayerPrefs.GetInt("QModManager_PrefabDebugger_PauseWhenWindowOpen", 0) == 1 ? true : false;

            if (skinUWE == null)
            {
                guiBundle = AssetBundle.LoadFromFile(Path.Combine(Application.dataPath, "Managed/QModManagerAssets.unity3d"));
                if (guiBundle.LoadAsset("SubnauticaGUI") != null)
                {
                    skinUWE = (GUISkin)guiBundle.LoadAsset("SubnauticaGUI");

                    stop_symbol = (Texture2D)guiBundle.LoadAsset("stop");
                    warning_symbol = (Texture2D)guiBundle.LoadAsset("warning");
                    log_symbol = (Texture2D)guiBundle.LoadAsset("speech");
                }
                else
                {
                    Logger.Error("Could not load assets from \"QModManagerAssets.unity3d\"");
                }
            }
            LoadSceneObjects();
        }

        internal void LoadSceneObjects()
        {
            Scene scene = SceneManager.GetActiveScene();
            numGameObjects = scene.rootCount;

            HierarchyItem sceneItem = new HierarchyItem(null)
            {
                opened = false
            };
            sceneTree = new TreeNode<HierarchyItem>(sceneItem);
            foreach (GameObject go in scene.GetRootGameObjects())
            {
                PopulateTreeRecursively(go, sceneTree);
            }
        }

        internal void LateUpdate()
        {
            if ((Input.GetKeyDown(KeyCode.F9) && OptionsManager.DebuggerEnabled) || (showDebugger && Input.GetKeyDown(KeyCode.Escape)))
            {
                showDebugger = !showDebugger;
                UWE.Utils.alwaysLockCursor = false;
                UWE.Utils.lockCursor = false;
                if (showDebugger)
                {
                    if (pauseWhenWindowOpen)
                    {
                        UWE.FreezeTime.Begin("PrefabDebugger", true);
                    }

                    inputGroup.Select(true);
                }
                else
                {
                    if (pauseWhenWindowOpen)
                    {
                        UWE.FreezeTime.End("PrefabDebugger");
                    }

                    inputGroup.Deselect();
                }
                GamepadInputModule.current.SetCurrentGrid(null);
            }

            if (screenResolution != new Vector2(Screen.width, Screen.height))
            {
                debuggerRect = new Rect(windowMargin, windowMargin, Screen.width - (windowMargin * 2), Screen.height - (windowMargin * 2));
                screenResolution = new Vector2(Screen.width, Screen.height);
                debuggerRectTransform.sizeDelta = debuggerRect.size;
            }
        }

        internal void OnGUI()
        {
            //Set the GUI's style
            GUI.skin = skinUWE;

            if (showDebugger)
            {
                debuggerRect = GUILayout.Window(0, debuggerRect, ShowDebuggerWindow, "Prefab Debugger", GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
            }
            if (draggedGameObject != null)
            {
                Vector2 mousePos = Event.current.mousePosition;
                dragRect = GUILayout.Window(1, new Rect(mousePos.x + 10, mousePos.y - 10, 200, 20), ShowChangeParentWindow, "", "Blank", GUILayout.ExpandWidth(true));
            }
            if (colorProperty != null && component != null)
            {
                colorRect = GUILayout.Window(2, colorRect, ShowColorPaletteWindow, "Color Picker");
            }
            if (addComponent)
            {
                addComponentRect = GUILayout.Window(3, addComponentRect, ShowAddComponentWindow, "Add Component");
            }
        }

        internal void ShowDebuggerWindow(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, debuggerRect.width - 24, 40));

            if (DrawCloseButton())
            {
                showDebugger = false;
            }

            selectedTab = GUILayout.Toolbar(selectedTab, tabs, "TabButton");
            GUILayout.Space(viewMargin);
            //Draw the scene view
            if (selectedTab == 0)
            {
                GUILayout.BeginVertical("AreaBackground");

                sceneScrollPos = GUILayout.BeginScrollView(sceneScrollPos);

                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    try
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(SceneManager.GetSceneAt(i).name);
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Load Scene", "BlueButton", GUILayout.Width(75)))
                        {
                            SceneManager.LoadSceneAsync(i);
                        }
                        GUILayout.Space(10);
                        if (GUILayout.Button("Delete Scene", "RedButton", GUILayout.Width(75)))
                        {
                            SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
                        }

                        GUILayout.EndHorizontal();
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Failed to load scene:" + e.StackTrace);
                    }
                }

                GUILayout.EndScrollView();

                /*GUILayout.BeginHorizontal();
                GUILayout.Label("Scene Name");

                newScene = GUILayout.TextField(newScene, GUILayout.Width(200));
                if (GUILayout.Button("Create Scene", "BlueButton", GUILayout.Width(100)))
                {
                    SceneManager.CreateScene(newScene);
                }
                GUILayout.EndHorizontal();*/

                GUILayout.EndVertical();
            }
            //Draw the hierarchy view
            else if (selectedTab == 1)
            {
                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical("AreaBackground");
                GUILayout.Label((numGameObjects) + " Root Transforms in the Scene (Right Click Nodes to Change Parent)");

                hierarchyScrollPos = GUILayout.BeginScrollView(
     hierarchyScrollPos, GUILayout.MaxWidth((debuggerRect.width / 2) - (viewMargin * 2)));
                NavigateNodeRecursively(sceneTree);

                GUILayout.EndScrollView();
                GUILayout.EndVertical();

                GUILayout.BeginVertical("AreaBackground");
                GUILayout.Label("Components");

                compScrollPos = GUILayout.BeginScrollView(
             compScrollPos, GUILayout.MaxWidth((debuggerRect.width / 2) - (viewMargin * 2)));
                ShowSelectedComponents();

                GUILayout.EndScrollView();
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();

                GUILayout.Space(viewMargin);

                GUILayout.BeginVertical("AreaBackground");
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Clear Console", GUILayout.Width(100), GUILayout.ExpandWidth(false)))
                {
                    debugMessages = new Stack<LogMessage>();
                }
                if (GUILayout.Button("Reload Hierarchy", GUILayout.Width(120), GUILayout.ExpandWidth(false)))
                {
                    foreach (var emitter in UnityEngine.Object.FindObjectsOfType<FMODUnity.StudioEventEmitter>())
                    {
                        emitter.enabled = false;
                    }
                    LoadSceneObjects();
                }
                showLogs = GUILayout.Toggle(showLogs, "Show Logs", GUILayout.Width(100), GUILayout.ExpandWidth(false));
                showWarnings = GUILayout.Toggle(showWarnings, "Show Warnings", GUILayout.Width(100), GUILayout.ExpandWidth(false));
                showErrors = GUILayout.Toggle(showErrors, "Show Errors", GUILayout.Width(100), GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();

                consoleScrollPos = GUILayout.BeginScrollView(consoleScrollPos, GUILayout.MinHeight(200));
                foreach (LogMessage message in debugMessages)
                {
                    //Inefficient, I know
                    if (showLogs && message.type == LogType.Log)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(log_symbol, "ConsoleSymbol");
                        GUILayout.Label(message.logString + message.stackTrace, "ConsoleText");
                        GUILayout.EndHorizontal();
                    }
                    else if (showWarnings && message.type == LogType.Warning)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(warning_symbol, "ConsoleSymbol");
                        GUILayout.Label(message.logString + message.stackTrace, "ConsoleText");
                        GUILayout.EndHorizontal();
                    }
                    else if (showErrors && message.type == LogType.Error)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(stop_symbol, "ConsoleSymbol");
                        GUILayout.Label(message.logString + message.stackTrace, "ConsoleText");
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
            //Draw the options
            else if (selectedTab == 2)
            {
                GUILayout.BeginVertical("AreaBackground");

                GUILayout.Label("Window Settings", "HeaderLabel");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Window Margin", "NormalLabel", GUILayout.Width(100));
                windowMargin = (int)GUILayout.HorizontalSlider(windowMargin, 0, 100);
                GUILayout.Label(windowMargin.ToString(), "NormalLabel");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Viewport Margin", "NormalLabel", GUILayout.Width(100));
                viewMargin = (int)GUILayout.HorizontalSlider(viewMargin, 5, 50);
                GUILayout.Label(viewMargin.ToString(), "NormalLabel");

                GUILayout.EndHorizontal();

                pauseWhenWindowOpen = GUILayout.Toggle(pauseWhenWindowOpen, "Pause When Prefab Debugger is Open");

                GUILayout.Label("Property Settings", "HeaderLabel");

                showReadonlyProperties = GUILayout.Toggle(showReadonlyProperties, "Show Readonly Properties");
                showBlacklistedProperties = GUILayout.Toggle(showBlacklistedProperties, "Show Blacklisted Properties");
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Save Settings", GUILayout.Width(200)))
                {
                    PlayerPrefs.SetInt("QModManager_PrefabDebugger_WindowMargin", windowMargin);
                    PlayerPrefs.SetInt("QModManager_PrefabDebugger_ViewMargin", viewMargin);
                    PlayerPrefs.SetInt("QModManager_PrefabDebugger_ShowReadonlyProperties", showReadonlyProperties ? 1 : 0);
                    PlayerPrefs.SetInt("QModManager_PrefabDebugger_ShowBlacklistedProperties", showBlacklistedProperties ? 1 : 0);
                    PlayerPrefs.SetInt("QModManager_PrefabDebugger_PauseWhenWindowOpen", pauseWhenWindowOpen ? 1 : 0);
                }

                GUILayout.EndVertical();
            }
        }

        internal void ShowChangeParentWindow(int windowID)
        {
            GUI.FocusWindow(0);
            GUILayout.Button(draggedGameObject.source.name, "TreeItemSelected");
        }

        internal void ShowColorPaletteWindow(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, colorRect.width - 24, 40));
            if (DrawCloseButton())
            {
                colorProperty = null;
                return;
            }

            GUILayout.Label(MakeTex(250, 150, selectedColor), "ColorLabel", GUILayout.Width(250));
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("R");
            selectedColor.r = GUILayout.HorizontalSlider(selectedColor.r, 0, 1);
            GUILayout.Label((int)(selectedColor.r * 255) + "", "NormalLabel", GUILayout.Width(30));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("G");
            selectedColor.g = GUILayout.HorizontalSlider(selectedColor.g, 0, 1);
            GUILayout.Label((int)(selectedColor.g * 255) + "", "NormalLabel", GUILayout.Width(30));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("B");
            selectedColor.b = GUILayout.HorizontalSlider(selectedColor.b, 0f, 1f);
            GUILayout.Label((int)(selectedColor.b * 255) + "", "NormalLabel", GUILayout.Width(30));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("A");
            selectedColor.a = GUILayout.HorizontalSlider(selectedColor.a, 0f, 1f);
            GUILayout.Label((int)(selectedColor.a * 255) + "", "NormalLabel", GUILayout.Width(30));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Done"))
            {
                colorProperty.SetValue(component, selectedColor, null);
                colorProperty = null;
            }
        }

        internal void ShowAddComponentWindow(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, addComponentRect.width - 24, 40));
            if (DrawCloseButton())
            {
                addComponent = false;
            }
            //Get UnityEngine Assembly
            addComponentScrollPos = GUILayout.BeginScrollView(addComponentScrollPos);

            if (GUILayout.Button(GetOpenPrefix(componentTabs["UnityEngine"]) + "UnityEngine", "TreeItem"))
            {
                componentTabs["UnityEngine"] = !componentTabs["UnityEngine"];
            }

            if (componentTabs["UnityEngine"])
            {
                Assembly assembly = Assembly.GetAssembly(typeof(Component));
                IEnumerable<Type> subclasses = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Component)) || t.IsSubclassOf(typeof(Behaviour)));
                ShowSubclassComponents(subclasses, "UnityEngine.");
            }

            if (GUILayout.Button(GetOpenPrefix(componentTabs["UnityEngine.UI"]) + "UnityEngine.UI", "TreeItem"))
            {
                componentTabs["UnityEngine.UI"] = !componentTabs["UnityEngine.UI"];
            }
            if (componentTabs["UnityEngine.UI"])
            {
                Assembly assembly = Assembly.GetAssembly(typeof(Image));
                IEnumerable<Type> subclasses = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Component)) || t.IsSubclassOf(typeof(Behaviour)));
                ShowSubclassComponents(subclasses, "UnityEngine.UI.");
            }

            if (GUILayout.Button(GetOpenPrefix(componentTabs["Subnautica"]) + "Subnautica", "TreeItem"))
            {
                componentTabs["Subnautica"] = !componentTabs["Subnautica"];
            }

            if (componentTabs["Subnautica"])
            {
                Assembly assembly = Assembly.GetAssembly(typeof(Fabricator));
                //Assembly assembly = Assembly.GetAssembly(typeof(Fabricator));
                IEnumerable<Type> subclasses = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Component)) || t.IsSubclassOf(typeof(Behaviour)));
                ShowSubclassComponents(subclasses, "UWE.");
            }
            GUILayout.EndScrollView();
        }

        internal void ShowSubclassComponents(IEnumerable<Type> subclasses, string removePrefix)
        {
            foreach (Type type in subclasses)
            {
                if (Array.IndexOf(componentBlacklist, type.Name) == -1)
                {
                    if (GUILayout.Button("     " + type.ToString().Replace(removePrefix, ""), "TreeItem"))
                    {
                        selectedGameObject.source.AddComponent(type);
                        addComponent = false;
                    }
                }
            }
        }

        internal bool DrawCloseButton()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            bool closed = GUILayout.Button("", "CloseWindow"); 
            GUILayout.EndHorizontal();
            GUILayout.Space(30);

            return closed;
        }

        internal void ShowSelectedComponents()
        {
            if (selectedGameObject != null)
            {
                GUILayout.Label("Transform");
                Transform trans = selectedGameObject.source.GetComponent<Transform>();
                GUILayout.BeginVertical();

                trans.position = ShowVectorField(trans.position, "Position");
                trans.eulerAngles = ShowVectorField(trans.eulerAngles, "Rotation");
                trans.localScale = ShowVectorField(trans.localScale, "Scale");

                GUILayout.EndVertical();

                //By calling GetComponents with the Behaviour class, we can get every component that can 100% be disabled. 
                //Needs to be refactored because this code is super bad
                //I'm not sure if there's a better way to do this, I'm thinking about an alternative. The reason I have to do this is 
                //because while all of these inherit from component, components cannot be inherently enabled or disabled. 
                GameObject go = selectedGameObject.source;

                GetComponentsOfType(go, typeof(Component));

                GUILayout.Space(20);
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Add Component"))
                {
                    //Open component add menu
                    addComponent = true;
                }
            }
        }

        internal void GetComponentsOfType(GameObject gameObject, Type componentType)
        {
            foreach (var comp in gameObject.GetComponents(componentType))
            {
                if (Array.IndexOf(componentBlacklist, comp.GetType().Name) == -1)
                {
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(comp.GetType().Name, GUILayout.ExpandWidth(false));
                    PropertyInfo property = comp.GetType().GetProperty("enabled");
                    if (property != null)
                    {
                        property.SetValue(comp, GUILayout.Toggle((bool)(property.GetValue(comp, null)), "", "SmallToggle"), null);
                    }
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Delete", "RedButton"))
                    {
                        Destroy(comp);
                    }
                    GUILayout.EndHorizontal();
                    DisplayComponentProperties(comp);
                    GUILayout.EndVertical();
                }
            }
        }

        internal void DisplayComponentProperties(object comp)
        {
            foreach (var property in comp.GetType().GetProperties())
            {
                GUILayout.BeginHorizontal();
                try
                {
                    //Use reflection to get the Attributes (i.e [Obsolete "yadda yadda"] and other things like that) of this property
                    object[] attrs = property.GetCustomAttributes(true);
                    //Checks if the parameter has a set method (we don't want readonly) and if the parameter is marked Obsolete
                    if (attrs.OfType<ObsoleteAttribute>().Any() == false)
                    {
                        if (showReadonlyProperties || (property.GetSetMethod() != null && property.GetSetMethod().IsPublic))
                        {
                            if (showBlacklistedProperties || Array.IndexOf(propertyBlacklist, property.Name) == -1)
                            {
                                object value = property.GetValue(comp, null);
                                if ((property.GetSetMethod() == null || !property.GetSetMethod().IsPublic))
                                {
                                    GUILayout.Label(property.Name + " (Read Only)", "NormalLabel", GUILayout.ExpandHeight(false), GUILayout.Width(300));
                                }
                                else
                                {
                                    GUILayout.Label(property.Name, "NormalLabel", GUILayout.ExpandHeight(false), GUILayout.Width(300));
                                }
                                if (value == null)
                                {
                                    GUILayout.Label("None/Null", "NormalLabel");
                                }
                                if (value.GetType() == typeof(float) || value.GetType() == typeof(double))
                                {
                                    float val = (float)Convert.ToDouble(GUILayout.TextField(value.ToString(), GUILayout.Width(200)));
                                    property.SetValue(comp, val, null);
                                }
                                else if (value.GetType() == typeof(int))
                                {
                                    int val = Convert.ToInt32(GUILayout.TextField(value.ToString(), GUILayout.Width(200)));
                                    property.SetValue(comp, val, null);
                                }
                                else if (value.GetType() == typeof(string))
                                {
                                    property.SetValue(comp, GUILayout.TextField(value.ToString(), GUILayout.Width(200)), null);
                                }
                                else if (value.GetType() == typeof(bool))
                                {
                                    property.SetValue(comp, GUILayout.Toggle((bool)(property.GetValue(comp, null)), ""), null);
                                }
                                else if (value.GetType() == typeof(Vector3))
                                {
                                    Vector3 val = (Vector3)value;
                                    GUILayout.Label("X", "NormalLabel", GUILayout.Width(10));
                                    val.x = float.Parse(GUILayout.TextField(val.x.ToString(), GUILayout.Width(50)));
                                    GUILayout.Label("Y", "NormalLabel", GUILayout.Width(10));
                                    val.y = float.Parse(GUILayout.TextField(val.y.ToString(), GUILayout.Width(50)));
                                    GUILayout.Label("Z", "NormalLabel", GUILayout.Width(10));
                                    val.z = float.Parse(GUILayout.TextField(val.z.ToString(), GUILayout.Width(50)));
                                    property.SetValue(comp, val, null);
                                }
                                else if (value.GetType() == typeof(Vector2))
                                {
                                    Vector2 val = (Vector2)value;
                                    GUILayout.Label("X", "NormalLabel", GUILayout.Width(10));
                                    val.x = float.Parse(GUILayout.TextField(val.x.ToString(), GUILayout.Width(84)));
                                    GUILayout.Label("Y", "NormalLabel", GUILayout.Width(10));
                                    val.y = float.Parse(GUILayout.TextField(val.y.ToString(), GUILayout.Width(84)));
                                    property.SetValue(comp, val, null);
                                }
                                else if (value.GetType().IsEnum)
                                {
                                    if (GUILayout.Button(value.ToString(), "EnumButton"))
                                    {
                                        try
                                        {
                                            if (Enum.GetValues(value.GetType()).Length - 1 == (int)value)
                                            {
                                                property.SetValue(comp, Enum.GetValues(value.GetType()).GetValue(0), null);
                                            }
                                            else
                                            {
                                                property.SetValue(comp, (int)(value) + 1, null);
                                            }
                                        }
                                        catch
                                        {
                                            Debug.Log("Kill me, expected error :D");
                                        }
                                    }
                                }
                                else if (value.GetType() == typeof(Sprite))
                                {
                                    //Exporting coming soon!
                                    Sprite sprite = value as Sprite;
                                    GUILayout.TextField(sprite.texture.name, GUILayout.Width(200));
                                }
                                else if (value.GetType() == typeof(Color))
                                {
                                    Color val = (Color)value;
                                    if (GUILayout.Button(MakeTex(200, 20, val), "Blank"))
                                    {
                                        if (Input.GetMouseButtonUp(0))
                                        {
                                            component = comp;
                                            colorProperty = property;
                                        }
                                    }
                                }
                                else
                                {
                                    GUILayout.Label(value.ToString());
                                }
                            }
                        }
                    }
                    //GUILayout.Label("val:" + property.GetValue(comp, null));
                }
                catch (Exception)
                {
                    //GUILayout.Label("DBG ERR:" + e.StackTrace);
                    //UnityEngine.Debug.Log("msg:" + e.Message + "err" + e.Source);
                }
                GUILayout.EndHorizontal();
            }
        }

        internal void NavigateNodeRecursively(TreeNode<HierarchyItem> node, int depth = 0)
        {
            try
            {
                if (node.Item.source != null)
                {
                    GUILayout.BeginHorizontal();
                    string text = "", skin = "TreeItem";

                    for (int i = 0; i < depth; i++)
                    {
                        text += "     ";
                    }

                    if (node.Item.source.transform.childCount > 0)
                    {
                        if (node.Item.opened)
                        {
                            text += down_arrow;
                        }
                        else
                        {
                            text += right_arrow;
                        }
                    }

                    text += node.Item.source.name;

                    if (selectedGameObject == node.Item)
                    {
                        skin = "TreeItemSelected";
                    }

                    if (GUILayout.Button(text, skin))
                    {
                        if (Input.GetMouseButtonUp(0))
                        {
                            //Left
                            node.Item.opened = !node.Item.opened;
                            selectedGameObject = node.Item;
                        }
                        else if (Input.GetMouseButtonUp(1))
                        {
                            if (draggedGameObject != null)
                            {
                                draggedGameObject.source.transform.SetParent(node.Item.source.transform);
                                draggedGameObject = null;
                                //Replace the load with a more computationally efficient reload (todo)
                                LoadSceneObjects();
                            }
                            else if (draggedGameObject == null)
                            {
                                draggedGameObject = node.Item;
                            }
                        }
                    }

                    GUILayout.EndHorizontal();
                }
                else if (node.Item.source == null && depth == 0)
                {
                    GUILayout.BeginHorizontal();
                    string text = "", skin = "TreeItem";

                    for (int i = 0; i < depth; i++)
                    {
                        text += "     ";
                    }
                    if (node.Item.opened)
                    {
                        text += down_arrow;
                    }
                    else
                    {
                        text += right_arrow;
                    }

                    text += "Scene";

                    if (selectedGameObject == node.Item)
                    {
                        skin = "TreeItemSelected";
                    }

                    if (GUILayout.Button(text, skin))
                    {
                        if (Input.GetMouseButtonUp(0))
                        {
                            //Left
                            node.Item.opened = !node.Item.opened;
                        }
                        else if (Input.GetMouseButtonUp(1))
                        {
                            if (draggedGameObject != null)
                            {
                                draggedGameObject.source.transform.SetParent(null);
                                draggedGameObject = null;
                                //Replace the load with a more computationally efficient reload (todo)
                                LoadSceneObjects();
                            }
                        }
                    }

                    GUILayout.EndHorizontal();
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace);
                Debug.Log(e.Message);
            }
            if (node.Item.opened)
            {
                depth++;
                foreach (TreeNode<HierarchyItem> child in node.Children)
                {
                    NavigateNodeRecursively(child, depth);
                }
            }
        }

        internal void PopulateTreeRecursively(GameObject target, TreeNode<HierarchyItem> parent)
        {
            HierarchyItem node = new HierarchyItem(target)
            {
                opened = false
            };

            TreeNode<HierarchyItem> child = parent.AddChild(node);
            for (int i = 0; i < target.transform.childCount; i++)
            {
                PopulateTreeRecursively(target.transform.GetChild(i).gameObject, child);
            }
        }

        //This is broken and in the process of being fixed
        //Current issue: Doesn't remove gameobjects that aren't present in reload
        //Working: Adds new gameobjects and children seen in reload
        [Obsolete("Broken", true)]
        internal void RepopulateTreeRecursively()
        {
            TreeNode<HierarchyItem> updatedSceneTree = new TreeNode<HierarchyItem>();

            Scene scene = SceneManager.GetActiveScene();
            //Get an updated version of the Scene to merge with
            foreach (GameObject go in scene.GetRootGameObjects())
            {
                PopulateTreeRecursively(go, updatedSceneTree);
            }

            sceneTree = MergeTrees(updatedSceneTree, sceneTree);
        }

        internal TreeNode<HierarchyItem> MergeTrees(TreeNode<HierarchyItem> source, TreeNode<HierarchyItem> target)
        {
            foreach (TreeNode<HierarchyItem> item in source.Children)
            {
                TreeNode<HierarchyItem> match = target.Children.Find(x => x.Item.source = item.Item.source);
                if (match == null)
                {
                    target.Children.Add(item);
                }
                else
                {
                    MergeTrees(item, match);
                }
            }
            return target;
        }

        internal Vector3 ShowVectorField(Vector3 value, string title)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(title, "NormalLabel", GUILayout.Width(300));

            GUILayout.Label("X", "NormalLabel", GUILayout.Width(10));
            value.x = float.Parse(GUILayout.TextField(value.x.ToString(), GUILayout.Width(50)));
            GUILayout.Label("Y", "NormalLabel", GUILayout.Width(10));
            value.y = float.Parse(GUILayout.TextField(value.y.ToString(), GUILayout.Width(50)));
            GUILayout.Label("Z", "NormalLabel", GUILayout.Width(10));
            value.z = float.Parse(GUILayout.TextField(value.z.ToString(), GUILayout.Width(50)));

            GUILayout.EndHorizontal();

            return value;
        }

        internal bool ShowBoolField(bool value, string title)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, "NormalLabel", GUILayout.Width(150));
            GUILayout.Toggle(value, "");
            GUILayout.EndHorizontal();

            return value;
        }

        internal string ShowTextField(string value, string title)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, "NormalLabel", GUILayout.Width(150));
            value = GUILayout.TextField(value);

            return value;
        }

        internal Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        internal void HandleLog(string logString, string stackTrace, LogType type)
        {
            //Cap the debug messages saved at 100 to prevent lag
            if (debugMessages.Count > 100)
            {
                debugMessages.Pop();
            }
            debugMessages.Push(new LogMessage(logString, stackTrace, type));
        }

        internal string GetOpenPrefix(bool boolean)
        {
            if (boolean)
            {
                return down_arrow;
            }
            else
            {
                return right_arrow;
            }
        }
    }
}