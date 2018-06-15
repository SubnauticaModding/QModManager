using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SMLHelper.V2.Patchers
{
    public class LanguagePatcher
    {
        public static Dictionary<string, string> customLines = new Dictionary<string, string>();
        private static Type languageType = typeof(Language);

        public static void Postfix(ref Language __instance)
        {
            var stringsField = languageType.GetField("strings", BindingFlags.NonPublic | BindingFlags.Instance);
            var strings = stringsField.GetValue(__instance) as Dictionary<string, string>;
            foreach (var a in customLines)
            {
                strings[a.Key] = a.Value;
            }
        }

        public static void Patch(HarmonyInstance harmony)
        {
            var method = languageType.GetMethod("LoadLanguageFile", BindingFlags.NonPublic | BindingFlags.Instance);

            harmony.Patch(method, null,
                new HarmonyMethod(typeof(LanguagePatcher).GetMethod("Postfix")));
            Logger.Log("LanguagePatcher is done.");
        }
    }
}
