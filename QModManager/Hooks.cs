using Harmony;
using UnityEngine.SceneManagement;

namespace QModManager
{
    public class Hooks
    {
        public static Delegates.Awake Awake;
        public static Delegates.SceneLoaded SceneLoaded;
        public static Delegates.Start Start;
        public static Delegates.FixedUpdate FixedUpdate;
        public static Delegates.Update Update;
        public static Delegates.LateUpdate LateUpdate;

        public static Delegates.OnLoadEnd OnLoadEnd;

        internal static void Patch()
        {
            HarmonyInstance.Create("qmodmanager.subnautica").PatchAll();
            SceneManager.sceneLoaded += (scene, loadSceneMode) => SceneLoaded(scene, loadSceneMode);
        }

        internal static class Patches
        {
            [HarmonyPatch(typeof(GameInput), "Awake")]
            internal static class GameInput_Awake_Patch
            {
                [HarmonyPostfix]
                internal static void Postfix()
                {
                    Awake();
                }
            }

            [HarmonyPatch(typeof(GameInput), "Start")]
            internal static class GameInput_Start_Patch
            {
                [HarmonyPostfix]
                internal static void Postfix()
                {
                    Start();
                }
            }

            [HarmonyPatch(typeof(GameInput), "FixedUpdate")]
            internal static class GameInput_FixedUpdate_Patch
            {
                [HarmonyPostfix]
                internal static void Postfix()
                {
                    FixedUpdate();
                }
            }

            [HarmonyPatch(typeof(GameInput), "Update")]
            internal static class GameInput_Update_Patch
            {
                [HarmonyPostfix]
                internal static void Postfix()
                {
                    Update();
                }
            }

            [HarmonyPatch(typeof(GameInput), "LateUpdate")]
            internal static class GameInput_LateUpdate_Patch
            {
                [HarmonyPostfix]
                internal static void Postfix()
                {
                    LateUpdate();
                }
            }
        }

        public class Delegates
        {
            public delegate void Awake();
            public delegate void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode);
            public delegate void Start();
            public delegate void FixedUpdate();
            public delegate void Update();
            public delegate void LateUpdate();

            public delegate void OnLoadEnd();
        }
    }
}
