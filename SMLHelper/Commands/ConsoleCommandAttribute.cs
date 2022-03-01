namespace SMLHelper.V2.Commands
{
    using Interfaces;
    using Handlers;
    using System;

    /// <summary>
    /// Attribute used to signify the decorated method should be called in response to a console command.
    /// 
    /// <para>Decorated method must be both <see langword="public"/> and <see langword="static"/>.</para>
    /// </summary>
    /// <remarks>
    /// <para>The command can take parameters and will respect optional parameters as outlined in the method's signature.<br/>
    /// Supported parameter types: <see cref="string"/>, <see cref="bool"/>, <see cref="int"/>, <see cref="float"/>,
    /// <see cref="double"/>.</para>
    /// 
    /// <para>If the method has a return type, it will be printed to both the screen and the log.</para>
    /// </remarks>
    /// <seealso cref="IConsoleCommandHandler.RegisterConsoleCommand(string, Type, string, Type[])"/>
    /// <seealso cref="IConsoleCommandHandler.RegisterConsoleCommand{T}(string, T)"/>
    /// <seealso cref="IConsoleCommandHandler.RegisterConsoleCommands(Type)"/>
    /// <seealso cref="ConsoleCommandsHandler"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ConsoleCommandAttribute : Attribute
    {
        /// <summary>
        /// The unique, case-insensitive command that when entered, will call the decorated method.
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// <para>Signifies the decorated method should be called when the given <paramref name="command"/> is entered
        /// in the dev console.</para>
        /// 
        /// <para>Decorated method must be both <see langword="public"/> and <see langword="static"/>.</para>
        /// </summary>
        /// <remarks>
        /// <para>The command can take parameters and will respect optional parameters as outlined in the method's signature.<br/>
        /// Supported parameter types: <see cref="string"/>, <see cref="bool"/>, <see cref="int"/>, <see cref="float"/>,
        /// <see cref="double"/>.</para>
        /// 
        /// <para>If the method has a return type, it will be printed to both the screen and the log.</para>
        /// </remarks>
        /// <param name="command">The unique, case-insensitive command that when entered into the dev console will call the
        /// decorated method.</param>
        /// <seealso cref="IConsoleCommandHandler.RegisterConsoleCommand(string, Type, string, Type[])"/>
        /// <seealso cref="IConsoleCommandHandler.RegisterConsoleCommand{T}(string, T)"/>
        /// <seealso cref="IConsoleCommandHandler.RegisterConsoleCommands(Type)"/>
        /// <seealso cref="ConsoleCommandsHandler"/>
        public ConsoleCommandAttribute(string command)
        {
            Command = command;
        }
    }
}
