using Harmony;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace QModManager
{
    internal static class NitroxCheck
    {
        internal static bool IsInstalled { get; private set; } = false;

        [HarmonyPatch(typeof(GameInput), "Awake")]
        private static class AwakePatch
        {
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                foreach (CodeInstruction instruction in instructions)
                {
                    if (instruction.opcode == OpCodes.Call)
                    {
                        MethodInfo method = (MethodInfo)instruction.operand;

                        if (method.DeclaringType.Name == "Main" && method.Name == "Execute")
                        {
                            IsInstalled = true;
                        }
                    }
                }

                return instructions;
            }
        }
    }
}
