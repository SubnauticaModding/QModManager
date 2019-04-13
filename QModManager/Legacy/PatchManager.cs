using System;
using System.Collections.Generic;
using System.Reflection;

namespace QModManager
{
    [Obsolete("Use QModAPI instead", true)]
    public static class PatchManager
    {
        public static void MarkAsErrored(Assembly modAssembly = null) => QModAPI.MarkAsErrored(modAssembly);
    }
}
