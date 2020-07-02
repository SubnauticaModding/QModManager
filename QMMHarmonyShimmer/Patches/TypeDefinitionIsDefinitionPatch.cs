using HarmonyLib;
using Mono.Cecil;

namespace QMMLoader.QMMHarmonyShimmer.Patches
{
    [HarmonyPatch(typeof(TypeDefinition), nameof(TypeDefinition.IsDefinition), MethodType.Getter)]
    internal static class TypeDefinitionIsDefinitionPatch
    {
        internal static bool definitionsAreReferences = false;
        internal static void Postfix(TypeDefinition __instance, ref bool __result)
        {
            if (definitionsAreReferences && __instance.Scope is AssemblyNameReference assRef && assRef.Name.StartsWith("0Harmony"))
            {
                __result = false;
            }
        }
    }
}
