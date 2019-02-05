using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Oculus.Newtonsoft.Json;

namespace QModInstaller
{
	public class QMod
	{
        public string Id = "mod_id";
        public string DisplayName = "Display name";
        public string Author = "author";
        public string Version = "0.0.0";
        public string[] Requires = new string[0];
        public bool Enable = false;
        public string AssemblyName = "dll filename";
        public string EntryMethod = "Namespace.Class.Method of Harmony.PatchAll or your equivalent";
        public string Priority = "Last or First";
        public Dictionary<string, object> Config = new Dictionary<string, object>();

        [JsonIgnore]
        public Assembly loadedAssembly;

        [JsonIgnore]
        public string modAssemblyPath;

        public static QMod FromJsonFile(string file)
		{
			QMod result;
			try
			{
				string text = File.ReadAllText(file);
				result = JsonConvert.DeserializeObject<QMod>(text);
			}
			catch (Exception ex)
			{
				Console.WriteLine("QMOD ERR: mod.json deserialization failed!");
				Console.WriteLine(ex.Message);
				result = null;
			}
			return result;
		}
	}
}
