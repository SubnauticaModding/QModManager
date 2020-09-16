using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using Mono.Cecil;
using Oculus.Newtonsoft.Json.Bson;
using Oculus.Newtonsoft.Json;
using QModManager.API;
using QModManager.Patching;
using QModManager.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using TypeloaderCache = System.Collections.Generic.Dictionary<string, BepInEx.Bootstrap.CachedAssembly<BepInEx.PluginInfo>>;
using QMMAssemblyCache = System.Collections.Generic.Dictionary<string, long>;

namespace QModManager
{
    public static class QModPluginGenerator
    {
        internal static readonly string QModsPath = Path.Combine(Paths.GameRootPath, "QMods");
        private static readonly string BepInExRootPath = Path.Combine(Paths.GameRootPath, "BepInEx");
        private static readonly string BepInExCachePath = Path.Combine(Paths.BepInExRootPath, "cache");
        private static readonly string BepInExPatchersPath = Path.Combine(Paths.BepInExRootPath, "patchers");
        private static readonly string BepInExPluginsPath = Path.Combine(Paths.BepInExRootPath, "plugins");
        private static readonly string QMMPatchersPath = Path.Combine(BepInExPatchersPath, "QModManager");
        private static readonly string QMMPluginsPath = Path.Combine(BepInExPluginsPath, "QModManager");
        private static readonly string QMMAssemblyCachePath = Path.Combine(BepInExCachePath, "qmodmanager.dat");
        private static QMMAssemblyCache QMMAssemblyCache;

        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("QModPluginGenerator");
        private const string GeneratedPluginCache = "qmodmanager_plugingenerator";

        internal static IEnumerable<QMod> QModsToLoad;
        internal static Dictionary<string, QMod> QModsToLoadById;
        internal static Dictionary<string, PluginInfo> QModPluginInfos;
        internal static List<PluginInfo> InitialisedQModPlugins;

        internal static IVersionParser VersionParserService { get; set; } = new VersionParser();

        private static TypeloaderCache PluginCache;

        [Obsolete("Should not be used!", true)]
        public static void Finish()
        {
            try
            {
                PluginCache = GetPluginCache();
                var harmony = new Harmony("QModManager.QModPluginGenerator");
                harmony.Patch(
                    typeof(TypeLoader).GetMethod(nameof(TypeLoader.FindPluginTypes)).MakeGenericMethod(typeof(PluginInfo)),
                    postfix: new HarmonyMethod(typeof(QModPluginGenerator).GetMethod(nameof(TypeLoaderFindPluginTypesPostfix))));
                harmony.PatchAll(typeof(QModPluginGenerator));
            }
            catch (Exception ex)
            {
                Logger.LogFatal($"An exception occurred while attempting to generate BepInEx PluginInfos: {ex.Message}.");
                Logger.LogFatal($"Beginning stacktrace:");
                Logger.LogFatal(ex.StackTrace);
            }
        }

        private static string[] QMMKnownAssemblyPaths = new[] {
            Path.Combine(QMMPatchersPath, "QModManager.QMMHarmonyShimmer.dll"),
            Path.Combine(QMMPatchersPath, "QModManager.QModPluginGenerator.dll"),
            Path.Combine(QMMPatchersPath, "QModManager.UnityAudioFixer.dll"),
            Path.Combine(QMMPatchersPath, "QModManager.exe"),
            Path.Combine(QMMPluginsPath, "QModInstaller.dll"),
            Path.Combine(QMMPluginsPath, "QModManager.QMMLoader.dll")
        };

        private static QMMAssemblyCache GetNewQMMAssemblyCache()
        {
            var qmmAssemblyCache = new QMMAssemblyCache();
            foreach (var assemblyPath in QMMKnownAssemblyPaths)
            {
                if (!File.Exists(assemblyPath))
                {
                    Logger.LogError($"Could not find QMM assembly: {assemblyPath}");
                    continue;
                }

                qmmAssemblyCache.Add(assemblyPath, File.GetLastWriteTimeUtc(assemblyPath).Ticks);
            }
            return qmmAssemblyCache;
        }

        private static void LoadQMMAssemblyCache()
        {
            Logger.LogInfo("Loading QMMAssemblyCache...");
            var stopwatch = Stopwatch.StartNew();

            if (!File.Exists(QMMAssemblyCachePath))
            {
                Logger.LogInfo("Could not find QMMAssemblyCache, skipping load.");
                return;
            }

            try
            {
                var data = File.ReadAllBytes(QMMAssemblyCachePath);
                using (var ms = new MemoryStream(data))
                using (var reader = new BsonReader(ms))
                {
                    var serializer = new JsonSerializer();
                    QMMAssemblyCache = serializer.Deserialize<QMMAssemblyCache>(reader);
                }
                stopwatch.Stop();
                Logger.LogInfo($"QMMAssemblyCache loaded in {stopwatch.ElapsedMilliseconds} ms.");
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to load QMMAssemblyCache!");
                Logger.LogError(ex);
            }
        }

