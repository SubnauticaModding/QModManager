namespace SMLHelper.V2.Patchers
{
    using Harmony;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal class DevConsolePatcher
    {
        public static List<CommandInfo> commands = new List<CommandInfo>();

        public static void Patch(HarmonyInstance harmony)
        {
            var devConsoleType = typeof(DevConsole);
            var thisType = typeof(DevConsolePatcher);
            var submitMethod = devConsoleType.GetMethod("Submit", BindingFlags.Instance | BindingFlags.NonPublic);

            harmony.Patch(submitMethod, null, new HarmonyMethod(thisType.GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic)));
            Logger.Log("DevConsolePatcher is done.");
        }

        internal static void Postfix(bool __result, string value)
        {
            var separator = new char[]
            {
                ' ',
                '\t'
            };

            var text = value.Trim();
            var args = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            if (args.Length != 0)
            {
                foreach (var command in commands)
                {
                    if (command.Name.Contains(args[0]))
                    {
                        var argsList = args.ToList();
                        argsList.RemoveAt(0);
                        var newArgs = argsList.ToArray();
                        command.CommandHandler.Invoke(null, new object[] { newArgs });
                        __result = true;
                        return;
                    }
                }
            }

            __result = false;
        }
    }

    internal class CommandInfo
    {
        public MethodInfo CommandHandler;
        public string Name;
        public bool CaseSensitive;
        public bool CombineArgs;
    }
}
