using Oculus.Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace QModManager
{
    public class QMod
    {
        public string Id = "Mod.ID";
        public string DisplayName = "Mod display name";
        public string Author = "Author name";
        public string Version = "0.0.0";
        //public string[] Requires = new string[] { };
        public bool Enable = true;
        public string AssemblyName = "Filename.dll";
        public string EntryMethod = "Namespace.Class.Method";
        public string Priority = "First or Last";
        //public Dictionary<string, object> Config = new Dictionary<string, object>();

        [JsonIgnore]
        public Assembly LoadedAssembly;
        [JsonIgnore]
        public string ModAssemblyPath;

        //public QMod() { }

        public static QMod FromJsonFile(string file)
        {
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                };

                string json = File.ReadAllText(file);
                QMod mod = JsonConvert.DeserializeObject<QMod>(json);

                return mod;
            }
            catch (Exception e)
            {
                AddLog("ERROR! mod.json deserialization failed!");
                AddLog(e.Message);
                AddLog(e.StackTrace);

                return null;
            }
        }
    }
}
