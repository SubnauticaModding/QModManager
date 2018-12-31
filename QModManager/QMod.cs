using Oculus.Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace QModManager
{
    public class QMod : IComparable<QMod>
    {
        public string Id = "Mod.ID";
        public string DisplayName = "Mod display name";
        public string Author = "Author name";
        public string Version = "0.0.0";
        public string[] LoadBefore = new string[] { };
        public string[] LoadAfter = new string[] { };
        public bool Enable = true;
        public string AssemblyName = "Filename.dll";
        public string EntryMethod = "Namespace.Class.Method";
        //public string Priority = "obsolete. use NewPriority Instead";
        //public int NewPriority = 0;
        //public Dictionary<string, object> Config = new Dictionary<string, object>();

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

        public int CompareTo(QMod other) => NewPriority.CompareTo(other.NewPriority);
    }
}