        private static void SaveQMMAssemblyCache()
        {
            if (QMMAssemblyCache == null)
                QMMAssemblyCache = GetNewQMMAssemblyCache();

            Logger.LogInfo("Saving QMMAssemblyCache...");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                Directory.CreateDirectory(BepInExCachePath);

                using (var ms = new MemoryStream())
                using (var writer = new BsonWriter(ms))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(writer, QMMAssemblyCache);
                    File.WriteAllBytes(QMMAssemblyCachePath, ms.ToArray());
                }

                stopwatch.Stop();
                Logger.LogInfo($"QMMAssemblyCache saved in {stopwatch.ElapsedMilliseconds} ms.");
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to save QMMAssemblyCache!");
                Logger.LogError(ex);
            }
        }

        private static void ClearBepInExCache()
        {
            if (BepInExCachePath.ToLowerInvariant().Contains("system32") && BepInExCachePath.ToLowerInvariant().Contains("windows"))
            {
                throw new InvalidOperationException($"BepInEx Cache Path invalid! ({BepInExCachePath})");
            }

            if (!Directory.Exists(BepInExCachePath))
                return;

            Logger.LogInfo("Clearing BepInEx cache...");
            var stopwatch = Stopwatch.StartNew();

            Directory.Delete(BepInExCachePath, true);
            stopwatch.Stop();
            Logger.LogInfo($"Cleared BepInEx cache in {stopwatch.ElapsedMilliseconds} ms.");
        }

        private static TypeloaderCache GetPluginCache()
        {
            LoadQMMAssemblyCache();

            if (QMMAssemblyCache == null)
            {
                ClearBepInExCache();
                QMMAssemblyCache = GetNewQMMAssemblyCache();
                SaveQMMAssemblyCache();
            }
            else
            {
                var qmmAssemblyCache = GetNewQMMAssemblyCache();

                if (!qmmAssemblyCache.Keys.SequenceEqual(QMMAssemblyCache.Keys) ||
                    qmmAssemblyCache.Any(x => x.Value != QMMAssemblyCache[x.Key]))
                {
                    ClearBepInExCache();
                    QMMAssemblyCache = qmmAssemblyCache;
                    SaveQMMAssemblyCache();
                }
            }
            return TypeLoader.LoadAssemblyCache<PluginInfo>(GeneratedPluginCache);
        }

        [Obsolete("Should not be used!", true)]
        public static void TypeLoaderFindPluginTypesPostfix(ref Dictionary<string, List<PluginInfo>> __result, string directory)
        {
            if (directory != Paths.PluginPath)
                return;

            if (!(__result.Values.SelectMany(x => x).SingleOrDefault(x => x.Metadata.GUID == "QModManager.QMMLoader") is PluginInfo qmmLoaderPluginInfo))
                return;

            try
            {
                var result = new Dictionary<string, List<PluginInfo>>();

                QModPluginInfos = new Dictionary<string, PluginInfo>();
                InitialisedQModPlugins = new List<PluginInfo>();

                IPluginCollection pluginCollection = new PluginCollection(__result.Values.SelectMany(x => x).ToList());

                IQModFactory factory = new QModFactory(pluginCollection);

                QModsToLoad = factory.BuildModLoadingList(QModsPath);
                QModServices.LoadKnownMods(QModsToLoad.ToList());
                QModsToLoadById = QModsToLoad.ToDictionary(qmod => qmod.Id);

                foreach (var mod in QModsToLoad.Where(mod => mod.Status == ModStatus.Success))
                {
                    var dll = Path.Combine(mod.SubDirectory, mod.AssemblyName);
                    var manifest = Path.Combine(mod.SubDirectory, "mod.json");

                    if (PluginCache != null && PluginCache.TryGetValue(dll, out var cacheEntry))
                    {
                        var lastWrite = Math.Max(File.GetLastWriteTimeUtc(dll).Ticks, File.GetLastWriteTimeUtc(manifest).Ticks);
                        if (lastWrite == cacheEntry.Timestamp)
                        {
                            result[dll] = cacheEntry.CacheItems;
                            QModPluginInfos[mod.Id] = cacheEntry.CacheItems.FirstOrDefault();
                            continue;
                        }
                    }

                    var loadBeforeQmodIds = mod.LoadBefore.Where(id => QModPluginInfos.ContainsKey(id));
                    foreach (var id in loadBeforeQmodIds)
                    {
                        QModPluginInfos[id].Dependencies.AddItem(new BepInDependency(mod.Id, BepInDependency.DependencyFlags.SoftDependency));
                    }
                    foreach (var id in mod.LoadBefore.Where(id => pluginCollection.AllPlugins.Select(x => x.Metadata.GUID).Contains(id)).Except(loadBeforeQmodIds))
                    {
                        if (__result.Values.SelectMany(x => x).SingleOrDefault(x => x.Metadata.GUID == id) is PluginInfo bepinexPlugin)
                        {
                            Traverse.Create(bepinexPlugin)
                                .Property<IEnumerable<BepInDependency>>(nameof(PluginInfo.Dependencies)).Value
                                = bepinexPlugin.Dependencies.Concat(new[] { new BepInDependency(mod.Id, BepInDependency.DependencyFlags.SoftDependency) });
                        }
                    }

                    var pluginInfo = new PluginInfo();
                    var traverseablePluginInfo = Traverse.Create(pluginInfo);
                    traverseablePluginInfo.Property<IEnumerable<BepInDependency>>(nameof(PluginInfo.Dependencies)).Value
                        = new List<BepInDependency>(new[] { new BepInDependency(qmmLoaderPluginInfo.Metadata.GUID) });

                    foreach (var id in mod.Dependencies)
                    {
                        traverseablePluginInfo.Property<IEnumerable<BepInDependency>>(nameof(PluginInfo.Dependencies)).Value
                            = pluginInfo.Dependencies.AddItem(new BepInDependency(id, BepInDependency.DependencyFlags.HardDependency));
                    }

                    foreach (var versionDependency in mod.VersionDependencies)
                    {
                        var version = VersionParserService.GetVersion(versionDependency.Value);
                        traverseablePluginInfo.Property<IEnumerable<BepInDependency>>(nameof(PluginInfo.Dependencies)).Value
                            = pluginInfo.Dependencies.AddItem(new BepInDependency(versionDependency.Key, version.ToString()));
                    }
                    foreach (var id in mod.LoadAfter)
                    {
                        traverseablePluginInfo.Property<IEnumerable<BepInDependency>>(nameof(PluginInfo.Dependencies)).Value
                            = pluginInfo.Dependencies.AddItem(new BepInDependency(id, BepInDependency.DependencyFlags.SoftDependency));
                    }

                    traverseablePluginInfo.Property<IEnumerable<BepInProcess>>(nameof(PluginInfo.Processes)).Value = new BepInProcess[0];
                    traverseablePluginInfo.Property<IEnumerable<BepInIncompatibility>>(nameof(PluginInfo.Incompatibilities)).Value = new BepInIncompatibility[0];
                    traverseablePluginInfo.Property<BepInPlugin>(nameof(PluginInfo.Metadata)).Value = new BepInPlugin(mod.Id, mod.DisplayName, mod.ParsedVersion.ToString());
                    traverseablePluginInfo.Property<string>("TypeName").Value = typeof(QModPlugin).FullName;
                    traverseablePluginInfo.Property<Version>("TargettedBepInExVersion").Value
                        = Assembly.GetExecutingAssembly().GetReferencedAssemblies().FirstOrDefault(x => x.Name == "BepInEx").Version;

                    result.Add(dll, new[] { pluginInfo }.ToList());
                    QModPluginInfos.Add(mod.Id, pluginInfo);
                }

                __result[Assembly.GetExecutingAssembly().Location] = QModPluginInfos.Values.Distinct().ToList();

                TypeLoader.SaveAssemblyCache(GeneratedPluginCache, result);
            }
            catch (Exception ex)
            {
                Logger.LogFatal($"Failed to emulate QMods as plugins");
                Logger.LogFatal(ex.ToString());
            }
        }

        [HarmonyPatch(typeof(MetadataHelper), nameof(MetadataHelper.GetMetadata), new Type[] { typeof(object) })]
        [HarmonyPrefix]
        private static bool MetadataHelperGetMetadataPrefix(object plugin, ref BepInPlugin __result)
        {
            if (plugin is QModPlugin)
            {
                var pluginInfo = Chainloader.PluginInfos.Values.LastOrDefault(x => QModPluginInfos.Values.Contains(x) && !InitialisedQModPlugins.Contains(x));
                if (pluginInfo is PluginInfo)
                {
                    InitialisedQModPlugins.Add(pluginInfo);
                    __result = pluginInfo.Metadata;
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// For BepInEx to identify your patcher as a patcher, it must match the patcher contract as outlined in the BepInEx docs:
        /// https://bepinex.github.io/bepinex_docs/v5.0/articles/dev_guide/preloader_patchers.html#patcher-contract
        /// It must contain a list of managed assemblies to patch as a public static <see cref="IEnumerable{T}"/> property named TargetDLLs
        /// </summary>
        [Obsolete("Should not be used!", true)]
        public static IEnumerable<string> TargetDLLs { get; } = new string[0];

        /// <summary>
        /// For BepInEx to identify your patcher as a patcher, it must match the patcher contract as outlined in the BepInEx docs:
        /// https://bepinex.github.io/bepinex_docs/v5.0/articles/dev_guide/preloader_patchers.html#patcher-contract
        /// It must contain a public static void method named Patch which receives an <see cref="AssemblyDefinition"/> argument,
        /// which patches each of the target assemblies in the TargetDLLs list.
        /// 
        /// We don't actually need to patch any of the managed assemblies, so we are providing an empty method here.
        /// </summary>
        /// <param name="ad"></param>
        [Obsolete("Should not be used!", true)]
        public static void Patch(AssemblyDefinition ad) { }
    }
}
