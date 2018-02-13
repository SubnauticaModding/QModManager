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
            var modAssemblies = LoadQMods();

            if (modAssemblies == null) return;

            foreach (var mod in modAssemblies)
            {
                //qPatchMethod.Invoke(mod, new object[] { });
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

            foreach (var subDir in subDirs)
            {
                QMod mod = QMod.FromJsonFile(Path.Combine(subDir, "mod.json"));

                if (mod.Equals(null))
                    continue;

                try
                {
                    var modAssembly = Assembly.LoadFrom(Path.Combine(subDir, mod.AssemblyName));
                    MethodInfo qPatchMethod = modAssembly.GetType(mod.Id + ".Qpatch").GetMethod("Patch");
                    mod.QPatchMethod = qPatchMethod;
                }
                catch(Exception e)
                {
                    Console.WriteLine("QMOD ERR: loading QPatch.Patch method failed: " + e.Message);
                    continue;
                }
            }

            return mods;
        }
    }
}
