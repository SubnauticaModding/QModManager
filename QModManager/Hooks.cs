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
        }

        [HarmonyPatch(typeof(DevConsole), "Start")]
        internal static class AddComponentPatch
        {
            [HarmonyPostfix]
            internal static void Postfix(DevConsole __instance)
            {
                __instance.gameObject.AddComponent<QMMHooks>();
                Start?.Invoke();
            }
        }

        internal class QMMHooks : MonoBehaviour
        {
            public void FixedUpdate() => Hooks.FixedUpdate?.Invoke();
            public void Update() => Hooks.Update?.Invoke();
            public void LateUpdate() => Hooks.LateUpdate?.Invoke();
            public void OnApplicationQuit() => Hooks.OnApplicationQuit?.Invoke();
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
