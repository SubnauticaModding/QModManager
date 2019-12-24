namespace QModManager.API.ModLoading
{
    using System.Collections.Generic;
    using QModManager.Patching;

    internal class Initializer
    {
        private readonly QModGame currentGame;

        internal Initializer(QModGame currentlyRunningGame)
        {
            currentGame = currentlyRunningGame;
        }

        internal void InitializeMods(List<QMod> modsToInitialize)
        {
            InitializeMods(modsToInitialize, PatchingOrder.PreInitialize);
            InitializeMods(modsToInitialize, PatchingOrder.NormalInitialize);
            InitializeMods(modsToInitialize, PatchingOrder.PostInitialize);
        }

        private void InitializeMods(List<QMod> modsToInitialize, PatchingOrder order)
        {
            foreach (QMod mod in modsToInitialize)
            {
                if (mod.Status != ModStatus.Success)
                    continue;

                ModLoadingResults result = mod.TryLoading(order, currentGame);
                switch (result)
                {
                    case ModLoadingResults.Failure:
                        mod.Status = ModStatus.PatchMethodFailed;
                        break;
                    case ModLoadingResults.AlreadyLoaded:
                        mod.Status = ModStatus.DuplicatePatchAttemptDetected;
                        break;
                    case ModLoadingResults.CurrentGameNotSupported:
                        mod.Status = ModStatus.CurrentGameNotSupported;
                        break;
                }
            }
        }
    }
}
