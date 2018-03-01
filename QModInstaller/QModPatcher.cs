using Oculus.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace QModInstaller
{
    public class QModPatcher
    {
        private static string qModBaseDir = Environment.CurrentDirectory + @"\QMods";

        public static void Patch()
        {
            List<QMod> mods = new List<QMod>();

            if (!Directory.Exists(qModBaseDir))
            {
                Console.WriteLine("QMOD ERR: QMod directory was not found");
                Directory.CreateDirectory(qModBaseDir);
                Console.WriteLine("QMOD INFO: Creaated QMod directory at {0}", qModBaseDir);
                return;
            }

            var subDirs = Directory.GetDirectories(qModBaseDir);
            var lastMods = new Dictionary<QMod, string>();
            var firstMods = new Dictionary<QMod, string>();
            var otherMods = new Dictionary<QMod, string>();

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

                if (mod.Priority.Equals("Last"))
                {
                    lastMods.Add(mod, modAssemblyPath);
                    continue;
                }
                else if(mod.Priority.Equals("First"))
                {
                    firstMods.Add(mod, modAssemblyPath);
                    continue;
                }
                else
                {
                    otherMods.Add(mod, modAssemblyPath);
                    continue;
                }
            }

            foreach(var mod in firstMods)
            {
                LoadMod(mod.Key, mod.Value);
            }

            foreach(var mod in otherMods)
            {
                LoadMod(mod.Key, mod.Value);
            }

            foreach(var mod in lastMods)
            {
                LoadMod(mod.Key, mod.Value);
            }
        }

        private static void LoadMod(QMod mod, string modAssemblyPath)
        {
            var modAssembly = Assembly.LoadFrom(modAssemblyPath);

            if (string.IsNullOrEmpty(mod.EntryMethod))
            {
                Console.WriteLine("QMOD ERR: No EntryMethod specified for {0}", mod.Id);
            }
            else
            {
                try
                {
                    var entryMethodSig = mod.EntryMethod.Split('.');
                    var entryType = String.Join(".", entryMethodSig.Take(entryMethodSig.Length - 1).ToArray());
                    var entryMethod = entryMethodSig[entryMethodSig.Length - 1];

                    MethodInfo qPatchMethod = modAssembly.GetType(entryType).GetMethod(entryMethod);
                    qPatchMethod.Invoke(modAssembly, new object[] { });
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine("QMOD ERR: Could not parse EntryMethod {0} for {1}", mod.AssemblyName, mod.Id);
                    Console.WriteLine(e.InnerException.Message);
                }
                catch (TargetInvocationException e)
                {
                    Console.WriteLine("QMOD ERR: Invoking the specified EntryMethod {0} failed for {1}", mod.EntryMethod, mod.Id);
                    Console.WriteLine(e.InnerException.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("QMOD ERR: something strange happened");
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
