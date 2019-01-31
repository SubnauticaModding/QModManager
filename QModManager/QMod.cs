using Oculus.Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace QModManager
{
    public class QMod
    {
        public static readonly Version QModManagerVersion = new Version(1, 4);

        public string Id = "Mod.ID";
        public string DisplayName = "Mod display name";
        public string Author = "Author name";
        public string Version = "0.0.0";
        public string[] Dependencies = new string[] { };
        public string[] LoadBefore = new string[] { };
        public string[] LoadAfter = new string[] { };
        public bool Enable = true;
        public bool ForBelowZero = false;
        public string AssemblyName = "Filename.dll";
        public string EntryMethod = "Namespace.Class.Method";

        [JsonIgnore] internal Assembly LoadedAssembly;
        [JsonIgnore] internal string ModAssemblyPath;
        [JsonIgnore] internal bool Loaded;
        [JsonIgnore] internal Patcher.Game Game;

        internal static QMod FromJsonFile(string file)
        {
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                };

                string json = File.ReadAllText(file);
                QMod mod = JsonConvert.DeserializeObject<QMod>(json);

                if (mod == null) return null;

                if (mod.ForBelowZero == true) mod.Game = Patcher.Game.BelowZero;
                else mod.Game = Patcher.Game.Subnautica;

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
