using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using CommandLine;
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
					Db4oRecordListManager ds = recordListManager as Db4oRecordListManager;
					if (ds != null)
					{
						backupService = new BackupService(project.PathToLocalBackup, ds.DataSource);
						ds.DataCommitted += new EventHandler(backupService.OnDataCommitted);
					}

					//builder = new SampleTaskBuilder(project, tabbedForm, recordListManager);
					using (FileStream config = new FileStream(project.PathToProjectTaskInventory, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
//MONO bug as of 1.1.18 cannot bitwise or FileShare on FileStream constructor
//                    using (FileStream config = new FileStream(project.PathToProjectTaskInventory, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
					{
						builder = new ConfigFileTaskBuilder(config, project, tabbedForm, recordListManager);
					}

					project.Tasks = builder.Tasks;
					Application.DoEvents();
					backupService.DoIncrementalXmlBackupNow(); //in case we are far behind
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
				IRecordList<LexEntry> masterRecordList = recordListManager.Get<LexEntry>();
				foreach (LexEntry entry in entries)
				{
					masterRecordList.Add(entry);
				}
			}
			else
			{
				// this configuration stuff should be moved down to a Db4oConfigurationClass in the LexicalModel
				// that can be run
				// I haven't done it yet because I'm still not entirely clear how it should get created.
				Db4objects.Db4o.Config.IConfiguration db4oConfiguration = Db4objects.Db4o.Db4oFactory.Configure();
				Db4objects.Db4o.Config.IObjectClass objectClass = db4oConfiguration.ObjectClass(typeof(Language.LanguageForm));
				objectClass.ObjectField("_writingSystemId").Indexed(true);
				objectClass.ObjectField("_form").Indexed(true);

				objectClass = db4oConfiguration.ObjectClass(typeof(LexEntry));
				objectClass.ObjectField("_modifiedDate").Indexed(true);
				objectClass.ObjectField("_lexicalForm").Indexed(true);
				objectClass.ObjectField("_sences").Indexed(true);
				objectClass.CascadeOnDelete(true);

				objectClass = db4oConfiguration.ObjectClass(typeof(LexSense));
				objectClass.ObjectField("_gloss").Indexed(true);
				objectClass.ObjectField("_exampleSentences").Indexed(true);
				objectClass.CascadeOnDelete(true);

				objectClass = db4oConfiguration.ObjectClass(typeof(LexExampleSentence));
				objectClass.ObjectField("_sentence").Indexed(true);
				objectClass.ObjectField("_translation").Indexed(true);
				objectClass.CascadeOnDelete(true);

				objectClass = db4oConfiguration.ObjectClass(typeof(MultiText));
				objectClass.ObjectField("_forms").Indexed(true);
				objectClass.CascadeOnDelete(true);

				objectClass = db4oConfiguration.ObjectClass(typeof(LanguageForm));
				objectClass.ObjectField("_writingSystemId").Indexed(true);
				objectClass.ObjectField("_form").Indexed(true);
				objectClass.CascadeOnDelete(true);

				recordListManager = new Db4oRecordListManager(project.PathToLexicalModelDB);
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
