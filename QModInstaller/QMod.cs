using Oculus.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace QModInstaller
{
    public class QMod
    {
        public string Id;
        public string DisplayName;
        public string Author;
        public string Version;
        public string[] Requires;
        public bool Enable;
        public string AssemblyName;
        public Dictionary<string, object> Config;
        public MethodInfo QPatchMethod;

        public QMod() { }

        public static QMod FromJsonFile(string file)
        {
            try
            {
                var json = File.ReadAllText(file);
                return JsonConvert.DeserializeObject<QMod>(json);
            }
            catch(Exception e)
            {
                Console.WriteLine("QMOD WARN: mod.json deserialization failed: " + e.Message);
                return null;
            }
        }
    }
}
