using Harmony;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QModManager
{
    public static class Hooks
    {
        public static Delegates.Start Start;
        public static Delegates.FixedUpdate FixedUpdate;
        public static Delegates.Update Update;
        public static Delegates.LateUpdate LateUpdate;
        public static Delegates.OnApplicationQuit OnApplicationQuit;

        public static Delegates.SceneLoaded SceneLoaded;

        public static Delegates.OnLoadEnd OnLoadEnd;

        internal static void Load()
        {
            SceneManager.sceneLoaded += (scene, loadSceneMode) => SceneLoaded?.Invoke(scene, loadSceneMode);
            Logger.Debug("Loaded 'SceneLoaded' hook");
        }

        [HarmonyPatch(typeof(DevConsole), "Start")]
        private static class AddComponentPatch
        {
            [HarmonyPostfix]
            private static void Postfix(DevConsole __instance)
            {
                __instance.gameObject.AddComponent<QMMHooks>();

                Logger.Debug("Loaded the rest of the hooks");

                Start?.Invoke();
            }
        }

        private class QMMHooks : MonoBehaviour
        {
            private void FixedUpdate() => Hooks.FixedUpdate?.Invoke();
            private void Update() => Hooks.Update?.Invoke();
            private void LateUpdate() => Hooks.LateUpdate?.Invoke();
            private void OnApplicationQuit() => Hooks.OnApplicationQuit?.Invoke();
        }

        public class Delegates
        {
            public delegate void Start();
            public delegate void FixedUpdate();
            public delegate void Update();
            public delegate void LateUpdate();
            public delegate void OnApplicationQuit();

            public delegate void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode);

            public delegate void OnLoadEnd();
        }
    }
}
