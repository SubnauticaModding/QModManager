using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace QModManager.Executable
{
    internal enum Action
    {
        Install,
        Uninstall,
        RunByUser,
    }

    internal static class Program
    {
        private static Action action = Action.RunByUser;
        internal static Patcher.Game game;

        private static void Main(string[] args)
        {
            try
            {
                Dictionary<string, string> parsedArgs = new Dictionary<string, string>();

                foreach (string arg in args)
                {
                    if (arg.Contains("="))
                    {
                        parsedArgs = args.Select(s => s.Split(new[] { '=' }, 1)).ToDictionary(s => s[0], s => s[1]);
                    }
                    else if (arg == "-i") action = Action.Install;
                    else if (arg == "-u") action = Action.Uninstall;
                }

                GetInfo(out game);

                if (game == Patcher.Game.None)
                {
                    Console.WriteLine("Could not find any game to patch!");
                    Console.WriteLine("Please make sure you have installed QModManager in the right folder.");
                    Console.WriteLine("If the problem persists, open a bug report on NexusMods or an issue on GitHub");
                    Console.WriteLine();
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    Environment.Exit(1);
                    return;
                }

                string managedDirectory = Path.Combine(Environment.CurrentDirectory, game == Patcher.Game.Subnautica ? "Subnautica_Data/Managed" : "SubnauticaZero_Data/Managed");

                if (!File.Exists(Path.Combine(managedDirectory, "Assembly-CSharp.dll")))
                {
                    Console.WriteLine("Could not find the assembly file.");
                    Console.WriteLine("Please make sure you have installed QModManager in the right folder.");
                    Console.WriteLine("If the problem persists, open a bug report on NexusMods or an issue on GitHub");
                    Console.WriteLine();
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    Environment.Exit(1);
                }

                Injector injector = new Injector(Environment.CurrentDirectory);

                bool isInjected = injector.IsInjected();

                if (action == Action.Install)
                {
                    if (!isInjected)
                    {
                        Console.WriteLine("Installing QModManager...");
                        injector.Inject();
                    }
                    else
                    {
                        Console.WriteLine("QModManager is already installed!");
                        Console.WriteLine("Skipping installation");
                        Environment.Exit(0);
                    }
                }
                else if (action == Action.Uninstall)
                {
                    if (isInjected)
                    {
                        Console.WriteLine("Uninstalling QModManager...");
                        injector.Remove();
                    }
                    else
                    {
                        Console.WriteLine("QModManager is already uninstalled!");
                        Console.WriteLine("Skipping uninstallation");
                        Environment.Exit(0);
                    }
                }
                else
                {
                    if (!isInjected)
                    {
                        Console.Write("No patch detected, install? [Y/N] > ");
                        ConsoleKey key = Console.ReadKey().Key;
                        Console.WriteLine();
                        if (key == ConsoleKey.Y)
                        {
                            Console.WriteLine("Installing QModManager...");
                            injector.Inject();
                        }
                        else if (key == ConsoleKey.N)
                        {
                            Console.WriteLine("Press any key to exit...");
                            Console.ReadKey();
                            Environment.Exit(0);
                        }
                    }
                    else
                    {
                        Console.Write("Patch installed, remove? [Y/N] > ");
                        ConsoleKey key = Console.ReadKey().Key;
                        Console.WriteLine();
                        if (key == ConsoleKey.Y)
                        {
                            Console.Write("Uninstalling QModManager...");
                            injector.Remove();
                        }
                        else if (key == ConsoleKey.N)
                        {
                            Console.WriteLine("Press any key to exit...");
                            Console.ReadKey();
                            Environment.Exit(0);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION CAUGHT!");
                Console.WriteLine(e.ToString());
                Console.WriteLine();
                Console.ReadKey();
                Environment.Exit(2);
            }
        }

        private static void GetInfo(out Patcher.Game game)
        {
            string windowsDirectory = Environment.CurrentDirectory;

            bool subnautica = false, belowzero = false;

            if (Directory.Exists(windowsDirectory))
            {
                try
                {
                    // Try to get the Subnautica executable
                    // This method throws a lot of exceptions
                    subnautica = Directory.GetFiles(windowsDirectory, "Subnautica.exe", SearchOption.TopDirectoryOnly).Length > 0;
                    belowzero = Directory.GetFiles(windowsDirectory, "SubnauticaZero.exe", SearchOption.TopDirectoryOnly).Length > 0;
                }
                catch { }
            }

            game = Patcher.Game.None;
            if (subnautica) game |= Patcher.Game.Subnautica;
            if (belowzero) game |= Patcher.Game.BelowZero;
        }

        #region Disable exit

        private static void DisableExit()
        {
            DisableExitButton();
            Console.CancelKeyPress += CancelKeyPress;
            Console.TreatControlCAsInput = true;
        }

        private static void DisableExitButton() => EnableMenuItem(GetSystemMenu(GetConsoleWindow(), false), 0xF060, 0x1);
        private static void CancelKeyPress(object sender, ConsoleCancelEventArgs e) => e.Cancel = true;
        [DllImport("user32.dll")] private static extern int EnableMenuItem(IntPtr tMenu, int targetItem, int targetStatus);
        [DllImport("user32.dll")] private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("kernel32.dll", ExactSpelling = true)] private static extern IntPtr GetConsoleWindow();

        #endregion
    }
}
