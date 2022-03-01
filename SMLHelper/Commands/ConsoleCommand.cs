namespace SMLHelper.V2.Commands
{
    using HarmonyLib;
    using QModManager.API;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents a console command.
    /// </summary>
    internal class ConsoleCommand
    {
        /// <summary>
        /// The string that triggers the command.
        /// </summary>
        public string Trigger { get; }

        /// <summary>
        /// The QMod that registered the command.
        /// </summary>
        public IQMod QMod { get; }

        /// <summary>
        /// The parameters for the command.
        /// </summary>
        public IEnumerable<Parameter> Parameters { get; }

        /// <summary>
        /// The types of the parameters.
        /// </summary>
        public Type[] ParameterTypes { get; }

        private Type DeclaringType { get; }
        private string MethodName { get; }
        private bool IsMethodStatic { get; }
        private bool IsDelegate { get; }
        private object Instance { get; }

        /// <summary>
        /// Creates an instance of <see cref="ConsoleCommand"/>.
        /// </summary>
        /// <param name="trigger">The string that triggers the command.</param>
        /// <param name="targetMethod">The method targeted by the command.</param>
        /// <param name="isDelegate">Whether or not the method is a delegate.</param>
        /// <param name="instance">The instance the method belongs to.</param>
        public ConsoleCommand(string trigger, MethodInfo targetMethod, bool isDelegate = false, object instance = null)
        {
            Trigger = trigger.ToLowerInvariant();
            DeclaringType = targetMethod.DeclaringType;
            MethodName = targetMethod.Name;
            IsMethodStatic = targetMethod.IsStatic;
            IsDelegate = isDelegate;
            Instance = instance;
            QMod = QModServices.Main.GetMod(DeclaringType.Assembly);
            Parameters = targetMethod.GetParameters().Select(param => new Parameter(param));
            ParameterTypes = Parameters.Select(param => param.ParameterType).ToArray();
        }

        /// <summary>
        /// Determines whether the targeted method is valid in terms of whether it is static or delegate.
        /// </summary>
        /// <returns></returns>
        public bool HasValidInvoke() => IsDelegate || Instance != null || IsMethodStatic;

        /// <summary>
        /// Determines whether the target methods parameters are valid.
        /// </summary>
        /// <returns></returns>
        public bool HasValidParameterTypes()
        {
            foreach (Parameter parameter in Parameters)
            {
                if (!parameter.IsValidParameterType)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a list of all invalid parameters.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Parameter> GetInvalidParameters()
            => Parameters.Where(param => !param.IsValidParameterType);

        /// <summary>
        /// Attempts to parse input parameters into appropriate types as defined in the target method.
        /// </summary>
        /// <param name="inputParameters">The parameters as input by the user.</param>
        /// <param name="parsedParameters">The parameters that have been successfully parsed.</param>
        /// <returns>Whether or not all parameters were succesfully parsed.</returns>
        public bool TryParseParameters(IEnumerable<string> inputParameters, out object[] parsedParameters)
        {
            parsedParameters = null;

            // Detect incorrect number of parameters (allow for optional)
            if (Parameters.Count() < inputParameters.Count() ||
                Parameters.Where(param => !param.IsOptional).Count() > inputParameters.Count())
            {
                return false;
            }

            parsedParameters = new object[Parameters.Count()];
            for (int i = 0; i < Parameters.Count(); i++)
            {
                Parameter parameter = Parameters.ElementAt(i);

                if (i >= inputParameters.Count()) // It's an optional parameter that wasn't passed by the user
                {
                    parsedParameters[i] = Type.Missing;
                    continue;
                }

                string input = inputParameters.ElementAt(i);

                try
                {
                    parsedParameters[i] = parameter.Parse(input);
                }
                catch (Exception)
                {
                    return false; // couldn't parse, wasn't a valid conversion
                }
            }

            return true;
        }

        /// <summary>
        /// Invokes the command with the given parameters.
        /// </summary>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>The string returned from the command.</returns>
        public string Invoke(object[] parameters)
        {
            if (Instance != null)
                return Traverse.Create(Instance).Method(MethodName, ParameterTypes).GetValue(parameters)?.ToString();
            else
                return Traverse.Create(DeclaringType).Method(MethodName, ParameterTypes).GetValue(parameters)?.ToString();
        }
    }
}
