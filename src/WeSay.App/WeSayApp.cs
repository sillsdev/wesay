using System;
using System.ComponentModel;
using System.Windows.Forms;
using CommandLine;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Tests;
using WeSay.UI;
using System.IO;
namespace WeSay.App
{
	class WeSayApp
	{
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
			project.StringCatalogSelector = cmdArgs.ui;
			project.LoadFromLexiconPath(cmdArgs.wordsPath);
#if GTK
			Gdk.Threads.Enter();
#endif
			WeSay.UI.ITaskBuilder builder = null;
			try
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				TabbedForm tabbedForm = new TabbedForm();

				BackupService backupService=null;

				//builder = new SampleTaskBuilder(project, tabbedForm);
				using (FileStream config = new FileStream(project.PathToProjectTaskInventory, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
				{
					AbstractRecordListManager recordListManager =MakeRecordListManager(project);
					builder = new ConfigFileTaskBuilder(config, project, tabbedForm, recordListManager);

					Db4oRecordListManager ds = recordListManager as Db4oRecordListManager;
					if(ds!=null)
					{
						backupService = new BackupService(project.PathToLocalBackup, ds.DataSource);
						recordListManager.DataCommitted += new EventHandler(backupService.OnDataCommitted);
					}
				}

				project.Tasks = builder.Tasks;
				Application.DoEvents();
				backupService.DoIncrementalXmlBackupNow(); //in case we are far behind
				Application.Run(tabbedForm);
				backupService.DoIncrementalXmlBackupNow();
				backupService.BackupToExternal("f:\\"+project.Name+".zip");
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



		private static AbstractRecordListManager MakeRecordListManager(WeSayWordsProject project)
		{
			AbstractRecordListManager recordListManager;

			if (project.PathToWeSaySpecificFilesDirectory.IndexOf("PRETEND") > -1)
			{
				IBindingList entries = new PretendRecordList();
				recordListManager = new InMemoryRecordListManager();
				IRecordList<LexEntry> masterRecordList = recordListManager.Get<LexEntry>();
				foreach (LexEntry entry in entries)
				{
					masterRecordList.Add(entry);
				}
			}
			else
			{
				com.db4o.config.Configuration db4oConfiguration = com.db4o.Db4o.Configure();
				com.db4o.config.ObjectClass objectClass = db4oConfiguration.ObjectClass(typeof(Language.LanguageForm));
				objectClass.ObjectField("_writingSystemId").Indexed(true);
				objectClass.ObjectField("_form").Indexed(true);

				objectClass = db4oConfiguration.ObjectClass(typeof(LexEntry));
				objectClass.ObjectField("_modifiedDate").Indexed(true);

				recordListManager = new Db4oRecordListManager(project.PathToLexicalModelDB);
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
