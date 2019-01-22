using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace QModInstaller
{
	public class QModInjector
	{

        private string subnauticaDirectory;
        private string managedDirectory;

        private string installerFilename = "QModInstaller.dll";
        private string mainFilename = "/Assembly-CSharp.dll";
        private string backupFilename = "/Assembly-CSharp.qoriginal.dll";

        public QModInjector(string dir, string managedDir = null)
		{
			this.subnauticaDirectory = dir;
			bool flag = managedDir == null;
			if (flag)
			{
				this.managedDirectory = Path.Combine(this.subnauticaDirectory, "Subnautica_Data/Managed");
			}
			else
			{
				this.managedDirectory = managedDir;
			}
			this.mainFilename = this.managedDirectory + this.mainFilename;
			this.backupFilename = this.managedDirectory + this.backupFilename;
		}

		public bool IsPatcherInjected()
		{
			return this.isInjected();
		}

		public bool Inject()
		{
			bool flag = this.isInjected();
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(this.mainFilename);
				bool flag2 = File.Exists(this.backupFilename);
				if (flag2)
				{
					File.Delete(this.backupFilename);
				}
				assemblyDefinition.Write(this.backupFilename);
				AssemblyDefinition assemblyDefinition2 = AssemblyDefinition.ReadAssembly(this.installerFilename);
				MethodDefinition method = assemblyDefinition2.MainModule.GetType("QModInstaller.QModPatcher").Methods.Single((MethodDefinition x) => x.Name == "Patch");
				TypeDefinition type = assemblyDefinition.MainModule.GetType("GameInput");
				MethodDefinition methodDefinition = type.Methods.First((MethodDefinition x) => x.Name == "Awake");
				methodDefinition.Body.GetILProcessor().InsertBefore(methodDefinition.Body.Instructions[0], Instruction.Create(OpCodes.Call, methodDefinition.Module.Import(method)));
				assemblyDefinition.Write(this.mainFilename);
				bool flag3 = !Directory.Exists(this.subnauticaDirectory + "\\QMods");
				if (flag3)
				{
					Directory.CreateDirectory(this.subnauticaDirectory + "\\QMods");
				}
				result = true;
			}
			return result;
		}

		public bool Remove()
		{
			bool flag = File.Exists(this.backupFilename);
			bool result;
			if (flag)
			{
				File.Delete(this.mainFilename);
				File.Move(this.backupFilename, this.mainFilename);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private bool isInjected()
		{
			AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(this.mainFilename);
			TypeDefinition type = assemblyDefinition.MainModule.GetType("GameInput");
			MethodDefinition methodDefinition = type.Methods.First((MethodDefinition x) => x.Name == "Awake");
			AssemblyDefinition assemblyDefinition2 = AssemblyDefinition.ReadAssembly(this.installerFilename);
			MethodDefinition methodDefinition2 = assemblyDefinition2.MainModule.GetType("QModInstaller.QModPatcher").Methods.FirstOrDefault((MethodDefinition x) => x.Name == "Patch");
			bool result = false;
			foreach (Instruction instruction in methodDefinition.Body.Instructions)
			{
				bool flag = instruction.OpCode.Equals(OpCodes.Call) && instruction.Operand.ToString().Equals("System.Void QModInstaller.QModPatcher::Patch()");
				if (flag)
				{
					return true;
				}
			}
			return result;
		}
	}
}
