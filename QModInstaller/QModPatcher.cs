using Oculus.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

                try
                {
                    var modAssembly = Assembly.LoadFrom(Path.Combine(subDir, mod.AssemblyName));

                    string entryNamespace = mod.AssemblyName.Replace(".dll", "");
                    string entryType = "QPatch";
                    string entryMethod = "Patch";

                    var entryMethodSig = mod.EntryMethod.Split('.');
                    if (!string.IsNullOrEmpty(mod.EntryMethod) && entryMethodSig[0] != "Namespace")
                    {
                        entryNamespace = entryMethodSig[0];
                        entryType = entryMethodSig[1];
                        entryMethod = entryMethodSig[2];
                    }

                    MethodInfo qPatchMethod = modAssembly.GetType(entryNamespace + "." + entryType).GetMethod(entryMethod);

                    if (mod.Enable)
                        qPatchMethod.Invoke(modAssembly, new object[] { });
                }
                catch (Exception e)
                {
                    Console.WriteLine("QMOD ERR: loading QPatch.Patch method failed: " + e.Message);
                    continue;
                }
            }
        }
    }
}
