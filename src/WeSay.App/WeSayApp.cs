using System;
using System.ComponentModel;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Windows.Forms;
using CommandLine;
using Palaso.DictionaryService.Client;
using Palaso.Progress;
using Palaso.Reporting;
using Palaso.Services;
using Palaso.UI.WindowsForms.Progress;
using WeSay.App;
using WeSay.App.Properties;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.LexicalModel.Tests;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.App
{
	public class WeSayApp
	{
		//private static Mutex _oneInstancePerProjectMutex;
		private  WeSayWordsProject _project;
		private  ServiceHost _dictionaryHost;
		private  DictionaryServiceProvider _dictionary;
		private  IRecordListManager _recordListManager ;
		private CommandLineArguments _commandLineArguments = new CommandLineArguments();
		private bool _inServerMode=true;


		/// <summary>
		/// for now, this support jumping.  Later, we'll make it a stack and support fwd/backward
		/// </summary>
		private string _currentUrl;

		private ServiceAppSingletonHelper _serviceAppSingletonHelper;

		[STAThread]
		static void Main(string[] args)
		{
			WeSayApp app = new WeSayApp(args);
			app.Run();
		}
		public WeSayApp(string[] args)
		{
			Application.EnableVisualStyles();
			//leave this at the top:
			Application.SetCompatibleTextRenderingDefault(false);
		   OsCheck();
			Logger.Init();
			SetupErrorHandling();
			 //problems with user.config: http://blogs.msdn.com/rprabhu/articles/433979.aspx

		   //bring in settings from any previous version
		   if (Settings.Default.NeedUpgrade)
		   {
			   Settings.Default.Upgrade();
			   Settings.Default.NeedUpgrade = false;
		   }
			UsageReporter.AppNameToUseInDialogs = "WeSay";
			UsageReporter.AppNameToUseInReporting = "WeSayApp";

			if (!Parser.ParseArguments(args, _commandLineArguments, new ReportError(ShowCommandLineError)))
			{
				Application.Exit();
			}
		}
		public bool ServerModeStartRequested
		{
			get { return _commandLineArguments.startInServerMode; }
		}

		public void Run()
		{
			_serviceAppSingletonHelper = ServiceAppSingletonHelper.CreateServiceAppSingletonHelperIfNeeded("WeSay", _commandLineArguments.startInServerMode);
			if (_serviceAppSingletonHelper == null)
			{
				return; // there's already an instance of this app running
			}

			DisplaySettings.Default.SkinName = Settings.Default.SkinName;

			_project = new WeSayWordsProject();

			if (!TryToLoad(_commandLineArguments.liftPath, _project))
			{
				return;
			}
			using (_recordListManager= MakeRecordListManager(_project))
			{
				using (_dictionary = new DictionaryServiceProvider(this,_project))
				{
					StartDictionaryServices();
					_dictionary.LastClientDeregistered += _serviceAppSingletonHelper.OnExitIfInServerMode;
					_serviceAppSingletonHelper.HandleRequestsUntilExitOrUIStart(RunUserInterface);

					_dictionary.LastClientDeregistered -= _serviceAppSingletonHelper.OnExitIfInServerMode;
				}
			}

			Logger.ShutDown();
			Settings.Default.Save();
		}

		private void StartDictionaryServices()
		{
			Palaso.Reporting.Logger.WriteMinorEvent("Staring Dictionary Services at {0}", DictionaryServiceAddress);

			 _dictionaryHost = new ServiceHost(_dictionary, new Uri[] { new Uri(DictionaryServiceAddress), });

			_dictionaryHost.AddServiceEndpoint(typeof(IDictionaryService), new NetNamedPipeBinding(),
												 DictionaryServiceAddress);
			_dictionaryHost.Open();


		}

		private string DictionaryServiceAddress
		{
			get
			{
				return "net.pipe://localhost/DictionaryServices/" + Uri.EscapeDataString(_project.PathToLiftFile);
			}
		}

		public bool IsInServerMode
		{
			get { return _inServerMode; }
		}

		public string CurrentUrl
		{
			get { return _currentUrl; }
			private set { _currentUrl = value; }
		}

		public void GoToUrl(string url)
		{
			if (IsInServerMode)
			{
				_inServerMode = false;
				_serviceAppSingletonHelper.EnsureUIRunningAndInFront();
			}
		}

		private void RunUserInterface()
		{
			_inServerMode = false;
#if GTK
			Gdk.Threads.Enter();
#endif
				ITaskBuilder builder = null;
				try
				{
					TabbedForm tabbedForm = new TabbedForm();


					tabbedForm.Show(); // so the user sees that we did launch
					tabbedForm.Text =
						StringCatalog.Get("~WeSay", "It's up to you whether to bother translating this or not.") + ": " +
						_project.Name + "        " + ErrorReport.UserFriendlyVersionString;
					Application.DoEvents();

					_project.LiftUpdateService = SetupUpdateService(_recordListManager);
					_project.LiftUpdateService.DoLiftUpdateNow(true);

					//MONO bug as of 1.1.18 cannot bitwise or FileShare on FileStream constructor
					//                    using (FileStream config = new FileStream(_project.PathTo_projectTaskInventory, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
					using (
						FileStream configFile =
							new FileStream(_project.PathToConfigFile, FileMode.Open, FileAccess.Read,
										   FileShare.ReadWrite))
					{
						builder = new ConfigFileTaskBuilder(configFile, _project,
															tabbedForm as ICurrentWorkTask, _recordListManager);
					}
					_project.Tasks = builder.Tasks;
					Application.DoEvents();

					tabbedForm.ContinueLaunchingAfterInitialDisplay();

					//run the ui
					Application.Run(tabbedForm);

					//do a last backup before exiting
					_project.LiftUpdateService.DoLiftUpdateNow(true);
					//      BackupMaker.BackupToExternal(_filesToBackup,_project.ProjectDirectoryPath, "h:\\" + project.Name + ".zip");


					Settings.Default.SkinName = DisplaySettings.Default.SkinName;
					Logger.WriteEvent("App Exiting Normally.");
				}
				catch (IOException e)
				{
					ErrorReport.ReportNonFatalMessage(e.Message);
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
						((IDisposable) builder).Dispose();
				}
		}

		private LiftUpdateService SetupUpdateService(IRecordListManager recordListManager)
		{
			LiftUpdateService liftUpdateService;
			Db4oRecordListManager ds = (Db4oRecordListManager)    recordListManager;
			liftUpdateService = new LiftUpdateService(ds.DataSource);
			ds.DataCommitted += new EventHandler(liftUpdateService.OnDataCommitted);
			ds.DataDeleted +=new EventHandler<DeletedItemEventArgs>(liftUpdateService.OnDataDeleted);
			return liftUpdateService;
		}

		private bool TryToLoad(string liftPath, WeSayWordsProject project)
		{
		  if (liftPath == null)
		  {
			if (!String.IsNullOrEmpty(Settings.Default.PreviousLiftPath))
			{
				if (File.Exists(Settings.Default.PreviousLiftPath))
				{
					liftPath = Settings.Default.PreviousLiftPath;
				}
			}
		  }
		  if(liftPath == null)
			{
			  ErrorReport.ReportNonFatalMessage("WeSay was unable to figure out what lexicon to work on. Try opening the LIFT file by double clicking on it. If you don't have one yet, run the WeSay Configuration Tool to make a new WeSay project.");
			  return false;
			}

		  liftPath = project.UpdateFileStructure(liftPath);

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
			if (!LiftUpdateService.ConsumePendingLiftUpdates())
			{
				return false;
			}

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

		private bool BringCachesUpToDate(string liftPath, WeSayWordsProject project)
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
						Exception err = dlg.ProgressStateResult.ExceptionThatWasEncountered;
						if (err !=null)
						{
							if (err is LiftIO.LiftFormatException)
							{
								Palaso.Reporting.ErrorReport.ReportNonFatalMessage(
									"WeSay had problems with the content of the dictionary file.\r\n\r\n" + err.Message);
							}
							else
							{
								ErrorNotificationDialog.ReportException(err, null, false);
							}
						}
						else if (dlg.ProgressStateResult.State == ProgressState.StateValue.StoppedWithError)
						{
							ErrorReport.ReportNonFatalMessage("Could not build caches. " + dlg.ProgressStateResult.LogString, null, false);
						}
						return false;
					}
					LiftUpdateService.LiftIsFreshNow();
				}
			return true;
			}

		private bool PreprocessLift()
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
					ErrorReport.ReportNonFatalMessage(
						String.Format("WeSay encountered an error while preprocessing the file '{0}'.  Error was: {1}",
						WeSayWordsProject.Project.PathToLiftFile,
						dlg.ProgressStateResult.ExceptionThatWasEncountered.Message));
				}
				return (dlg.DialogResult == DialogResult.OK);
			}
		}

		void OnDoPreprocessLiftWork(object sender, DoWorkEventArgs e)
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


		private void MoveTempOverRealAndBackup(string existingPath, string newFilePath)
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


		private void OsCheck()
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

