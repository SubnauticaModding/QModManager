using BepInEx;
using BepInEx.Logging;
using Mono.Cecil;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace QModManager
{
    public static class OculusNewtonsoftRedirect
    {
        private static string OculusNewtonsoftJsonPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "Oculus.Newtonsoft.Json.dll");

        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("OculusNewtonsoftRedirect");

        public static IEnumerable<string> TargetDLLs { get; } = new[] { "Newtonsoft.Json.dll" };

        public static void Patch(AssemblyDefinition newtonsoftAssemblyDef)
        {
            if (newtonsoftAssemblyDef.MainModule.Types.Any(t => t.Namespace.StartsWith("Oculus")))
            {
                Logger.LogInfo("Newtonsoft.Json.dll already uses Oculus.Newtonsoft.Json namespace, skipping shim.");
                return;
            }

            var oculusNewtonsoftAssemblyDef = AssemblyDefinition.ReadAssembly(OculusNewtonsoftJsonPath);

            var oculusAssemblyNameRef = new AssemblyNameReference(
                    oculusNewtonsoftAssemblyDef.Name.Name, oculusNewtonsoftAssemblyDef.Name.Version);

            newtonsoftAssemblyDef.MainModule.AssemblyReferences.Add(oculusAssemblyNameRef);

            foreach (var type in oculusNewtonsoftAssemblyDef.MainModule.Types)
            {
                if (!type.IsPublic)
                    continue;

                var exportedType = new ExportedType(
                        type.Namespace, type.Name,
                        newtonsoftAssemblyDef.MainModule, oculusAssemblyNameRef);

                newtonsoftAssemblyDef.MainModule.ExportedTypes.Add(exportedType);
            }
        }
    }
}
