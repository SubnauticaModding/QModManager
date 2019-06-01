namespace QModManager.API.ModLoading.Internal
{
    using System.Reflection;

    internal class PatchMethod
    {
        public PatchMethod(MethodInfo method)
        {
            this.Method = method;
        }

        public MethodInfo Method { get; set; }
        public bool IsPatched { get; set; }
    }
}