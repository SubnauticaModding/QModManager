namespace QModManager.Patching
{
    internal interface IManifestValidator
    {
        void ValidateManifest(QMod mod);

        void CheckRequiredMods(QMod mod);

        void FindPatchMethods(QMod qMod);

        void LoadAssembly(QMod mod);
    }
}