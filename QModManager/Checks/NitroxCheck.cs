using Harmony;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace QModManager.Checks
{
    internal static class NitroxCheck
    {
        internal static bool IsInstalled { get; set; } = false;

        [HarmonyPatch(typeof(GameInput), "Awake")]
        internal static class AwakePatch
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                // Loop through all IL instructions
                foreach (CodeInstruction instruction in instructions)
                {
                    // Check if the opcode is call
                    if (instruction.opcode == OpCodes.Call)
                    {
                        // Get the method from the operand
                        MethodInfo method = (MethodInfo)instruction.operand;

                        // Check if the method is the entry point for Nitrox
                        if (method.DeclaringType.Name == "Main" && method.Name == "Execute")
                        {
                            // Let everyone know
                            IsInstalled = true;
                        }
                    }
                }

                // Return the unmodified instructions
                return instructions;
            }
        }
    }
}
