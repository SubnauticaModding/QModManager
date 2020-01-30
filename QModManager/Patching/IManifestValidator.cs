namespace QModManager.Patching
{
    internal interface IManifestValidator
    {
        void ValidateManifest(QMod mod);
    }
}