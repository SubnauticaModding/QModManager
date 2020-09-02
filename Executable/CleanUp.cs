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

            return child.Parent.FullName == parent.FullName || (recursive && child.Parent.IsChildOf(parent, recursive));
        }

        private static bool IsChildOf(this FileInfo child, DirectoryInfo parent, bool recursive = true)
        {
            if (child.Directory == null)
                return false;

            return child.Directory.FullName == parent.FullName || (recursive && child.Directory.IsChildOf(parent, recursive));
        }
        private static bool IsChildOf(this FileInfo child, string parentPath, bool recursive = true)
            => child.IsChildOf(new DirectoryInfo(parentPath), recursive);

        internal static void Initialize(string gameRootDirectory, string managedDirectory)
        {
            string qmodsDirectory = Path.Combine(gameRootDirectory, "QMods");
            string bepinexCoreDirectory = Path.Combine(gameRootDirectory, "BepInEx", "core");

            string[] pathsToCheck = new[] { managedDirectory, qmodsDirectory };

            foreach (var path in pathsToCheck)
            {
                foreach (var file in Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories))
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.FullName.Contains("system32") || fileInfo.FullName.Contains("Windows") || fileInfo.IsChildOf(bepinexCoreDirectory, true))
                    {
                        Console.WriteLine($"Path is unsafe! {path}");
                        continue;
                    }

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
