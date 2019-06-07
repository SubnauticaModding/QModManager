namespace QModManager.API.ModLoading.Internal
{
    using System;
    using System.Reflection;
    using QModManager.Utility;

    internal class PatchMethod
    {
        public PatchMethod(MethodInfo method, IQModLoadable qmod)
        {
            this.ModId = qmod.Id;
            this.Method = method;
            this.LoadedAssembly = qmod.LoadedAssembly;
        }

        internal string ModId { get; }
        internal MethodInfo Method { get; }
        internal Assembly LoadedAssembly { get; }
        internal bool IsPatched { get; private set; }

        internal PatchResults TryInvoke()
        {
            try
            {
                // TODO - Add handling for non-static methods
                if (this.Method.ReturnType == typeof(PatchResults))
                {
                    var value = (PatchResults)this.Method.Invoke(null, new object[] { });
                    this.IsPatched = value == PatchResults.OK;
                    return value;
                }
                else
                {
                    this.Method.Invoke(null, new object[] { });
                    this.IsPatched = true;
                    return PatchResults.OK;
                } 
            }
            catch (ArgumentNullException e)
            {
                Logger.Error($"Could not parse entry method \"{this.Method.Name}\" for mod \"{this.ModId}\"");
                Logger.Exception(e);

                return PatchResults.Error;
            }
            catch (TargetInvocationException e)
            {
                Logger.Error($"Invoking the specified entry method \"{this.Method.Name}\" failed for mod \"{this.ModId}\"");
                Logger.Exception(e);
                return PatchResults.Error;
            }
            catch (Exception e)
            {
                Logger.Error($"An unexpected error occurred whilst trying to load mod \"{this.ModId}\"");
                Logger.Exception(e);
                return PatchResults.Error;
            }
        }
    }
}
