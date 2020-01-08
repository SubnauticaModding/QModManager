namespace QModManager.Patching
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using API;
    using API.ModLoading;
    using Checks;
    using Harmony;
    using Utility;

    /// <summary>
    /// The main class which handles all of QModManager's patching
    /// </summary>
    internal static class Patcher
    {
        internal const string IDRegex = "[^0-9a-z_]";

        internal static string QModBaseDir
        {
            get
            {
                if (Environment.CurrentDirectory.Contains("system32") && Environment.CurrentDirectory.Contains("Windows"))
                    return null;
                else
                    return Path.Combine(Environment.CurrentDirectory, "QMods");
            }
        }

        private static bool Patched = false;
        internal static QModGame CurrentlyRunningGame { get; private set; } = QModGame.None;
        internal static int ErrorModCount { get; private set; }

        internal static void Patch()
        {
            try
            {
                if (Patched)
                {
                    Logger.Warn("Patch method was called multiple times!");
                    return; // Halt patching
                }

                Patched = true;

                Logger.Info($"Loading QModManager v{Assembly.GetExecutingAssembly().GetName().Version.ToStringParsed()}...");

                if (QModBaseDir == null)
                {
                    Logger.Fatal("A fatal error has occurred.");
                    Logger.Fatal("There was an error with the QMods directory");
                    Logger.Fatal("Please make sure that you ran Subnautica from Steam/Epic/Discord, and not from the executable file!");

                    Dialog.Show("A fatal error has occurred. QModManager could not be initialized.", Dialog.Button.close, Dialog.Button.Disabled, false);

                    return;
                }

                try
                {
                    Logger.Info($"Folder structure:\n{IOUtilities.GetFolderStructureAsTree()}\n");
                }
                catch (Exception e)
                {
                    Logger.Error("There was an error while trying to display the folder structure.");
                    Logger.Exception(e);
                }

                PirateCheck.IsPirate(Environment.CurrentDirectory);

                var gameDetector = new GameDetector();

                if (!gameDetector.IsValidGameRunning)
                    return;

                CurrentlyRunningGame = gameDetector.CurrentlyRunningGame;

                PatchHarmony();

                if (NitroxCheck.IsInstalled)
                {
                    Logger.Fatal($"Nitrox was detected!");

                    Dialog.Show("Both QModManager and Nitrox detected. QModManager is not compatible with Nitrox. Please uninstall one of them.", Dialog.Button.Disabled, Dialog.Button.Disabled, false);

                    return;
                }

                VersionCheck.Check();

                Logger.Info("Started loading mods");

                AddAssemblyResolveEvent();

                var modFactory = new QModFactory();
                List<QMod> modsToLoad = modFactory.BuildModLoadingList(QModBaseDir);

                QModServices.LoadKnownMods(modsToLoad);

                var initializer = new Initializer(CurrentlyRunningGame);
                initializer.InitializeMods(modsToLoad);

                int loadedMods = 0;
                int erroredMods = 0;
                foreach (QMod mod in modsToLoad)
                {
                    if (mod.IsLoaded)
                        loadedMods++;
                    else
                        erroredMods++;
                }

                ErrorModCount = erroredMods;

                Logger.Info($"Finished loading QModManager. Loaded {loadedMods} mods");

                if (ErrorModCount > 0)
                    Logger.Warn($"A total of {ErrorModCount} mods failed to load");

                SummaryLogger.LogSummaries(modsToLoad);
            }
            catch (FatalPatchingException pEx)
            {
                Logger.Fatal($"A fatal patching exception has been caught! Patching ended prematurely!");
                Logger.Exception(pEx);

                Dialog.Show("A fatal patching exception has been caught. QModManager could not be initialized.", Dialog.Button.close, Dialog.Button.Disabled, false);
            }
            catch (Exception e)
            {
                Logger.Fatal("An unhandled exception has been caught! Patching ended prematurely!");
                Logger.Exception(e);

                Dialog.Show("An unhandled exception has been caught. QModManager could not be initialized.", Dialog.Button.close, Dialog.Button.Disabled, false);
            }
        }

        private static void AddAssemblyResolveEvent()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                FileInfo[] allDlls = new DirectoryInfo(QModBaseDir).GetFiles("*.dll", SearchOption.AllDirectories);
                foreach (FileInfo dll in allDlls)
                {
                    if (args.Name.Contains(Path.GetFileNameWithoutExtension(dll.Name)))
                    {
                        return Assembly.LoadFrom(dll.FullName);
                    }
                }

                return null;
            };

            Logger.Debug("Added AssemblyResolve event");
        }

        private static void PatchHarmony()
        {
            Logger.Debug("Applying Harmony patches...");
            HarmonyInstance.Create("qmodmanager").PatchAll();
            Logger.Debug("Patched!");
        }
    }
}
