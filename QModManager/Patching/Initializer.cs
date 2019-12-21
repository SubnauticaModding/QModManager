namespace QModManager.API.ModLoading
{
    using QModManager.DataStructures;
    using QModManager.Patching;

    internal class Initializer
    {
        private readonly QModGame currentGame;

        internal Initializer(QModGame currentlyRunningGame)
        {
            currentGame = currentlyRunningGame;
        }

        internal void InitializeMods(PairedList<QMod, ModStatus> modsToInitialize)
        {
            InitializeMods(modsToInitialize, PatchingOrder.PreInitialize);
            InitializeMods(modsToInitialize, PatchingOrder.NormalInitialize);
            InitializeMods(modsToInitialize, PatchingOrder.PostInitialize);
        }

        private void InitializeMods(PairedList<QMod, ModStatus> modsToInitialize, PatchingOrder order)
        {
            foreach (Pair<QMod, ModStatus> pair in modsToInitialize)
            {
                if (pair.Value != ModStatus.Success)
                    continue;

                QMod mod = pair.Key;
                ModLoadingResults result = mod.TryLoading(order, currentGame);
                switch (result)
                {
                    case ModLoadingResults.Success:
                        break;
                    case ModLoadingResults.Failure:
                        pair.Value = ModStatus.PatchMethodFailed;
                        break;
                    case ModLoadingResults.AlreadyLoaded:
                        pair.Value = ModStatus.DuplicatePatchAttemptDetected;
                        break;
                    case ModLoadingResults.CurrentGameNotSupported:
                        pair.Value = ModStatus.CurrentGameNotSupported;
                        break;
                }
            }
        }

        private string GetOtherGame()
        {
            switch (currentGame)
            {
                case QModGame.Subnautica:
                    return "BelowZero";
                case QModGame.BelowZero:
                    return "Subnautica";
                default:
                    return "Unknown";
            }
        }
    }
}
