using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Oculus.Newtonsoft.Json;

namespace QModInstaller
{
	public class QModPatcher
	{
        private static string qModBaseDir = Environment.CurrentDirectory + "/QMods";
        private static List<QMod> loadedMods = new List<QMod>();
        private static bool patched = false;

        public static void Patch()
		{
			AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs args)
			{
                FileInfo[] files = new DirectoryInfo(QModPatcher.qModBaseDir).GetFiles("*.dll", SearchOption.AllDirectories);
				foreach (FileInfo fileInfo in files)
				{
					Console.WriteLine(Path.GetFileNameWithoutExtension(fileInfo.Name) + " " + args.Name);
					bool flag12 = args.Name.Contains(Path.GetFileNameWithoutExtension(fileInfo.Name));
					if (flag12)
					{
						return Assembly.LoadFrom(fileInfo.FullName);
					}
				}
				return null;
			};
			bool flag = QModPatcher.patched;
			if (!flag)
			{
				QModPatcher.patched = true;
				bool flag2 = !Directory.Exists(QModPatcher.qModBaseDir);
				if (flag2)
				{
					Console.WriteLine("QMOD ERR: QMod directory was not found");
					Directory.CreateDirectory(QModPatcher.qModBaseDir);
					Console.WriteLine("QMOD INFO: Created QMod directory at {0}", QModPatcher.qModBaseDir);
				}
				else
				{
					string[] directories = Directory.GetDirectories(QModPatcher.qModBaseDir);
					List<QMod> list = new List<QMod>();
					List<QMod> list2 = new List<QMod>();
					List<QMod> list3 = new List<QMod>();
					foreach (string path in directories)
					{
						string text = Path.Combine(path, "mod.json");
						bool flag3 = !File.Exists(text);
						if (flag3)
						{
							Console.WriteLine("QMOD ERR: Mod is missing a mod.json file");
							File.WriteAllText(text, JsonConvert.SerializeObject(new QMod()));
							Console.WriteLine("QMOD INFO: A template for mod.json was generated at {0}", text);
						}
						else
						{
							QMod qmod = QMod.FromJsonFile(Path.Combine(path, "mod.json"));
							bool flag4 = qmod.Equals(null);
							if (!flag4)
							{
								bool flag5 = qmod.Enable.Equals(false);
								if (flag5)
								{
									Console.WriteLine("QMOD WARN: {0} is disabled via config, skipping", qmod.DisplayName);
								}
								else
								{
									string text2 = Path.Combine(path, qmod.AssemblyName);
									bool flag6 = !File.Exists(text2);
									if (flag6)
									{
										Console.WriteLine("QMOD ERR: No matching dll found at {0} for {1}", text2, qmod.Id);
									}
									else
									{
										qmod.loadedAssembly = Assembly.LoadFrom(text2);
										qmod.modAssemblyPath = text2;
										bool flag7 = qmod.Priority.Equals("Last");
										if (flag7)
										{
											list.Add(qmod);
										}
										else
										{
											bool flag8 = qmod.Priority.Equals("First");
											if (flag8)
											{
												list2.Add(qmod);
											}
											else
											{
												list3.Add(qmod);
											}
										}
									}
								}
							}
						}
					}
					foreach (QMod qmod2 in list2)
					{
						bool flag9 = qmod2 != null;
						if (flag9)
						{
							QModPatcher.loadedMods.Add(QModPatcher.LoadMod(qmod2));
						}
					}
					foreach (QMod qmod3 in list3)
					{
						bool flag10 = qmod3 != null;
						if (flag10)
						{
							QModPatcher.loadedMods.Add(QModPatcher.LoadMod(qmod3));
						}
					}
					foreach (QMod qmod4 in list)
					{
						bool flag11 = qmod4 != null;
						if (flag11)
						{
							QModPatcher.loadedMods.Add(QModPatcher.LoadMod(qmod4));
						}
					}
				}
			}
		}

		private static QMod LoadMod(QMod mod)
		{
			bool flag = mod == null;
			QMod result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = string.IsNullOrEmpty(mod.EntryMethod);
				if (flag2)
				{
					Console.WriteLine("QMOD ERR: No EntryMethod specified for {0}", mod.Id);
				}
				else
				{
					try
					{
						string[] array = mod.EntryMethod.Split(new char[]
						{
							'.'
						});
						string name = string.Join(".", array.Take(array.Length - 1).ToArray<string>());
						string name2 = array[array.Length - 1];
						MethodInfo method = mod.loadedAssembly.GetType(name).GetMethod(name2);
						method.Invoke(mod.loadedAssembly, new object[0]);
					}
					catch (ArgumentNullException ex)
					{
						Console.WriteLine("QMOD ERR: Could not parse EntryMethod {0} for {1}", mod.AssemblyName, mod.Id);
						Console.WriteLine(ex.InnerException.Message);
						return null;
					}
					catch (TargetInvocationException ex2)
					{
						Console.WriteLine("QMOD ERR: Invoking the specified EntryMethod {0} failed for {1}", mod.EntryMethod, mod.Id);
						Console.WriteLine(ex2.InnerException.Message);
						return null;
					}
					catch (Exception ex3)
					{
						Console.WriteLine("QMOD ERR: something strange happened");
						Console.WriteLine(ex3.Message);
						return null;
					}
				}
				result = mod;
			}
			return result;
		}
	}
}
