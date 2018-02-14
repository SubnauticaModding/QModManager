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
                Directory.CreateDirectory(qModBaseDir);
                return;
            }

            var subDirs = Directory.GetDirectories(qModBaseDir);

            foreach (var subDir in subDirs)
            {
                var jsonFile = Path.Combine(subDir, "mod.json");

                if (!File.Exists(jsonFile))
                {
                    File.WriteAllText(jsonFile, JsonConvert.SerializeObject(new QMod()));
                    continue;
                }

                QMod mod = QMod.FromJsonFile(Path.Combine(subDir, "mod.json"));

                if (mod.Equals(null))
                    continue;

                var modAssembly = Assembly.LoadFrom(Path.Combine(subDir, mod.AssemblyName));

                if (!string.IsNullOrEmpty(mod.EntryMethod))
                {
                    try
                    {
                        var entryMethodSig = mod.EntryMethod.Split('.');
                        var entryType = String.Join(".", entryMethodSig.Take(entryMethodSig.Length - 1).ToArray());
                        var entryMethod = entryMethodSig[entryMethodSig.Length - 1];

                        MethodInfo qPatchMethod = modAssembly.GetType(entryType).GetMethod(entryMethod);

                        if (mod.Enable)
                            qPatchMethod.Invoke(modAssembly, new object[] { });
                    }
                    catch(ArgumentNullException e)
                    {
                        Console.WriteLine("QMOD ERR: Could not parse EntryMethod {0} for {1}", mod.AssemblyName, mod.Id);
                        continue;
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("QMOD ERR: Invoking the specified EntryMethod {0} failed for {1}", mod.EntryMethod, mod.Id);
                        Console.WriteLine(e.Message);
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("QMOD ERR: Could not open the assembly file specificed: {0}", mod.AssemblyName);
                }
            }
        }
    }
}
