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

                string directory = Path.Combine(Environment.CurrentDirectory, @"..\..");
                string managedDirectory = Environment.CurrentDirectory;

                if (!File.Exists(managedDirectory + "/Assembly-CSharp.dll"))
                {
                    Console.WriteLine(LanguageLines.Executable.AssemblyMissing);
                    Console.WriteLine(LanguageLines.General.PressAnyKey);
                    Console.ReadKey();
                    Environment.Exit(1);
                }

                if (Directory.GetFiles(directory, "*Subnautica*.exe", SearchOption.TopDirectoryOnly).Length <= 0)
                {
                    Console.WriteLine(LanguageLines.Executable.ExeMissing);
                    Console.WriteLine(LanguageLines.General.PressAnyKey);
                    Console.ReadKey();
                    Environment.Exit(1);
                }

#warning TODO: Improve injector code. It's 2018 out there...
                QModInjector injector = new QModInjector(directory, managedDirectory);

                bool isInjected = injector.IsInjected();

                if (action == Action.Install)
                {
                    if (!isInjected)
                    {
                        Console.WriteLine(LanguageLines.Executable.Installing);
                        injector.Inject();
                    }
                    else
                    {
                        Console.WriteLine(LanguageLines.Executable.AlreadyInstalled);
                        Console.WriteLine(LanguageLines.General.PressAnyKey);
                        Console.ReadKey();
                        Environment.Exit(0);
                    }
                }
                else if (action == Action.Uninstall)
                {
                    if (isInjected)
                    {
                        Console.WriteLine(LanguageLines.Executable.Uninstalling);
                        injector.Remove();
                    }
                    else
                    {
                        Console.WriteLine(LanguageLines.Executable.AlreadyUninstalled);
                        Console.WriteLine(LanguageLines.General.PressAnyKey);
                        Console.ReadKey();
                        Environment.Exit(0);
                    }
                }
                else
                {
                    if (!isInjected)
                    {
                        Console.Write(LanguageLines.Executable.AskInstall);
                        ConsoleKey key = Console.ReadKey().Key;
                        Console.WriteLine();
                        if (key == ConsoleKey.Y)
                        {
                            Console.WriteLine(LanguageLines.Executable.Installing);
                            injector.Inject();
                        }
                        else if (key == ConsoleKey.N)
                        {
                            Console.WriteLine(LanguageLines.Executable.Installing);
                            Console.ReadKey();
                            Environment.Exit(0);
                        }
                    }
                    else
                    {
                        Console.Write(LanguageLines.Executable.AskUninstall);
                        ConsoleKey key = Console.ReadKey().Key;
                        Console.WriteLine();
                        if (key == ConsoleKey.Y)
                        {
                            Console.WriteLine(LanguageLines.Executable.Uninstalling);
                            injector.Remove();
                        }
                        else if (key == ConsoleKey.N)
                        {
                            Console.WriteLine(LanguageLines.General.PressAnyKey);
                            Console.ReadKey();
                            Environment.Exit(0);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(LanguageLines.General.ExceptionCaught);
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
