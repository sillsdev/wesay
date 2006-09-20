#define SampleBuilder
using System;
using System.Collections;
using System.Windows.Forms;
using CommandLine;
using WeSay.UI;
using System.IO;
namespace WeSay.App
{
	class WeSayApp
	{

		class CommandLineArguments
		{
			[DefaultArgument(ArgumentType.AtMostOnce,
				DefaultValue=@"..\..\SampleProjects\Thai",
				HelpText="Path to the folder containing the project.")]
			public string projectPath=null;

			[Argument(ArgumentType.AtMostOnce,
				HelpText="Language to show the user interface in.",
				LongName="ui",
				ShortName="")]
			public string ui=null;
		}

		static void ShowCommandLineError(string e)
		{
			CommandLine.Parser p = new Parser(typeof(CommandLineArguments), new ErrorReporter(ShowCommandLineError));
			e += "\r\n\r\n" + p.GetUsageString(200);
			MessageBox.Show(e,"WeSay Command Line Problem");
		}

		[STAThread]
		static void Main(string[] args)
		{

			CommandLineArguments cmdArgs = new CommandLineArguments();
			if (!CommandLine.Parser.ParseArguments(args, cmdArgs, new ErrorReporter(ShowCommandLineError)))
			{
				return;
			}

#if GTK
			GLib.Thread.Init();
			Gdk.Threads.Init();
			Application.Init();
#endif
			WeSayWordsProject project = new WeSayWordsProject();
			project.Load(cmdArgs.projectPath);
			project.StringCatalogSelector = cmdArgs.ui;
#if GTK
			Gdk.Threads.Enter();
#endif
			WeSay.UI.ITaskBuilder builder = null;
			try
			{

#if GTK
				WeSay.UI.ISkin shell = new TabbedSkin(project, builder);
				Application.Run();
#else
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
#if SampleBuilder
				builder = new SampleTaskBuilder(project);
#else
				using (FileStream config = new FileStream(project.PathToTaskConfig, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
				{
					builder = new ConfigFileTaskBuilder(project, config);
				}
#endif
				Form f =  new TabbedForm(project, builder);
				Application.Run(f);
#endif

			}
			finally
			{
#if GTK
				Gdk.Threads.Leave();
#endif
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
