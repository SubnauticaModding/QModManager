namespace QModManager
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;

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

    internal static class Executable
    {
        internal static Action action = Action.RunByUser;
        internal static OS os;

        internal static void Main(string[] args)
        {
            try
            {
                InitAssemblyResolver();

                var parsedArgs = new Dictionary<string, string>();

                foreach (string arg in args)
                {
                    if (arg.Contains("="))
                    {
                        parsedArgs = args.Select(s => s.Split(new[] { '=' }, 1)).ToDictionary(s => s[0], s => s[1]);
                    }
                    else if (arg == "-i")
                        action = Action.Install;
                    else if (arg == "-u")
                        action = Action.Uninstall;
                }

                string gameRootDirectory = Path.Combine(Environment.CurrentDirectory, "../../..");
                string snManagedDirectory = Path.Combine(gameRootDirectory, "Subnautica_Data", "Managed");
                string bzManagedDirectory = Path.Combine(gameRootDirectory, "SubnauticaZero_Data", "Managed");
                string managedDirectory;
                if (Directory.Exists(snManagedDirectory))
                {
                    managedDirectory = snManagedDirectory;
                }
                else if (Directory.Exists(bzManagedDirectory))
                {
                    managedDirectory = bzManagedDirectory;
                }
                else
                {
                    Console.WriteLine("Could not find Managed directory.");
                    Console.WriteLine("Please make sure you have installed QModManager in the right folder.");
                    Console.WriteLine("If the problem persists, open a bug report on NexusMods or an issue on GitHub");
                    Console.WriteLine();
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    Environment.Exit(1);
                    return;
                }
                string globalgamemanagers = Path.Combine(managedDirectory, "../globalgamemanagers");

                os = GetOS();

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
                else if (os == OS.None)
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

                if (action == Action.Install)
                {
                    Console.WriteLine("Attempting to enable Unity audio...");
                    UnityAudioFixer.EnableUnityAudio();
                    Console.WriteLine("Unity audio enabled.");
                    Environment.Exit(0);
                }
                else if (action == Action.Uninstall)
                {
                    Console.WriteLine("Attempting to disable Unity audio...");
                    UnityAudioFixer.DisableUnityAudio();
                    Console.WriteLine("Unity audio disabled.");
                    Environment.Exit(0);
                }
                else
                {
                    Console.Write("Enable Unity sound? [Y/N] > ");
                    ConsoleKey key = Console.ReadKey().Key;
                    Console.WriteLine();

                    if (key == ConsoleKey.Y)
                    {
                        Console.WriteLine("Attempting to enable Unity audio...");
                        UnityAudioFixer.EnableUnityAudio();
                        Console.WriteLine("Unity audio enabled.");
                        Console.WriteLine("Press any key to exit...");
                        Console.ReadKey();
                        Environment.Exit(0);
                    }
                    else if (key == ConsoleKey.N)
                    {
                        Console.WriteLine("Attempting to disable Unity audio...");
                        UnityAudioFixer.DisableUnityAudio();
                        Console.WriteLine("Unity audio disabled.");
                        Console.WriteLine("Press any key to exit...");
                        Console.ReadKey();
                        Environment.Exit(0);
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

        private static OS GetOS()
        {
            string windowsDirectory = Path.Combine(Environment.CurrentDirectory, "../../..");
            string macDirectory = Path.Combine(Environment.CurrentDirectory, "../../../../../..");

            bool subnautica = false, belowzero = false;

            // Check if the device is running Windows OS
            bool onWindows = false, onWindowsSN, onWindowsBZ;
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
            bool onMac = false, onMacSN, onMacBZ;
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

            var os = OS.None;
            if (onWindows)
            {
                os |= OS.Windows;
            }
            if (onMac)
            {
                os |= OS.Mac;
            }
            return os;
        }

        private static void InitAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssemblies;
        }

        internal static string GameRootDirectory => Path.Combine(Environment.CurrentDirectory, "../../..");
        internal static string BepInExRootDirectory => Path.Combine(GameRootDirectory, "BepInEx");
        internal static string BepInExAssemblyDirectory => Path.Combine(BepInExRootDirectory, "core");
        internal static string QModManagerPluginDirectory => Path.Combine(BepInExRootDirectory, "plugins", "QModManager");
        internal static string QModManagerPatcherDirectory => Path.Combine(BepInExRootDirectory, "patchers", "QModManager");

        private static Assembly ResolveAssemblies(object sender, ResolveEventArgs e)
        {
            var name = new AssemblyName(e.Name);

            if (Utility.TryResolveDllAssembly(name, BepInExRootDirectory, out var assembly) || // First try BepInEx assemblies
                Utility.TryResolveDllAssembly(name, QModManagerPluginDirectory, out assembly) || // Then QModManager plugins
                Utility.TryResolveDllAssembly(name, QModManagerPatcherDirectory, out assembly)) // Then QModManager patchers
            {

                return assembly;
            }
            else
            {
                return null;
            }
        }

        #region Disable exit

        internal static void DisableExit()
        {
            DisableExitButton();
            Console.CancelKeyPress += CancelKeyPress;
            Console.TreatControlCAsInput = true;
        }

        private static void DisableExitButton()
        {
            EnableMenuItem(GetSystemMenu(GetConsoleWindow(), false), 0xF060, 0x1);
        }

        private static void CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
        }

        [DllImport("user32.dll")] private static extern int EnableMenuItem(IntPtr tMenu, int targetItem, int targetStatus);
        [DllImport("user32.dll")] private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("kernel32.dll", ExactSpelling = true)] private static extern IntPtr GetConsoleWindow();

        #endregion
    }
}
