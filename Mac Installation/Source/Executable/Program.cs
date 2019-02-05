using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QModInstaller;

namespace QModManager
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			bool flag = false;
			bool flag2 = false;
			foreach (string text in args)
			{
				if (text.Contains("="))
				{
					dictionary = (from s in args
					select s.Split(new char[]
					{
						'='
					}, 1)).ToDictionary((string[] s) => s[0], (string[] s) => s[1]);
				}
				else if (text.StartsWith("-"))
				{
					if (text == "-i")
					{
						flag = true;
					}
					if (text == "-u")
					{
						flag2 = true;
					}
				}
			}
			string dir = Path.Combine(Environment.CurrentDirectory, "../..");
			string currentDirectory = Environment.CurrentDirectory;
			if (dictionary.Keys.Contains("SubnauticaDirectory"))
			{
				dir = dictionary["SubnauticaDirectory"];
			}
			QModInjector qmodInjector = new QModInjector(dir, currentDirectory);
			bool flag3 = qmodInjector.IsPatcherInjected();
			if (flag)
			{
				if (!flag3)
				{
					Console.WriteLine("Installing QMods...");
					qmodInjector.Inject();
					return;
				}
				Console.WriteLine("Tried to Force Install, was already injected. Skipping installation.");
				return;
			}
			else
			{
				if (!flag2)
				{
					if (!qmodInjector.IsPatcherInjected())
					{
						Console.WriteLine("No patch detected, type 'yes' to install: ");
						string a = Console.ReadLine().Replace("'", "");
						if (a == "yes" || a == "YES")
						{
							if (qmodInjector.Inject())
							{
								Console.WriteLine("QMods was installed!");
							}
							else
							{
								Console.WriteLine("Error installed QMods. Please contact us on Discord");
							}
						}
					}
					else
					{
						Console.WriteLine("Patch already installed! Type 'yes' to remove: ");
						string a2 = Console.ReadLine().Replace("'", "");
						if (a2 == "yes" || a2 == "YES")
						{
							if (qmodInjector.Remove())
							{
								Console.WriteLine("QMods was removed!");
							}
							else
							{
								Console.WriteLine("Error removing QMods. Please contact us on Discord");
							}
						}
					}
					Console.WriteLine("Press any key to exit ...");
					Console.ReadKey();
					return;
				}
				if (flag3)
				{
					Console.WriteLine("Uninstalling QMods...");
					qmodInjector.Remove();
					return;
				}
				Console.WriteLine("Tried to Force Uninstall, was not injected. Skipping uninstallation.");
				return;
			}
		}
	}
}
