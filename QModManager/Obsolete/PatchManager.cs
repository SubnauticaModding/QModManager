#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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
