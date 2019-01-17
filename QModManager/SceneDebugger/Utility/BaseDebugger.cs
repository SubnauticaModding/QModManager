// CODE FROM https://github.com/SubnauticaNitrox/Nitrox/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QModManager.SceneDebugger
{
    internal abstract class BaseDebugger
    {
        internal readonly string DebuggerName;
        internal readonly KeyCode Hotkey;
        internal readonly bool HotkeyAltRequired;
        internal readonly bool HotkeyControlRequired;
        internal readonly bool HotkeyShiftRequired;
        internal readonly GUISkinCreationOptions SkinCreationOptions;

        /// <summary>
        /// Currently active tab. This is the index used with <see cref="tabs"/>.
        /// </summary>
        internal DebuggerTab ActiveTab;

        internal bool CanDragWindow = true;

        internal bool Enabled;

        internal Rect WindowRect;

        /// <summary>
        /// Optional rendered tabs of the current debugger.
        /// </summary>
        /// <remarks>
        /// <see cref="ActiveTab"/> gives the index of the currently selected tab.
        /// </remarks>
        internal readonly Dictionary<string, DebuggerTab> tabs = new Dictionary<string, DebuggerTab>();

        internal BaseDebugger(int desiredWidth, string debuggerName = null, KeyCode hotkey = KeyCode.None, bool control = false, bool shift = false, bool alt = false, GUISkinCreationOptions skinOptions = GUISkinCreationOptions.DEFAULT)
        {
            if (desiredWidth < 200)
            {
                desiredWidth = 200;
            }

            WindowRect = new Rect(Screen.width / 2 - (desiredWidth / 2), 100, desiredWidth, 0); // Default position in center of screen.
            Hotkey = hotkey;
            HotkeyAltRequired = alt;
            HotkeyShiftRequired = shift;
            HotkeyControlRequired = control;
            SkinCreationOptions = skinOptions;

            if (string.IsNullOrEmpty(debuggerName))
            {
                string name = GetType().Name;
                DebuggerName = name.Substring(0, name.IndexOf("Debugger"));
            }
            else
            {
                DebuggerName = debuggerName;
            }
        }

        internal enum GUISkinCreationOptions
        {
            /// <summary>
            /// Uses the NitroxDebug skin.
            /// </summary>
            DEFAULT,

            /// <summary>
            /// Creates a copy of the default Unity IMGUI skin and sets the copied skin as render skin.
            /// </summary>
            UNITYCOPY,

            /// <summary>
            /// Creates a copy based on the NitroxDebug skin and sets the copied skin as render skin.
            /// </summary>
            DERIVEDCOPY
        }

        internal DebuggerTab AddTab(DebuggerTab tab)
        {
            Validate.NotNull(tab);

            tabs.Add(tab.Name, tab);
            return tab;
        }

        internal DebuggerTab AddTab(string name, Action render)
        {
            return AddTab(new DebuggerTab(name, render));
        }

        internal string GetHotkeyString()
        {
            if (Hotkey == KeyCode.None)
            {
                return "";
            }

            return $"{(HotkeyControlRequired ? "CTRL+" : "")}{(HotkeyAltRequired ? "ALT+" : "")}{(HotkeyShiftRequired ? "SHIFT+" : "")}{Hotkey}";
        }

        internal Optional<DebuggerTab> GetTab(string name)
        {
            Validate.NotNull(name);

            tabs.TryGetValue(name, out DebuggerTab tab);
            return Optional<DebuggerTab>.OfNullable(tab);
        }

        internal virtual void Update()
        {
            // Defaults to a no-op but can be overriden
        }

        /// <summary>
        /// Call this inside a <see cref="MonoBehaviour.OnGUI"/> method.
        /// </summary>
        internal void OnGUI()
        {
            if (!Enabled)
            {
                return;
            }

            GUISkin skin = GetSkin();
            GUISkinUtils.RenderWithSkin(skin, () =>
            {
                WindowRect = GUILayout.Window(GUIUtility.GetControlID(FocusType.Keyboard), WindowRect, RenderInternal, $"[DEBUGGER] {DebuggerName}", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            });
        }

        /// <summary>
        /// Optionally adjust the skin that is used during render.
        /// </summary>
        /// <remarks>
        /// Set <see cref="SkinCreationOptions"/> on <see cref="GUISkinCreationOptions.UNITYCOPY"/> or <see cref="GUISkinCreationOptions.DERIVEDCOPY"/> in constructor before using this method.
        /// </remarks>
        /// <param name="skin">Skin that is being used during <see cref="Render(int)"/>.</param>
        internal virtual void OnSetSkin(GUISkin skin)
        {
        }

        /// <summary>
        /// Optionally use a custom render solution for the debugger by overriding this method.
        /// </summary>
        internal virtual void Render()
        {
            ActiveTab?.Render();
        }

        /// <summary>
        /// Gets (a copy of) a skin specified by <see cref="GUISkinCreationOptions"/>.
        /// </summary>
        /// <returns>A reference to an existing or copied skin.</returns>
        internal GUISkin GetSkin()
        {
            GUISkin skin = GUI.skin;
            string skinName = GetSkinName();
            switch (SkinCreationOptions)
            {
                case GUISkinCreationOptions.DEFAULT:
                    skin = GUISkinUtils.RegisterDerivedOnce("debuggers.default", s =>
                    {
                        SetBaseStyle(s);
                        OnSetSkinImpl(s);
                    });
                    break;

                case GUISkinCreationOptions.UNITYCOPY:
                    skin = GUISkinUtils.RegisterDerivedOnce(skinName, OnSetSkinImpl);
                    break;

                case GUISkinCreationOptions.DERIVEDCOPY:
                    GUISkin baseSkin = GUISkinUtils.RegisterDerivedOnce("debuggers.default", SetBaseStyle);
                    skin = GUISkinUtils.RegisterDerivedOnce(skinName, OnSetSkinImpl, baseSkin);
                    break;
            }

            return skin;
        }

        internal string GetSkinName()
        {
            string name = GetType().Name;
            return $"debuggers.{name.Substring(0, name.IndexOf("Debugger")).ToLowerInvariant()}";
        }

        internal void OnSetSkinImpl(GUISkin skin)
        {
            if (SkinCreationOptions == GUISkinCreationOptions.DEFAULT)
            {
                Enabled = false;
                throw new NotSupportedException($"Cannot change {nameof(GUISkin)} for {GetType().FullName} when accessing the default skin. Change {nameof(SkinCreationOptions)} to something else than {nameof(GUISkinCreationOptions.DEFAULT)}.");
            }

            OnSetSkin(skin);
        }

        internal void RenderInternal(int windowId)
        {
            using (new GUILayout.HorizontalScope("Box"))
            {
                if (tabs.Count == 1)
                {
                    GUILayout.Label(tabs.First().Key, "tabActive");
                }
                else
                {
                    foreach (DebuggerTab tab in tabs.Values)
                    {
                        if (GUILayout.Button(tab.Name, ActiveTab == tab ? "tabActive" : "tab"))
                        {
                            ActiveTab = tab;
                        }
                    }
                }
            }

            Render();
            if (CanDragWindow)
            {
                GUI.DragWindow();
            }
        }

        internal void SetBaseStyle(GUISkin skin)
        {
            skin.label.alignment = TextAnchor.MiddleLeft;
            skin.label.margin = new RectOffset();
            skin.label.padding = new RectOffset();

            skin.SetCustomStyle("header", skin.label, s =>
            {
                s.margin.top = 10;
                s.margin.bottom = 10;
                s.alignment = TextAnchor.MiddleCenter;
                s.fontSize = 16;
                s.fontStyle = FontStyle.Bold;
            });

            skin.SetCustomStyle("tab", skin.button, s =>
            {
                s.fontSize = 16;
                s.margin = new RectOffset(5, 5, 5, 5);
            });

            skin.SetCustomStyle("tabActive", skin.button, s =>
            {
                s.fontStyle = FontStyle.Bold;
                s.fontSize = 16;
            });
        }

        internal class DebuggerTab
        {
            internal DebuggerTab(string name, Action render)
            {
                Validate.NotNull(name, $"Expected a name for the {nameof(DebuggerTab)}");
                Validate.NotNull(render, $"Expected an action for the {nameof(DebuggerTab)}");

                Name = name;
                Render = render;
            }

            internal string Name { get; }
            internal Action Render { get; }
        }
    }
}