using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System;
using System.Collections.Generic;
using System.IO;

namespace QModManager
{
    internal static class AudioFixer
    {
        internal static void ChangeDisableUnityAudio(string path, bool newValue, Patcher.Game game)
        {
            if (game != Patcher.Game.Subnautica && game != Patcher.Game.BelowZero)
                throw new ArgumentException("Neither Subnautica nor Below Zero detected!");
            AssetsManager am = new AssetsManager();
            AssetsFileInstance afi = am.LoadAssetsFile(path, false);
            if (game == Patcher.Game.Subnautica)
                am.LoadClassPackage("cldb.dat");
            else
                am.LoadClassPackage("cldb2018.dat");
            AssetFileInfoEx audioInfo = afi.table.getAssetInfo(4);
            AssetTypeInstance audioAti = am.GetATI(afi.file, audioInfo);
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
                afi.file.Write(writer, 0, rep.ToArray(), 0);
                afi.stream.Close();
                File.WriteAllBytes(path, memStream.ToArray());
            }
        }
    }
}