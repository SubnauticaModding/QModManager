using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QModManager.Debugger
{
    public class PrefabDebugger : MonoBehaviour
    {
        //THINGS I NEED TO ADD

        //Raycasting objects to view their components (in game highlighting of the object would be cool)
        //Adding/Removing objects and their components
        //Make the UI look much better (it sucks)
        //Change the width of a component's property labels to not be hardcoded
        //Fix the repopulate method so it works properly
        //Refactor ShowSelectedComponents method to not be so bad

        private HierarchyItem selectedGameObject = null;
        private HierarchyItem draggedGameObject = null;
        private GUISkin skinUWE;
        private TreeNode<HierarchyItem> sceneTree;
        private Vector2 compScrollPos, hierarchyScrollPos, consoleScrollPos;
        private int numGameObjects = 0;
        private string[] propertyBlacklist = { "enabled", "transform", "gameObject", "tag", "name", "hideFlags" };
        private Rect hierarchyRect = new Rect(100, 25, 500, 600);
        private Rect componentRect = new Rect(600, 25, 500, 600);
        private Rect consoleRect = new Rect(100, Screen.height - 300, 1000, 200);
        private Rect dragRect = new Rect(0, 0, 200, 50);
        private Stack<LogMessage> debugMessages = new Stack<LogMessage>();
        private bool showDebugger = false;
        private bool showErrors, showLogs, showWarnings;

        private const string right_arrow = "▶";
        private const string down_arrow = "▼";

        internal static void Main()
        {
            new GameObject("PrefabDebugger").AddComponent<PrefabDebugger>();
        }

        private void Start()
        {
            Scene scene = SceneManager.GetActiveScene();
            numGameObjects = scene.rootCount;
            sceneTree = new TreeNode<HierarchyItem>();
            Application.logMessageReceived += HandleLog;
            DontDestroyOnLoad(this);

            if (skinUWE == null)
            {
                var assets = AssetBundle.LoadFromFile(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace('\\', '/') + "/PrefabDebugger.unity3d");
                if (assets.LoadAsset("SubnauticaGUI") != null)
                {
                    skinUWE = (GUISkin)assets.LoadAsset("SubnauticaGUI");
                }
            }
            LoadSceneObjects();
        }

        private void LoadSceneObjects()
        {
            Scene scene = SceneManager.GetActiveScene();
            numGameObjects = scene.rootCount;
            sceneTree = new TreeNode<HierarchyItem>();
            foreach (GameObject go in scene.GetRootGameObjects())
            {
                PopulateTreeRecursively(go, sceneTree);
            }
        }

        private void LateUpdate()
        {
            if (Input.GetKeyUp(KeyCode.F9))
            {
                showDebugger = !showDebugger;
                UWE.Utils.alwaysLockCursor = false;
                UWE.Utils.lockCursor = false;
            }
        }

        private void OnGUI()
        {
            //Set the GUI's style
            GUI.skin = skinUWE;

            if (showDebugger)
            {
                hierarchyRect = GUILayout.Window(0, hierarchyRect, ShowHierarchyWindow, "Hierarchy Window");
                componentRect = GUILayout.Window(1, componentRect, ShowComponentsWindow, "Component Window");
                consoleRect = GUILayout.Window(2, consoleRect, ShowDebugWindow, "Debug Window");
            }
            if (draggedGameObject != null)
            {
                var mousePos = Event.current.mousePosition;
                dragRect = GUILayout.Window(3, new Rect(mousePos.x + 10, mousePos.y - 10, 200, 20), ShowChangeParentWindow, "", "Blank", GUILayout.ExpandWidth(true));
            }
        }

        private void ShowChangeParentWindow(int windowID)
        {
            GUI.FocusWindow(0);
            GUILayout.Button(draggedGameObject.source.name, "TreeItemSelected");
        }

        private void ShowHierarchyWindow(int windowID)
        {
            //Ensure that the window can be dragged
            GUI.DragWindow(new Rect(0, 0, 500, 40));

            GUILayout.Label((numGameObjects) + " Root Transforms in the Scene (Right Click Nodes to Change Parent)");

            hierarchyScrollPos = GUILayout.BeginScrollView(
 hierarchyScrollPos, GUILayout.Width(490), GUILayout.Height(600));
            NavigateNodeRecursively(sceneTree);
            GUILayout.EndScrollView();
        }

        private void ShowComponentsWindow(int windowID)
        {
            //Ensure that the window can be dragged
            GUI.DragWindow(new Rect(0, 0, 500, 40));

            GUILayout.Label("Components");
            compScrollPos = GUILayout.BeginScrollView(
         compScrollPos, GUILayout.Width(490), GUILayout.Height(600));
            ShowSelectedComponents();
            GUILayout.EndScrollView();
        }

        private void ShowDebugWindow(int windowID)
        {
            //Ensure that the window can be dragged
            GUI.DragWindow(new Rect(0, 0, 900, 40));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear Console", GUILayout.Width(100), GUILayout.ExpandWidth(false)))
            {
                debugMessages = new Stack<LogMessage>();
            }
            if (GUILayout.Button("Reload Hierarchy", GUILayout.Width(120), GUILayout.ExpandWidth(false)))
            {
                LoadSceneObjects();
            }
            showLogs = GUILayout.Toggle(showLogs, "Show Logs", GUILayout.Width(100), GUILayout.ExpandWidth(false));
            showWarnings = GUILayout.Toggle(showWarnings, "Show Warnings", GUILayout.Width(100), GUILayout.ExpandWidth(false));
            showErrors = GUILayout.Toggle(showErrors, "Show Errors", GUILayout.Width(100), GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();

            consoleScrollPos = GUILayout.BeginScrollView(consoleScrollPos, GUILayout.Width(900), GUILayout.Height(200));
            foreach (LogMessage message in debugMessages)
            {
                //Inefficient, I know
                if (showLogs && message.type == LogType.Log)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label(message.type.ToString() + ": " + message.logString);
                    GUILayout.Label(message.stackTrace);
                    GUILayout.EndVertical();
                }
                else if (showWarnings && message.type == LogType.Warning)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label(message.type.ToString() + ": " + message.logString);
                    GUILayout.Label(message.stackTrace);
                    GUILayout.EndVertical();
                }
                else if (showErrors && message.type == LogType.Error)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label(message.type.ToString() + ": " + message.logString);
                    GUILayout.Label(message.stackTrace);
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndScrollView();
        }

        private void ShowSelectedComponents()
        {
            if (selectedGameObject != null)
            {
                GUILayout.Label("Transform");
                Transform trans = selectedGameObject.source.GetComponent<Transform>();
                GUILayout.BeginVertical();

                ShowVectorField(trans.position, "Position");
                ShowVectorField(trans.eulerAngles, "Rotation");
                ShowVectorField(trans.localScale, "Scale");


                GUILayout.EndVertical();


                //By calling GetComponents with the Behaviour class, we can get every component that can 100% be disabled. 
                //Needs to be refactored because this code is super bad
                //I'm not sure if there's a better way to do this, I'm thinking about an alternative. The reason I have to do this is 
                //because while all of these inherit from component, components cannot be inherently enabled or disabled. 

                if (selectedGameObject.source.GetComponent<MeshFilter>() != null)
                {
                    var mesh = selectedGameObject.source.GetComponent<MeshFilter>();
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Mesh Filter", GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();
                    DisplayComponentProperties(mesh);
                    GUILayout.EndVertical();
                }

                //By calling GetComponents with the Behaviour class, we can get every component that can 100% be disabled. 
                foreach (var comp in selectedGameObject.source.GetComponents<Behaviour>())
                {
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(comp.GetType().Name, GUILayout.ExpandWidth(false));
                    comp.enabled = GUILayout.Toggle(comp.enabled, "", "SmallToggle");
                    GUILayout.EndHorizontal();
                    DisplayComponentProperties(comp);
                    GUILayout.EndVertical();
                }
                //Temporary Band-Aid fix, check for different component subtypes
                foreach (var comp in selectedGameObject.source.GetComponents<Collider>())
                {
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(comp.GetType().Name, GUILayout.ExpandWidth(false));
                    comp.enabled = GUILayout.Toggle(comp.enabled, "", "SmallToggle");
                    GUILayout.EndHorizontal();
                    DisplayComponentProperties(comp);
                    GUILayout.EndVertical();
                }
                foreach (var comp in selectedGameObject.source.GetComponents<Renderer>())
                {
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(comp.GetType().Name, GUILayout.ExpandWidth(false));
                    comp.enabled = GUILayout.Toggle(comp.enabled, "", "SmallToggle");
                    GUILayout.EndHorizontal();
                    DisplayComponentProperties(comp);
                    GUILayout.EndVertical();
                }
                if (GUILayout.Button("Add Component"))
                {
                    //Open component add menu
                }
            }
        }

        private void DisplayComponentProperties(object comp)
        {
            foreach (var property in comp.GetType().GetProperties())
            {
                GUILayout.BeginHorizontal();
                try
                {
                    //Use reflection to get the Attributes (i.e [Obsolete "yadda yadda"] and other things like that) of this property
                    object[] attrs = property.GetCustomAttributes(true);
                    //Checks if the parameter has a set method (we don't want readonly) and if the parameter is marked Obsolete
                    if (attrs.OfType<ObsoleteAttribute>().Any() == false && property.GetSetMethod() != null && property.GetSetMethod().IsPublic && Array.IndexOf(propertyBlacklist, property.Name) == -1)
                    {
                        var value = property.GetValue(comp, null);
                        GUILayout.Label(property.Name, "NormalLabel", GUILayout.ExpandHeight(false), GUILayout.Width(150));

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
                            var val = (Color)value;
                            GUILayout.Label(MakeTex(200, 20, val));
                        }
                        else
                        {
                            GUILayout.Label(value.ToString());
                        }
                    }
                    //GUILayout.Label("val:" + property.GetValue(comp, null));
                }
                catch (Exception e)
                {
                    GUILayout.Label("DBG ERR:" + e.Message);
                    //UnityEngine.Debug.Log("msg:" + e.Message + "err" + e.Source);
                }
                GUILayout.EndHorizontal();
            }
        }

        private void NavigateNodeRecursively(TreeNode<HierarchyItem> node, int depth = 0)
        {
            if (node.Item != null)
            {
                try
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
                                Debug.Log("Release");
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
                catch
                {

                }

            }
            if (node.Item == null || node.Item.opened)
            {
                depth++;
                foreach (TreeNode<HierarchyItem> child in node.Children)
                {
                    NavigateNodeRecursively(child, depth);
                }
            }
        }

        private void PopulateTreeRecursively(GameObject target, TreeNode<HierarchyItem> parent)
        {
            HierarchyItem node = new HierarchyItem(target)
            {
                opened = false
            };

            var child = parent.AddChild(node);
            for (int i = 0; i < target.transform.childCount; i++)
            {
                PopulateTreeRecursively(target.transform.GetChild(i).gameObject, child);
            }
        }

        //This is broken and in the process of being fixed
        //Current issue: Doesn't remove gameobjects that aren't present in reload
        //Working: Adds new gameobjects and children seen in reload
        private void RepopulateTreeRecursively()
        {
            var updatedSceneTree = new TreeNode<HierarchyItem>();

            Scene scene = SceneManager.GetActiveScene();
            //Get an updated version of the Scene to merge with
            foreach (GameObject go in scene.GetRootGameObjects())
            {
                PopulateTreeRecursively(go, updatedSceneTree);
            }

            sceneTree = MergeTrees(updatedSceneTree, sceneTree);
        }

        private TreeNode<HierarchyItem> MergeTrees(TreeNode<HierarchyItem> source, TreeNode<HierarchyItem> target)
        {
            foreach (TreeNode<HierarchyItem> item in source.Children)
            {
                var match = target.Children.Find(x => x.Item.source = item.Item.source);
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

        private Vector3 ShowVectorField(Vector3 value, string title)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(title, "NormalLabel", GUILayout.Width(150));

            GUILayout.Label("X", "NormalLabel", GUILayout.Width(10));
            value.x = float.Parse(GUILayout.TextField(value.x.ToString(), GUILayout.Width(50)));
            GUILayout.Label("Y", "NormalLabel", GUILayout.Width(10));
            value.y = float.Parse(GUILayout.TextField(value.y.ToString(), GUILayout.Width(50)));
            GUILayout.Label("Z", "NormalLabel", GUILayout.Width(10));
            value.z = float.Parse(GUILayout.TextField(value.z.ToString(), GUILayout.Width(50)));

            GUILayout.EndHorizontal();

            return value;
        }

        private bool ShowBoolField(bool value, string title)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, "NormalLabel", GUILayout.Width(150));
            GUILayout.Toggle(value, "");
            GUILayout.EndHorizontal();

            return value;
        }

        private string ShowTextField(string value, string title)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, "NormalLabel", GUILayout.Width(150));
            value = GUILayout.TextField(value);

            return value;
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            //Cap the debug messages saved at 500 to prevent lag
            if (debugMessages.Count > 100)
            {
                debugMessages.Pop();
            }
            debugMessages.Push(new LogMessage(logString, stackTrace, type));
        }
    }

}