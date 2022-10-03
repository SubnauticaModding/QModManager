namespace QModManager.HarmonyPatches
{
    using HarmonyLib;
    using QModManager.Utility;

    //transpiller
    using System.Collections.Generic;
    using System.Reflection.Emit;

#if BELOWZERO
    [HarmonyPatch(typeof(Telemetry),MethodType.Enumerator)]
    [HarmonyPatch(nameof(Telemetry.SendSesionStart))]
    static class MaskTelemetryLogging
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            string thistranspiler = "Telemetry_SendSesionStart";
            Logger.Log(Logger.Level.Debug, $"{thistranspiler} - Start Transpiler");
            var Index = -1;
            var codes = new List<CodeInstruction>(instructions);

            //analyse the code to find the right place for injection
            for (var i = 0; i < codes.Count; i++)
            {
                //determinde the check to find the right place and not missmatch it accidentally with a other one
                if (codes[i].opcode == OpCodes.Ldstr && codes[i + 17].opcode == OpCodes.Box && codes[i + 21].opcode == OpCodes.Ret)
                {
                    Index = i;
                    break;
                }
            }

            //Check if Index was changed to any other Value. If yes the Position was likely found.
            if (Index > -1)
            {
                Logger.Log(Logger.Level.Debug, $"{thistranspiler} - Transpiler injectection position found");
                codes[Index] = new CodeInstruction(OpCodes.Ldstr, "Telemetry session started. Platform: '{0}', UserId: ->Masked by QModManager for privacy Reason<-, SessionId: {1}");
                codes.RemoveRange(Index, 0);
                codes.RemoveRange(Index+10, 5);
            }
            else
            {
                Logger.Log(Logger.Level.Error, $"{thistranspiler} - Index was not found");
            }

            return codes;
        }
    }
#endif
}
