using System.Reflection;
using System.Collections.Generic;
using DevConsolePatcher2 = SMLHelper.V2.Patchers.DevConsolePatcher;
using CommandInfo2 = SMLHelper.V2.Patchers.CommandInfo;

namespace SMLHelper.Patchers
{
    [System.Obsolete("Use SMLHelper.V2 instead.")]
    public class DevConsolePatcher
    {
        [System.Obsolete("Use SMLHelper.V2 instead.")]
        public static List<CommandInfo> commands = new List<CommandInfo>();

        internal static void Patch()
        {
            commands.ForEach(x => DevConsolePatcher2.commands.Add(x.GetV2CommandInfo()));

            V2.Logger.Log("Old DevConsolePatcher is done.");
        }
    }

    [System.Obsolete("Use SMLHelper.V2 instead.")]
    public class CommandInfo
    {
        public MethodInfo CommandHandler;
        public string Name;
        public bool CaseSensitive;
        public bool CombineArgs;

        internal CommandInfo2 GetV2CommandInfo()
        {
            CommandInfo2 commandInfo = new CommandInfo2
            {
                CommandHandler = CommandHandler,
                Name = Name,
                CaseSensitive = CaseSensitive,
                CombineArgs = CombineArgs
            };

            return commandInfo;
        }
    }
}
