namespace QModManager.HarmonyPatches
{
    using HarmonyLib;
    using QModManager.Utility;

    //transpiller
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

#if BELOWZERO
    [HarmonyPatch(typeof(Telemetry),MethodType.Enumerator)]
    //[HarmonyPatch(nameof(Telemetry.SendSesionStart))]
    [HarmonyPatch(nameof(Telemetry.SendSesionStart))]
    static class MaskTelemetryLogging
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            string thistranspiler = "Telemetry_SendSesionStart";
            Logger.Log(Logger.Level.Debug, $"{thistranspiler} - Start Transpiler");

            //Deep Logging - Avoid setting on true in final public release
            bool deeplogging = false;
            //----------------------------------------
            if (!deeplogging)
            {
                Logger.Log(Logger.Level.Debug, $"{thistranspiler} - Deeploging deactivated");
            }

            bool found = false;
            var Index = -1;
            var codes = new List<CodeInstruction>(instructions);

            //logging before
            if (deeplogging)
            {
                Logger.Log(Logger.Level.Debug, $"{thistranspiler} - Deep Logging pre-transpiler:");
                for (int k = 0; k < codes.Count; k++)
                {
                    Logger.Log(Logger.Level.Debug, (string.Format("0x{0:X4}", k) + $" : {codes[k].opcode.ToString()}	{(codes[k].operand != null ? codes[k].operand.ToString() : "")}"));
                }
            }

            //analyse the code to find the right place for injection
            Logger.Log(Logger.Level.Debug, $"{thistranspiler} - Start code analyses");
            for (var i = 0; i < codes.Count; i++)
            {
                #region ILAnalysis
                /*
                DNSPY                                                                                                       v1      v2
                IL_024D: call      class SessionStartResponse SessionStartResponse::CreateFromJSON(string)                  -17     -6
                IL_0252: stloc.s   V_5                                                                                      -16     -5
                IL_0254: ldloc.1                                                                                            -15     -4
                IL_0255: ldloc.s   V_5                                                                                      -14     -3
                IL_0257: ldfld     int32 SessionStartResponse::session_id                                                   -13     -2
                IL_025C: stfld     int32 Telemetry::sessionId                                                               -12     -1
                IL_0261: ldstr     "Telemetry session started. Platform: '{0}', UserId: '{1}', SessionId: {2}" <----------- -11      0
                IL_0266: ldc.i4.3                                                                                           -10     +1
                IL_0267: newarr    [mscorlib]System.Object                                                                  -9      +2
                IL_026C: dup                                                                                                -8      +3
                IL_026D: ldc.i4.0                                                                                           -7      +4
                IL_026E: ldloc.1                                                                                            -6      +5
                IL_026F: ldfld     string Telemetry::platformName                                                           -5      +6
                IL_0274: stelem.ref                                                                                         -4      +7
                IL_0275: dup                                                                                                -3      +8
                IL_0276: ldc.i4.1                                                                                           -2      +9
                IL_0277: ldloc.1                                                                                            -1      +10
                IL_0278: ldfld     string Telemetry::userId  <-------------------------------------------------------------  0      +11
                IL_027D: stelem.ref                                                                                         +1      +12
                IL_027E: dup                                                                                                +2      +13
                IL_027F: ldc.i4.2                                                                                           +3      +14
                IL_0280: ldloc.1                                                                                            +4      +15
                IL_0281: ldfld     int32 Telemetry::sessionId                                                               +5      +16
                IL_0286: box       [mscorlib]System.Int32                                                                   +6      +17
                IL_028B: stelem.ref                                                                                         +7      +18
                IL_028C: call      void [UnityEngine.CoreModule]UnityEngine.Debug::LogFormat(string, object[])              +8      +19
                IL_0291: ldc.i4.0                                                                                           +9      +20
                IL_0292: ret                                                                                                +10     +21
                ###########################################################################################################################
                */
                #endregion

                #region ILResult
                /*
                BEFORE
                0x00B3 : ldstr	Telemetry session started. Platform: '{0}', UserId: '{1}', SessionId: {2}   | Alter
                0x00B4 : ldc.i4.3
                0x00B5 : newarr	System.Object
                0x00B6 : dup
                0x00B7 : ldc.i4.0
                0x00B8 : ldloc.1
                0x00B9 : ldfld	System.String platformName
                0x00BA : stelem.ref
                0x00BB : dup
                0x00BC : ldc.i4.1
                0x00BD : ldloc.1                                                                            | Delete
                0x00BE : ldfld	System.String userId                                                        | Delete
                0x00BF : stelem.ref                                                                         | Delete
                0x00C0 : dup                                                                                | Delete
                0x00C1 : ldc.i4.2                                                                           | Delete
                0x00C2 : ldloc.1
                0x00C3 : ldfld	System.Int32 sessionId
                0x00C4 : box	System.Int32
                0x00C5 : stelem.ref
                0x00C6 : call	Void LogFormat(System.String, System.Object[])
                0x00C7 : ldc.i4.0
                0x00C8 : ret

                AFTER
                0x00B3 : ldstr	Telemetry session started. Platform: '{0}', UserId: ->Masked by QModManager for privacy Reason<-, SessionId: {1}
                0x00B4 : ldc.i4.3
                0x00B5 : newarr	System.Object
                0x00B6 : dup
                0x00B7 : ldc.i4.0
                0x00B8 : ldloc.1
                0x00B9 : ldfld	System.String platformName
                0x00BA : stelem.ref
                0x00BB : dup
                0x00BC : ldc.i4.1
                0x00BD : ldloc.1
                0x00BE : ldfld	System.Int32 sessionId
                0x00BF : box	System.Int32
                0x00C0 : stelem.ref
                0x00C1 : call	Void LogFormat(System.String, System.Object[])
                0x00C2 : ldc.i4.0
                0x00C3 : ret
                */
                #endregion

                //determinde the check to find the right place and not missmatch it accidentally with a other one
                //v1
                //if (codes[i].opcode == OpCodes.Ldfld && codes[i + 6].opcode == OpCodes.Box && codes[i +10].opcode == OpCodes.Ret)
                //v2
                if (codes[i].opcode == OpCodes.Ldstr && codes[i + 17].opcode == OpCodes.Box && codes[i + 21].opcode == OpCodes.Ret)
                {
                    Logger.Log(Logger.Level.Debug, $"{thistranspiler} - Found IL Code Line for Index. Index = {i.ToString()}");
                    found = true;
                    Index = i;
                    break;
                }
            }

