namespace QModManager.Patching
{
    using System;
    using System.Linq;
    using System.Reflection;
    using QModManager.API;
    using QModManager.API.ModLoading;

    internal class PatchMethodFinder
    {
        public void LoadPatchMethods(QModJson qMod)
        {
            if (!string.IsNullOrEmpty(qMod.EntryMethod))
            {
                // Legacy
                string[] entryMethodSig = qMod.EntryMethod.Split('.');
                string entryType = string.Join(".", entryMethodSig.Take(entryMethodSig.Length - 1).ToArray());
                string entryMethod = entryMethodSig[entryMethodSig.Length - 1];

                MethodInfo jsonPatchMethod = qMod.LoadedAssembly.GetType(entryType).GetMethod(entryMethod, BindingFlags.Static | BindingFlags.Public);

                if (jsonPatchMethod != null && jsonPatchMethod.GetParameters().Length == 0)
                {
                    qMod.PatchMethods[PatchingOrder.NormalInitialize] = new QModPatchMethod(jsonPatchMethod, qMod, PatchingOrder.NormalInitialize);
                }
            }

            // QMM 3.0
            foreach (Type type in qMod.LoadedAssembly.GetTypes())
            {
                foreach (QModCoreAttribute core in type.GetCustomAttributes(typeof(QModCoreAttribute), false))
                {
                    foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
                    {
                        foreach (QModPatchAttributeBase patch in method.GetCustomAttributes(typeof(QModPatchAttributeBase), false))
                        {
                            qMod.PatchMethods[patch.PatchOrder] = new QModPatchMethod(method, qMod, patch.PatchOrder);
                        }
                    }
                }
            }
        }
    }
}
