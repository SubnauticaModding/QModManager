namespace QModManager.Utility
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Collections;
    using System.Collections.Generic;
    using HarmonyLib;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using QModManager.Patching;

    /// <summary>
    /// Allows to add critical messages to the main menu.
    /// Messages will stay in the main menu and on loading screen.
    /// </summary>
    internal static class MainMenuMessages
    {
        internal const int defaultSize = 25;
        internal const string defaultColor = "red";

        private static readonly Vector2 newOffset = new Vector2(20f, 20f);
        private static Vector2 prevOffset;
        private static Vector2? prevSize;

        private static List<string> messageQueue;
        private static List<ErrorMessage._Message> messages;

        private static bool inited => messageQueue != null;

        // we'll restore original offset for messages if position is changed by some other mod (e.g. HudConfig)
        private class CorrectMsgPos: MonoBehaviour
        {
            public void Update()
            {
                if (ErrorMessage.main.messageCanvas.localPosition == Vector3.zero)
                    return;

                ErrorMessage.main.offset = prevOffset;
                Destroy(this);
            }
        }


        /// <summary>Adds an error message to the main menu.</summary>
        /// <param name="msg">The message to add.</param>
        /// <param name="callerID">The ID of the caller (or null for "QModManager").</param>
        /// <param name="size">The size of the text.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="autoformat">Whether or not to apply formatting tags to the message, or show it as it is.</param>
        internal static void Add(string msg, string callerID = null, int size = defaultSize, string color = defaultColor, bool autoformat = true)
        {
            if (Patcher.hInstance == null) // just in case
            {
                Logger.Error($"Tried to add main menu message before Harmony was initialized. (Message: \"{msg}\")");
                return;
            }

            if (SceneManager.GetSceneByName("Main").isLoaded) // it works just like regular ErrorMessage outside of main menu
            {
                ErrorMessage.AddDebug(msg);
                return;
            }

            Init();
            Logger.Debug($"Created message: \"{msg}\"");

            if (autoformat)
                msg = $"<size={size}><color={color}><b>[{callerID ?? "QModManager"}]:</b> {msg}</color></size>";

            if (ErrorMessage.main != null)
                AddInternal(msg);
            else
                messageQueue.Add(msg);
        }

        private static void Init()
        {
            if (inited)
                return;

            LoadDynamicAssembly();
            messageQueue = new List<string>();
            messages = new List<ErrorMessage._Message>();
            Patches.Patch();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void AddInternal(string msg)
        {
            ErrorMessage.AddDebug(msg);

            var message = ErrorMessage.main.GetExistingMessage(msg);
            messages.Add(message);
            message.timeEnd += 1e6f;

            prevSize ??= GetRectTransform(message).sizeDelta;
            GetRectTransform(message).sizeDelta = new Vector2(1920f - ErrorMessage.main.offset.x * 2f, 0f);

            ErrorMessage.main.gameObject.EnsureComponent<CorrectMsgPos>();
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode _)
        {
            if (scene.name != "Main")
                return;

            SceneManager.sceneLoaded -= OnSceneLoaded;
            ErrorMessage.main.StartCoroutine(_waitForLoad());

            static IEnumerator _waitForLoad()
            {
                yield return new WaitForSeconds(1f);

                yield return new WaitWhile(() => SaveLoadManager.main.isLoading);

                messages.ForEach(msg => msg.timeEnd = Time.time + 1f);
                yield return new WaitForSeconds(1.1f); // wait for messages to dissapear

                messages.ForEach(msg => GetRectTransform(msg).sizeDelta = (Vector2)prevSize);
                messages.Clear();

                Patches.Unpatch();

                yield return new WaitForSeconds(0.5f);

                ErrorMessage.main.offset = prevOffset;
                Component.Destroy(ErrorMessage.main.GetComponent<CorrectMsgPos>());
            }
        }

        #region Dynamic assembly loading

        private static Type SelectedTextType;
        private static Func<object, RectTransform> GetRectTransform;

        private static void LoadDynamicAssembly()
        {
            if (SelectedTextType != null)
                return;

            Type TxtType = typeof(UnityEngine.UI.Text);
            Type TxtProType = Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro", false, false);

            SelectedTextType = TxtProType ?? TxtType;

            FieldInfo entryField = typeof(ErrorMessage._Message).GetField("entry");
            if (TxtProType != null)
            {
                // Using TextMeshPro
                FieldInfo recTransformField = TxtProType.GetField("m_rectTransform");

                GetRectTransform = (obj) =>
                {
                    var entry = entryField.GetValue(obj);
                    return (RectTransform)recTransformField.GetValue(entry);
                };
            }
            else
            {
                // Using Text
                PropertyInfo recTransformProperty = TxtType.GetProperty("rectTransform");

                GetRectTransform = (obj) =>
                {
                    var entry = entryField.GetValue(obj);
                    return (RectTransform)recTransformProperty.GetValue(entry, null);
                };
            }
        }

        #endregion Dynamic assembly loading

        private static class Patches
        {
            public static void Patch()
            {
                Logger.Debug("Patching ErrorMessage");

                Patcher.hInstance.PatchAll(typeof(Patches));
            }

            public static void Unpatch()
            {
                Logger.Debug("Unpatching ErrorMessage");

                Patcher.hInstance.Unpatch(AccessTools.Method(typeof(ErrorMessage), nameof(ErrorMessage.Awake)),
                    HarmonyPatchType.Postfix, Patcher.hInstance.Id);

                Patcher.hInstance.Unpatch(AccessTools.Method(typeof(ErrorMessage), nameof(ErrorMessage.OnUpdate)),
                    HarmonyPatchType.Transpiler, Patcher.hInstance.Id);
            }

            [HarmonyPostfix, HarmonyPatch(typeof(ErrorMessage), nameof(ErrorMessage.Awake))]
            private static void ErrorMessage_Awake_Postfix()
            {
                prevOffset = ErrorMessage.main.offset;

                if (!inited)
                    return;

                ErrorMessage.main.offset = newOffset;

                messageQueue.ForEach(msg => AddInternal(msg));
                messageQueue.Clear();
            }


            // we changing result for 'float value = Mathf.Clamp01(MathExtensions.EvaluateLine(...' to 1.0f
            // so text don't stay in the center of the screen (because of changed 'timeEnd')
            [HarmonyTranspiler, HarmonyPatch(typeof(ErrorMessage), nameof(ErrorMessage.OnUpdate))]
            private static IEnumerable<CodeInstruction> ErrorMessage_OnUpdate_Transpiler(IEnumerable<CodeInstruction> cins)
            {
                try
                {
                    return new CodeMatcher(cins).
                        End().
                        // searching for line '_Message message2 = messages[j]' in the second loop (first from the end)
                        MatchBack(true, new CodeMatch(OpCodes.Callvirt, typeof(List<ErrorMessage._Message>).GetMethod("get_Item"))).
                        Advance(1).
                        // copying current 'message2' on stack
                        Insert(new CodeInstruction(OpCodes.Dup)).
                        // searching for line 'float value = Mathf.Clamp01(MathExtensions.EvaluateLine(...'
                        MatchForward(true, new CodeMatch(OpCodes.Call, typeof(Mathf).GetMethod(nameof(Mathf.Clamp01)))).
                        Advance(1).
                        // if 'message2' is added by MainMenuMessages, then use 1.0f instead of calculated value (using copied 'message2' and current 'value')
                        Insert(Transpilers.EmitDelegate<Func<ErrorMessage._Message, float, float>>((message, val) => messages.Contains(message)? 1f: val)).
                        InstructionEnumeration();
                }
                catch (Exception e)
                {
                    Logger.Error($"Failed to patch ErrorMessage.OnUpdate() ({e})");
                    return cins;
                }
            }
        }
    }
}
