using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace QModManager
{
    internal enum Action
    {
        Install,
        Uninstall,
        RunByUser
    }

    internal static class ConsoleExecutable
    {
        internal static Action action = Action.RunByUser;

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

                if (!File.Exists(managedDirectory + "/Assembly-CSharp.dll"))
                {
                    Console.WriteLine("Could not find the assembly file.");
                    Console.WriteLine("Please make sure you have installed QModManager in the right folder.");
                    Console.WriteLine("If the problem persists, open a bug report on NexusMods or an issue on GitHub");
                    Console.WriteLine();
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    Environment.Exit(1);
                }

                string windowsDirectory = Path.Combine(Environment.CurrentDirectory, "../..");
                string macDirectory = Path.Combine(Environment.CurrentDirectory, "../../../../..");

                bool onWindows;
                try
                {
                    onWindows = Directory.GetFiles(windowsDirectory, "*Subnautica*.exe", SearchOption.TopDirectoryOnly).Length > 0;
                }
                catch
                {
                    onWindows = false;
                }
                bool onMac;
                try
                {
                    onMac = Directory.GetFiles(macDirectory, "*Subnautica*.app", SearchOption.TopDirectoryOnly).Length > 0 || Directory.GetDirectories(macDirectory, "*Subnautica*.app", SearchOption.TopDirectoryOnly).Length > 0;
                }
                catch
                {
                    onMac = false;
                }

                if (!onWindows && !onMac)
                {
                    Console.WriteLine("Could not find any game to patch!");
                    Console.WriteLine("An assembly file was found, but no executable was detected.");
                    Console.WriteLine("Please make sure you have installed QModManager in the right folder.");
                    Console.WriteLine("If the problem persists, open a bug report on NexusMods or an issue on GitHub");
                    Console.WriteLine();
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    Environment.Exit(1);
                }

#warning TODO: Improve injector code. It's 2019 out there...
                QModInjector injector;
                if (onWindows) injector = new QModInjector(windowsDirectory, managedDirectory);
                else injector = new QModInjector(macDirectory, managedDirectory);

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
                        Console.WriteLine("Press any key to exit...");
                        Console.ReadKey();
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
                        Console.WriteLine("Press any key to exit...");
                        Console.ReadKey();
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
                Console.WriteLine(e.ToString());
                Environment.Exit(2);
            }
        }

        #region Disable exit

        internal static void DisableExit()
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
