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

    [Flags]
    internal enum OS
    {
        None = 0b00,
        Windows = 0b01,
        Mac = 0b10,
        Both = Windows | Mac,
    }

    internal static class Program
    {
        internal static Action action = Action.RunByUser;
        internal static OS os;
        internal static Patcher.Game game;

        internal static void Main(string[] args)
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

                string managedDirectory = Environment.CurrentDirectory;
                string globalgamemanagers = Path.Combine(managedDirectory, "../globalgamemanagers");

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

                GetInfo(out os, out string directory, out game);

                Injector injector;

                if (os == OS.Both)
                {
                    // This runs if both windows and mac files were detected, but it should NEVER happen.
                    Console.WriteLine("An unexpected error has occurred.");
                    Console.WriteLine("Both Windows and Mac files detected!");
                    Console.WriteLine("Is this a Windows or a Mac environment?");
                    Console.WriteLine();
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    Environment.Exit(1);
                    return;
                }
                else if (os == OS.Windows || os == OS.Mac)
                {
                    injector = new Injector(directory, managedDirectory);
                }
                else
                {
                    Console.WriteLine("Could not find any game to patch!");
                    Console.WriteLine("An assembly file was found, but no executable was detected.");
                    Console.WriteLine("Please make sure you have installed QModManager in the right folder.");
                    Console.WriteLine("If the problem persists, open a bug report on NexusMods or an issue on GitHub");
                    Console.WriteLine();
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    Environment.Exit(1);
                    return;
                }

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
                        Console.WriteLine();
                        Console.WriteLine("Trying to enable Unity sound...");

                        AudioFixer.ChangeDisableUnityAudio(globalgamemanagers, false, game);

                        Console.WriteLine("Unity sound enabled successfully");
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
                        Console.WriteLine();
                        Console.WriteLine("Trying to disable Unity sound...");

                        AudioFixer.ChangeDisableUnityAudio(globalgamemanagers, true, game);

                        Console.WriteLine("Unity sound disabled successfully");
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

        internal static void GetInfo(out OS os, out string directory, out Patcher.Game game)
        {
            string windowsDirectory = Path.Combine(Environment.CurrentDirectory, "../..");
            string macDirectory = Path.Combine(Environment.CurrentDirectory, "../../../../..");

            bool subnautica = false, belowzero = false;

            // Check if the device is running Windows OS
            bool onWindows = false, onWindowsSN = false, onWindowsBZ = false;
            if (Directory.Exists(windowsDirectory))
            {
                try
                {
                    // Try to get the Subnautica executable
                    // This method throws a lot of exceptions
                    onWindowsSN = Directory.GetFiles(windowsDirectory, "Subnautica.exe", SearchOption.TopDirectoryOnly).Length > 0;
                    onWindowsBZ = Directory.GetFiles(windowsDirectory, "SubnauticaZero.exe", SearchOption.TopDirectoryOnly).Length > 0;

                    onWindows = onWindowsSN || onWindowsBZ;

                    subnautica = subnautica || onWindowsSN;
                    belowzero = belowzero || onWindowsBZ;
                }
                catch (Exception)
                {
                    // If an exception was thrown, the file probably isn't there
                    onWindows = false;
                }
            }

            // Check if the device is running Mac OS
            bool onMac = false, onMacSN = false, onMacBZ = false;
            if (Directory.Exists(macDirectory))
            {
                try
                {
                    // Try to get the Subnautica executable
                    // This method throws a lot of exceptions
                    // On mac, .app files act as files and folders at the same time, but they are detected as folders.
                    onMacSN = Directory.GetDirectories(macDirectory, "Subnautica.app", SearchOption.TopDirectoryOnly).Length > 0;
                    onMacBZ = Directory.GetDirectories(macDirectory, "SubnauticaZero.app", SearchOption.TopDirectoryOnly).Length > 0;

                    onMac = onMacSN || onMacBZ;

                    subnautica = subnautica || onMacSN;
                    belowzero = belowzero || onMacBZ;
                }
                catch (Exception)
                {
                    // If an exception was thrown, the file probably isn't there
                    onMac = false;
                }
            }

            os = OS.None;
            directory = null;
            if (onWindows)
            {
                os |= OS.Windows;
                directory = windowsDirectory;
            }
            if (onMac)
            {
                os |= OS.Mac;
                directory = windowsDirectory;
            }

            game = Patcher.Game.None;
            if (subnautica) game |= Patcher.Game.Subnautica;
            if (belowzero) game |= Patcher.Game.BelowZero;
        }

        #region Disable exit

        internal static void DisableExit()
        {
            DisableExitButton();
            Console.CancelKeyPress += CancelKeyPress;
            Console.TreatControlCAsInput = true;
        }

        internal static void DisableExitButton() => EnableMenuItem(GetSystemMenu(GetConsoleWindow(), false), 0xF060, 0x1);
        internal static void CancelKeyPress(object sender, ConsoleCancelEventArgs e) => e.Cancel = true;
        [DllImport("user32.dll")] internal static extern int EnableMenuItem(IntPtr tMenu, int targetItem, int targetStatus);
        [DllImport("user32.dll")] internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("kernel32.dll", ExactSpelling = true)] internal static extern IntPtr GetConsoleWindow();

        #endregion
    }
}
