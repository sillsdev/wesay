using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Windows.Forms;
using CommandLine;
using Palaso.Reporting;
using Palaso.Services.Dictionary;
using Palaso.Services.ForClients;
using Palaso.Services.ForServers;
using Palaso.UI.WindowsForms.i8n;
using WeSay.App.Properties;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.LexicalModel.Tests;
using WeSay.LexicalTools;
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




		private ServiceAppSingletonHelper _serviceAppSingletonHelper;
		private TabbedForm _tabbedForm;

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
			try
			{
				Application.SetCompatibleTextRenderingDefault(false);
			}
			catch (Exception swallow)
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
			string path = DetermineActualLiftPath(_commandLineArguments.liftPath);
			if (!String.IsNullOrEmpty(path))
			{
				path = path.Replace(Path.DirectorySeparatorChar, '-');
				path = path.Replace(Path.VolumeSeparatorChar, '-');

				_serviceAppSingletonHelper =
					ServiceAppSingletonHelper.CreateServiceAppSingletonHelperIfNeeded("WeSay-" + path,
									  _commandLineArguments.startInServerMode);
				if (_serviceAppSingletonHelper == null)
				{
					return; // there's already an instance of this app running
				}
			}

			DisplaySettings.Default.SkinName = Settings.Default.SkinName;

			_project = InitializeProject(_commandLineArguments.liftPath);
			if (_project == null)
			{
				return;
			}

			if (_project.PathToWeSaySpecificFilesDirectoryInProject.IndexOf("PRETEND") <0)
			{
				RecoverUnsavedDataIfNeeded();
			}
			LiftPreparer preparer = new LiftPreparer(_project);
			if (!preparer.MakeCacheAndLiftReady())
			{
				return;
			}
			using (_recordListManager= MakeRecordListManager(_project))
			{
				using (_dictionary = new DictionaryServiceProvider(this,_project))
				{

					_project.LiftUpdateService = SetupUpdateService(_recordListManager);
					_project.LiftUpdateService.DoLiftUpdateNow(true);

					StartDictionaryServices();
					_dictionary.LastClientDeregistered += _serviceAppSingletonHelper.OnExitIfInServerMode;
					_serviceAppSingletonHelper.BringToFrontRequest += new EventHandler(OnBringToFrontRequest);
					_serviceAppSingletonHelper.HandleEventsUntilExit(StartUserInterface);

					_dictionary.LastClientDeregistered -= _serviceAppSingletonHelper.OnExitIfInServerMode;
				}
			}

			Logger.ShutDown();
			Settings.Default.Save();
		}

		private void RecoverUnsavedDataIfNeeded()
		{
			if (!File.Exists(_project.PathToDb4oLexicalModelDB))
			{
				return;
			}
			WeSayWordsDb4oModelConfiguration config = new WeSayWordsDb4oModelConfiguration();
			config.Configure();
			using (Db4oDataSource ds = new Db4oDataSource(_project.PathToDb4oLexicalModelDB))
			{
				Db4oLexModelHelper.Initialize(ds.Data);
				LiftUpdateService updateServiceForCrashRecovery = new LiftUpdateService(ds);
				updateServiceForCrashRecovery.RecoverUnsavedChangesOutOfCacheIfNeeded();
				Db4oLexModelHelper.Deinitialize(ds.Data);
			}
		}

		void OnBringToFrontRequest(object sender, EventArgs e)
		{
			Debug.Assert(_tabbedForm != null);
			if (_tabbedForm != null)
			{
				_tabbedForm.synchronizationContext.Send(
					   delegate
						   {
							   _tabbedForm.MakeFrontMostWindow();
						   }, null);
			}
		}

		private void StartDictionaryServices()
		{
			//Problem: if there is already a cache miss, this will be slow, and somebody will time out
			StartCacheWatchingStuff();


			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				Palaso.Reporting.Logger.WriteMinorEvent("Starting Dictionary Services at {0}", DictionaryServiceAddress);

				_dictionaryHost = new ServiceHost(_dictionary, new Uri[] {new Uri(DictionaryServiceAddress),});

				_dictionaryHost.AddServiceEndpoint(typeof (IDictionaryService), IPCUtils.CreateBinding(),
												   DictionaryServiceAddress);
				_dictionaryHost.Open();
			}
		}

		private string DictionaryServiceAddress
		{
			get
			{
				return IPCUtils.URLPrefix + "DictionaryServices/" + Uri.EscapeDataString(_project.PathToLiftFile);
			}
		}

		public bool IsInServerMode
		{
			get { return _serviceAppSingletonHelper.CurrentState  ==  ServiceAppSingletonHelper.State.ServerMode; }
		}


		/// <summary>
		/// Without this, if we add entries with no UI up, there is not dictionary task up, and the cache
		/// ignores new entries being added (and someday other stuff). Then when we do eventually pull
		/// the ui up, they'll get a painful cache rebuild.
		/// </summary>
		private void StartCacheWatchingStuff()
		{
			DictionaryTask dictionaryTask = new DictionaryTask(_recordListManager, _project.DefaultViewTemplate);
			dictionaryTask.RegisterWithCache(_project.DefaultViewTemplate);
		}

		public string CurrentUrl
		{
			get
			{
				if (_tabbedForm != null)
				{
					return _tabbedForm.CurrentUrl;
				}
				Debug.Fail("hmmm");
				return string.Empty;
			}
		}

		public void GoToUrl(string url)
		{

			_serviceAppSingletonHelper.EnsureUIRunningAndInFront();

			//if it didn't timeout
			if(_serviceAppSingletonHelper.CurrentState == ServiceAppSingletonHelper.State.UiMode)
			{
				Debug.Assert(_tabbedForm != null, "tabbed form should have been started.");
				_tabbedForm.GoToUrl(url);
			}
		}

		private void StartUserInterface()
		{
			ITaskBuilder builder = null;
			try
			{
				_tabbedForm = new TabbedForm();
				_tabbedForm.Show(); // so the user sees that we did launch
				_tabbedForm.Text =
					StringCatalog.Get("~WeSay", "It's up to you whether to bother translating this or not.") + ": " +
					_project.Name + "        " + ErrorReport.UserFriendlyVersionString;
				Application.DoEvents();


				//MONO bug as of 1.1.18 cannot bitwise or FileShare on FileStream constructor
				//                    using (FileStream config = new FileStream(_project.PathTo_projectTaskInventory, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
				using (
					FileStream configFile =
						new FileStream(_project.PathToConfigFile, FileMode.Open, FileAccess.Read,
									   FileShare.ReadWrite))
				{
					builder = new ConfigFileTaskBuilder(configFile, _project,
														_tabbedForm as ICurrentWorkTask, _recordListManager);
				}
				_project.Tasks = builder.Tasks;
				Application.DoEvents();
				_tabbedForm.IntializationComplete += new EventHandler(OnTabbedForm_IntializationComplete);
				_tabbedForm.ContinueLaunchingAfterInitialDisplay();
				_tabbedForm.Activate();
				_tabbedForm.BringToFront();//needed if we were previously in server mode

				//run the ui
				Application.Run(_tabbedForm);

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
				//TODO(JH): having a builder than needs to be kept around so it can be disposed of is all wrong.
				//either I want to change it to something like TaskList rather than ITaskBuilder, or
				//it needs to create some disposable object other than a IList<>.
				//The reason we need to be able to dispose of it is because we need some way to
				//dispose of things that it might create, such as a data source.
				if (builder is IDisposable)
					((IDisposable) builder).Dispose();
			}
		}

		void OnTabbedForm_IntializationComplete(object sender, EventArgs e)
		{
			_serviceAppSingletonHelper.UiReadyForEvents();

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

		private WeSayWordsProject InitializeProject(string liftPath)
		{
			WeSayWordsProject project = new WeSayWordsProject();
			liftPath = DetermineActualLiftPath(liftPath);
			if (liftPath == null)
			{
				ErrorReport.ReportNonFatalMessage("WeSay was unable to figure out what lexicon to work on. Try opening the LIFT file by double clicking on it. If you don't have one yet, run the WeSay Configuration Tool to make a new WeSay project.");
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

			WeSayWordsProject.Project.LockLift(); // Consume will expect it to be locked already

			return project;
		}

		private string DetermineActualLiftPath(string liftPath)
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
			return liftPath;
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
