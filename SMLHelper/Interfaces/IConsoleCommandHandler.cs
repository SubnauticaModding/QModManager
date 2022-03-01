namespace SMLHelper.V2.Interfaces
{
    using Commands;
    using System;

    /// <summary>
    /// A handler class for registering your custom console commands.
    /// </summary>
    public interface IConsoleCommandHandler
    {
        /// <summary>
        /// Registers your custom console command by targeting a <see langword="public"/> <see langword="static"/> method.
        /// </summary>
        /// <remarks>
        /// <para>Target method must be <see langword="static"/>.</para>
        /// 
        /// <para>The command can take parameters and will respect optional parameters as outlined in the method's signature.<br/>
        /// Supported parameter types: <see cref="string"/>, <see cref="bool"/>, <see cref="int"/>, <see cref="float"/>,
        /// <see cref="double"/>.</para>
        /// 
        /// <para>If the method has a return type, it will be printed to both the screen and the log.</para>
        /// </remarks>
        /// <param name="command">The case-insensitive command to register.</param>
        /// <param name="declaringType">The declaring type that holds the method to call when the command is entered.</param>
        /// <param name="methodName">The name of the method to call within the declaring type when the command is entered. 
        /// Method must be <see langword="static"/>.</param>
        /// <param name="parameters">The parameter types the method receives, for targeting overloads.</param>
        /// <seealso cref="RegisterConsoleCommand{T}(string, T)"/>
        /// <seealso cref="RegisterConsoleCommands(Type)"/>
        /// <seealso cref="ConsoleCommandAttribute"/>
        void RegisterConsoleCommand(string command, Type declaringType, string methodName, Type[] parameters = null);

        /// <summary>
        /// Registers your custom console command by passing a <see langword="delegate"/>.
        /// </summary>
        /// <remarks>
        /// <para>Supported parameter types: <see cref="string"/>, <see cref="bool"/>, <see cref="int"/>, <see cref="float"/>,
        /// <see cref="double"/>.</para>
        /// 
        /// <para>If the delegate has a return type, it will be printed to both the screen and the log.</para>
        /// </remarks>
        /// <typeparam name="T">The delegate type.</typeparam>
        /// <param name="command">The case-insensitive command to register.</param>
        /// <param name="callback">The callback to handle the command.</param>
        /// <seealso cref="RegisterConsoleCommand(string, Type, string, Type[])"/>
        /// <seealso cref="RegisterConsoleCommands(Type)"/>
        /// <seealso cref="ConsoleCommandAttribute"/>
        void RegisterConsoleCommand<T>(string command, T callback) where T : Delegate;

        /// <summary>
        /// Registers <see langword="public"/> <see langword="static"/> methods decorated with the
        /// <see cref="ConsoleCommandAttribute"/> within the <paramref name="type"/> as console commands.
        /// </summary>
        /// <remarks>
        /// <para>Target methods must be <see langword="static"/>.</para>
        /// 
        /// <para>Commands can take parameters and will respect optional parameters as outlined in the method's signature.<br/>
        /// Supported parameter types: <see cref="string"/>, <see cref="bool"/>, <see cref="int"/>, <see cref="float"/>,
        /// <see cref="double"/>.</para>
        /// 
        /// <para>If a decorated method has a return type, it will be printed to both the screen and the log.</para>
        /// </remarks>
        /// <seealso cref="RegisterConsoleCommand(string, Type, string, Type[])"/>
        /// <seealso cref="RegisterConsoleCommand{T}(string, T)"/>
        /// <seealso cref="ConsoleCommandAttribute"/>
        void RegisterConsoleCommands(Type type);
    }
}
