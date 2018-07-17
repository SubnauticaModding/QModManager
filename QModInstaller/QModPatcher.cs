using Oculus.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace QModInstaller
{
    public class QModPatcher
    {
        private static string qModBaseDir = Environment.CurrentDirectory + @"\QMods";
        private static List<QMod> loadedMods = new List<QMod>();
        private static bool patched = false;
        private static Stopwatch sw = null; // Used for performance review.

        public static void Patch()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var allDlls = new DirectoryInfo(qModBaseDir).GetFiles("*.dll", SearchOption.AllDirectories);
                foreach(var dll in allDlls)
                {
                    Console.WriteLine(Path.GetFileNameWithoutExtension(dll.Name) + " " + args.Name);
                    if(args.Name.Contains(Path.GetFileNameWithoutExtension(dll.Name)))
                    {
                        return Assembly.LoadFrom(dll.FullName);
                    }
                }

                return null;
            };

            if (patched) return;

            patched = true;

            if (!Directory.Exists(qModBaseDir))
            {
                Console.WriteLine("QMOD ERR: QMod directory was not found");
                Directory.CreateDirectory(qModBaseDir);
                Console.WriteLine("QMOD INFO: Created QMod directory at {0}", qModBaseDir);
                return;
            }

            var subDirs = Directory.GetDirectories(qModBaseDir);
            var lastMods = new List<QMod>();
            var firstMods = new List<QMod>();
            var otherMods = new List<QMod>();

            foreach (var subDir in subDirs)
            {
                var jsonFile = Path.Combine(subDir, "mod.json");

                if (!File.Exists(jsonFile))
                {
                    Console.WriteLine("QMOD ERR: Mod is missing a mod.json file");
                    File.WriteAllText(jsonFile, JsonConvert.SerializeObject(new QMod()));
                    Console.WriteLine("QMOD INFO: A template for mod.json was generated at {0}", jsonFile);
                    continue;
                }

                QMod mod = QMod.FromJsonFile(Path.Combine(subDir, "mod.json"));

                if (mod.Equals(null)) // QMod.FromJsonFile will throw parser errors
                    continue;

                if (mod.Enable.Equals(false))
                {
                    Console.WriteLine("QMOD WARN: {0} is disabled via config, skipping", mod.DisplayName);
                    continue;
                }

                var modAssemblyPath = Path.Combine(subDir, mod.AssemblyName);

                if (!File.Exists(modAssemblyPath))
                {
                    Console.WriteLine("QMOD ERR: No matching dll found at {0} for {1}", modAssemblyPath, mod.Id);
                    continue;
                }

                mod.loadedAssembly = Assembly.LoadFrom(modAssemblyPath);
                mod.modAssemblyPath = modAssemblyPath;

                if (mod.Priority.Equals("Last"))
                {
                    lastMods.Add(mod);
                    continue;
                }
                else if(mod.Priority.Equals("First"))
                {
                    firstMods.Add(mod);
                    continue;
                }
                else
                {
                    otherMods.Add(mod);
                    continue;
                }
            }

            // Enable Stopwatch just before loading mods.
            if (sw == null)
                sw = new Stopwatch();

            foreach (var mod in firstMods)
            {
                if (mod != null)
                    loadedMods.Add(LoadMod(mod));
            }

            foreach(var mod in otherMods)
            {
                if (mod != null)
                    loadedMods.Add(LoadMod(mod));
            }

            foreach(var mod in lastMods)
            {
                if(mod != null)
                    loadedMods.Add(LoadMod(mod));
            }

            // Disable Stopwatch when all loading is done.
            if (sw.IsRunning)
                sw.Stop();
            sw = null;
        }

        private static QMod LoadMod(QMod mod)
        {
            if (mod == null) return null;

            if (string.IsNullOrEmpty(mod.EntryMethod))
            {
                Console.WriteLine("QMOD ERR: No EntryMethod specified for {0}", mod.Id);
            }
            else
            {
                try
                {
                    // Reset Stopwatch
                    if (sw.IsRunning)
                        sw.Stop();
                    sw.Reset();
                    // Start Stopwatch
                    sw.Start();

                    var entryMethodSig = mod.EntryMethod.Split('.');
                    var entryType = String.Join(".", entryMethodSig.Take(entryMethodSig.Length - 1).ToArray());
                    var entryMethod = entryMethodSig[entryMethodSig.Length - 1];

                    MethodInfo qPatchMethod = mod.loadedAssembly.GetType(entryType).GetMethod(entryMethod);
                    qPatchMethod.Invoke(mod.loadedAssembly, new object[] { });

                    // Stop Stopwatch
                    sw.Stop();
                    // Parse elapsed time
                    string elapsedTime = "";
                    if (sw.Elapsed.Hours == 0)
                    {
                        if (sw.Elapsed.Minutes == 0)
                        {
                            if (sw.Elapsed.Seconds == 0)
                            {
                                if (sw.Elapsed.Milliseconds == 0)
                                {
                                    elapsedTime = "";
                                }
                                else
                                {
                                    elapsedTime = String.Format("{0:00}ms", sw.Elapsed.Milliseconds / 10);
                                }
                            }
                            else
                            {
                                elapsedTime = String.Format("{0:00}s{1:00}ms", sw.Elapsed.Seconds, sw.Elapsed.Milliseconds / 10);
                            }
                        }
                        else
                        {
                            elapsedTime = String.Format("{0:00}m{1:00}s{2:00}ms", sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.Milliseconds / 10);
                        }
                    }
                    else
                    {
                        elapsedTime = String.Format("{0:00}h{1:00}m{2:00}s{3:00}ms", sw.Elapsed.Hours, sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.Milliseconds / 10);
                    }
                    string _modname = (!String.IsNullOrEmpty(mod.Id) ? mod.Id : mod.AssemblyName);
                    // Log elapsed time
                    if (elapsedTime == "")
                    {
                        Console.WriteLine($"QMOD INFO: Mod \"{_modname}\" loaded IMMEDIATELY! This shouldn't even be possible ;)");
                    }
                    else
                    {
                        Console.WriteLine($"QMOD INFO: Mod \"{_modname}\" took {elapsedTime} to load.");
                    }
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine("QMOD ERR: Could not parse EntryMethod {0} for {1}", mod.AssemblyName, mod.Id);
                    Console.WriteLine(e.InnerException.Message);
                    return null;
                }
                catch (TargetInvocationException e)
                {
                    Console.WriteLine("QMOD ERR: Invoking the specified EntryMethod {0} failed for {1}", mod.EntryMethod, mod.Id);
                    Console.WriteLine(e.InnerException.Message);
                    return null;
                }
                catch (Exception e)
                {
                    Console.WriteLine("QMOD ERR: something strange happened");
                    Console.WriteLine(e.Message);
                    return null;
                }
            }

            return mod;
        }
    }
}
