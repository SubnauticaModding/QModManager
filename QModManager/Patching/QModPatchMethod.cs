namespace QModManager.Patching
{
    using System;
    using System.Reflection;
    using QModManager.API.ModLoading;
    using QModManager.Utility;

    internal class QModPatchMethod
    {
        internal QModPatchMethod(MethodInfo method, QMod qmod, PatchingOrder order)
        {
            this.Origin = qmod;
            this.ModId = qmod.Id;
            this.Order = order;
            this.Method = method;
        }

        internal QMod Origin { get; }
        internal string ModId { get; }
        internal PatchingOrder Order { get; }
        internal MethodInfo Method { get; }
        internal bool IsPatched { get; private set; }

        internal bool TryInvoke()
        {
            try
            {
                if (!this.Method.IsStatic)
                {
                    this.Origin.legacyinstance = Activator.CreateInstance(this.Method.DeclaringType);
                }

                this.Method.Invoke(this.Origin.legacyinstance, new object[] { });
                this.IsPatched = true;
                return true;
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
        }
    }
}
