using QModInstaller;
using System;

namespace QModManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();

            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                if (string.IsNullOrEmpty(options.SubnauticaDirectory))
                    options.SubnauticaDirectory = @"C:\Program Files (x86)\Steam\steamapps\common\Subnautica";
                
                // TODO check the subnautica path is valid

                Console.WriteLine(options.SubnauticaDirectory);
            }

            QModInjector injector = new QModInjector(options.SubnauticaDirectory);

            if (!injector.IsPatcherInjected())
            {
                Console.WriteLine("No patch detected, type 'yes' to install: ");
                string consent = Console.ReadLine();
                if (consent == "yes" || consent == "YES")
                {
                    if (injector.Inject())
                        Console.WriteLine("QMods was installed!");
                    else
                        Console.WriteLine("Error installed QMods. Please contact us on Discord");
                }
            }
            else
            {
                Console.WriteLine("Patch already installed! Type 'yes' to remove: ");
                string consent = Console.ReadLine();
                if (consent == "yes" || consent == "YES")
                {
                    if (injector.Remove())
                        Console.WriteLine("QMods was removed!");
                    else
                        Console.WriteLine("Error removing QMods. Please contact us on Discord");
                }
            }

            Console.WriteLine("Press any key to exit ...");

            Console.ReadKey();
        }
    }
}
