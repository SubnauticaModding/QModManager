using AssetsTools.NET;
using AssetsTools.NET.Extra;
using BepInEx.Logging;
using Mono.Cecil;
using QModManager.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace QModManager
{
    /// <summary>
    /// A patcher which runs ahead of UnityPlayer to enable Unity Audio.
    /// </summary>
    public static class UnityAudioFixer
    {
        internal static string UnityAudioFixerPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        internal static string DataPath => Directory.GetDirectories(Paths.GameRootPath, "*_Data", SearchOption.TopDirectoryOnly).SingleOrDefault();
        internal static QModGame Game
        {
            get
            {
                switch (new DirectoryInfo(DataPath).Name)
                {
                    case "Subnautica_Data":
                        return QModGame.Subnautica;
                    case "SubnauticaZero_Data":
                        return QModGame.BelowZero;
                    default:
                        return QModGame.None;
                }
            }
        }
        internal static string ManagedPath => Path.Combine(DataPath, "Managed");

        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("UnityAudioFixer");

        /// <summary>
        /// Called from BepInEx while patching, our entry point for patching.
        /// Do not change the method name as it is identified by BepInEx. Method must remain public.
        /// </summary>
        [Obsolete("Should not be used!", true)]
        public static void Initialize()
        {
            EnableUnityAudio();
        }

        /// <summary>
        /// Enable Unity Audio. Should only be called when UnityPlayer is not loaded.
        /// </summary>
        public static void EnableUnityAudio()
        {
            try
            {
                Logger.LogInfo("Attempting to enable Unity audio...");
                ChangeDisableUnityAudio(Path.Combine(ManagedPath, "../globalgamemanagers"), false, Game);
                Logger.LogInfo("Unity audio enabled.");
            }
            catch (Exception ex)
            {
                Logger.LogError($"An exception was encountered while attempting to enable Unity audio: {ex.Message}");
            }
        }

        /// <summary>
        /// Disable Unity Audio. Should only be called when UnityPlayer is not loaded.
        /// </summary>
        public static void DisableUnityAudio()
        {
            try
            {
                Logger.LogInfo("Attempting to disable Unity audio...");
                ChangeDisableUnityAudio(Path.Combine(ManagedPath, "../globalgamemanagers"), true, Game);
                Logger.LogInfo("Unity audio disabled.");
            }
            catch (Exception ex)
            {
                Logger.LogError($"An exception was encountered while attempting to enable Unity audio: {ex.Message}");
            }
        }

        private static void ChangeDisableUnityAudio(string path, bool newValue, QModGame game)
        {
            if (game != QModGame.Subnautica && game != QModGame.BelowZero)
                throw new ArgumentException("Neither Subnautica nor Below Zero detected!");
            AssetsManager am = new AssetsManager();
            AssetsFileInstance afi = am.LoadAssetsFile(path, false);
            am.LoadClassDatabase(Path.Combine(UnityAudioFixerPath, "cldb.dat"));
            AssetFileInfoEx audioInfo = afi.table.GetAssetInfo(4);
            AssetTypeInstance audioAti = am.GetTypeInstance(afi.file, audioInfo);
            AssetTypeValueField audioBaseField = audioAti.GetBaseField();
            audioBaseField.Get("m_DisableAudio").GetValue().Set(newValue);
            byte[] audioAsset;
            using (MemoryStream memStream = new MemoryStream())
            using (AssetsFileWriter writer = new AssetsFileWriter(memStream))
            {
                writer.bigEndian = false;
                audioBaseField.Write(writer);
                audioAsset = memStream.ToArray();
            }
            List<AssetsReplacer> rep = new List<AssetsReplacer>() { new AssetsReplacerFromMemory(0, 4, 0x0B, 0xFFFF, audioAsset) };
            using (MemoryStream memStream = new MemoryStream())
            using (AssetsFileWriter writer = new AssetsFileWriter(memStream))
            {
                afi.file.Write(writer, 0, rep, 0);
                afi.stream.Close();
                File.WriteAllBytes(path, memStream.ToArray());
            }
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
