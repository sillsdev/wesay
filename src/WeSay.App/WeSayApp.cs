using System;
using Gtk;

namespace WeSay.App
{
	class WeSayApp
	{

		[STAThread]
		static void Main()
		{
			Application.Init();
			BasilProject project = new BasilProject(@"..\..\SampleProjects\Thai");
			WeSay.UI.IAppShell shell = new TabAppShell(project, new SampleTaskBuilder());
			Application.Run();
		}
	}
}