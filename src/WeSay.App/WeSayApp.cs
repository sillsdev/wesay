using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Autofac;
using CommandLine;
using Palaso.Reporting;
using Palaso.Services;
using Palaso.Services.Dictionary;
using Palaso.Services.ForServers;
using Palaso.UI.WindowsForms.i8n;
using WeSay.App.Properties;
using WeSay.App.Services;
using WeSay.LexicalModel;
using WeSay.LexicalTools;
using WeSay.LexicalTools.GatherByWordList;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.App
{
	public class WeSayApp
	{
		//private static Mutex _oneInstancePerProjectMutex;
		private WeSayWordsProject _project;
		private DictionaryServiceProvider _dictionary;
		private readonly CommandLineArguments _commandLineArguments = new CommandLineArguments();
		private ServiceAppSingletonHelper _serviceAppSingletonHelper;
		private TabbedForm _tabbedForm;
		private IDisposable _serviceLifeTimeHelper;

		[STAThread]
		private static void Main(string[] args)
		{
			WeSayApp app = new WeSayApp(args);
			app.Run();
		}

		public WeSayApp(string[] args)
		{
			// Palaso.Services.ForClients.IpcSystem.IsWcfAvailable = false;
			Application.EnableVisualStyles();
			//leave this at the top:
			try
			{
				Application.SetCompatibleTextRenderingDefault(false);
			}
			catch (Exception) //swallow
			{
				//this fails in some test scenarios; perhaps the unit testing framework is leaving us in
				//the same appdomain, and that remembers that we called this once before?
			}
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

			if (!Parser.ParseArguments(args, _commandLineArguments, ShowCommandLineError))
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
			string path = DetermineActualLiftPath(_commandLineArguments.liftPath);
			if (!String.IsNullOrEmpty(path))
			{
				path = path.Replace(Path.DirectorySeparatorChar, '-');
				path = path.Replace(Path.VolumeSeparatorChar, '-');

				_serviceAppSingletonHelper =
						ServiceAppSingletonHelper.CreateServiceAppSingletonHelperIfNeeded(
								"WeSay-" + path, _commandLineArguments.startInServerMode);
				if (_serviceAppSingletonHelper == null)
				{
					return; // there's already an instance of this app running
				}
			}

			try
			{
				DisplaySettings.Default.SkinName = Settings.Default.SkinName;
				using (_project = InitializeProject(_commandLineArguments.liftPath))
				{
					if (_project == null)
					{
						return;
					}


					using (_dictionary =
						   new DictionaryServiceProvider(GetLexEntryRepository(), this, _project))
					{

						StartDictionaryServices();
						_dictionary.LastClientDeregistered +=
							_serviceAppSingletonHelper.OnExitIfInServerMode;

						WireUpChorusEvents();

						_serviceAppSingletonHelper.HandleEventsUntilExit(StartUserInterface);

						_dictionary.LastClientDeregistered -=
							_serviceAppSingletonHelper.OnExitIfInServerMode;

						//do a last backup before exiting
						Logger.WriteEvent("App Exiting Normally.");
					}
					_project.BackupNow();
			   }
		   }
			finally
			{
				if (_serviceLifeTimeHelper != null)
				{
					_serviceLifeTimeHelper.Dispose();
				}
				if (_serviceAppSingletonHelper != null)
				{
					_serviceAppSingletonHelper.Dispose();
				}
			}
			Logger.ShutDown();
			Settings.Default.Save();
		}

		private LexEntryRepository GetLexEntryRepository()
		{
			return _project.GetLexEntryRepository();
		}

		private void WireUpChorusEvents()
		{
			//this is something of a hack... it seems weird to me that the app has the repository, but the project doesn't.
			//maybe only the project should posses it.
			_project.BackupMaker.Repository = GetLexEntryRepository();//needed so it can unlock the lift file as needed

			GetLexEntryRepository().AfterEntryModified += OnModifyLift;
			GetLexEntryRepository().AfterEntryDeleted += OnModifyLift;
		}

		private void OnModifyLift(object sender, LexEntryRepository.EntryEventArgs e)
		{
			_project.ConsiderSynchingOrBackingUp("checkpoint");
		}



		private void OnBringToFrontRequest(object sender, EventArgs e)
		{
			if (_tabbedForm == null)
			{
				_serviceAppSingletonHelper.EnsureUIRunningAndInFront();
			}
			else
			{
				_tabbedForm.synchronizationContext.Send(
						delegate { _tabbedForm.MakeFrontMostWindow(); }, null);
			}
		}

		private void StartDictionaryServices()
		{
			////Problem: if there is already a cache miss, this will be slow, and somebody will time out
			//StartCacheWatchingStuff();

			Logger.WriteMinorEvent("Starting Dictionary Services at {0}",
								   DictionaryAccessor.GetServiceName(_project.PathToLiftFile));
			_serviceLifeTimeHelper =
					IpcSystem.StartServingObject(
							DictionaryAccessor.GetServiceName(_project.PathToLiftFile), _dictionary);
		}

		public bool IsInServerMode
		{
			get
			{
				return _serviceAppSingletonHelper.CurrentState ==
					   ServiceAppSingletonHelper.State.ServerMode;
			}
		}

		///// <summary>
		///// Only show a dialog if the operation takes more than two seconds
		///// </summary>
		///// <param name="message"></param>
		//public void NotifyOfLongStartupThread(object message)
		//{
		//    try
		//    {
		//        Thread.Sleep(2000);

		//        LongStartupNotification dlg = new LongStartupNotification();
		//        dlg.Message = (string) message;
		//        dlg.Show();
		//        Application.DoEvents();
		//        try
		//        {
		//            while (true)
		//            {
		//                Thread.Sleep(100);
		//                Application.DoEvents(); //otherwise we get (Not Responding)
		//            }
		//        }
		//        catch (ThreadInterruptedException)
		//        {
		//            dlg.Close();
		//            dlg.Dispose();
		//        }
		//    }
		//    catch (ThreadInterruptedException) {}
		//}

		///// <summary>
		///// Without this, if we add entries with no UI up, there is not dictionary task up, and the cache
		///// ignores new entries being added (and someday other stuff). Then when we do eventually pull
		///// the ui up, they'll get a painful cache rebuild.
		///// </summary>
		//private void StartCacheWatchingStuff()
		//{
		//    Thread notify = new Thread(NotifyOfLongStartupThread);

		//    notify.Start(StringCatalog.Get("~Please wait while WeSay prepares your data",
		//                                   "This is shown in rare circumstances where WeSay finds it needs to prepare some indices so it can run faster.  The main point to get across is that the user should settle in for a long wait, not think something is broken or try to run WeSay again."));

		//    try
		//    {
		//        DictionaryTask dictionaryTask = new DictionaryTask(GetLexEntryRepository(),
		//                                                           _project.DefaultViewTemplate);
		//    }
		//    finally
		//    {
		//        notify.Interrupt();
		//    }

		//    //            LexEntryRepository manager = GetLexEntryRepository() as LexEntryRepository;
		//    //            if (manager != null)
		//    //            {
		//    //                HeadwordSortedListHelper helper = new HeadwordSortedListHelper(manager,
		//    //                                                                     this._project.HeadWordWritingSystem);
		//    //              manager.GetSortedList(helper);//installs it
		//    //            }
		//}

		public string CurrentUrl
		{
			get
			{
				if (_tabbedForm != null)
				{
					return _tabbedForm.CurrentUrl;
				}
				return string.Empty;
			}
		}

		public void GoToUrl(string url)
		{
			_serviceAppSingletonHelper.EnsureUIRunningAndInFront();

			//if it didn't timeout
			if (_serviceAppSingletonHelper.CurrentState == ServiceAppSingletonHelper.State.UiMode)
			{
				Debug.Assert(_tabbedForm != null, "tabbed form should have been started.");
				_tabbedForm.GoToUrl(url);
			}
		}

		private void StartUserInterface()
		{
			try
			{
				_project.AddToContainer(b => b.Register<StatusBarController>());
				_project.AddToContainer(b => b.Register<TabbedForm>());
				_tabbedForm = _project.Container.Resolve<TabbedForm>();
				_tabbedForm.Show(); // so the user sees that we did launch
				_tabbedForm.Text =
						StringCatalog.Get("~WeSay",
										  "It's up to you whether to bother translating this or not.") +
						": " + _project.Name + "        " + ErrorReport.UserFriendlyVersionString;
				Application.DoEvents();

			   //todo: this is what we're supposed to use the autofac "modules" for
				//couldn't get this to work: _project.AddToContainer(typeof(ICurrentWorkTask), _tabbedForm as ICurrentWorkTask);
				_project.AddToContainer(b => b.Register<ICurrentWorkTask>(_tabbedForm));
				_project.AddToContainer(b => b.Register<StatusStrip>(_tabbedForm.StatusStrip));
				_project.AddToContainer(b => b.Register(TaskMemoryRepository.CreateOrLoadTaskMemoryRepository(_project.Name, _project.PathToWeSaySpecificFilesDirectoryInProject )));


				_project.LoadTasksFromConfigFile();

				Application.DoEvents();
				_tabbedForm.IntializationComplete += OnTabbedForm_IntializationComplete;
				_tabbedForm.ContinueLaunchingAfterInitialDisplay();
				_tabbedForm.Activate();
				_tabbedForm.BringToFront(); //needed if we were previously in server mode

				RtfRenderer.HeadWordWritingSystemId =
						_project.DefaultViewTemplate.HeadwordWritingSystem.Id;

				//run the ui
				Application.Run(_tabbedForm);

				Settings.Default.SkinName = DisplaySettings.Default.SkinName;
			}
			catch (IOException e)
			{
				ErrorReport.NotifyUserOfProblem(e.Message);
			}
		}



		private void OnTabbedForm_IntializationComplete(object sender, EventArgs e)
		{
			_serviceAppSingletonHelper.BringToFrontRequest += OnBringToFrontRequest;
			_serviceAppSingletonHelper.UiReadyForEvents();
			_dictionary.UiSynchronizationContext = _tabbedForm.synchronizationContext;
		}

		//private static LiftUpdateService SetupUpdateService(LexEntryRepository lexEntryRepository)
		//{
		//    LiftUpdateService liftUpdateService;
		//    liftUpdateService = new LiftUpdateService(lexEntryRepository);
		//    return liftUpdateService;
		//}

		private static WeSayWordsProject InitializeProject(string liftPath)
		{
			WeSayWordsProject project = new WeSayWordsProject();
			liftPath = DetermineActualLiftPath(liftPath);
			if (liftPath == null)
			{
				ErrorReport.NotifyUserOfProblem(
						"WeSay was unable to figure out what lexicon to work on. Try opening the LIFT file by double clicking on it. If you don't have one yet, run the WeSay Configuration Tool to make a new WeSay project.");
				return null;
			}

			liftPath = project.UpdateFileStructure(liftPath);

			if (project.LoadFromLiftLexiconPath(liftPath))
			{
				Settings.Default.PreviousLiftPath = liftPath;
			}
			else
			{
				return null;
			}

			try
			{
				project.MigrateConfigurationXmlIfNeeded();
			}
			catch
			{
				ErrorReport.NotifyUserOfProblem(
						"WeSay was unable to migrate the WeSay configuration file for the new version of WeSay. This may cause WeSay to not function properly. Try opening the project in the WeSay Configuration Tool to fix this.");
			}

			return project;
		}

		private static string DetermineActualLiftPath(string liftPath)
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
			if (!File.Exists(liftPath))
			{
				return null;
			}
			if (!liftPath.Contains(Path.DirectorySeparatorChar.ToString()))
			{
				Logger.WriteEvent("Converting filename only liftPath {0} to full path {1}", liftPath, Path.GetFullPath(liftPath));
				liftPath = Path.GetFullPath(liftPath);
			}
			return liftPath;
		}

		private static void OsCheck()
		{
#if DEBUG
			string runtime = (Type.GetType("Mono.Runtime") == null) ? "Microsoft .Net" : "Mono";
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				Console.WriteLine("running on Unix with " + runtime);
			}
			else
			{
				Console.WriteLine("running on Windows with " + runtime);
			}
#endif
		}

		private static void SetupErrorHandling()
		{
			ErrorReport.EmailAddress = "issues@wesay.org";
			if (BasilProject.IsInitialized)
			{
				ErrorReport.AddProperty("ProjectPath", BasilProject.Project.ProjectDirectoryPath);
			}
			ErrorReport.AddStandardProperties();
			ExceptionHandler.Init();
		}

		private class CommandLineArguments
		{
			[DefaultArgument(ArgumentTypes.AtMostOnce,
					// DefaultValue = @"..\..\SampleProjects\Thai\WeSay\thai5000.words",
					HelpText =
							"Path to the Lift Xml file (e.g. on windows, \"c:\\thai\\wesay\\thai.lift\")."
					)]
			public string liftPath;

			//            [Argument(ArgumentTypes.AtMostOnce,
			//                HelpText = "Language to show the user interface in.",
			//                LongName = "ui",
			//                ShortName = "")]
			//            public string ui = null;

			[Argument(ArgumentTypes.AtMostOnce,
					HelpText =
							"Start without a user interface (will have no effect if WeSay is already running with a UI."
					, LongName = "server", DefaultValue = false, ShortName = "")]
			public bool startInServerMode;
		}

		private static void ShowCommandLineError(string e)
		{
			Parser p = new Parser(typeof (CommandLineArguments), ShowCommandLineError);
			e = e.Replace("Duplicate 'liftPath' argument",
						  "Please enclose project path in quotes if it contains spaces.");
			e += "\r\n\r\n" + p.GetUsageString(200);
			MessageBox.Show(e, "WeSay Command Line Problem");
		}
	}

	internal class ThreadExceptionHandler
	{
		///
		/// Handles the thread exception.
		///
		public void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			MessageBox.Show("caught");
		}
	}
}