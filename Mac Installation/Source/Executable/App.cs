using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;

namespace QModManager
{
	public class App : Application
	{
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void InitializeComponent()
		{
			base.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020D4 File Offset: 0x000002D4
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[STAThread]
		[DebuggerNonUserCode]
		public static void Main()
		{
			App app = new App();
			app.InitializeComponent();
			app.Run();
		}
	}
}
