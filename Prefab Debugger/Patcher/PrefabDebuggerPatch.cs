using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

namespace BlueFire.Debugger
{
    [HarmonyPatch(typeof(DevConsole), "Awake")]
    public static class DebugConsolePatch
    {
        [HarmonyPostfix]
        public static void PostFix(DevConsole __instance)
        {
            new GameObject("PrefabDebugger").AddComponent<PrefabDebugger>();
        }
    }
}
