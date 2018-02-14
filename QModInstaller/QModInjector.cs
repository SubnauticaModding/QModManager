using Mono.Cecil;
using Mono.Cecil.Cil;
using System.IO;
using System.Linq;

namespace QModInstaller
{
    public class QModInjector
    {
        private string subnauticaDirectory;
        private string managedDirectory;
        private string installerFilename = @"QModInstaller.dll";
        private string mainFilename = @"\Assembly-CSharp.dll";
        private string backupFilename = @"\Assembly-CSharp.qoriginal.dll";


        public QModInjector(string dir)
        {
            subnauticaDirectory = dir;
            managedDirectory = Path.Combine(subnauticaDirectory, @"Subnautica_Data\Managed");
            mainFilename = managedDirectory + mainFilename;
            backupFilename = managedDirectory + backupFilename;
        }


        public bool IsPatcherInjected()
        {
            return isInjected();
        }


        public bool Inject()
        {
            if (isInjected()) return false;

            // read dll
            var game = AssemblyDefinition.ReadAssembly(mainFilename);

            // delete old backups
            if (File.Exists(backupFilename))
                File.Delete(backupFilename);

            // save a copy of the dll as a backup
            game.Write(backupFilename);

            // load patcher module
            var installer = AssemblyDefinition.ReadAssembly(installerFilename);
            var patchMethod = installer.MainModule.GetType("QModInstaller.QModPatcher").Methods.Single(x => x.Name == "Patch");

            // target the injection method
            var type = game.MainModule.GetType("GameInput");
            var method = type.Methods.First(x => x.Name == "Awake");

            // inject
            method.Body.GetILProcessor().InsertBefore(method.Body.Instructions[0], Instruction.Create(OpCodes.Call, method.Module.Import(patchMethod)));

            // save changes under original filename
            game.Write(mainFilename);

            if (!Directory.Exists(subnauticaDirectory + @"\QMods"))
                Directory.CreateDirectory(subnauticaDirectory + @"\QMods");

            return true;
        }


        public bool Remove()
        {
            // if a backup exists
            if (File.Exists(backupFilename))
            {
                // remove the dirty dll
                File.Delete(mainFilename);

                // move the backup into its place
                File.Move(backupFilename, mainFilename);

                return true;
            }

            return false;
        }


        private bool isInjected()
        {
            var game = AssemblyDefinition.ReadAssembly(mainFilename);

            var type = game.MainModule.GetType("GameInput");
            var method = type.Methods.First(x => x.Name == "Awake");

            var installer = AssemblyDefinition.ReadAssembly(installerFilename);
            var patchMethod = installer.MainModule.GetType("QModInstaller.QModPatcher").Methods.FirstOrDefault(x => x.Name == "Patch");

            bool patched = false;

            foreach (var instruction in method.Body.Instructions)
            {
                if (instruction.OpCode.Equals(OpCodes.Call) && instruction.Operand.ToString().Equals("System.Void QModInstaller.QModPatcher::Patch()"))
                {
                    return true;
                }
            }

            return patched;
        }
    }
}
