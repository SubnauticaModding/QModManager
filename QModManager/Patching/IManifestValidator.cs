namespace QModManager.Patching
{
    internal interface IManifestValidator
    {
        void ValidateBasicManifest(QMod mod);

        void CheckRequiredMods(QMod mod);
        void FindPatchMethods(QMod qMod);
        void LoadAssembly(QMod mod);
    }
}