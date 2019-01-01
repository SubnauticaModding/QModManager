using Harmony;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QModManager
{
    public class Hooks
    {
        public static Delegates.OnLoadEnd OnLoadEnd;
        public static Delegates.SceneLoaded SceneLoaded;
        public static Delegates.Start Start;
        public static Delegates.FixedUpdate FixedUpdate;
        public static Delegates.Update Update;
        public static Delegates.LateUpdate LateUpdate;
        public static Delegates.OnApplicationQuit OnApplicationQuit;

        internal static void Patch()
        {
            HarmonyInstance.Create("qmodmanager.subnautica").PatchAll();
            SceneManager.sceneLoaded += (scene, loadSceneMode) => SceneLoaded(scene, loadSceneMode);
        }

        [HarmonyPatch(typeof(GameInput), "Start")]
        internal static class AddComponentPatch
        {
            [HarmonyPostfix]
            internal static void Postfix(GameInput __instance)
            {
                __instance.gameObject.AddComponent<QMMHooks>();
            }
        }

        internal class QMMHooks : MonoBehaviour
        {
            public void Start() => Hooks.Start();
            public void FixedUpdate() => Hooks.FixedUpdate();
            public void Update() => Hooks.Update();
            public void LateUpdate() => Hooks.LateUpdate();
            public void OnApplicationQuit() => Hooks.OnApplicationQuit();
        }

        public class Delegates
        {
            public delegate void Awake();
            public delegate void OnLoadEnd();
            public delegate void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode);
            public delegate void Start();
            public delegate void FixedUpdate();
            public delegate void Update();
            public delegate void LateUpdate();
            public delegate void OnApplicationQuit();
        }
    }
}
