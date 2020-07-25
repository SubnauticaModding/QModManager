using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using Mono.Cecil;
using QModManager.API;
using QModManager.Patching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace QModManager
{
    public static class QModPluginGenerator
    {
        internal static string QModsPath => Path.Combine(Paths.GameRootPath, "QMods");

        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("PluginEmulator");
        private const string pluginCache = "qmodmanager_pluginemulator";

        internal static IEnumerable<QMod> QModsToLoad;
        internal static Dictionary<string, QMod> QModsToLoadById;
        internal static Dictionary<string, PluginInfo> QModPluginInfos;
        internal static List<PluginInfo> InitialisedQModPlugins;

        [Obsolete("Should not be used!", true)]
        public static void Finish()
        {
            var harmony = new Harmony("PluginEmulator");
            harmony.Patch(
                typeof(TypeLoader).GetMethod(nameof(TypeLoader.FindPluginTypes)).MakeGenericMethod(typeof(PluginInfo)),
                postfix: new HarmonyMethod(typeof(QModPluginGenerator).GetMethod(nameof(TypeLoaderFindPluginTypesPostfix))));
            harmony.PatchAll(typeof(QModPluginGenerator));
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
                var cache = TypeLoader.LoadAssemblyCache<PluginInfo>(pluginCache);
                QModPluginInfos = new Dictionary<string, PluginInfo>();
                InitialisedQModPlugins = new List<PluginInfo>();

                var factory = new QModFactory();
                factory.BepInExPlugins = __result.Values.SelectMany(x => x).ToList();
                QModsToLoad = factory.BuildModLoadingList(QModsPath);
                QModServices.LoadKnownMods(QModsToLoad.ToList());
                QModsToLoadById = QModsToLoad.ToDictionary(qmod => qmod.Id);
                foreach (var mod in QModsToLoad.Where(mod => mod.Status == ModStatus.Success))
                {
                    var dll = Path.Combine(mod.SubDirectory, mod.AssemblyName);
                    var manifest = Path.Combine(mod.SubDirectory, "mod.json");

                    if (cache != null && cache.TryGetValue(dll, out var cacheEntry))
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
                    foreach (var id in mod.LoadBefore.Where(id => factory.BepInExPlugins.Select(x => x.Metadata.GUID).Contains(id)).Except(loadBeforeQmodIds))
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
                        var cleanVersion = ManifestValidator.VersionRegex.Matches(versionDependency.Value)?[0]?.Value;
                        if (string.IsNullOrEmpty(cleanVersion))
                        {
                            traverseablePluginInfo.Property<IEnumerable<BepInDependency>>(nameof(PluginInfo.Dependencies)).Value
                                = pluginInfo.Dependencies.AddItem(new BepInDependency(versionDependency.Key, new Version().ToString()));
                        }
                        else if (Version.TryParse(cleanVersion, out Version version))
                        {
                            traverseablePluginInfo.Property<IEnumerable<BepInDependency>>(nameof(PluginInfo.Dependencies)).Value
                                = pluginInfo.Dependencies.AddItem(new BepInDependency(versionDependency.Key, version.ToString()));
                        }
                        else
                        {
                            traverseablePluginInfo.Property<IEnumerable<BepInDependency>>(nameof(PluginInfo.Dependencies)).Value
                                = pluginInfo.Dependencies.AddItem(new BepInDependency(versionDependency.Key, new Version().ToString()));
                        }
                    }
                    foreach (var id in mod.LoadAfter)
                    {
                        traverseablePluginInfo.Property<IEnumerable<BepInDependency>>(nameof(PluginInfo.Dependencies)).Value
                            = pluginInfo.Dependencies.AddItem(new BepInDependency(id, BepInDependency.DependencyFlags.SoftDependency));
                    }

                    traverseablePluginInfo.Property<IEnumerable<BepInProcess>>(nameof(PluginInfo.Processes)).Value = new BepInProcess[0];
                    traverseablePluginInfo.Property<IEnumerable<BepInIncompatibility>>(nameof(PluginInfo.Incompatibilities)).Value = new BepInIncompatibility[0];
                    traverseablePluginInfo.Property<BepInPlugin>(nameof(PluginInfo.Metadata)).Value = new BepInPlugin(mod.Id, mod.DisplayName, mod.Version);
                    traverseablePluginInfo.Property<string>("TypeName").Value = typeof(QModPlugin).FullName;
                    traverseablePluginInfo.Property<Version>("TargettedBepInExVersion").Value
                        = Assembly.GetExecutingAssembly().GetReferencedAssemblies().FirstOrDefault(x => x.Name == "BepInEx").Version;

                    result.Add(dll, new[] { pluginInfo }.ToList());
                    QModPluginInfos.Add(mod.Id, pluginInfo);
                }

                __result[Assembly.GetExecutingAssembly().Location] = QModPluginInfos.Values.Distinct().ToList();

                TypeLoader.SaveAssemblyCache(pluginCache, result);
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
