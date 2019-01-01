using Harmony;
using System;
using UnityEngine.SceneManagement;

namespace QModManager
{
    public class Hooks
    {
        public static event EventHandler<SceneLoadedEventArgs> SceneLoaded;
        public static event EventHandler Awake;
        public static event EventHandler Start;
        public static event EventHandler FixedUpdate;
        public static event EventHandler Update;
        public static event EventHandler LateUpdate;

        internal static void Patch()
        {
            HarmonyInstance.Create("qmodmanager.subnautica").PatchAll();
            SceneManager.sceneLoaded += (scene, loadSceneMode) => 
            {
                SceneLoaded(null, new SceneLoadedEventArgs()
                {
                    scene = scene,
                    loadSceneMode = loadSceneMode
                });
            };
        }

        internal static class Patches
        {
            [HarmonyPatch(typeof(GameInput), "Awake")]
            internal static class GameInput_Awake_Patch
            {
                [HarmonyPostfix]
                internal static void Postfix()
                {
                    Awake(null, null);
                }
            }

            [HarmonyPatch(typeof(GameInput), "Start")]
            internal static class GameInput_Start_Patch
            {
                [HarmonyPostfix]
                internal static void Postfix()
                {
                    Start(null, null);
                }
            }

            [HarmonyPatch(typeof(GameInput), "FixedUpdate")]
            internal static class GameInput_FixedUpdate_Patch
            {
                [HarmonyPostfix]
                internal static void Postfix()
                {
                    FixedUpdate(null, null);
                }
            }

            [HarmonyPatch(typeof(GameInput), "Update")]
            internal static class GameInput_Update_Patch
            {
                [HarmonyPostfix]
                internal static void Postfix()
                {
                    Update(null, null);
                }
            }

            [HarmonyPatch(typeof(GameInput), "Start")]
            internal static class GameInput_LateUpdate_Patch
            {
                [HarmonyPostfix]
                internal static void Postfix()
                {
                    LateUpdate(null, null);
                }
            }
        }

        public class SceneLoadedEventArgs : EventArgs
        {
            public Scene scene;
            public LoadSceneMode loadSceneMode;
        }
    }
}
