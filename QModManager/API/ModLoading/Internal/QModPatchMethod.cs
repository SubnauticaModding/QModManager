namespace QModManager.API.ModLoading.Internal
{
    using System;
    using System.Reflection;
    using QModManager.Utility;

    internal class QModPatchMethod
    {
        internal QModPatchMethod(MethodInfo method, IQMod qmod, PatchingOrder order)
        {
            this.ModId = qmod.Id;
            this.Order = order;
            this.Method = method;
            this.LoadedAssembly = qmod.LoadedAssembly;
        }

        internal string ModId { get; }
        internal PatchingOrder Order { get; }
        internal MethodInfo Method { get; }
        internal Assembly LoadedAssembly { get; }
        internal bool IsPatched { get; private set; }

        internal PatchResults TryInvoke()
        {
            try
            {
                object instance = null;

                if (!this.Method.IsStatic)
                {
                    instance = Activator.CreateInstance(this.Method.DeclaringType);
                }

                if (this.Method.ReturnType == typeof(PatchResults))
                {
                    var value = (PatchResults)this.Method.Invoke(instance, new object[] { });
                    this.IsPatched = value == PatchResults.OK;
                    return value;
                }
                else
                {
                    this.Method.Invoke(instance, new object[] { });
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
