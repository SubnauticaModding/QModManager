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
            var qmods = LoadQMods();

            if (qmods == null) return;

            foreach (var mod in qmods)
            {
                mod.QPatchMethod.Invoke(mod.ModAssembly, new object[] { });
            }
        }


        public static List<QMod> LoadQMods()
        {
            List<QMod> mods = new List<QMod>();

            if (!Directory.Exists(qModBaseDir))
            {
                Directory.CreateDirectory(qModBaseDir);
                return mods;
            }

            var subDirs = Directory.GetDirectories(qModBaseDir);

            File.AppendAllText(@"C:\users\qwiso\desktop\log.txt", "found "+subDirs.Length+" subdirectories" + Environment.NewLine);

            foreach (var subDir in subDirs)
            {
                File.AppendAllText(@"C:\users\qwiso\desktop\log.txt", "scanning for qmods: " + subDir + Environment.NewLine);

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

                    File.AppendAllText(@"C:\users\qwiso\desktop\log.txt", "added new QPatch.Patch method for " + mod.AssemblyName + Environment.NewLine);
                }
                catch(Exception e)
                {
                    File.AppendAllText(@"C:\users\qwiso\desktop\log.txt", "didn't find method" + Environment.NewLine);
                    Console.WriteLine("QMOD ERR: loading QPatch.Patch method failed: " + e.Message);
                    continue;
                }
            }

            return mods;
        }
    }
}
