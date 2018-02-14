using QModInstaller;
using System;
using System.Linq;

namespace QModManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var parsedArgs = args.Select(s => s.Split(new[] { '=' }, 1)).ToDictionary(s => s[0], s => s[1]);

            //string SubnauticaDirectory = @"C:\Program Files (x86)\Steam\steamapps\common\Subnautica";
            string SubnauticaDirectory = Environment.CurrentDirectory;

            if (parsedArgs.Keys.Contains("SubnauticaDirectory"))
                SubnauticaDirectory = parsedArgs["SubnauticaDirectory"];
                
            // TODO check the subnautica path is valid

            Console.WriteLine(SubnauticaDirectory);

            QModInjector injector = new QModInjector(SubnauticaDirectory);

            if (!injector.IsPatcherInjected())
            {
                Console.WriteLine("No patch detected, type 'yes' to install: ");
                string consent = Console.ReadLine().Replace("'", "");
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
