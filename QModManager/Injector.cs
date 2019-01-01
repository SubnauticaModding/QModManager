using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.IO;
using System.Linq;

namespace QModManager
{
    public class QModInjector
    {
        internal string gameDirectory;
        internal string managedDirectory;
        internal string installerFilename = "QModInstaller.dll";
        internal string mainFilename = "Assembly-CSharp.dll";
        internal string backupFilename = "Assembly-CSharp.qoriginal.dll";

        internal QModInjector(string dir, string managedDir = null)
        {
            gameDirectory = dir;
			if (managedDir == null)
			{
				managedDirectory = Path.Combine(gameDirectory, @"Subnautica_Data\Managed");
			}
			else
			{
				managedDirectory = managedDir;
			}
            mainFilename = Path.Combine(managedDirectory, mainFilename);
            backupFilename = Path.Combine(managedDirectory, backupFilename);
        }

#warning TODO: Implement installer rollback in Inno Setup
        internal void Inject()
        {
            try
            {
                if (IsInjected())
                {
                    Console.WriteLine("Tried to install, but it was already injected");
                    Console.WriteLine("Skipping installation");
                    Console.WriteLine();
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                AssemblyDefinition game = AssemblyDefinition.ReadAssembly(mainFilename);

                if (File.Exists(backupFilename)) File.Delete(backupFilename);

                game.Write(backupFilename);

                AssemblyDefinition installer = AssemblyDefinition.ReadAssembly(installerFilename);
                MethodDefinition patchMethod = installer.MainModule.GetType("QModInstaller.QModPatcher").Methods.First(x => x.Name == "Patch");

                TypeDefinition type = game.MainModule.GetType("GameInput");
                MethodDefinition method = type.Methods.Single(x => x.Name == "Awake");

                method.Body.GetILProcessor().InsertBefore(method.Body.Instructions[0], Instruction.Create(OpCodes.Call, method.Module.Import(patchMethod)));

                game.Write(mainFilename);

                string qmodsDirectory = Path.Combine(gameDirectory, "QMods");

                if (!Directory.Exists(qmodsDirectory)) Directory.CreateDirectory(qmodsDirectory);

                Console.WriteLine();
                Console.WriteLine("QModManager installed successfully");
                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION CAUGHT!");
                Console.WriteLine(e.ToString());
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
                    Console.WriteLine();
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                if (File.Exists(backupFilename))
                {
                    File.Delete(mainFilename);

                    File.Move(backupFilename, mainFilename);

                    Console.WriteLine();
                    Console.WriteLine("QModManager was uninstalled successfully");
                    Console.WriteLine();
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                Console.WriteLine();
                Console.WriteLine("Cannot uninstall, file 'Assembly-CSharp-qoriginal.dll' is missing");
                Console.WriteLine("To uninstall, you will need to verify game contents ons steam");
                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION CAUGHT!");
                Console.WriteLine(e.ToString());
            }
        }

        internal bool IsInjected()
        {
            try
            {
                AssemblyDefinition game = AssemblyDefinition.ReadAssembly(mainFilename);

                AssemblyDefinition installer = AssemblyDefinition.ReadAssembly(installerFilename);
                MethodDefinition patchMethod = installer.MainModule.GetType("QModInstaller.QModPatcher").Methods.Single(x => x.Name == "Patch");

                TypeDefinition type = game.MainModule.GetType("GameInput");
                MethodDefinition method = type.Methods.First(x => x.Name == "Awake");

                foreach (Instruction instruction in method.Body.Instructions)
                {
                    if (instruction.OpCode.Equals(OpCodes.Call) && instruction.Operand.ToString().Equals("System.Void QModInstaller.QModPatcher::Patch()"))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
