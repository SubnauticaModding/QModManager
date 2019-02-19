using RGiesecke.DllExport;
using System;
using System.IO;

namespace QModManager.Installer.Extensions
{
    public static class Extensions
    {
        [DllExport("PathsEqual", System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool PathsEqual(string path1, string path2)
        {
            string path1parsed = Path.GetFullPath(path1);
            string path2parsed = Path.GetFullPath(path2);

            return string.Equals(path1parsed, path2parsed, StringComparison.OrdinalIgnoreCase);
        }
    }
}
