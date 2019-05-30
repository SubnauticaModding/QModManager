using System;
using System.IO;

namespace QModManager.Executable
{
    internal class Injector
    {
        private readonly string gameDirectory;

        internal Injector(string dir)
        {
            gameDirectory = dir;
        }

        internal void Inject()
        {
            try
            {
                if (IsInjected())
                {
                    Console.WriteLine("Tried to install, but is already installed.");
                    Console.WriteLine("Skipping installation");
                    Environment.Exit(0);
                }

                if (File.Exists(Path.Combine(gameDirectory, "QModManager/doorstop_config.ini.doorstop")))
                {
                    File.Copy(Path.Combine(gameDirectory, "QModManager/doorstop_config.ini.doorstop"), Path.Combine(gameDirectory, "doorstop_config.ini"), true);
                }
                else
                {
                    Console.WriteLine("Doorstop config file not found.");
                    Console.WriteLine("QModManager cannot be installed.");
                    Console.ReadKey();
                    Environment.Exit(1);
                }

                if (File.Exists(Path.Combine(gameDirectory, "QModManager/version.dll.doorstop")))
                {
                    File.Copy(Path.Combine(gameDirectory, "QModManager/version.dll.doorstop"), Path.Combine(gameDirectory, "version.dll"), true);
                }
                else
                {
                    Console.WriteLine("Doorstop assembly file not found.");
                    Console.WriteLine("QModManager cannot be installed.");
                    Console.ReadKey();
                    Environment.Exit(1);
                }

                if (!Directory.Exists(Path.Combine(gameDirectory, "QMods")))
                    Directory.CreateDirectory(Path.Combine(gameDirectory, "QMods"));

                Console.WriteLine("QModManager installed successfully");
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION CAUGHT!");
                Console.WriteLine(e.ToString());
                Console.WriteLine();
                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        internal void Remove()
        {
            try
            {
                if (!IsInjected())
                {
                    Console.WriteLine("Tried to uninstall, but patch was not present");
                    Console.WriteLine("Skipping uninstallation");
                    Environment.Exit(0);
                }

                if (File.Exists(Path.Combine(gameDirectory, "doorstop_config.ini")))
                    File.Delete(Path.Combine(gameDirectory, "doorstop_config.ini"));

                if (File.Exists(Path.Combine(gameDirectory, "version.dll")))
                    File.Delete(Path.Combine(gameDirectory, "version.dll"));

                Console.WriteLine("QModManager was uninstalled successfully");
                Environment.Exit(0);
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

        internal bool IsInjected()
        {
            return File.Exists(Path.Combine(gameDirectory, "doorstop_config.ini")) && File.Exists(Path.Combine(gameDirectory, "version.dll"));
        }
    }
}