            if (deeplogging)
            {
                if (found)
                {
                    Logger.Log(Logger.Level.Debug, $"{thistranspiler} - found: true");
                }
                else
                {
                    Logger.Log(Logger.Level.Debug, $"{thistranspiler} - found: false");
                }
            }

            if (Index > -1)
            {
                Logger.Log(Logger.Level.Debug, $"{thistranspiler} - Transpiler injectection position found");
                //v1
                /*
                codes[Index] = new CodeInstruction(OpCodes.Ldstr, "Masked by QModManager for privacy Reason");
                codes.RemoveRange(Index, 0);
                */
                //v2
                codes[Index] = new CodeInstruction(OpCodes.Ldstr, "Telemetry session started. Platform: '{0}', UserId: ->Masked by QModManager for privacy Reason<-, SessionId: {1}");
                codes.RemoveRange(Index, 0);
                codes.RemoveRange(Index+10, 5);
            }
            else
            {
                Logger.Log(Logger.Level.Error, $"{thistranspiler} - Index was not found");
            }

            //logging after
            if (deeplogging)
            {
                Logger.Log(Logger.Level.Debug, $"{thistranspiler} - Deep Logging after-transpiler:");
                for (int k = 0; k < codes.Count; k++)
                {
                    Logger.Log(Logger.Level.Debug, (string.Format("0x{0:X4}", k) + $" : {codes[k].opcode.ToString()}	{(codes[k].operand != null ? codes[k].operand.ToString() : "")}"));
                }
            }

            Logger.Log(Logger.Level.Debug, $"{thistranspiler} - Transpiler end going to return");
            return codes.AsEnumerable();
        }
    }
#endif
}
