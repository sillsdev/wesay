using System;
using System.Collections;
using Gtk;



namespace WeSay.App
{
	class WeSayApp
	{

		[STAThread]
		static void Main(string[] args)
		{
			string FilePath = @"..\..\SampleProjects\Thai";
			if (args.Length > 0)
			{
				FilePath = args[0];
			}
			Application.Init();
			BasilProject project = new BasilProject(FilePath);

			WeSay.UI.ITaskBuilder builder = new SampleTaskBuilder(project);


			try
			{
				WeSay.UI.ISkin shell = new TabbedSkin(project, builder);

				Application.Run();

			}
			finally
			{
				//TODO(JH): having a builder than needs to be kept around so it can be disposed of is all wrong.
				//either I want to change it to something like TaskList rather than ITaskBuilder, or
				//it needs to create some disposable object other than a IList<>.
				//The reason we need to be able to dispose of it is because we need some way to
				//dispose of things that it might create, such as a data source.
				if (builder as IDisposable != null)
					((IDisposable)builder).Dispose();
			}
		}


	}
}