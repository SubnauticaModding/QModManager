namespace QModManager.API.ModLoading.Internal
{
    internal interface IPatchMethod
    {
        PatchingOrder PatchOrder { get; }
    }
}
