using Mono.Cecil;
using System;
using System.IO;

namespace QModManager
{
    internal static class CleanUp
    {
        private static bool IsChildOf(this DirectoryInfo child, DirectoryInfo parent, bool recursive = false)
        {
            if (child.Parent == null)
                return false;

            return child.Parent.FullName == parent.FullName || (recursive && child.Parent.IsChildOf(parent));
        }
        private static bool IsChildOf(this DirectoryInfo child, string parentPath, bool recursive = false)
            => child.IsChildOf(new DirectoryInfo(parentPath), recursive);

        internal static void Initialize(string gameRootDirectory, string managedDirectory)
        {
            string qmodsDirectory = Path.Combine(gameRootDirectory, "QMods");
            string bepinexCoreDirectory = Path.Combine(gameRootDirectory, "BepInEx", "core");

            string[] pathsToCheck = new[] { managedDirectory, qmodsDirectory };

            foreach (var path in pathsToCheck)
            {
                if (path.Contains("system32") || path.Contains("Windows") || new DirectoryInfo(path).IsChildOf(bepinexCoreDirectory))
                {
                    Console.WriteLine($"Path is unsafe! {path}");
                    continue;
                }

                foreach (var file in Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories))
                {
                    try
                    {
                        using (var stream = new MemoryStream(File.ReadAllBytes(file)))
                        {
                            if (AssemblyDefinition.ReadAssembly(stream).MainModule.Name == "0Harmony" && File.Exists(file))
                            {
                                File.Delete(file);
                                Console.WriteLine($"Deleted {new DirectoryInfo(file).FullName}...");
                            }
                        }
                    }
                    catch (BadImageFormatException)
                    {
                        if (Path.GetFileName(file).StartsWith("0Harmony") && File.Exists(file))
                        {
                            File.Delete(file);
                            Console.WriteLine($"Deleted {new DirectoryInfo(file).FullName}...");
                        }
                    }
                }
            }
        }
    }
}
