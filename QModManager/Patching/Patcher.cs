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
    using QModManager.HarmonyPatches.FixSignsLoading;
    using Utility;

    internal static class Patcher
    {
        internal const string IDRegex = "[^0-9a-zA-Z_]";

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

                Logger.Info("Game Version: " + SNUtils.GetPlasticChangeSetOfBuild() + " Build Date: " + SNUtils.GetDateTimeOfBuild().ToLongDateString());
                Logger.Info($"Loading QModManager v{Assembly.GetExecutingAssembly().GetName().Version.ToStringParsed()}...");
                Logger.Info($"Today is {DateTime.Today:dd-MMMM-yyyy}");

                if (QModBaseDir == null)
                {
                    Logger.Fatal("A fatal error has occurred.");
                    Logger.Fatal("There was an error with the QMods directory");
                    Logger.Fatal("Please make sure that you ran Subnautica from Steam/Epic/Discord, and not from the executable file!");

                    new Dialog()
                    {
                        message = "A fatal error has occurred. QModManager could not be initialized.",
                        color = Dialog.DialogColor.Red,
                        leftButton = Dialog.Button.SeeLog,
                        rightButton = Dialog.Button.Close,
                    }.Show();

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

                try
                {
                    PatchHarmony();
                }
                catch (Exception e)
                {
                    Logger.Error("There was an error while trying to apply Harmony patches.");
                    Logger.Exception(e);
                }

                if (NitroxCheck.IsInstalled)
                {
                    Logger.Fatal($"Nitrox was detected!");

                    new Dialog()
                    {
                        message = "Both QModManager and Nitrox detected. QModManager is not compatible with Nitrox. Please uninstall one of them.",
                        leftButton = Dialog.Button.Disabled,
                        rightButton = Dialog.Button.Disabled,
                        color = Dialog.DialogColor.Red,
                    }.Show();

                    return;
                }

                VersionCheck.Check();

                Logger.Info("Started loading mods");

                AddAssemblyResolveEvent();

                IQModFactory modFactory = new QModFactory();
                List<QMod> modsToLoad = modFactory.BuildModLoadingList(QModBaseDir);

                QModServices.LoadKnownMods(modsToLoad);

                var initializer = new Initializer(CurrentlyRunningGame);
                initializer.InitializeMods(modsToLoad);

                SummaryLogger.ReportIssues(modsToLoad);

                SummaryLogger.LogSummaries(modsToLoad);
            }
            catch (FatalPatchingException pEx)
            {
                Logger.Fatal($"A fatal patching exception has been caught! Patching ended prematurely!");
                Logger.Exception(pEx);

                new Dialog()
                {
                    message = "A fatal patching exception has been caught. QModManager could not be initialized.",
                    color = Dialog.DialogColor.Red,
                    leftButton = Dialog.Button.SeeLog,
                }.Show();
            }
            catch (Exception e)
            {
                Logger.Fatal("An unhandled exception has been caught! Patching ended prematurely!");
                Logger.Exception(e);

                new Dialog()
                {
                    message = "An unhandled exception has been caught. QModManager could not be initialized.",
                    color = Dialog.DialogColor.Red,
                    leftButton = Dialog.Button.SeeLog,
                }.Show();
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

            // Create harmony instance.
            HarmonyInstance harmony = HarmonyInstance.Create("qmodmanager");

            // Apply all patches.
            harmony.PatchAll();

            // If game is Below Zero (any CS version), or if game is Subnautica (CS version < 65271), apply patch to Sign objects.
            if (Patcher.CurrentlyRunningGame == QModGame.BelowZero || SNUtils.GetPlasticChangeSetOfBuild(65271) < 65271)
            {
                MethodInfo toPatch = typeof(uGUI_SignInput).GetMethod(nameof(uGUI_SignInput.UpdateScale), BindingFlags.NonPublic | BindingFlags.Instance);
                MethodInfo postfix = typeof(uGUI_SignInput_UpdateScale_Patch).GetMethod(nameof(uGUI_SignInput_UpdateScale_Patch.Postfix), BindingFlags.NonPublic | BindingFlags.Static);
                harmony.Patch(toPatch, null, new HarmonyMethod(postfix));
            }

            Logger.Debug("Patched!");
        }
    }
}
