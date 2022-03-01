namespace SMLHelper.V2.Handlers
{
    using Commands;
    using HarmonyLib;
    using Interfaces;
    using SMLHelper.V2.Patchers;
    using System;
    using System.Reflection;

    /// <summary>
    /// A handler class for registering your custom console commands.
    /// </summary>
    public class ConsoleCommandsHandler : IConsoleCommandHandler
    {
        /// <summary>
        /// Main entry point for all calls to this handler.
        /// </summary>
        public static IConsoleCommandHandler Main { get; } = new ConsoleCommandsHandler();

        void IConsoleCommandHandler.RegisterConsoleCommand(string command, Type declaringType, string methodName, Type[] parameters)
        {
            MethodInfo targetMethod = parameters == null
                ? AccessTools.Method(declaringType, methodName)
                : AccessTools.Method(declaringType, methodName, parameters);
            ConsoleCommandsPatcher.AddCustomCommand(command, targetMethod);
        }

        void IConsoleCommandHandler.RegisterConsoleCommand<T>(string command, T callback)
            => ConsoleCommandsPatcher.AddCustomCommand(command, callback.Method, true, callback.Target);

        void IConsoleCommandHandler.RegisterConsoleCommands(Type type)
            => ConsoleCommandsPatcher.ParseCustomCommands(type);
    }
}
