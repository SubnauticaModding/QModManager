namespace QModManager.Patching
{
    using System;
    using System.Reflection;
    using QModManager.Utility;

    internal class QModPatchMethod
    {
        internal QModPatchMethod(MethodInfo method, QMod qmod, PatchingOrder order)
        {
            this.Method = method;
            this.Origin = qmod;
            this.Order = order;            
        }

        internal QMod Origin { get; }
        internal PatchingOrder Order { get; }
        internal MethodInfo Method { get; }
        internal bool IsPatched { get; private set; }

        internal bool TryInvoke()
        {
            try
            {
                if (!this.Method.IsStatic)
                {
                    this.Origin.instance = Activator.CreateInstance(this.Method.DeclaringType);
                }

                this.Method.Invoke(this.Origin.instance, new object[] { });
                this.IsPatched = true;
                return true;
            }
            catch (ArgumentNullException e)
            {
                Logger.Error($"Could not parse entry method \"{this.Method.Name}\" for mod \"{this.Origin.Id}\"");
                Logger.Exception(e);

                return false;
            }
            catch (TargetInvocationException e)
            {
                Logger.Error($"Invoking the specified entry method \"{this.Method.Name}\" failed for mod \"{this.Origin.Id}\"");
                Logger.Exception(e);
                return false;
            }
            catch (Exception e)
            {
                Logger.Error($"An unexpected error occurred whilst trying to load mod \"{this.Origin.Id}\"");
                Logger.Exception(e);
                return false;
            }
        }
    }
}
