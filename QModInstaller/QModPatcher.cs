using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace QModInstaller
{
    public class QModPatcher
    {
        public static void Patch()
        {
            var modAssemblies = GetQMods();

            if (modAssemblies == null) return;

            foreach (var mod in modAssemblies)
            {
                var moduleName = mod.GetTypes()[0].ToString();
                var modNamespace = moduleName.Split(new string[] { "." }, StringSplitOptions.None);

                var qPatchMethod = mod.GetType(modNamespace[0] + ".QPatch").GetMethod("Patch");
                qPatchMethod.Invoke(mod, new object[] { });
            }
        }


        public static List<Assembly> GetQMods()
        {
            var modDirectory = Environment.CurrentDirectory + @"\QMods";

            if (!Directory.Exists(modDirectory))
            {
                Directory.CreateDirectory(modDirectory);
                return null;
            }

            var mods = new List<Assembly>();

            var files = Directory.GetFiles(modDirectory, "*.dll", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                var mod = Assembly.LoadFrom(file);
                mods.Add(mod);
            }

            return mods;
        }
    }
}
