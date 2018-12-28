using Oculus.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace QModManager
{
    public class QModPatcher
    {
        internal static string QModBaseDir = Environment.CurrentDirectory.Contains("system32") && Environment.CurrentDirectory.Contains("Windows") ? "ERR" : Path.Combine(Environment.CurrentDirectory, "QMods");
        internal static List<QMod> loadedMods = new List<QMod>();
        internal static bool patched = false;

        public static void Patch()
        {
            try
            {
                LoadMods();
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION CAUGHT!");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                if (e.InnerException != null)
                {
                    Console.WriteLine(e.InnerException.Message);
                    Console.WriteLine(e.InnerException.StackTrace);
                }
            }
        }
        internal static void LoadMods()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var allDlls = new DirectoryInfo(QModBaseDir).GetFiles("*.dll", SearchOption.AllDirectories);
                foreach (var dll in allDlls)
                {
                    Console.WriteLine(Path.GetFileNameWithoutExtension(dll.Name) + " " + args.Name);
                    if (args.Name.Contains(Path.GetFileNameWithoutExtension(dll.Name)))
                    {
                        return Assembly.LoadFrom(dll.FullName);
                    }
                }

                return null;
            };

            if (patched) return;
            patched = true;

            if (!Directory.Exists(QModBaseDir))
            {
                Console.WriteLine("QMods directory was not found! Creating...");
                if (QModBaseDir == "ERR")
                {
                    Console.WriteLine("There was an error creating the QMods directory");
                    Console.WriteLine("Please make sure that you ran Subnautica from Steam");
                }
                try
                {
                    Directory.CreateDirectory(QModBaseDir);
                    Console.WriteLine("QMods directory created successfully!");
                }
                catch (Exception e)
                {
                    Console.WriteLine("EXCEPTION CAUGHT!");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    if (e.InnerException != null)
                    {
                        Console.WriteLine("INNER EXCEPTION:");
                        Console.WriteLine(e.InnerException.Message);
                        Console.WriteLine(e.InnerException.StackTrace);
                    }
                }
                return;
            }

            var subDirs = Directory.GetDirectories(QModBaseDir);
            var Mods = new List<QMod>();

            foreach (var subDir in subDirs)
            {
                var jsonFile = Path.Combine(subDir, "mod.json");

                if (!File.Exists(jsonFile))
                {
                    Console.WriteLine($"ERROR! No \"mod.json\" file found in folder \"{subDir}\"");
                    File.WriteAllText(jsonFile, JsonConvert.SerializeObject(new QMod()));
                    Console.WriteLine("A template file was created");
                    continue;
                }

                QMod mod = QMod.FromJsonFile(Path.Combine(subDir, "mod.json"));

                if (mod == null) continue;

                if (mod.Enable == false)
                {
                    Console.WriteLine($"{mod.DisplayName} is disabled via config, skipping");
                    continue;
                }

                var modAssemblyPath = Path.Combine(subDir, mod.AssemblyName);

                if (!File.Exists(modAssemblyPath))
                {
                    Console.WriteLine($"ERROR! No matching dll found at \"{modAssemblyPath}\" for mod \"{mod.DisplayName}\"");
                    continue;
                }

                mod.LoadedAssembly = Assembly.LoadFrom(modAssemblyPath);
                mod.ModAssemblyPath = modAssemblyPath;

                if (mod.Priority == "Last" && mod.NewPriority == 0)
                {
                    mod.NewPriority = 1000;
                }
                else if (mod.Priority == "First" && mod.NewPriority == 0)
                {
                    mod.NewPriority = -1000;
                }

                Mods.Add(mod);
            }

            Mods.Sort();

            foreach (var mod in Mods)
            {
                if (mod != null)
                    loadedMods.Add(LoadMod(mod));
            }

            Console.WriteLine(" ");
            Console.WriteLine("Installed mods:");

            foreach (var mod in Mods)
            {
                Console.WriteLine($"- {mod.DisplayName} ({mod.Id})");
            }
        }

        internal static QMod LoadMod(QMod mod)
        {
            if (mod == null) return null;

            if (string.IsNullOrEmpty(mod.EntryMethod))
            {
                Console.WriteLine($"ERROR! No EntryMethod specified for mod {mod.DisplayName}");
            }
            else
            {
                try
                {
                    var entryMethodSig = mod.EntryMethod.Split('.');
                    var entryType = string.Join(".", entryMethodSig.Take(entryMethodSig.Length - 1).ToArray());
                    var entryMethod = entryMethodSig[entryMethodSig.Length - 1];

                    MethodInfo qPatchMethod = mod.LoadedAssembly.GetType(entryType).GetMethod(entryMethod);
                    qPatchMethod.Invoke(mod.LoadedAssembly, new object[] { });
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine($"ERROR! Could not parse entry method {mod.AssemblyName} for mod {mod.DisplayName}");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    if (e.InnerException != null)
                    {
                        Console.WriteLine(e.InnerException.Message);
                        Console.WriteLine(e.InnerException.StackTrace);
                    }
                    return null;
                }
                catch (TargetInvocationException e)
                {
                    Console.WriteLine($"ERROR! Invoking the specified entry method {mod.EntryMethod} failed for mod {mod.Id}");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    if (e.InnerException != null)
                    {
                        Console.WriteLine(e.InnerException.Message);
                        Console.WriteLine(e.InnerException.StackTrace);
                    }
                    return null;
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR! An unexpected error occurred!");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    if (e.InnerException != null)
                    {
                        Console.WriteLine(e.InnerException.Message);
                        Console.WriteLine(e.InnerException.StackTrace);
                    }
                    return null;
                }
            }

            return mod;
        }
    }
}
