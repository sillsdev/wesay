using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using CommandLine;
using Reporting;
using WeSay.App.Properties;
using WeSay.Data;
using WeSay.Foundation.Progress;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.LexicalModel.Tests;
using WeSay.Project;

namespace WeSay.App
{
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
			Logger.Init();
		   SetupErrorHandling();

		   //bring in settings from any previous version
		   if (Settings.Default.NeedUpgrade)
		   {
			   Settings.Default.Upgrade();
			   Settings.Default.NeedUpgrade = false;
		   }

			UsageEmailDialog.IncrementLaunchCount();
			UsageEmailDialog.DoTrivialUsageReport("usage@wesay.org", "(This will not be asked of users in the released version.)", new int[] { 1,5,20,40,60,80,100 });

			CommandLineArguments cmdArgs = new CommandLineArguments();
			if (!Parser.ParseArguments(args, cmdArgs, new ReportError(ShowCommandLineError)))
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

			string path = cmdArgs.liftPath;
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

				LiftUpdateService liftUpdateService=null;

				using (IRecordListManager recordListManager = MakeRecordListManager(project))
				{
					tabbedForm.Show(); // so the user sees that we did launch
					tabbedForm.Text = "WeSay: " + project.Name;
					Application.DoEvents();

					Db4oRecordListManager ds = recordListManager as Db4oRecordListManager;

					if (ds != null)
					{
						liftUpdateService = new LiftUpdateService(ds.DataSource);
						ds.DataCommitted += new EventHandler(liftUpdateService.OnDataCommitted);
						ds.DataDeleted +=new EventHandler<DeletedItemEventArgs>(liftUpdateService.OnDataDeleted);
					}

					//MONO bug as of 1.1.18 cannot bitwise or FileShare on FileStream constructor
					//                    using (FileStream config = new FileStream(project.PathToProjectTaskInventory, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
					using (FileStream configFile = new FileStream(project.PathToProjectTaskInventory, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						builder = new ConfigFileTaskBuilder(configFile, project,
							tabbedForm as ICurrentWorkTask, recordListManager);
					}

					project.Tasks = builder.Tasks;
					Application.DoEvents();

					//in case we are far behind, as in the case of a recent import or deleted backups
					if (liftUpdateService!=null)
					{
						liftUpdateService.DoLiftUpdateNow(false);
					}
					tabbedForm.ContinueLaunchingAfterInitialDisplay();

					//run the ui
					Application.Run(tabbedForm);

					//do a last backup before exiting
					liftUpdateService.DoLiftUpdateNow(true);
					BackupService.BackupToExternal("h:\\" + project.Name + ".zip");
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

			Logger.WriteEvent("App Exiting Normally.");
			Logger.ShutDown();
			Settings.Default.Save();
			ReleaseMutexForThisProject();
		}

		private static bool TryToLoad(CommandLineArguments cmdArgs, string liftPath, WeSayWordsProject project)
		{
			if (liftPath == null)
			{
				if (!String.IsNullOrEmpty(Settings.Default.PreviousLiftPath))
				{
					liftPath = Settings.Default.PreviousLiftPath;
				}
				else
				{
					ErrorReporter.ReportNonFatalMessage("WeSay was unable to figure out what lexicon to work on. It will use an argument from a shortcut if given one, otherwise it tries to find the lexicon that it was last used with.  In this, case, it found neither.  You can use the WeSay Setup program to reset the location of the lift file of the project.");
					return false;
				}
			}

			if (!File.Exists(liftPath))
			{
				ErrorReporter.ReportNonFatalMessage(
					String.Format(
						"WeSay tried to find the lexicon at '{0}', but could not find it.\r\n\r\nOn Windows, the location argument can be specified like this:\r\n wesay.exe \"c:\\some directory\\mylanguage\\wesay\\mylanguage.lift\". \r\n\r\nSince WeSay intentionally does not ask users to ever deal with the file system of their computer, you need to setup a shortcut to WeSay which gives the correct path, or use the WeSay Setup program to launch WeSay once. After that, WeSay will remember where it is.",
						liftPath));
				return false;
			}

			string name =Path.GetFileNameWithoutExtension(liftPath);
			string parentName = Directory.GetParent(liftPath).Parent.Name;
			if (!(Environment.OSVersion.Platform == PlatformID.Unix))
			{
				name = name.ToLower();
				parentName = parentName.ToLower();
			}

			if (!File.Exists(liftPath))
			{
				ErrorReporter.ReportNonFatalMessage(
					String.Format(
						"WeSay tried to find the lexicon at '{0}', but could not find it.\r\n\r\nOn Windows, the location argument can be specified like this:\r\n wesay.exe \"c:\\some directory\\mylanguage\\wesay\\mylanguage.lift\". \r\n\r\nSince WeSay intentionally does not ask users to ever deal with the file system of their computer, you need to setup a shortcut to WeSay which gives the correct path, or use the WeSay Setup program to launch WeSay once. After that, WeSay will remember where it is.",
						liftPath));
				return false;
			}

			if (!BringCachesUpToDate(liftPath, project))
			{
				return false;
			}
			if (project.LoadFromLiftLexiconPath(liftPath))
			{
				Settings.Default.PreviousLiftPath = liftPath;
				return true;
			}
			else
			{
				return false;
			}
		}

		private static bool BringCachesUpToDate(string liftPath, WeSayWordsProject project)
		{
			CacheBuilder builder = project.GetCacheBuilderIfNeeded(liftPath);
			if (builder == null)
			{
				return true;
			}

			ProgressState progressState = new WeSay.Foundation.ConsoleProgress();//new ProgressState(progressDialogHandler);
			using (WeSay.UI.ProgressDialog dlg = new WeSay.UI.ProgressDialog())
			{
				if (!PreprocessLift())
				{
					return false;
				}

				dlg.Overview = "Please wait while WeSay updates its caches to match the new or modified LIFT file.";
				BackgroundWorker cacheBuildingWork = new BackgroundWorker();
				cacheBuildingWork.DoWork += new DoWorkEventHandler(builder.OnDoWork);
				dlg.BackgroundWorker = cacheBuildingWork;
				dlg.CanCancel = true;
				dlg.ShowDialog();
				if (dlg.DialogResult != DialogResult.OK)
				{
					return false;
				}
				LiftUpdateService.LiftIsFreshNow();
			}
			return true;
		}

		private static bool PreprocessLift()
		{
			using (WeSay.UI.ProgressDialog dlg = new WeSay.UI.ProgressDialog())
			{
				dlg.Overview = "Please wait while WeSay preprocesses your LIFT file.";
				BackgroundWorker preprocessWorker = new BackgroundWorker();
				preprocessWorker.DoWork += new DoWorkEventHandler(OnDoPreprocessLiftWork);
				dlg.BackgroundWorker = preprocessWorker;
				dlg.CanCancel = true;
				dlg.ShowDialog();
				return (dlg.DialogResult == DialogResult.OK);
			}
		}

		static void OnDoPreprocessLiftWork(object sender, DoWorkEventArgs e)
		{
			//((BackgroundWorker)sender).ReportProgress(
			((BackgroundWorkerState) e.Argument).StatusLabel = "Preprocessing...";
			string lift = Project.WeSayWordsProject.Project.PathToLiftFile;
			string output = LiftIO.Utilities.ProcessLiftForLaterMerging(lift);
			MoveTempOverRealAndBackup(lift, output);
		}


		private static void MoveTempOverRealAndBackup(string existingPath, string newFilePath)
		{
			string backupName = existingPath + ".old";

			int i = 0;
			while (File.Exists(backupName + i))
			{
				i++;
			}
			backupName += i;

			File.Move(existingPath, backupName);

			File.Copy(newFilePath, existingPath);
			File.Delete(newFilePath);
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
			string mutexId = cmdArgs.liftPath;
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

					MessageBox.Show("WeSay is already open with " + cmdArgs.liftPath + ".");
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
				recordListManager = new Db4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), project.PathToDb4oLexicalModelDB);
				Db4oLexModelHelper.Initialize(((Db4oRecordListManager)recordListManager).DataSource.Data);
			}
			return recordListManager;
		}

		class CommandLineArguments
		{
			[DefaultArgument(ArgumentTypes.AtMostOnce,
			   // DefaultValue = @"..\..\SampleProjects\Thai\WeSay\thai5000.words",
				HelpText = "Path to the Lift Xml file (e.g. on windows, \"c:\\thai\\wesay\\thai.lift\").")]
			public string liftPath = null;

			[Argument(ArgumentTypes.AtMostOnce,
				HelpText = "Language to show the user interface in.",
				LongName = "ui",
				ShortName = "")]
			public string ui = null;
		}

		static void ShowCommandLineError(string e)
		{
			Parser p = new Parser(typeof(CommandLineArguments), new ReportError(ShowCommandLineError));
			e = e.Replace("Duplicate 'wordsPath' argument", "Please enclose project path in quotes if it contains spaces.");
			e += "\r\n\r\n" + p.GetUsageString(200);
			MessageBox.Show(e, "WeSay Command Line Problem");
		}
	}

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

}
