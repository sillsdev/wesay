using System;
using System.Collections;
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

			WeSay.UI.ITaskBuilder builder = new SampleTaskBuilder(project);
			WeSay.UI.ISkin shell= new TabbedSkin(project, builder);

			Application.Run();

			//TODO(JH): having a builder than needs to be kept around so it can be disposed of is all wrong.
			//either I want to change it to something like TaskList rather than ITaskBuilder, or
			//it needs to create some disposable object other than a IList<>.
			//The reason we need to be able to dispose of it is because we need some way to
			//dispose of things that it might create, such as a data source.
			if(builder as IDisposable != null)
				((IDisposable) builder).Dispose();
		}


	}
}