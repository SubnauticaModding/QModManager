using System.Reflection;
using System.Collections.Generic;
using DevConsolePatcher2 = SMLHelper.V2.Patchers.DevConsolePatcher;
using CommandInfo2 = SMLHelper.V2.Patchers.CommandInfo;

namespace SMLHelper.Patchers
{
    public class DevConsolePatcher
    {
        public static List<CommandInfo> commands = new List<CommandInfo>();

        public static void Patch()
        {
            commands.ForEach(x => DevConsolePatcher2.commands.Add(x.GetV2CommandInfo()));
        }
    }

    public class CommandInfo
    {
        public MethodInfo CommandHandler;
        public string Name;
        public bool CaseSensitive;
        public bool CombineArgs;

        public CommandInfo2 GetV2CommandInfo()
        {
            var commandInfo = new CommandInfo2();
            commandInfo.CommandHandler = CommandHandler;
            commandInfo.Name = Name;
            commandInfo.CaseSensitive = CaseSensitive;
            commandInfo.CombineArgs = CombineArgs;

            return commandInfo;
        }
    }
}
