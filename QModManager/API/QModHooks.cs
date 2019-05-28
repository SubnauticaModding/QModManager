using Harmony;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = QModManager.Utility.Logger;

namespace QModManager.API
{
    /// <summary>
    /// Contains some handy delegates to default Unity methods
    /// </summary>
    public static class QModHooks
    {
        /// <summary>
        /// Hooks into the <see cref="MonoBehaviour"/>.Start() method
        /// </summary>
        public static Delegates.Start Start;
        /// <summary>
        /// Hooks into the <see cref="MonoBehaviour"/>.FixedUpdate() method
        /// </summary>
        public static Delegates.FixedUpdate FixedUpdate;
        /// <summary>
        /// Hooks into the <see cref="MonoBehaviour"/>.Update() method, but is only called once and after <see cref="Start"/>
        /// </summary>
        public static Delegates.LateStart LateStart;
        /// <summary>
        /// Hooks into the <see cref="MonoBehaviour"/>.Update() method
        /// </summary>
        public static Delegates.Update Update;
        /// <summary>
        /// Hooks into the <see cref="MonoBehaviour"/>.LateUpdate() method
        /// </summary>
        public static Delegates.LateUpdate LateUpdate;
        /// <summary>
        /// Hooks into the <see cref="MonoBehaviour"/>.OnApplicationQuit() method
        /// </summary>
        public static Delegates.OnApplicationQuit OnApplicationQuit;

        /// <summary>
        /// Hooks into the <see cref="SceneManager.sceneLoaded"/> event
        /// </summary>
        public static Delegates.SceneLoaded SceneLoaded;

        /// <summary>
        /// Invoked after QModManager has finished loading
        /// </summary>
        public static Delegates.OnLoadEnd OnLoadEnd;

        /// <summary>
        /// Whether or not <see cref="LateStart"/> has been invoked
        /// </summary>
        public static bool LateStartInvoked { get; internal set; } = false;

        internal static void Load()
        {
            SceneManager.sceneLoaded += (scene, loadSceneMode) => SceneLoaded?.Invoke(scene, loadSceneMode);

#pragma warning disable CS0618 // Type or member is obsolete
            Hooks.Load();

            OnLoadEnd += () => Hooks.OnLoadEnd?.Invoke();
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [HarmonyPatch(typeof(DevConsole), "Start")]
        internal static class AddComponentPatch
        {
            internal static bool hooksLoaded = false;

            [HarmonyPostfix]
            internal static void Postfix(DevConsole __instance)
            {
                if (hooksLoaded) return;
                hooksLoaded = true;

                __instance.gameObject.AddComponent<QMMHooks>();

                Logger.Debug("Hooks loaded");

                Start?.Invoke();
            }
        }

        internal class QMMHooks : MonoBehaviour
        {
            internal void FixedUpdate()
            {
                if (!LateStartInvoked)
                {
                    LateStart?.Invoke();
                    LateStartInvoked = true;
                }
                QModHooks.FixedUpdate?.Invoke();
            }
            internal void Update() => QModHooks.Update?.Invoke();
            internal void LateUpdate() => QModHooks.LateUpdate?.Invoke();
            internal void OnApplicationQuit() => QModHooks.OnApplicationQuit?.Invoke();
        }

        /// <summary>
        /// The class where all of the delegates are defined
        /// </summary>
        public class Delegates
        {
            /// <summary>
            /// Delegate for <see cref="QModHooks.Start"/>
            /// </summary>
            public delegate void Start();
            /// <summary>
            /// Delegate for <see cref="QModHooks.FixedUpdate"/>
            /// </summary>
            public delegate void FixedUpdate();
            /// <summary>
            /// Delegate for <see cref="QModHooks.LateStart"/>
            /// </summary>
            public delegate void LateStart();
            /// <summary>
            /// Delegate for <see cref="QModHooks.Update"/>
            /// </summary>
            public delegate void Update();
            /// <summary>
            /// Delegate for <see cref="QModHooks.LateUpdate"/>
            /// </summary>
            public delegate void LateUpdate();
            /// <summary>
            /// Delegate for <see cref="QModHooks.OnApplicationQuit"/>
            /// </summary>
            public delegate void OnApplicationQuit();

            /// <summary>
            /// Delegate for <see cref="QModHooks.SceneLoaded"/>
            /// </summary>
            public delegate void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode);

            /// <summary>
            /// Delegate for <see cref="QModHooks.OnLoadEnd"/>
            /// </summary>
            public delegate void OnLoadEnd();
        }
    }
}
