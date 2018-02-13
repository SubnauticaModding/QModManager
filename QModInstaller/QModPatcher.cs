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
                    mod.ModAssembly = modAssembly;

                    MethodInfo qPatchMethod = modAssembly.GetType(mod.AssemblyName.Replace(".dll", "") + ".QPatch").GetMethod("Patch");
                    mod.QPatchMethod = qPatchMethod;

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
