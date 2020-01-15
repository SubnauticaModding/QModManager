namespace QModManager.HarmonyPatches.EnableConsoleSetting
{
    using Harmony;
    using QModManager.Utility;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using UnityEngine;

    [HarmonyPatch(typeof(DevConsole), nameof(DevConsole.Awake))]
    internal static class DevConsole_Awake_Patch
    {
        // This patch toggles the console based on the mod option

        [HarmonyPostfix]
        internal static void Postfix()
        {
            DevConsole.disableConsole = !Config.EnableConsole;
        }
    }

    [HarmonyPatch(typeof(GraphicsDebugGUI), nameof(GraphicsDebugGUI.OnGUI))]
    internal static class GraphicsDebugGUI_OnGUI_Patch
    {
        // This patch prevents the F3 debug menu from toggling the console

        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Stsfld && (FieldInfo)instruction.operand == AccessTools.Field(typeof(DevConsole), nameof(DevConsole.disableConsole)))
                    yield return new CodeInstruction(OpCodes.Nop);
                else
                    yield return instruction;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerPrefsUtils), nameof(PlayerPrefsUtils.PrefsToggle))]
    internal static class PlayerPrefsUtils_PrefsToggle_Patch
    {
        // This patch disables the "Disable console" UI element in the F3 debug menu

        [HarmonyPrefix]
        public static bool Prefix(bool __result, string key, string label)
        {
            if (key != "UWE.DisableConsole") return true;

            GUI.enabled = false;
            GUILayout.Toggle(!Config.EnableConsole, " " + label);
            GUI.enabled = true;

            __result = !Config.EnableConsole;

            return false;
        }
    }
}
