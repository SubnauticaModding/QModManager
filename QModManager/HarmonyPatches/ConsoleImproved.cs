// Original mod by zorgesho
namespace QModManager.HarmonyPatches.ConsoleImproved
{
    using Harmony;
    using QModManager.Utility;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using UnityEngine;

    internal static class ConsoleHelper
    {
        internal static bool dirty = true;
        internal static List<string> lastMatch = new List<string>();

        internal static string CompleteText(string input)
        {
            int lastSpace = input.LastIndexOf(' ');
            if (lastSpace <= 0)
                return CompleteString(input, DevConsole.commands.Keys);

            string argument = input.Substring(0, lastSpace + 1).Trim();
            List<string> values = Enum.GetValues(typeof(TechType)).OfType<TechType>().Select(t => t.AsString()).ToList();
            values.Sort();

            return argument + " " + CompleteString(input.Substring(lastSpace + 1), values);
        }

        internal static string CompleteString(string input, IEnumerable<string> possibleValues)
        {
            if (dirty) lastMatch = possibleValues.Where(v => v.ToLower().StartsWith(input.ToLower())).ToList();
            if (lastMatch.Count > 1)
                ErrorMessage.AddDebug($"{string.Join(", ", lastMatch.Take(30))}{(lastMatch.Count > 15 ? $", and {lastMatch.Count - 15} more..." : "")}");
            return GetNextValue(input, lastMatch);
        }

        internal static string GetNextValue(string input, IEnumerable<string> list)
        {
            if (!list.Contains(input)) return list.ElementAt(0);
            if (list.Last() == input) return list.ElementAt(0);

            return list.ElementAt(Array.IndexOf(list.ToArray(), input) + 1);
        }

        internal static void LoadHistory()
        {
            var history = Config.Get(Config.FIELDS.CONSOLE_HISTORY, new List<string>());
            DevConsole.instance.history = history;
            DevConsole.instance.inputField.SetHistory(history);
        }

        internal static void SaveHistory()
        {
            var history = DevConsole.instance.history.Take(20).ToList();
            Config.Set(Config.FIELDS.CONSOLE_HISTORY, history);
        }
    }

    [HarmonyPatch(typeof(ConsoleInput), nameof(ConsoleInput.KeyPressedOverride))]
    internal static class ConsoleInput_KeyPressedOverride_Patch
    {
        // This patch auto-completes the console text if the tab key was pressed

        [HarmonyPrefix]
        internal static bool Prefix(ConsoleInput __instance, ref bool __result)
        {
            if (__instance.processingEvent.keyCode != KeyCode.Tab)
                ConsoleHelper.dirty = true;

            if (__instance.processingEvent.keyCode == KeyCode.Tab && __instance.text.Length > 0 && __instance.caretPosition == __instance.text.Length)
            {
                string complete = ConsoleHelper.CompleteText(__instance.text);
                if (!string.IsNullOrEmpty(complete))
                {
                    __instance.text = complete;
                    ConsoleHelper.dirty = false;
                }
                __result = true;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(ConsoleInput), nameof(ConsoleInput.Validate))]
    internal static class ConsoleInput_Validate_Patch
    {
        // This patch removes an assignment to the historyIndex field

        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> i = instructions.ToList();
            i.Find(c => c.opcode == OpCodes.Ldfld).operand = AccessTools.Field(typeof(ConsoleInput), nameof(ConsoleInput.historyIndex));
            i.Find(c => c.opcode == OpCodes.Callvirt).opcode = OpCodes.Nop;
            return i;
        }
    }

    [HarmonyPatch(typeof(DevConsole), nameof(DevConsole.Awake))]
    internal static class DevConsole_Awake_Patch
    {
        // This patch loads the history and toggles the console based on the mod option

        [HarmonyPostfix]
        internal static void Postfix()
        {
            ConsoleHelper.LoadHistory();
            DevConsole.disableConsole = !Config.Get(Config.FIELDS.ENABLE_CONSOLE, false);
        }
    }

    [HarmonyPatch(typeof(DevConsole), nameof(DevConsole.OnDisable))]
    internal static class DevConsole_OnDisable_Patch
    {
        // This patch saves the console history when it's disabled

        [HarmonyPostfix]
        internal static void Postfix()
        {
            ConsoleHelper.SaveHistory();
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
            GUILayout.Toggle(!Config.Get(Config.FIELDS.ENABLE_CONSOLE, false), " " + label);
            GUI.enabled = true;

            __result = !Config.Get(Config.FIELDS.ENABLE_CONSOLE, false);

            return false;
        }
    }
}
