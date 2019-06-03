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

        internal bool TryInvoke()
        {
            try
            {
                this.Method.Invoke(this.LoadedAssembly, new object[] { });

            }
            catch (ArgumentNullException e)
            {
                Logger.Error($"Could not parse entry method \"{this.Method.Name}\" for mod \"{this.ModId}\"");
                Logger.Exception(e);

                return false;
            }
            catch (TargetInvocationException e)
            {
                Logger.Error($"Invoking the specified entry method \"{this.Method.Name}\" failed for mod \"{this.ModId}\"");
                Logger.Exception(e);
                return false;
            }
            catch (Exception e)
            {
                Logger.Error($"An unexpected error occurred whilst trying to load mod \"{this.ModId}\"");
                Logger.Exception(e);
                return false;
            }
            
            this.IsPatched = true;
            return true;
        }
    }
}
