using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.IO;
using System.Linq;

namespace QModManager
{
    internal class Injector
    {
        internal readonly string gameDirectory;
        internal readonly string managedDirectory;
        internal readonly string globalgamemanagers;
        internal readonly string mainFilename = "Assembly-CSharp.dll";
        internal const string installerFilename = "QModInstaller.dll";

        internal Injector(string dir, string managedDir)
        {
            gameDirectory = dir;
			managedDirectory = managedDir;
            mainFilename = Path.Combine(managedDirectory, mainFilename);
            globalgamemanagers = Path.Combine(managedDirectory, "../globalgamemanagers");
        }

        internal void Inject()
        {
            try
            {
                if (IsInjected())
                {
                    Console.WriteLine("Tried to install, but it was already injected");
                    Console.WriteLine("Skipping installation");
                    Console.WriteLine();
                    Console.WriteLine("Trying to enable Unity sound...");

                    AudioFixer.ChangeDisableUnityAudio(globalgamemanagers, false, Executable.game);

                    Console.WriteLine("Unity sound enabled successfully");
                    Environment.Exit(0);
                }

                // Remove backup file if it exists
                string backupFilePath = Path.Combine(managedDirectory, "Assembly-CSharp.qoriginal.dll");
                if (File.Exists(backupFilePath))
                    File.Delete(backupFilePath);

                AssemblyDefinition game = AssemblyDefinition.ReadAssembly(mainFilename);

                AssemblyDefinition installer = AssemblyDefinition.ReadAssembly(installerFilename);
                MethodDefinition patchMethod = installer.MainModule.GetType("QModInstaller.QModPatcher").Methods.First(x => x.Name == "Patch");

                TypeDefinition type = game.MainModule.GetType("GameInput");
                MethodDefinition method = type.Methods.Single(x => x.Name == "Awake");

                method.Body.GetILProcessor().InsertBefore(method.Body.Instructions[0], Instruction.Create(OpCodes.Call, method.Module.Import(patchMethod)));

                game.Write(mainFilename);

                string qmodsDirectory = Path.Combine(gameDirectory, "QMods");

                if (!Directory.Exists(qmodsDirectory)) Directory.CreateDirectory(qmodsDirectory);

                Console.WriteLine("QModManager installed successfully");
                Console.WriteLine();
                Console.WriteLine("Trying to enable Unity sound...");

                AudioFixer.ChangeDisableUnityAudio(globalgamemanagers, false, Executable.game);

                Console.WriteLine("Unity sound enabled successfully");
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
                    Console.WriteLine();
                    Console.WriteLine("Trying to disable Unity sound...");

                    AudioFixer.ChangeDisableUnityAudio(globalgamemanagers, true, Executable.game);

                    Console.WriteLine("Unity sound disabled successfully");
                    Environment.Exit(0);
                }

                // Remove backup file if it exists
                string backupFilePath = Path.Combine(managedDirectory, "Assembly-CSharp.qoriginal.dll");
                if (File.Exists(backupFilePath))
                    File.Delete(backupFilePath);

                AssemblyDefinition game = AssemblyDefinition.ReadAssembly(mainFilename);

                TypeDefinition gameInputDef = game.MainModule.GetType("GameInput");
                MethodDefinition awakeMethod = gameInputDef.Methods.First(x => x.Name == "Awake");

                Instruction patchMethodCall = null;

                foreach (Instruction instruction in awakeMethod.Body.Instructions)
                {
                    if (instruction.OpCode == OpCodes.Call && instruction.Operand.ToString().Equals("System.Void QModInstaller.QModPatcher::Patch()"))
                    {
                        patchMethodCall = instruction;
                    }
                }

                if (patchMethodCall != null)
                {
                    awakeMethod.Body.GetILProcessor().Remove(patchMethodCall);
                }
                else
                {
                    Console.WriteLine("An unexpected error has occurred.");
                    Console.WriteLine("The patch method couldn't be found. Was it even injected?");
                    Console.WriteLine();
                    Console.WriteLine("Press any key to exit");
                    Console.ReadKey();
                    Environment.Exit(3);
                }

                game.Write(mainFilename);

                Console.WriteLine("QModManager was uninstalled successfully");
                Console.WriteLine();
                Console.WriteLine("Trying to disable Unity sound...");

                AudioFixer.ChangeDisableUnityAudio(globalgamemanagers, true, Executable.game);

                Console.WriteLine("Unity sound disabled successfully");
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
            try
            {
                AssemblyDefinition game = AssemblyDefinition.ReadAssembly(mainFilename);

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
