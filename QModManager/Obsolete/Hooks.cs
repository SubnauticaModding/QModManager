#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Harmony;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = QModManager.Utility.Logger;

namespace QModManager
{
    [Obsolete("Use QModManager.API.QModHooks instead")]
    public static class Hooks
    {
        [Obsolete("Use QModManager.API.QModHooks instead")]
        public static Delegates.Start Start;
        [Obsolete("Use QModManager.API.QModHooks instead")]
        public static Delegates.FixedUpdate FixedUpdate;
        [Obsolete("Use QModManager.API.QModHooks instead")]
        public static Delegates.LateStart LateStart;
        [Obsolete("Use QModManager.API.QModHooks instead")]
        public static Delegates.Update Update;
        [Obsolete("Use QModManager.API.QModHooks instead")]
        public static Delegates.LateUpdate LateUpdate;
        [Obsolete("Use QModManager.API.QModHooks instead")]
        public static Delegates.OnApplicationQuit OnApplicationQuit;

        [Obsolete("Use QModManager.API.QModHooks instead")]
        public static Delegates.SceneLoaded SceneLoaded;

        [Obsolete("Use QModManager.API.QModHooks instead")]
        public static Delegates.OnLoadEnd OnLoadEnd;

        [Obsolete("Use QModManager.API.QModHooks instead")]
        public static bool LateStartInvoked { get; internal set; } = false;

        internal static void Load()
        {
            SceneManager.sceneLoaded += (scene, loadSceneMode) => SceneLoaded?.Invoke(scene, loadSceneMode);
        }

        [HarmonyPatch(typeof(DevConsole), "Start")]
        private static class AddComponentPatch
        {
            private static bool hooksLoaded = false;

            [HarmonyPostfix]
            private static void Postfix(DevConsole __instance)
            {
                if (hooksLoaded) return;
                hooksLoaded = true;
                
                __instance.gameObject.AddComponent<QMMHooks>();

                Logger.Debug("Legacy hooks loaded");

                Start?.Invoke();
            }
        }

        private class QMMHooks : MonoBehaviour
        {
            private void FixedUpdate()
            {
                if (!LateStartInvoked)
                {
                    LateStart?.Invoke();
                    LateStartInvoked = true;
                }
                Hooks.FixedUpdate?.Invoke();
            }
            private void Update() => Hooks.Update?.Invoke();
            private void LateUpdate() => Hooks.LateUpdate?.Invoke();
            private void OnApplicationQuit() => Hooks.OnApplicationQuit?.Invoke();
        }

        [Obsolete("Use QModManager.API.QModHooks instead")]
        public class Delegates
        {
            [Obsolete("Use QModManager.API.QModHooks instead")]
            public delegate void Start();
            [Obsolete("Use QModManager.API.QModHooks instead")]
            public delegate void FixedUpdate();
            [Obsolete("Use QModManager.API.QModHooks instead")]
            public delegate void LateStart();
            [Obsolete("Use QModManager.API.QModHooks instead")]
            public delegate void Update();
            [Obsolete("Use QModManager.API.QModHooks instead")]
            public delegate void LateUpdate();
            [Obsolete("Use QModManager.API.QModHooks instead")]
            public delegate void OnApplicationQuit();

            [Obsolete("Use QModManager.API.QModHooks instead")]
            public delegate void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode);

            [Obsolete("Use QModManager.API.QModHooks instead")]
            public delegate void OnLoadEnd();
        }
    }
}
