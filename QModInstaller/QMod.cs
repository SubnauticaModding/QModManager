using Oculus.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace QModInstaller
{
    public class QMod
    {
        public string Id = "mod_id";
        public string DisplayName = "Display name";
        public string Author = "author";
        public string Version = "0.0.0";
        public string[] Requires = new string[] { };
        public bool Enable = false;
        public string AssemblyName = "dll filename";
        public string EntryMethod = "Namespace.Class.Method of harmony.PatchAll or your equivalent";
        public Dictionary<string, object> Config = new Dictionary<string, object>();

        [JsonIgnore]
        public Assembly ModAssembly;
        [JsonIgnore]
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
