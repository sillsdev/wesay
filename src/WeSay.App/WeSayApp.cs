using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using CommandLine;
using Reporting;
using WeSay.App;
using WeSay.App.Properties;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Progress;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.LexicalModel.Tests;
using WeSay.Project;
using WeSay.UI;

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

			Reporter.RecordLaunch();
			Reporter.DoTrivialUsageReport("usage@wesay.org", "(This will not be asked of users in the released version.)", new int[] { 1,5,20,40,60,80,100 });

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
			//project.StringCatalogSelector = cmdArgs.ui;

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


				using (IRecordListManager recordListManager = MakeRecordListManager(project))
				{
					tabbedForm.Show(); // so the user sees that we did launch
					tabbedForm.Text = "WeSay: " + project.Name + "        "  + ErrorReporter.UserFriendlyVersionString;
					Application.DoEvents();

					project.LiftUpdateService = SetupUpdateService(recordListManager);
					project.LiftUpdateService.DoLiftUpdateNow(true);

					//MONO bug as of 1.1.18 cannot bitwise or FileShare on FileStream constructor
					//                    using (FileStream config = new FileStream(project.PathToProjectTaskInventory, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
					using (FileStream configFile = new FileStream(project.PathToConfigFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						builder = new ConfigFileTaskBuilder(configFile, project,
							tabbedForm as ICurrentWorkTask, recordListManager);
					}
					project.Tasks = builder.Tasks;
					Application.DoEvents();

					tabbedForm.ContinueLaunchingAfterInitialDisplay();

					//run the ui
					Application.Run(tabbedForm);

					//do a last backup before exiting
					project.LiftUpdateService.DoLiftUpdateNow(true);
			  //      BackupMaker.BackupToExternal(_filesToBackup,project.ProjectDirectoryPath, "h:\\" + project.Name + ".zip");
				}
				Logger.WriteEvent("App Exiting Normally.");
			}
			catch (IOException e)
			{
				ErrorReporter.ReportNonFatalMessage(e.Message);
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

			Logger.ShutDown();
			Settings.Default.Save();
			ReleaseMutexForThisProject();
		}

		private static LiftUpdateService SetupUpdateService(IRecordListManager recordListManager)
		{
			LiftUpdateService liftUpdateService;
			Db4oRecordListManager ds = (Db4oRecordListManager)    recordListManager;
			liftUpdateService = new LiftUpdateService(ds.DataSource);
			ds.DataCommitted += new EventHandler(liftUpdateService.OnDataCommitted);
			ds.DataDeleted +=new EventHandler<DeletedItemEventArgs>(liftUpdateService.OnDataDeleted);
			return liftUpdateService;
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
			  ErrorReporter.ReportNonFatalMessage("WeSay was unable to figure out what lexicon to work on. Try opening the LIFT file by double clicking on it. If you don't have one yet, run the WeSay Configuration Tool to make a new WeSay proejct.");
			  return false;
			}
		  }

		  if (project.LoadFromLiftLexiconPath(liftPath))
		  {
			Settings.Default.PreviousLiftPath = liftPath;
		  }
		  else
		  {
			return false;
		  }

			WeSayWordsProject.Project.LockLift(); // Consume will expect it to be locked already

			//NB: it's very important that any updates are consumed before the cache is rebuilt.
			//Otherwise, the cache and lift will fall out of sync.
			LiftUpdateService.ConsumePendingLiftUpdates();

			if (!BringCachesUpToDate(liftPath, project))
			{
				return false;
			}
			if (CacheManager.AssumeCacheIsFresh(WeSayWordsProject.Project.PathToCache))
			{
				//prevent the update service from thinking the LIFT file is really old
				//compared to the cache, due to the installer messing with the dates.
				LiftUpdateService.LiftIsFreshNow();
			}
			//whether or not we're out of date, remove this indicator file, which is only to get
			//fresh-from-install launchign without an installer-induced
			//false dirty cache signal
			CacheManager.RemoveAssumeCacheIsFreshIndicator();

		  return true;
		}

		private static bool BringCachesUpToDate(string liftPath, WeSayWordsProject project)
		{
			project.PathToLiftFile = liftPath;
			CacheBuilder builder = CacheManager.GetCacheBuilderIfNeeded(project);

			if (builder == null)
			{
				return true;
			}

				//ProgressState progressState = new WeSay.Foundation.ConsoleProgress();//new ProgressState(progressDialogHandler);
				using (ProgressDialog dlg = new ProgressDialog())
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
						if (dlg.ProgressStateResult.ExceptionThatWasEncountered !=null)
						{
							ErrorReporter.ReportException(dlg.ProgressStateResult.ExceptionThatWasEncountered, null, false);
						}
						else if (dlg.ProgressStateResult.State == ProgressState.StateValue.StoppedWithError)
						{
							ErrorReporter.ReportNonFatalMessage("Could not build caches. " + dlg.ProgressStateResult.LogString, null, false);
						}
						return false;
					}
					LiftUpdateService.LiftIsFreshNow();
				}
			return true;
			}

		private static bool PreprocessLift()
			{
			using (ProgressDialog dlg = new ProgressDialog())
			{
				dlg.Overview = "Please wait while WeSay preprocesses your LIFT file.";
				BackgroundWorker preprocessWorker = new BackgroundWorker();
				preprocessWorker.DoWork += new DoWorkEventHandler(OnDoPreprocessLiftWork);
				dlg.BackgroundWorker = preprocessWorker;
				dlg.CanCancel = true;
				dlg.ShowDialog();
				if (dlg.ProgressStateResult.ExceptionThatWasEncountered != null)
				{
					ErrorReporter.ReportNonFatalMessage(
						String.Format("WeSay encountered an error while preprocessing the file '{0}'.  Error was: {1}",
						WeSayWordsProject.Project.PathToLiftFile,
						dlg.ProgressStateResult.ExceptionThatWasEncountered.Message));
				}
				return (dlg.DialogResult == DialogResult.OK);
			}
		}

		static void OnDoPreprocessLiftWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorkerState state = (BackgroundWorkerState) e.Argument;
			state.StatusLabel = "Preprocessing...";
			try
			{
				WeSayWordsProject.Project.ReleaseLockOnLift();
				string lift = WeSayWordsProject.Project.PathToLiftFile;
				string output = LiftIO.Utilities.ProcessLiftForLaterMerging(lift);
				MoveTempOverRealAndBackup(lift, output);
			}
			catch (Exception error)
			{
				state.ExceptionThatWasEncountered = error;
				state.State = ProgressState.StateValue.StoppedWithError;
				throw; // this will put the exception in the e.Error arg of the RunWorkerCompletedEventArgs
			}
			finally
			{
				WeSayWordsProject.Project.LockLift();
			}
		}


		private static void MoveTempOverRealAndBackup(string existingPath, string newFilePath)
		{
			string backupName = existingPath + ".old";

			//this was just making a mess
//            int i = 0;
//            while (File.Exists(backupName + i))
//            {
//                i++;
//            }
//            backupName += i;

			try
			{
				if (File.Exists(backupName))
				{
					File.Delete(backupName);
				}

				File.Move(existingPath, backupName);
			}
			catch
			{
				Logger.WriteEvent(String.Format("Couldn't write out to {0} ", backupName));
			}

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

		//private static void HandleTopLevelError(object sender, ThreadExceptionEventArgs e)
		//{
		//    throw new NotImplementedException();
		//}


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
			e = e.Replace("Duplicate 'liftPath' argument", "Please enclose project path in quotes if it contains spaces.");
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
