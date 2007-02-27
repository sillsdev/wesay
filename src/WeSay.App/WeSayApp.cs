using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Reflection;

using CommandLine;
using Reporting;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.LexicalModel.Tests;
using WeSay.Project;
namespace WeSay.App
{

	internal class ThreadExceptionHandler
	{
		///
		/// Handles the thread exception.
		///
		public void Application_ThreadException(
			object sender, ThreadExceptionEventArgs e)
		{
			MessageBox.Show("caught");
		}
	}
	class WeSayApp
	{
		private static Mutex _oneInstancePerProjectMutex;



		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			//problems with user.config: http://blogs.msdn.com/rprabhu/articles/433979.aspx

			OsCheck();
			Reporting.Logger.Init();
		   SetupErrorHandling();

		   //bring in settings from any previous version
		   if (Properties.Settings.Default.NeedUpgrade)
		   {
			   WeSay.App.Properties.Settings.Default.Upgrade();
			   Properties.Settings.Default.NeedUpgrade = false;
		   }

			UsageEmailDialog.IncrementLaunchCount();
			UsageEmailDialog.DoTrivialUsageReport("usage@wesay.org", "(This will not be asked of users in the released version.)", new int[] { 1,5,20,40,60,80,100 });

			CommandLineArguments cmdArgs = new CommandLineArguments();
			if (!CommandLine.Parser.ParseArguments(args, cmdArgs, new ReportError(ShowCommandLineError)))
			{
				return;
			}

			if (!GrabTokenForThisProject(cmdArgs))
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

			string path = cmdArgs.wordsPath;
			if (!TryToLoad(cmdArgs, path, project))
			{
				return;
			}


#if GTK
			Gdk.Threads.Enter();
#endif
			ITaskBuilder builder = null;
			try
			{
				TabbedForm tabbedForm = new TabbedForm();

				BackupService backupService=null;

				using (IRecordListManager recordListManager = MakeRecordListManager(project))
				{
					tabbedForm.Show(); // so the user sees that we did launch
					tabbedForm.Text = "WeSay: " + project.Name;
					Application.DoEvents();

					Db4oRecordListManager ds = recordListManager as Db4oRecordListManager;

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

					//run the ui
					Application.Run(tabbedForm);

					//do a last backup before exiting
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

			Reporting.Logger.WriteEvent("App Exiting Normally.");
			Reporting.Logger.ShutDown();
			WeSay.App.Properties.Settings.Default.Save();
			ReleaseMutexForThisProject();
		}

		private static bool TryToLoad(CommandLineArguments cmdArgs, string path, WeSayWordsProject project)
		{
			if (path == null)
			{
				if (WeSay.App.Properties.Settings.Default.PreviousDBPath != null
					&& WeSay.App.Properties.Settings.Default.PreviousDBPath != string.Empty)
				{
					path = WeSay.App.Properties.Settings.Default.PreviousDBPath;
				}
				else
				{
					MessageBox.Show(
						String.Format(
							"WeSay was unable to figure out what lexicon to work on. It will use an argument from a shortcut if given one, otherwise it tries to find the lexicon that it was last used with.  In this, case, it found neither.  You can use the WeSay Setup program to reset the location of the project.",
							cmdArgs.wordsPath), "WeSay error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return false;
				}
			}

			if (!File.Exists(path))
			{
				MessageBox.Show(String.Format("WeSay tried to find the lexicon at '{0}', but could not find it.\r\n\r\nOn Windows, the location argument can be specified like this:\r\n wesay.exe \"c:\\some directory\\mylanguage\\wesay\\mylanguage.words\". \r\n\r\nSince WeSay intentionally does not ask users to ever deal with the file system of their computer, you need to setup a shortcut to WeSay which gives the correct path, or use the WeSay Admin program to launch WeSay once. After that, WeSay will remember where it is.",
					path), "WeSay error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}

			project.LoadFromLexiconPath(path);
			WeSay.App.Properties.Settings.Default.PreviousDBPath = path;
			//WeSay.App.Properties.Settings.Default.Save();
			//MessageBox.Show("saved " + WeSay.App.Properties.Settings.Default.PreviousDBPath);
			//WeSay.App.Properties.Settings.Default.Reload();
			//Debug.Assert(WeSay.App.Properties.Settings.Default.PreviousDBPath == path);
			return true;
		}

		private static void OsCheck()
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
		}

		private static void ReleaseMutexForThisProject()
		{
			if (_oneInstancePerProjectMutex != null)
			{
				_oneInstancePerProjectMutex.ReleaseMutex();
			}
		}

		private static bool GrabTokenForThisProject(CommandLineArguments cmdArgs)
		{
			string mutexId = cmdArgs.wordsPath;
			if (mutexId != null)
			{
				 bool mutexCreated;
				mutexId = mutexId.Replace(Path.DirectorySeparatorChar, '-');
				mutexId = mutexId.Replace(Path.VolumeSeparatorChar, '-');
				_oneInstancePerProjectMutex = new Mutex(true, mutexId, out mutexCreated);
				if (!mutexCreated) // can I acquire?
				{
					//    Process[] processes = Process.GetProcessesByName("WeSay.App");
					//    foreach (Process process in processes)
					//    {

					//        // we should make window title include the database name.
					//        if(process.MainWindowTitle == "WeSay: " + project.Name)
					//        {
					//            process.WaitForInputIdle(4000); // wait four seconds at most
					//            //process.MainWindowHandle;
					//            break;
					//        }
					//    }

					MessageBox.Show("WeSay is already open with " + cmdArgs.wordsPath + ".");
					return false;
				}
			 }
			 return true;
		}


		private static void SetupErrorHandling()
		{
			ErrorReporter.EmailAddress = "issues@wesay.org";
			if (BasilProject.IsInitialized)
			{
				ErrorReporter.AddProperty("ProjectPath", BasilProject.Project.ProjectDirectoryPath);
			}
			ErrorReporter.AddStandardProperties();
			ExceptionHandler.Init();
		}

		private static void HandleTopLevelError(object sender, ThreadExceptionEventArgs e)
		{
			throw new NotImplementedException();
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
			   // DefaultValue = @"..\..\SampleProjects\Thai\WeSay\thai5000.words",
				HelpText = "Path to the words file (e.g. on windows, \"c:\\thai\\wesay\\thai.words\").")]
			public string wordsPath = null;

			[Argument(ArgumentTypes.AtMostOnce,
				HelpText = "Language to show the user interface in.",
				LongName = "ui",
				ShortName = "")]
			public string ui = null;
		}

		static void ShowCommandLineError(string e)
		{
			CommandLine.Parser p = new Parser(typeof(CommandLineArguments), new ReportError(ShowCommandLineError));
			e = e.Replace("Duplicate 'wordsPath' argument", "Please enclose project path in quotes if it contains spaces.");
			e += "\r\n\r\n" + p.GetUsageString(200);
			MessageBox.Show(e, "WeSay Command Line Problem");
		}
	}

}