//        private static void ReleaseMutexForThisProject()
//        {
//            if (_oneInstancePerProjectMutex != null)
//            {
//                _oneInstancePerProjectMutex.ReleaseMutex();
//            }
//        }

//        private static bool GrabTokenForThisProject(CommandLineArguments cmdArgs)
//        {
//            string mutexId = cmdArgs.liftPath;
//            if (mutexId != null)
//            {
//                 bool mutexCreated;
//                mutexId = mutexId.Replace(Path.DirectorySeparatorChar, '-');
//                mutexId = mutexId.Replace(Path.VolumeSeparatorChar, '-');
//                _oneInstancePerProjectMutex = new Mutex(true, mutexId, out mutexCreated);
//                if (!mutexCreated) // can I acquire?
//                {
//                    //    Process[] processes = Process.GetProcessesByName("WeSay.App");
//                    //    foreach (Process process in processes)
//                    //    {
//
//                    //        // we should make window title include the database name.
//                    //        if(process.MainWindowTitle == "WeSay: " + project.Name)
//                    //        {
//                    //            process.WaitForInputIdle(4000); // wait four seconds at most
//                    //            //process.MainWindowHandle;
//                    //            break;
//                    //        }
//                    //    }
//
//                    MessageBox.Show("WeSay is already open with " + cmdArgs.liftPath + ".");
//                    return false;
//                }
//             }
//             return true;
//        }


		private void SetupErrorHandling()
		{
			ErrorReport.EmailAddress = "issues@wesay.org";
			if (BasilProject.IsInitialized)
			{
				ErrorReport.AddProperty("ProjectPath", BasilProject.Project.ProjectDirectoryPath);
			}
			ErrorReport.AddStandardProperties();
			ExceptionHandler.Init();
		}

		//private static void HandleTopLevelError(object sender, ThreadExceptionEventArgs e)
		//{
		//    throw new NotImplementedException();
		//}


		private IRecordListManager MakeRecordListManager(WeSayWordsProject project)
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
				Lexicon.Init(recordListManager as Db4oRecordListManager);
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

			[Argument(ArgumentTypes.AtMostOnce,
			HelpText = "Start without a user interface (will have no effect if WeSay is already running with a UI.",
			LongName = "server",
				DefaultValue=false,
			ShortName = "")]
			public bool startInServerMode = false;
		}

		void ShowCommandLineError(string e)
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
