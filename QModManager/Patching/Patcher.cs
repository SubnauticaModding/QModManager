namespace QModManager.Patching
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using API;
    using Checks;
    using HarmonyLib;
    using Utility;

    internal static class Patcher
    {
        internal const string IDRegex = "[^0-9a-zA-Z_]";

        internal static string QModBaseDir => Path.Combine(BepInEx.Paths.BepInExRootPath, "../QMods");

        private static bool Patched = false;
        internal static QModGame CurrentlyRunningGame { get; private set; } = QModGame.None;
        internal static int ErrorModCount { get; private set; }

        internal static List<Dialog> Dialogs { get; } = new List<Dialog>();

        internal static void Patch()
        {
            try
            {
                if (Patched)
                    return; // Halt patching

                Patched = true;

                var gameDetector = new GameDetector();

                if (!gameDetector.IsValidGameRunning || !gameDetector.IsValidGameVersion)
                    return;

                CurrentlyRunningGame = gameDetector.CurrentlyRunningGame;

                if (QModBaseDir == null)
                {
                    Logger.Fatal("A fatal error has occurred.");
                    Logger.Fatal("There was an error with the QMods directory");
                    Logger.Fatal("Please make sure that you ran Subnautica from Steam/Epic/Discord, and not from the executable file!");

                    Dialogs.Add(new Dialog()
                    {
                        message = "A fatal error has occurred. QModManager could not be initialized.",
                        color = Dialog.DialogColor.Red,
                        leftButton = Dialog.Button.SeeLog,
                        rightButton = Dialog.Button.Close,
                    });

                    return;
                }


                try
                {
                    Logger.Info("Game Folder structure:");
                    IOUtilities.LogFolderStructureAsTree();
                    Logger.Info("Game Folder structure ended.");
                }
                catch (Exception e)
                {
                    Logger.Error("There was an error while trying to display the folder structure.");
                    Logger.Exception(e);
                }

                var normalizedModDir = Path.GetFullPath(QModBaseDir);
                if (!normalizedModDir.EndsWith($"{Path.DirectorySeparatorChar}") && !normalizedModDir.EndsWith($"{Path.AltDirectorySeparatorChar}"))
                    normalizedModDir += $"{Path.DirectorySeparatorChar}";
                var ModDirUri = new Uri(normalizedModDir);

                var normalizedGameDir = Path.GetFullPath(Environment.CurrentDirectory);
                
                if (!normalizedGameDir.EndsWith($"{Path.DirectorySeparatorChar}") && !normalizedGameDir.EndsWith($"{Path.AltDirectorySeparatorChar}"))
                    normalizedGameDir += $"{Path.DirectorySeparatorChar}";
                var GameDirUri = new Uri(normalizedGameDir);


                if (!GameDirUri.IsBaseOf(ModDirUri))
                {
                    try
                    {
                        Logger.Info("Mods Folder structure:");
                        IOUtilities.LogFolderStructureAsTree(Path.Combine(BepInEx.Paths.BepInExRootPath, ".."));
                        Logger.Info("Mods Folder structure ended.");
                    }
                    catch (Exception e)
                    {
                        Logger.Error("There was an error while trying to display the folder structure.");
                        Logger.Exception(e);
                    }
                }

                PirateCheck.IsPirate(Environment.CurrentDirectory);

                PatchHarmony();

                if (NitroxCheck.IsRunning)
                {
                    Logger.Warn($"Nitrox was detected running!");

                    Dialogs.Add(new Dialog()
                    {
                        message = "Nitrox detected. \nNitrox compatibility with QModManager is HIGHLY EXPERIMENTAL Expect bugs!.",
                        leftButton = Dialog.Button.Close,
                        rightButton = Dialog.Button.Disabled,
                        color = Dialog.DialogColor.Red,
                    });

                    return;
                }

                VersionCheck.Check();

                Logger.Info("Started loading mods");

            }
            catch (FatalPatchingException pEx)
            {
                Logger.Fatal($"A fatal patching exception has been caught! Patching ended prematurely!");
                Logger.Exception(pEx);

                Dialogs.Add(new Dialog()
                {
                    message = "A fatal patching exception has been caught. QModManager could not be initialized.",
                    color = Dialog.DialogColor.Red,
                    leftButton = Dialog.Button.SeeLog,
                });
            }
            catch (Exception e)
            {
                Logger.Fatal("An unhandled exception has been caught! Patching ended prematurely!");
                Logger.Exception(e);

                Dialogs.Add(new Dialog()
                {
                    message = "An unhandled exception has been caught. QModManager could not be initialized.",
                    color = Dialog.DialogColor.Red,
                    leftButton = Dialog.Button.SeeLog,
                });
            }
        }

        // Store the instance for use by MainMenuMessages
        internal static Harmony hInstance;
        private static void PatchHarmony()
        {
            try
            {
                Logger.Debug("Applying Harmony patches...");

                hInstance = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "qmodmanager");

                Logger.Debug("Patched!");
            }
            catch (Exception e)
            {
                Logger.Error("There was an error while trying to apply Harmony patches.");
                Logger.Exception(e);
            }
        }
    }
}
