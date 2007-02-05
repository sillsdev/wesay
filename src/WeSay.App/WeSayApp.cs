using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using CommandLine;
using Db4objects.Db4o.Query;
using WeSay.Data;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.LexicalModel.Tests;
using WeSay.Project;
namespace WeSay.App
{
	class WeSayApp
	{
		[STAThread]
		static void Main(string[] args)
		{

#if DEBUG
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				Console.WriteLine("running on Unix");
			}
			else
			{
				Console.WriteLine("running on Windows");
			}
#endif

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
			project.StringCatalogSelector = cmdArgs.ui;
			project.LoadFromLexiconPath(cmdArgs.wordsPath);
#if GTK
			Gdk.Threads.Enter();
#endif
			ITaskBuilder builder = null;
			try
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				TabbedForm tabbedForm = new TabbedForm();

				BackupService backupService=null;

				using (IRecordListManager recordListManager = MakeRecordListManager(project))
				{
					tabbedForm.Show(); // so the user sees that we did launch
					Application.DoEvents();

					Db4oRecordListManager ds = recordListManager as Db4oRecordListManager;


//                    //*********************
					MessageBox.Show("test");
					IRecordList<LexEntry> l = ds.GetListOfType<LexEntry>();
					l.RemoveAt(3);
//                    //*********************


					if (ds != null)
					{
						backupService = new BackupService(project.PathToLocalBackup, ds.DataSource);
						ds.DataCommitted += new EventHandler(backupService.OnDataCommitted);
					}

					//MONO bug as of 1.1.18 cannot bitwise or FileShare on FileStream constructor
					//                    using (FileStream config = new FileStream(project.PathToProjectTaskInventory, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
					using (FileStream config = new FileStream(project.PathToProjectTaskInventory, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						builder = new ConfigFileTaskBuilder(config, project, tabbedForm, recordListManager);
					}

					project.Tasks = builder.Tasks;
					Application.DoEvents();

					//in case we are far behind, as in the case of a recent import or deleted backups
					backupService.DoIncrementalXmlBackupNow();
					tabbedForm.ContinueLaunchingAfterInitialDisplay();
					Application.Run(tabbedForm);
					backupService.DoIncrementalXmlBackupNow();
					backupService.BackupToExternal("h:\\" + project.Name + ".zip");
				}
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
				if (builder is IDisposable)
					((IDisposable)builder).Dispose();
			}
		}



		private static IRecordListManager MakeRecordListManager(WeSayWordsProject project)
		{
			IRecordListManager recordListManager;

			if (project.PathToWeSaySpecificFilesDirectoryInProject.IndexOf("PRETEND") > -1)
			{
				IBindingList entries = new PretendRecordList();
				recordListManager = new InMemoryRecordListManager();
				IRecordList<LexEntry> masterRecordList = recordListManager.GetListOfType<LexEntry>();
				foreach (LexEntry entry in entries)
				{
					masterRecordList.Add(entry);
				}
			}
			else
			{
				recordListManager = new Db4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), project.PathToLexicalModelDB);
				Db4oLexModelHelper.Initialize(((Db4oRecordListManager)recordListManager).DataSource.Data);

			}
			return recordListManager;
		}

		class CommandLineArguments
		{
			[DefaultArgument(ArgumentTypes.AtMostOnce,
				DefaultValue = @"..\..\SampleProjects\Thai\WeSay\thai5000.words",
				HelpText = @"Path to the words file (e.g. c:\thai\wesay\thai500.words).")]
			public string wordsPath = null;

			[Argument(ArgumentTypes.AtMostOnce,
				HelpText = "Language to show the user interface in.",
				LongName = "ui",
				ShortName = "")]
			public string ui = null;
		}

		static void ShowCommandLineError(string e)
		{
			CommandLine.Parser p = new Parser(typeof(CommandLineArguments), new ErrorReporter(ShowCommandLineError));
			e = e.Replace("Duplicate 'wordsPath' argument", "Please enclose project path in quotes if it contains spaces.");
			e += "\r\n\r\n" + p.GetUsageString(200);
			MessageBox.Show(e, "WeSay Command Line Problem");
		}
	}

}
