using QModManager.API;
using System;
using System.Reflection;

namespace QModManager
{
    [Obsolete("Use QModManager.API.QModAPI instead")]
    public static class PatchManager
    {
        [Obsolete("Use QModManager.API.QModAPI instead ")]
        public static void MarkAsErrored(Assembly modAssembly = null) => QModAPI.MarkAsErrored(modAssembly);
    }
}
