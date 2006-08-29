using System;
using System.Collections;
using System.Windows.Forms;

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

			BasilProject project = new BasilProject(FilePath);

			WeSay.UI.ITaskBuilder builder = new SampleTaskBuilder(project);


			try
			{
				 Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
			  // WeSay.UI.ISkin shell = new TabbedSkin(project, builder);
				Form f =  new TabbedForm(project, builder);
				Application.Run(f);


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
