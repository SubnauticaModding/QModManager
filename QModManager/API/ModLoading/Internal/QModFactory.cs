namespace QModManager.API.ModLoading.Internal
{
    using System;
    using System.IO;
    using System.Reflection;
    using Oculus.Newtonsoft.Json;
    using QModManager.Utility;

    internal static class QModFactory
    {
        internal static QMod FromDll(string subDirectory, string[] dllFilePaths)
        {
            foreach (string dllFile in dllFilePaths)
            {
                var assembly = Assembly.LoadFrom(dllFile);

                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    object[] coreInfos = type.GetCustomAttributes(typeof(QModCoreInfo), false);
                    if (coreInfos.Length == 1)
                    {
                        var mod = new QMod((QModCoreInfo)coreInfos[0], type, assembly, subDirectory);

                        if (mod.IsValid)
                            return mod;
                    }
                }
            }

            return null;
        }

        internal static QMod FromJsonFile(string subDirectory)
        {
            string jsonFile = Path.Combine(subDirectory, "mod.json");

            if (!File.Exists(jsonFile))
            {
                return null;
            }

            try
            {
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };

                string jsonText = File.ReadAllText(jsonFile);
                QMod mod = JsonConvert.DeserializeObject<QMod>(jsonText);

                if (mod == null)
                    return null;

                bool success = mod.TryCompletingJsonLoading(subDirectory) && mod.IsValid;

                if (!success)
                    return null;

                return mod;
            }
            catch (Exception e)
            {
                Logger.Error($"\"mod.json\" deserialization failed for file \"{jsonFile}\"!");
                Logger.Exception(e);

                return null;
            }
        }

        internal static QMod FakePlaceholder(string name)
        {
            return new QMod(name);
        }
    }
}
