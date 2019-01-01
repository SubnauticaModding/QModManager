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
        public string[] Dependencies = new string[] { };
        public string[] LoadBefore = new string[] { };
        public string[] LoadAfter = new string[] { };
        public bool Enable = true;
        public string AssemblyName = "Filename.dll";
        public string EntryMethod = "Namespace.Class.Method";

        [JsonIgnore]
        public Assembly LoadedAssembly;
        [JsonIgnore]
        public string ModAssemblyPath;
        [JsonIgnore]
        public bool Loaded;

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
                Console.WriteLine("ERROR! mod.json deserialization failed!");
                Console.WriteLine(e.ToString());

                return null;
            }
        }
    }
}
