using Autofac;
using Chorus;
using Chorus.FileTypeHandlers.lift;
using Chorus.sync;
using Chorus.UI.Notes;
using Chorus.UI.Notes.Bar;
using Microsoft.Practices.ServiceLocation;
using SIL.Data;
using SIL.DictionaryServices.Lift;
using SIL.DictionaryServices.Model;
using SIL.IO;
using SIL.Lexicon;
using SIL.Lift;
using SIL.Lift.Options;
using SIL.Progress;
using SIL.Reporting;
using SIL.UiBindings;
using SIL.Windows.Forms.Progress;
using SIL.WritingSystems;
using SIL.Xml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using WeSay.AddinLib;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation.Options;
using WeSay.Project.ConfigMigration.UserConfig;
using WeSay.Project.ConfigMigration.WeSayConfig;
using WeSay.Project.ConfigMigration.WritingSystem;
using WeSay.Project.LocalizedList;
using WeSay.Project.Synchronize;
using WeSay.UI;
using WeSay.UI.AutoCompleteTextBox;
using WeSay.UI.TextBoxes;
using IContainer = Autofac.IContainer;

namespace WeSay.Project
{
	public enum WeSayDataFormat { Lift, LCM, LiftToLCM, LCMToLift };

	public class WeSayWordsProject : BasilProject, IFileLocator
	{
		private IList<ITask> _tasks;
		private IEnumerable<ITaskConfiguration> _taskconfigurations;
		private ViewTemplate _defaultViewTemplate;
		private IList<ViewTemplate> _viewTemplates;
		private readonly Dictionary<string, OptionsList> _optionLists;
		private readonly List<OptionsList> _modifiedOptionsLists = new List<OptionsList>();
		private string _pathToLiftFile;
		private string _cacheLocationOverride;

		private readonly AddinSet _addins;
		private IList<LexRelationType> _relationTypes;
		private ChorusBackupMaker _backupMaker;
		private IContainer _container;
		private static bool _geckoOption;
		readonly Dictionary<string, string> _changedWritingSystemIds = new Dictionary<string, string>();
		private bool _alreadyReportedWsLookupFailure;

		//public const int CurrentWeSayConfigFileVersion = 9; // This variable must be updated with every new version of the WeSayConfig file
		public const int CurrentWeSayUserSpecificConfigFileVersion = 3; // This variable must be updated with every new version of the WeSayUserConfig file

		public event EventHandler EditorsSaveNow;

		public class StringPair : EventArgs
		{
			public string from;
			public string to;
		}

		public const string VernacularWritingSystemIdForProjectCreation = WellKnownSubtags.UnlistedLanguage;
		public const string AnalysisWritingSystemIdForProjectCreation = "en";

		public event EventHandler<StringPair> WritingSystemChanged;
		public event EventHandler<WritingSystemDeletedEventArgs> WritingSystemDeleted;

		public event EventHandler<StringPair> MeaningFieldChanged;

		public WeSayWordsProject()
		{
			_addins = AddinSet.Create(GetAddinNodes, LocateFile);
			_optionLists = new Dictionary<string, OptionsList>();
			DataFormat = WeSayDataFormat.Lift;
			//            BackupMaker = new ChorusBackupMaker();
		}

		public WeSayDataFormat DataFormat { get; internal set; }

		public IList<ITask> Tasks
		{
			get { return _tasks; }
			set { _tasks = value; }
		}

		public IEnumerable<ITaskConfiguration> TaskConfigurations
		{
			get { return _taskconfigurations; }
			set { _taskconfigurations = value; }
		}

		public static bool ProjectExists
		{
			get { return Singleton != null; }
		}

		public static bool GeckoOption
		{
			get
			{
				return _geckoOption;
			}
			set
			{
				_geckoOption = value;
			}
		}

		public new static WeSayWordsProject Project
		{
			get
			{
				if (Singleton == null)
				{
					throw new InvalidOperationException(
							"WeSayWordsProject Not initialized. For tests, call BasilProject.InitializeForTests().");
				}
				return (WeSayWordsProject)Singleton;
			}
		}

		public static string PathToPretendLiftFile
		{
			get { return Path.Combine(GetPretendProjectDirectory(), "PRETEND.lift"); }
		}

		public static string PathToPretendWritingSystemPrefs
		{
			get { return Path.Combine(GetPretendProjectDirectory(), "WritingSystemPrefs.xml"); }
		}

		public void SetupProjectDirForTests(string pathToLift, IProgressNotificationProvider progressProvider)
		{
			ProjectDirectoryPath = Directory.GetParent(pathToLift).Parent.FullName;
			PathToLiftFile = pathToLift;
			if (File.Exists(PathToConfigFile))
			{
				File.Delete(PathToConfigFile);
			}
			string configName = Path.GetFileName(Project.PathToConfigFile);
			File.Copy(Path.Combine(ApplicationTestDirectory, configName),
					  Project.PathToConfigFile,
					  true);
			RemoveCache();
			ErrorReport.IsOkToInteractWithUser = false;
			LoadFromProjectDirectoryPath(ProjectDirectoryPath, progressProvider);
			UiOptions.Language = "en";
		}

		public void RemoveCache()
		{
			if (Directory.Exists(PathToCache))
			{
				Directory.Delete(PathToCache, true);
			}
		}

		/// <summary>
		/// exception handlers should call this when the database or other caches seem broken or out of sync
		/// </summary>
		/// <param name="error"></param>
		public void HandleProbableCacheProblem(Exception error)
		{
#if DEBUG
			ErrorReport.NotifyUserOfProblem(
					"WeSay had a problem. You should quit now and let WeSay try to fix the problem when you run it again.\r\n\r\nIn the release build, the cache would now be invalidated and the user would not see the following crash dialog.");
			throw error;
#else
			//todo: make a way to pass on this error to us
			SIL.Reporting.ErrorReport.NotifyUserOfProblem(
				"WeSay had a problem. You should quit now and let WeSay try to fix the problem when you run it again.");
#endif
		}

		public bool LoadFromLiftLexiconPath(string liftPath)
		{
			return LoadFromLiftLexiconPath(liftPath, null);
		}

		public bool LoadFromLiftLexiconPath(string liftPath, IProgressNotificationProvider progressProvider)
		{
			try
			{
				PathToLiftFile = LiftFileLocator.LocateAt(liftPath);

				if (!File.Exists(PathToConfigFile))
				{
					var result = MessageBox.Show(
						"This project does not have the WeSay-specific configuration files.  A new set of files will be created, and later you can use the WeSayConfiguration Tool to set up things the way you want.",
						"WeSay", MessageBoxButtons.OKCancel);

					if (result != DialogResult.OK)
						return false;
				}
				else
				{
					try
					{
						using (FileStream fs = File.OpenRead(PathToConfigFile))
						{
							fs.Close();
						}
					}
					catch (UnauthorizedAccessException)
					{
						ErrorReport.NotifyUserOfProblem(
							String.Format(
								"WeSay was unable to open the file at '{0}' for reading, because the system won't allow it. Investigate your user permissions to write to this file.",
								PathToConfigFile));
						return false;
					}
					catch (IOException e)
					{
						ErrorReport.NotifyUserOfProblem(
							String.Format(
								"WeSay was unable to open the file at '{0}' for reading. \n Further information: {1}",
								PathToConfigFile,
								e.Message));
						return false;
					}
				}

				//ProjectDirectoryPath = Directory.GetParent(Directory.GetParent(liftPath).FullName).FullName;
				ProjectDirectoryPath = Directory.GetParent(liftPath).FullName;

				try
				{
					LoadFromProjectDirectoryPath(ProjectDirectoryPath, progressProvider);
				}
				catch (LiftFormatException)
				{
					return false;//it's already been reported, not a crash, but we can't go on
				}
				return true;
			}
			catch (ConfigurationException e)
			{
				ErrorReport.NotifyUserOfProblem(e.Message);
				return false;
			}
		}

		public string UpdateFileStructure(string liftPath)
		{
			string projectDir;
			if (HasOldStructure(liftPath, out projectDir))
			{
				MoveFilesFromOldDirLayout(projectDir);
				liftPath = Path.Combine(projectDir, Path.GetFileName(liftPath));
			}
			return liftPath;
		}

		private static bool HasOldStructure(string liftPath, out string projectDirectory)
		{
			projectDirectory = null;
			Debug.Assert(File.Exists(liftPath));
			string parentName = Directory.GetParent(liftPath).FullName;
			DirectoryInfo parent = Directory.GetParent(parentName);
			if (parent == null)//like if the file was at c:\foo.lift
			{
				return false;
			}
			projectDirectory = parent.FullName;
			string commonDir = Path.Combine(projectDirectory, "common");
			string dirHoldingLift = Path.GetFileName(Path.GetDirectoryName(liftPath));
			return dirHoldingLift == "wesay" && Directory.Exists(commonDir);
		}

		public bool CreatedByFLEx(string liftPath)
		{
			string producerNode;
			if (liftPath == "")
			{
				return false;
			}
			using (XmlReader reader = XmlReader.Create(liftPath))
			{
				reader.MoveToContent();
				while (reader.NodeType != XmlNodeType.Element && reader.Name != "lift")
				{
					reader.Read();
				}
				producerNode = reader.GetAttribute("producer");
			}
			return producerNode.Contains("SIL.FLEx");
		}

		public override void LoadFromProjectDirectoryPath(string projectDirectoryPath, IProgressNotificationProvider progressProvider = null)
		{
			ProjectDirectoryPath = projectDirectoryPath;

			// Avoid the possibility of getting two identical error messages by getting the
			// path to the lift file first, then using that result to get the path to the
			// config file.
			string preferredLiftFile = LiftFileLocator.LocateInDirectory(projectDirectoryPath);
			if (String.IsNullOrEmpty(preferredLiftFile))
			{
				return;
			}
			var configFile = GetPathToConfigFile(PathToWeSaySpecificFilesDirectoryInProject,
				GetProjectNameFromLiftFilePath(preferredLiftFile));
			if (!File.Exists(configFile))
			{
				PathToLiftFile = preferredLiftFile;
				string projectName = Path.GetFileName(Path.GetFileNameWithoutExtension(preferredLiftFile));

				CreateEmptyProjectFiles(projectDirectoryPath, projectName, WellKnownSubtags.UnlistedLanguage);
				LoadFromProjectDirectoryPathInner(projectDirectoryPath, progressProvider);

				//will rarely be needed... only when we're starting with a raw lift folder
				ProjectFromLiftFolderCreator.PrepareLiftFolderForWeSay(this);
				Save();
			}
			else
			{
				LoadFromProjectDirectoryPathInner(projectDirectoryPath, progressProvider);
			}
		}

		private void LoadFromProjectDirectoryPathInner(string projectDirectoryPath, IProgressNotificationProvider progressProvider = null)
		{
			ProjectDirectoryPath = projectDirectoryPath;

			//may have already been done, but maybe not
			MoveFilesFromOldDirLayout(projectDirectoryPath);
			if (SIL.Reporting.ErrorReport.IsOkToInteractWithUser)
			{
				var dialog = new ProgressDialog();
				var worker = new BackgroundWorker();
				worker.DoWork += OnDoMigration;
				worker.RunWorkerCompleted += OnWorkerCompleted;
				dialog.BackgroundWorker = worker;
				dialog.CanCancel = false;
				dialog.BarStyle = ProgressBarStyle.Marquee;
				dialog.Text = "Checking file...";
				dialog.StatusText = "Checking files...";
				dialog.ShowDialog();
			}
			else
			{
				OnDoMigration(null, null);  // this ensures that migration will occur even when the dialog box isn't shown i.e. during tests
			}

			base.LoadFromProjectDirectoryPath(projectDirectoryPath);
			//review: is this the right place for this?
			PopulateDIContainer(progressProvider);

			LoadUserConfig();
			InitStringCatalog();
		}

		private static void OnWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e != null && e.Error != null)
			{
				throw new ApplicationException("Error during migration.", e.Error);
			}
		}

		private void OnDoMigration(object sender, DoWorkEventArgs e)
		{
			MigrateProjectFilesAndCheckForOrphanedWritingSystems(ProjectDirectoryPath);
		}

		private static void MigrateProjectFilesAndCheckForOrphanedWritingSystems(string projectDirectory)
		{
			string liftFilePath = LiftFileLocator.LocateInDirectoryQuietly(projectDirectory);
			string projectName = GetProjectNameFromLiftFilePath(liftFilePath);
			string configFilePath = GetPathToConfigFile(projectDirectory, projectName);
			string userConfigPath = PathToUserSpecificConfigFile(projectDirectory);

			//migrate writing systems
			var writingSystemMigrator = new WritingSystemsMigrator(projectDirectory);
			writingSystemMigrator.MigrateIfNecessary();

			if (!Directory.Exists(Path.Combine(projectDirectory, "SharedSettings")))
				Directory.CreateDirectory(Path.Combine(projectDirectory, "SharedSettings"));
			var userSettingsStore = new FileSettingsStore(LexiconSettingsFileHelper.GetUserLexiconSettingsPath(projectDirectory));
			var userSettingsDataMapper = new UserLexiconSettingsWritingSystemDataMapper(userSettingsStore);
			var projectSettingsStore = new FileSettingsStore(LexiconSettingsFileHelper.GetProjectLexiconSettingsPath(projectDirectory));
			var projectSettingsDataMapper = new ProjectLexiconSettingsWritingSystemDataMapper(projectSettingsStore);

			ICustomDataMapper<WritingSystemDefinition>[] customDataMapper =
			{
				userSettingsDataMapper,
				projectSettingsDataMapper
			};

			// Load the writing systems
			var writingSystemRepository = LdmlInFolderWritingSystemRepository.Initialize(
				GetPathToLdmlWritingSystemsFolder(projectDirectory),
				customDataMapper,
				null,
				OnWritingSystemMigration,
				OnWritingSystemLoadProblem);

			var wf = (LdmlInFolderWritingSystemFactory)writingSystemRepository.WritingSystemFactory;
			wf.TemplateFolder = GetPathToLdmlWritingSystemsFolder(ApplicationCommonDirectory);

			//migrate the project config file
			if (File.Exists(configFilePath)) // will be null if we're creating a new project
			{
				var projectConfigFile = new ConfigFile(configFilePath);
				projectConfigFile.MigrateIfNecassary();
				//check for orphaned writing systems in the config file
				projectConfigFile.CreateWritingSystemsForIdsInFileWhereNecassary(writingSystemRepository);
			}

			if (File.Exists(liftFilePath)) // will be null if we're creating a new project
			{
				//check for orphaned writing systems in Lift
				var wsCreator = new WritingSystemsInLiftFileHelper(writingSystemRepository, liftFilePath);
				wsCreator.CreateNonExistentWritingSystemsFoundInFile();
			}

			//migrate user config
			var userConfigMigrator = new WeSayUserConfigMigrator(userConfigPath, writingSystemRepository);
			userConfigMigrator.MigrateIfNeeded();
		}

		private static string GetProjectNameFromLiftFilePath(string liftFilePath)
		{
			return String.IsNullOrEmpty(liftFilePath) ? "" : Path.GetFileNameWithoutExtension(liftFilePath);
		}

		[Serializable]
		//    [ComVisible(true)]
		public delegate object ServiceCreatorCallback(
		   IServiceContainer container,
		   Type serviceType
		);

		//TODO: figure out how to move this to where it belongs... should be doable if/when we
		//can do container building from within the lexical assemblies.
		public static string GetUrlFromLexEntry(LexEntry entry)
		{
			var filename = Path.GetFileName(Project.PathToLiftFile);
			//review: see also: HttpUtility.UrlEncode
			string url = string.Format("lift://{0}?type=entry&", filename);
			url += "label=" + entry.GetSimpleFormForLogging() + "&";
			url += "id=" + entry.Guid;
			url = url.Trim('&');
			return url;
		}

		private void PopulateDIContainer(IProgressNotificationProvider progressProvider = null)
		{
			var builder = new ContainerBuilder();

			builder.RegisterInstance(new WordListCatalog());

			builder.RegisterInstance<IProgressNotificationProvider>(progressProvider ?? new DialogProgressNotificationProvider());

			//NB: these are delegates because the viewtemplate is not yet avaialbe when were're building the container
			builder.Register<OptionsList>(c => GetSemanticDomainsList()).SingleInstance();//todo: figure out how to limit this with a name... currently, it's for any OptionList

			// I (CP) don't think this is needed
			builder.Register<IEnumerable<string>>(c => GetIdsOfSingleOptionFields()).SingleInstance();//todo: figure out how to limit this with a name... currently, it's for any IEnumerable<string>

			// start registering ViewTemplate related things before LiftDataMapper
			var catalog = new TaskTypeCatalog();
			catalog.RegisterAllTypes(builder);

			builder.RegisterInstance<TaskTypeCatalog>(catalog).SingleInstance();

			//this is a bit weird, did it to get around a strange problem where it was left open,
			//never found out by whom.  But note, it does affect behavior.  It means that
			//the first time the reader is asked for, it will be reading the value as it was
			//back when we did this assignment.
			string configFileText = File.ReadAllText(PathToConfigFile);

			string defaultXmlConfigText = File.ReadAllText(PathToDefaultConfig);

			builder.Register(c => new ConfigFileReader(configFileText, defaultXmlConfigText, catalog)).SingleInstance();

			builder.RegisterType<TaskCollection>().SingleInstance();

			DataFormat = ConfigFileReader.GetDataFormat(configFileText);

			var viewTemplates = ConfigFileReader.CreateViewTemplates(configFileText, WritingSystems);
			foreach (var viewTemplate in viewTemplates)
			{
				//todo: this isn't going to work if we start using multiple tempates.
				//will have to go to a naming system.
				builder.RegisterInstance<ViewTemplate>(viewTemplate).SingleInstance();
			}


			builder.Register<LiftDataMapper>(c =>
												 {
													 try
													 {
														 var semanticDomains = GetSemanticDomainsList();
														 return c.Resolve<IProgressNotificationProvider>()
																 .Go<LiftDataMapper>(
																	 "Loading Dictionary", progressState =>
																							   {
																								   var mapper = new WeSayLiftDataMapper
																									   (
																									   _pathToLiftFile,
																									   semanticDomains,
																									   GetIdsOfSingleOptionFields
																										   (),
																									   progressState
																									   );

																								   return mapper;
																							   }
															 );
													 }
													 catch (LiftFormatException error)
													 {
														 ErrorReport.NotifyUserOfProblem(error.Message);
														 throw;
													 }
												 }
			).SingleInstance();

			builder.RegisterType<LexEntryRepository>().SingleInstance();
			//            builder.Register<LexEntryRepository>(
			//                 c => c.Resolve<IProgressNotificationProvider>().Go<LexEntryRepository>("Loading Dictionary",
			//                         progressState => new LexEntryRepository(_pathToLiftFile, progressState)));


			builder.Register<ICountGiver>(c => c.Resolve<LexEntryRepository>());


			//finish registering ViewTemplate related things
			builder.Register<ViewTemplate>(c => DefaultPrintingTemplate).Named<ViewTemplate>("PrintingTemplate").SingleInstance();
			builder.Register<IWritingSystemRepository>(c => DefaultViewTemplate.WritingSystems).ExternallyOwned().SingleInstance();

			RegisterChorusStuff(builder, viewTemplates.First().CreateChorusDisplaySettings());


			builder.Register<PublicationFontStyleProvider>(c => new PublicationFontStyleProvider(c.ResolveNamed<ViewTemplate>("PrintingTemplate"))).SingleInstance();

			builder.Register<IOptionListReader>(c => new DdpListReader()).Named<IOptionListReader>(LexSense.WellKnownProperties.SemanticDomainDdp4).SingleInstance();
			builder.Register<IOptionListReader>(c => new GenericOptionListReader()).SingleInstance();
			builder.Register<LocalizedListParser>(c => new LocalizedListParser()
			{
				SemanticDomainWs = WritingSystemIdForNamesAndQuestions,
				ApplicationCommonDirectory = BasilProject.ApplicationCommonDirectory,
				PathToWeSaySpecificFilesDirectoryInProject = Project.PathToWeSaySpecificFilesDirectoryInProject
			});


			builder.Register<PictureControl>(c => new PictureControl(Path.GetDirectoryName(PathToLiftFile), PathToPictures, GetFileLocator())).InstancePerDependency();

			//  builder.Register<ViewTemplate>(DefaultViewTemplate);

			//  builder.Register(DefaultViewTemplate);
			// can't currently get at the instance
			//someday: builder.Register<StringCatalog>(new StringCatalog()).ExternallyOwned();

			builder.RegisterType<CheckinDescriptionBuilder>().SingleInstance();
			var configuration = new ProjectFolderConfiguration(Path.GetDirectoryName(PathToConfigFile));
			LiftFolder.AddLiftFileInfoToFolderConfiguration(configuration);
			builder.RegisterInstance<Chorus.sync.ProjectFolderConfiguration>(configuration).SingleInstance();
			builder.RegisterType<ChorusBackupMaker>().SingleInstance();
			builder.RegisterType<UiConfigurationOptions>().SingleInstance();


			//it is sad that we initially used a static for logger, and that hasn't been completely undone yet.
			//but by registering it here, we at least make it possible for components to get access to it this
			//"proper" way.
			builder.Register<Logger>(c => Logger.Singleton).SingleInstance();
			builder.Register<ILogger>(c =>
										  {
											  var m = new MultiLogger();
											  Logger.Init();//it's ok if this was already done
											  m.Add(Logger.Singleton);
											  m.Add(c.Resolve<CheckinDescriptionBuilder>());
											  return m;
										  }).SingleInstance();

			//            var ap = new AudioPathProvider(Project.WeSayWordsProject.Project.PathToAudio,
			//                        () => entry.LexicalForm.GetBestAlternativeString(lexicalUnitField.WritingSystemIds));

			//            var x = _defaultViewTemplate.GetField(
			//            builder.Register<AudioPathProvider>(c=>new AudioPathProvider(PathToAudio, )));

			builder.Register(c =>
				new MediaNamingHelper(c.Resolve<ViewTemplate>().GetField(LexEntry.WellKnownProperties.LexicalUnit).WritingSystemIds)).InstancePerLifetimeScope();

			RegisterTextObjects(builder);

			_container = builder.Build();
		}

		private void RegisterTextObjects(ContainerBuilder builder)
		{
			if (GeckoOption)
			{
				builder.Register<IWeSayTextBox>(c =>
				{
					var m = new GeckoBox();
					return m;
				});
				builder.Register<IWeSayComboBox>(c =>
				{
					var m = new GeckoComboBox();
					return m;
				});
				builder.Register<IWeSayListView>(c =>
				{
					var m = new GeckoListView();
					return m;
				});
				builder.Register<IWeSayListBox>(c =>
				{
					var m = new GeckoListBox();
					return m;
				});
				builder.Register<IWeSayAutoCompleteTextBox>(c =>
				{
					var m = new GeckoAutoCompleteTextBox();
					return m;
				});
			}
			else
			{
				builder.Register<IWeSayTextBox>(c =>
				{
					var m = new WeSayTextBox();
					return m;
				});
				builder.Register<IWeSayComboBox>(c =>
				{
					var m = new WeSayComboBox();
					return m;
				});
				builder.Register<IWeSayListView>(c =>
				{
					var m = new WeSayListView();
					return m;
				});
				builder.Register<IWeSayListBox>(c =>
				{
					var m = new WeSayListBox();
					return m;
				});
				builder.Register<IWeSayAutoCompleteTextBox>(c =>
				{
					var m = new WeSayAutoCompleteTextBox();
					return m;
				});
			}
		}
		private void RegisterChorusStuff(ContainerBuilder builder, ChorusNotesDisplaySettings displaySettings)
		{
			//NB: currently, the ctor for ChorusSystem requires hg, since it gets or creates a repo in the path.
			if (!string.IsNullOrEmpty(Chorus.VcsDrivers.Mercurial.HgRepository.GetEnvironmentReadinessMessage("en")))
				return;

			//TODO: move all this stuff to ChorusSystem
			ChorusUIComponentsInjector.Inject(builder, Path.GetDirectoryName(PathToConfigFile));
			var chorusSystem = new ChorusSystem(Path.GetDirectoryName(PathToConfigFile));
			chorusSystem.DisplaySettings = displaySettings;
			chorusSystem.Init(String.Empty);

			builder.RegisterInstance<Chorus.UI.Review.NavigateToRecordEvent>(chorusSystem.NavigateToRecordEvent);
			builder.RegisterInstance<ChorusSystem>(chorusSystem);

			//            builder.Register<ChorusNotesSystem>(c=>
			//            {
			//                var system =c.Resolve<ChorusSystem>().GetNotesSystem(PathToLiftFile,
			//                                                         new NullProgress());
			//                system.IdGenerator = (target) => ((LexEntry) target).Guid.ToString();
			//              //  system.UrlGenerator = (target, id) => GetUrlFromLexEntry(target as LexEntry);
			//                return system;
			//            }).ContainerScoped();//TODO

			//add a factory which takes no parameters, which autofac can give to the NotesBrowserTask to use
			//to create this only if/when it is activated
			builder.Register<System.Func<Chorus.UI.Notes.Browser.NotesBrowserPage>>(c =>
			{
				var chorus = c.Resolve<ChorusSystem>();
				return () => chorus.WinForms.CreateNotesBrowser();
			}).SingleInstance();

			var mapping = new NotesToRecordMapping();
			mapping.FunctionToGetCurrentUrlForNewNotes = (entry, id) => GetUrlFromLexEntry(entry as LexEntry);
			mapping.FunctionToGoFromObjectToItsId = (entry) => (entry as LexEntry).Guid.ToString();
			builder.RegisterInstance<NotesToRecordMapping>(mapping);

			builder.Register<NotesBarView>(c =>
			{
				var system = c.Resolve<ChorusSystem>();
				var bar = system.WinForms.CreateNotesBar(PathToLiftFile, c.Resolve<NotesToRecordMapping>(), new NullProgress());
				bar.LabelWritingSystem = system.DisplaySettings.WritingSystemForNoteLabel;
				bar.MessageWritingSystem = system.DisplaySettings.WritingSystemForNoteContent;
				return bar;
			}).InstancePerDependency();
		}

		public IEnumerable<string> GetIdsOfSingleOptionFields()
		{
			foreach (Field field in DefaultViewTemplate.Fields)
			{
				if (field.DataTypeName == "Option")
					yield return field.FieldName;
			}
		}

		private void OnTouchCrossReferences(object sender, DoWorkEventArgs e)
		{
			DelegateQuery<LexEntry> xrefQuery = new DelegateQuery<LexEntry>(
				delegate (LexEntry entryToQuery)
				{
					IDictionary<string, object> tokenFieldsAndValues = new Dictionary<string, object>();

					LexRelationCollection relations = entryToQuery.GetProperty<LexRelationCollection>(LexEntry.WellKnownProperties.CrossReference);
					if (relations == null)
					{
						return new IDictionary<string, object>[0];
					}
					else
					{
						tokenFieldsAndValues.Add("Relation", relations.Relations);
						return new[] { tokenFieldsAndValues };
					}
				});
			GetLexEntryRepository().TouchAndSaveEntriesFromQuery(xrefQuery, "confer");
		}


		// This is only used in Addin.Transform.SfmTransformer to fix cross references
		// that have been save in the lift as NFD when they should be NFC - see WS-356
		public void TouchAllIfCrossReferences()
		{
			ViewTemplate template = ViewTemplates.First();
			// only touch anything if the CrossReference field is enabled, it isn't by default
			if (template.GetField(LexEntry.WellKnownProperties.CrossReference).Enabled == true)
			{
#if TRUE
				// only show dialog if lots of entries
				if (GetLexEntryRepository().CountAllItems() < 10000)
				{
					OnTouchCrossReferences(null, null);
				}
				else using (SIL.Windows.Forms.SimpleMessageDialog msgdialog = new SIL.Windows.Forms.SimpleMessageDialog("Migrating cross references.", "Cross reference migration"))
					{
						msgdialog.Show();
						OnTouchCrossReferences(null, null);
					}
#else // doesn't work, shows dialog but doesn't get workerended event so it hangs until you kill it
				if (SIL.Reporting.ErrorReport.IsOkToInteractWithUser)
				{
					var dialog = new ProgressDialog();
					var worker = new BackgroundWorker();
					worker.DoWork += OnTouchCrossReferences;
					worker.RunWorkerCompleted += OnWorkerCompleted;
					dialog.BackgroundWorker = worker;
					dialog.CanCancel = false;
					dialog.BarStyle = ProgressBarStyle.Marquee;
					dialog.Text = "Cross Reference migration...";
					dialog.StatusText = "Cross Reference migration...";
					dialog.ShowDialog();
				}
				else
				{
					OnTouchCrossReferences(null, null);  // this ensures that migration will occur even when the dialog box isn't shown i.e. during tests
				}
#endif
			}
		}

		public OptionsList GetSemanticDomainsList()
		{
			return GetOptionsList(LexSense.WellKnownProperties.SemanticDomainDdp4);
		}

		public LexEntryRepository GetLexEntryRepository()
		{
			return _container.Resolve<LexEntryRepository>();

		}

		//        //provide an IServiceProvider facade around our DI Container
		//        public object GetService(Type serviceType)
		//        {
		//            return _container.Resolve(serviceType);
		//        }

		private void LoadUserConfig()
		{
			var dom = new XmlDocument();
			BackupMaker = null;
			if (File.Exists(PathToUserSpecificConfigFile(ProjectDirectoryPath)))
			{
				dom.Load(PathToUserSpecificConfigFile(ProjectDirectoryPath));
				BackupMaker = ChorusBackupMaker.CreateFromDom(dom, _container.Resolve<CheckinDescriptionBuilder>());
				UiOptions = UiConfigurationOptions.CreateFromDom(dom);
			}

			if (BackupMaker == null)
			{
				BackupMaker = _container.Resolve<ChorusBackupMaker>();
			}

			if (UiOptions == null)
			{
				UiOptions = _container.Resolve<UiConfigurationOptions>();
			}
		}

		private static void MoveFilesFromOldDirLayout(string projectDir)
		{
			MoveWeSayContentsToProjectDir(projectDir, "common");
			MoveWeSayContentsToProjectDir(projectDir, "wesay");
			MoveExportAndLexiqueProFilesToNewDirStructure(projectDir);
		}

		private static void MoveWeSayContentsToProjectDir(string projectDir, string subDirName)
		{
			try
			{
				string sourceDir = Path.Combine(projectDir, subDirName);
				//overcome a problem someone might have, where the installer places the new sample files
				//right in there alongside the old.
				if (projectDir.Contains("biatah"))
				{
					if (Directory.Exists(sourceDir))
					{
						Directory.Delete(sourceDir, true);
						return;
					}
				}
				string targetDir = projectDir;
				if (Directory.Exists(sourceDir))
				{
					MoveSubDirectory(projectDir, sourceDir, "pictures");
					MoveSubDirectory(projectDir, sourceDir, "cache");

					foreach (string source in Directory.GetFiles(sourceDir))
					{
						if (source.Contains("liftold") || source.Contains("lift.bak"))
						{
							File.Delete(source);
						}
						else
						{
							string target = Path.Combine(targetDir, Path.GetFileName(source));
							if (File.Exists(target))
							{
								File.Delete(target);
							}
							File.Move(source, target);
						}
					}
					try
					{
						Directory.Delete(sourceDir);
					}
					catch (Exception)
					{
						//no big deal if other files prevent deleting it
					}
				}
			}
			catch (Exception err)
			{
				ApplicationException e =
						new ApplicationException(
								"Error while trying to migrate to new file structure. ", err);
				ErrorReport.ReportFatalException(e);
			}
		}

		private static void MoveSubDirectory(string targetParentDir,
											 string subDirName,
											 string directoryToMoveName)
		{
			string moveDir = Path.Combine(subDirName, directoryToMoveName);
			if (Directory.Exists(moveDir))
			{
				string dest = Path.Combine(targetParentDir, directoryToMoveName);
				Directory.Move(moveDir, dest);
			}
		}

		private static void MoveExportAndLexiqueProFilesToNewDirStructure(string projectDir)
		{
			try
			{
				string presumedExportName = Path.GetFileName(projectDir);
				string targetDir = Path.Combine(projectDir, "export");
				Directory.CreateDirectory(targetDir);
				foreach (string source in
						Directory.GetFiles(projectDir, presumedExportName + "-sfm*.*"))
				{
					string target = Path.Combine(targetDir, Path.GetFileName(source));
					if (File.Exists(target))
					{
						File.Delete(target);
					}
					File.Move(source, target);
				}
			}
			catch (Exception err)
			{
				ApplicationException e =
						new ApplicationException(
								"Error while trying to move export files to new structure. ", err);
				ErrorReport.ReportFatalException(e);
			}
		}

		public bool MigrateConfigurationXmlIfNeeded()
		{
			var m = new ConfigurationMigrator();
			return m.MigrateConfigurationXmlIfNeeded(PathToConfigFile, PathToConfigFile);
		}

		public XPathNodeIterator GetAddinNodes()
		{
			return GetAddinNodes(GetConfigurationDoc());
		}

		private static XPathNodeIterator GetAddinNodes(IXPathNavigable configDoc)
		{
			try
			{
				if (configDoc != null)
				{
					return configDoc.CreateNavigator().Select("configuration/addins/addin");
				}
				return null;
			}
			catch (Exception error)
			{
				ErrorReport.NotifyUserOfProblem(
						"There was a problem reading the addins-settings xml. {0}", error.Message);
				return null;
			}
		}

		public ProjectInfo GetProjectInfoForAddin()
		{
			return new ProjectInfo(Name,
#if __MonoCS__
								   ApplicationSharedDirectory,
#else
								   ApplicationRootDirectory,
#endif
								   ProjectDirectoryPath,
								   PathToLiftFile,
								   PathToExportDirectory,
								   GetFilesBelongingToProject(ProjectDirectoryPath),
								   AddinSet.Singleton.LocateFile,
								   WritingSystems,
								   ServiceLocator,
								   this);
		}
		public IServiceLocator ServiceLocator
		{
			get { return new ServiceLocatorAdapter(_container); }
		}

		private XPathDocument GetConfigurationDoc()
		{
			XPathDocument projectDoc = null;
			if (File.Exists(PathToConfigFile))
			{
				try
				{
					projectDoc = new XPathDocument(PathToConfigFile);
					//projectDoc.Load(Project.PathToConfigFile);
				}
				catch (Exception e)
				{
					ErrorReport.NotifyUserOfProblem("There was a problem reading the wesay config xml: " + e.Message);
					projectDoc = null;
				}
			}
			return projectDoc;
		}



		public static string PathToDefaultConfig
		{
			get { return Path.Combine(ApplicationCommonDirectory, "default.WeSayConfig"); }
		}


		public static void CreateEmptyProjectFiles(string projectDirectoryPath, string languageTag)
		{
			string name = Path.GetFileName(projectDirectoryPath);
			CreateEmptyProjectFiles(projectDirectoryPath, name, languageTag);
		}

		/// <summary>
		/// note this now works for folders that have lift stuff, just no wesay stuff
		/// </summary>
		/// <param name="projectDirectoryPath"></param>
		/// <param name="projectName"></param>
		public static void CreateEmptyProjectFiles(string projectDirectoryPath, string projectName, string languageTag)
		{
			if (!Directory.Exists(projectDirectoryPath))
			{
				Directory.CreateDirectory(projectDirectoryPath);
			}

			MigrateProjectFilesAndCheckForOrphanedWritingSystems(projectDirectoryPath);
			if (Directory.GetFiles(GetPathToLdmlWritingSystemsFolder(projectDirectoryPath)).Count() == 0)
			{
				CopyWritingSystemsFromApplicationCommonDirectoryToNewProject(projectDirectoryPath);
			}
			if (!Directory.Exists(GetPathToSharedSettingsFolder(projectDirectoryPath)))
			{
				Directory.CreateDirectory(GetPathToSharedSettingsFolder(projectDirectoryPath));
			}

			string pathToConfigFile = GetPathToConfigFile(projectDirectoryPath, projectName);
			File.Copy(PathToDefaultConfig, pathToConfigFile, true);

			var pathToLiftFile = Path.Combine(projectDirectoryPath, projectName + ".lift");
			if (!File.Exists(pathToLiftFile))
			{
				Utilities.CreateEmptyLiftFile(pathToLiftFile, LiftWriter.ProducerString, false);
			}

			//hack
			StickDefaultViewTemplateInNewConfigFile(projectDirectoryPath, pathToConfigFile, languageTag);
		}

		/// <summary>
		/// this is something of a hack, because we currently create the default viewtemplate from
		/// code, but everything else from template xml files.  So this opens up the default config
		/// and sticks a nice new code-computed default view template into it.
		/// </summary>
		/// <param name="projectPath"></param>
		/// <param name="pathToConfigFile"></param>
		private static void StickDefaultViewTemplateInNewConfigFile(string projectPath, string pathToConfigFile, string languageTag)
		{
			var userSettingsDataMapper =
				new UserLexiconSettingsWritingSystemDataMapper(new FileSettingsStore(
					LexiconSettingsFileHelper.GetUserLexiconSettingsPath(projectPath)));
			var projectSettingsDataMapper =
				new ProjectLexiconSettingsWritingSystemDataMapper(new FileSettingsStore(
					LexiconSettingsFileHelper.GetProjectLexiconSettingsPath(projectPath)));
			ICustomDataMapper<WritingSystemDefinition>[] customDataMapper =
			{
				userSettingsDataMapper,
				projectSettingsDataMapper
			};

			var writingSystems = LdmlInFolderWritingSystemRepository.Initialize(
				GetPathToLdmlWritingSystemsFolder(projectPath),
				customDataMapper,
				null,
				OnWritingSystemMigration,
				OnWritingSystemLoadProblem);

			var template = ViewTemplate.MakeMasterTemplate(writingSystems, languageTag);
			var builder = new StringBuilder();
			using (var writer = XmlWriter.Create(builder, CanonicalXmlSettings.CreateXmlWriterSettings(ConformanceLevel.Fragment)))
			{
				template.Write(writer);
			}

			var doc = new XmlDocument();
			doc.Load(pathToConfigFile);
			var old_components = doc.SelectSingleNode("configuration").SelectSingleNode("components");
			var new_components = doc.CreateElement("components");
			if (old_components == null)
			{
				//add node
				var elem = doc.SelectSingleNode("configuration").AppendChild(doc.CreateElement("components"));
				elem.InnerXml = builder.ToString();
			}
			else
			{
				//replace node
				new_components.InnerXml = builder.ToString();
				doc.SelectSingleNode("configuration").ReplaceChild(new_components, old_components);
			}
			doc.Save(pathToConfigFile);
		}

		public string PathToConfigFile
		{
			get
			{
				return GetPathToConfigFile(
					PathToWeSaySpecificFilesDirectoryInProject,
					GetProjectNameFromLiftFilePath(PathToLiftFile)
				);
			}
		}

		public static string PathToUserSpecificConfigFile(string projectDirectory)
		{
			return Path.Combine(projectDirectory, System.Environment.UserName + ".WeSayUserConfig");
		}

		private static string GetPathToConfigFile(string directoryInProject, string name)
		{
			return String.IsNullOrEmpty(name) ? "" : Path.Combine(directoryInProject, name + ".WeSayConfig");
		}

		/// <summary>
		/// used for upgrading
		/// </summary>
		public string PathToOldProjectTaskInventory
		{
			get { return Path.Combine(PathToWeSaySpecificFilesDirectoryInProject, "tasks.xml"); }
		}

		public string PathToExportDirectory
		{
			get { return Path.Combine(ProjectDirectoryPath, "export"); }
		}

		public override void Dispose()
		{
			base.Dispose();
			if (_container != null)
			{
				_container.Dispose();//this will dispose of objects in the container (at least those with the normal "lifetype" setting)
			}
		}

		public override string Name
		{
			get { return GetProjectNameFromLiftFilePath(PathToLiftFile); }
		}

		public string PathToLiftFile
		{
			get
			{
				if (String.IsNullOrEmpty(_pathToLiftFile))
				{
					_pathToLiftFile = LiftFileLocator.LocateInDirectory(ProjectDirectoryPath);
				}
				return _pathToLiftFile;
			}

			set
			{
				_pathToLiftFile = value;
				if (value == null)
				{
					ProjectDirectoryPath = null;
				}
				else
				{
					ProjectDirectoryPath = Path.GetDirectoryName(value);
					// Directory.GetParent(value).Parent.FullName;
				}
			}
		}

		public string PathToLiftBackupDir
		{
			get { return PathToLiftFile + " incremental xml backup"; }
		}

		public string PathToCache
		{
			get
			{
				if (_cacheLocationOverride != null)
				{
					return _cacheLocationOverride;
				}
				else
				{
					return GetPathToCacheFromPathToLift(PathToLiftFile);
				}
			}
		}

		public string PathToPictures
		{
			get { return Path.Combine(PathToWeSaySpecificFilesDirectoryInProject, "pictures"); }
		}
		public string PathToAudio
		{
			get { return Path.Combine(PathToWeSaySpecificFilesDirectoryInProject, "audio"); }
		}

		private static string GetPathToCacheFromPathToLift(string pathToLift)
		{
			return Path.Combine(Path.GetDirectoryName(pathToLift), "Cache");
		}

		public string PathToRepository
		{
			get { return PathToLiftFile; }
		}


		/// <summary>
		/// at the momement, project is a file locator for old code, but we want to move
		/// towards removing that responsibility, perhaps by adding a locator to the container
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public string LocateFile(string fileName)
		{
			return GetFileLocator().LocateFile(fileName);
		}
		public string LocateFile(string fileName, string descriptionForErrorMessage)
		{
			return GetFileLocator().LocateFile(fileName, descriptionForErrorMessage);
		}

		public string LocateOptionalFile(string fileName)
		{
			return GetFileLocator().LocateOptionalFile(fileName);
		}

		public string LocateFileWithThrow(string fileName)
		{
			return GetFileLocator().LocateFileWithThrow(fileName);
		}

		public string LocateDirectory(string directoryName)
		{
			return GetFileLocator().LocateDirectory(directoryName);
		}

		public string LocateDirectoryWithThrow(string directoryName)
		{
			return GetFileLocator().LocateDirectoryWithThrow(directoryName);
		}

		public string LocateDirectory(string directoryName, string descriptionForErrorMessage)
		{
			return GetFileLocator().LocateDirectory(directoryName, descriptionForErrorMessage);
		}

		public IFileLocator CloneAndCustomize(IEnumerable<string> addedSearchPaths)
		{
			throw new NotImplementedException(); // just wouldn't make sense, since this entire thing has the IFileLocator interface
		}

		/// <summary>
		/// Find the file, starting with the project dirs and moving to the app dirs.
		/// This allows a user to override an installed file by making thier own.
		/// </summary>
		/// <returns></returns>
		private FileLocator GetFileLocator()
		{
			return new FileLocator(new[] { PathToWeSaySpecificFilesDirectoryInProject,
												  ApplicationCommonDirectory, DirectoryOfTheApplicationExecutable, TopDevDirectory});
		}

		public string PathToWeSaySpecificFilesDirectoryInProject => ProjectDirectoryPath;

		public string PathOldToWeSaySpecificFilesDirectoryInProject => Path.Combine(ProjectDirectoryPath, "wesay");

		public ViewTemplate DefaultViewTemplate
		{
			get
			{
				if (_defaultViewTemplate == null)
				{
					_defaultViewTemplate = _container.Resolve<ViewTemplate>();//enhance won't work when there's multiple
				}
				return _defaultViewTemplate;
			}
		}

		public IList<ViewTemplate> ViewTemplates
		{
			get
			{
				if (_viewTemplates == null)
				{
					// //container change
					//enhance to handle multiple templates
					_viewTemplates = new List<ViewTemplate>();
					//if (_container != null)
					{
						ViewTemplate template;
						if (_container.TryResolve<ViewTemplate>(out template))
						{
							_viewTemplates.Add(template);
						}
					}
					//                    InitializeViewTemplatesFromProjectFiles();
				}
				return _viewTemplates;
			}
		}

		public IEnumerable OptionFieldNames
		{
			get
			{
				List<string> names = new List<string>();

				foreach (Field field in DefaultViewTemplate.Fields)
				{
					if (field.DataTypeName == "Option")
					{
						names.Add(field.FieldName);
					}
				}
				return names;
			}
		}

		public IEnumerable OptionCollectionFieldNames
		{
			get
			{
				List<string> names = new List<string>();

				foreach (Field field in DefaultViewTemplate.Fields)
				{
					if (field.DataTypeName == "OptionCollection")
					{
						names.Add(field.FieldName);
					}
				}
				return names;
			}
		}

		//used when building the cache, so we can build it in a temp directory
		public string CacheLocationOverride
		{
			set { _cacheLocationOverride = value; }
		}



		public AddinSet Addins
		{
			get { return _addins; }
		}

		public IList<LexRelationType> RelationTypes
		{
			get
			{
				if (_relationTypes == null)
				{
					_relationTypes = new List<LexRelationType>();
					_relationTypes.Add(new LexRelationType("RelationToOneEntry",
														   LexRelationType.Multiplicities.One,
														   LexRelationType.TargetTypes.Entry));
				}
				return _relationTypes;
			}
		}

		public WritingSystemDefinition HeadWordWritingSystem
		{
			get
			{
				return DefaultViewTemplate.GetDefaultWritingSystemForField(LexEntry.WellKnownProperties.LexicalUnit);
			}
		}

		public ViewTemplate DefaultPrintingTemplate
		{
			get { return DefaultViewTemplate; }
		}

		static public bool PreventBackupForTests
		{
			get;
			set;
		}

		public ChorusBackupMaker BackupMaker
		{
			get { return _backupMaker; }
			set
			{
				if (!PreventBackupForTests)
					_backupMaker = value;
			}
		}

		public ILifetimeScope Container
		{
			get { return _container; }
		}

		public static string NewProjectDirectory
		{
			get
			{
				return Path.Combine(
				   Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WeSay");
			}
		}

		public override void Save()
		{
			_addins.InitializeIfNeeded(); // must be done before locking file for writing

			var defaultWs = DefaultViewTemplate.GetMeaningField().WritingSystemIds.FirstOrDefault();
			//this adds a writing system to any enabled fields that don't have one
			foreach (var field in ViewTemplates.SelectMany(x => x.Fields))
			{
				if (field.Enabled && field.WritingSystemIds.Count == 0)
				{
					field.WritingSystemIds.Add(defaultWs);
				}
			}

			var pendingConfigFile = new TempFileForSafeWriting(Project.PathToConfigFile);

			var writer = XmlWriter.Create(pendingConfigFile.TempFilePath, CanonicalXmlSettings.CreateXmlWriterSettings());
			writer.WriteStartDocument();
			writer.WriteStartElement("configuration");
			writer.WriteAttributeString("version", ConfigFile.LatestVersion.ToString());


			writer.WriteStartElement("components");
			writer.WriteElementString("dataFormat", DataFormat.ToString());
			foreach (ViewTemplate template in ViewTemplates)
			{
				template.Write(writer);
			}
			writer.WriteEndElement();

			TaskCollection tasks;
			if (_container.TryResolve<TaskCollection>(out tasks))
				tasks.Write(writer);


			if (EditorsSaveNow != null)
			{
				EditorsSaveNow.Invoke(writer, null);
			}

			_addins.Save(writer);

			writer.WriteEndDocument();
			writer.Close();
			foreach (var modifiedOptionsList in _modifiedOptionsLists)
			{
				var fileAssociatedWithOptionsList =
					_optionLists.Where(opt => opt.Value == modifiedOptionsList).Select(opt => opt.Key).First();
				//notice that we always save to the project directory, even if we started with the
				//one in the program files directory.
				string path = Path.Combine(PathToWeSaySpecificFilesDirectoryInProject, fileAssociatedWithOptionsList);

				try
				{
					_optionLists[fileAssociatedWithOptionsList].SaveToFile(path);
				}
				catch (Exception error)
				{
					ErrorReport.NotifyUserOfProblem(
						"WeSay Config could not save the options list {0}.  Please make sure it is not marked as 'read-only'.  The error was: {1}",
						path,
						error.Message);
				}
			}

			//Now let's replace and delete writing systems in OptionLists
			foreach (var kvp in _changedWritingSystemIds)
			{
				foreach (var filePath in Directory.GetFiles(ProjectDirectoryPath))
				{
					try
					{
						var helper = new WritingSystemsInOptionsListFileHelper(WritingSystems, filePath);
						if (String.IsNullOrEmpty(kvp.Value))
						{
							helper.DeleteWritingSystemId(kvp.Key);
						}
						else
						{
							helper.ReplaceWritingSystemId(kvp.Key, kvp.Value);
						}
					}
					catch (IOException e)
					{
						ErrorReport.NotifyUserOfProblem(e.Message + " " + PathToLiftFile);
					}
				}
			}


			pendingConfigFile.WriteWasSuccessful();

			base.Save();
			try
			{
				CommitWritingSystemIdChangesToLiftFile();
			}
			catch (IOException e)
			{
				ErrorReport.NotifyUserOfProblem(e.Message + " " + PathToLiftFile);
			}

			SaveUserSpecificConfiguration();
			BackupNow();

		}

		private void SaveUserSpecificConfiguration()
		{
			var pendingConfigFile = new TempFileForSafeWriting(PathToUserSpecificConfigFile(ProjectDirectoryPath));

			var writer = XmlWriter.Create(pendingConfigFile.TempFilePath, CanonicalXmlSettings.CreateXmlWriterSettings());
			writer.WriteStartDocument();
			writer.WriteStartElement("configuration");
			writer.WriteAttributeString("version", CurrentWeSayUserSpecificConfigFileVersion.ToString());


			if (BackupMaker != null)
				BackupMaker.Save(writer);

			if (UiOptions != null)
				UiOptions.Save(writer);

			writer.WriteEndDocument();
			writer.Close();

			pendingConfigFile.WriteWasSuccessful();
		}

		public Field GetFieldFromDefaultViewTemplate(string fieldName)
		{
			foreach (Field field in DefaultViewTemplate.Fields)
			{
				if (field.FieldName == fieldName)
				{
					return field;
				}
			}
			return null;
		}

		public OptionsList GetOptionsList(string fieldName)
		{
			Field field = GetFieldFromDefaultViewTemplate(fieldName);
			if (field == null)
			{
				return null;
			}
			return GetOptionsList(field, false);
		}

		public OptionsList GetOptionsList(Field field, bool createIfFileMissing)
		{
			if (String.IsNullOrEmpty(field.OptionsListFile))
			{
				throw new ConfigurationException(
						"The administrator needs to declare an options list file for the field {0}. This can be done under the Fields tab of the WeSay Configuration Tool.",
						field.FieldName);
			}
			OptionsList list;
			if (_optionLists.TryGetValue(field.OptionsListFile, out list))
			{
				return list;
			}

			string pathInProject = Path.Combine(PathToWeSaySpecificFilesDirectoryInProject,
												field.OptionsListFile);
			// Semantic domain field does its own checking for the location and existence of the file
			if ((field.FieldName == LexSense.WellKnownProperties.SemanticDomainDdp4) || (File.Exists(pathInProject)))
			{
				LoadOptionsList(field.FieldName, pathInProject);
			}
			else
			{
				string pathInProgramDir = Path.Combine(ApplicationCommonDirectory,
													   field.OptionsListFile);
				if (!File.Exists(pathInProgramDir))
				{
					if (createIfFileMissing)
					{
						list = new OptionsList();
						_optionLists.Add(Path.GetFileName(pathInProject), list);
						return list;
					}
					else
					{
						throw new ConfigurationException(
								"Could not find the optionsList file {0}. Expected to find it at: {1} or {2}",
								field.OptionsListFile,
								pathInProject,
								pathInProgramDir);
					}
				}
				LoadOptionsList(field.FieldName, pathInProgramDir);
			}

			return _optionLists[field.OptionsListFile];
		}

		public void MarkOptionListAsUpdated(OptionsList list)
		{
			if (!_modifiedOptionsLists.Contains(list))
			{
				_modifiedOptionsLists.Add(list);
			}
		}

		private void LoadOptionsList(string fieldName, string pathToOptionsList)
		{
			OptionsList list;
			string name = Path.GetFileName(pathToOptionsList);
			if (fieldName == LexSense.WellKnownProperties.SemanticDomainDdp4)
			{
				var parser = _container.Resolve<LocalizedListParser>();
				parser.ReadListFile();
				list = parser.OptionsList;
			}
			else
			{
				IOptionListReader reader;
				object r;
				//first, try for a reader named after the field
				if (_container.TryResolveNamed(fieldName, typeof(IOptionListReader), out r))
				{
					reader = r as IOptionListReader;
				}
				else
				{
					reader = _container.Resolve<IOptionListReader>();
				}
				list = reader.LoadFromFile(pathToOptionsList);
			}
			foreach (var oldNewId in _changedWritingSystemIds)
			{
				ChangeIdInLoadedOptionListIfNecassary(oldNewId.Key, oldNewId.Value, list);
			}
			_optionLists.Add(name, list);
		}

		/// <summary>
		/// Used with xml export, e.g. with LIFT to set the proper "range" for option fields
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, string> GetFieldToOptionListNameDictionary()
		{
			Dictionary<string, string> fieldToOptionListName = new Dictionary<string, string>();
			foreach (Field field in DefaultViewTemplate.Fields)
			{
				if (field.OptionsListFile != null && field.OptionsListFile.Trim() != "")
				{
					fieldToOptionListName.Add(field.FieldName,
											  GetListNameFromFileName(field.OptionsListFile));
				}
			}
			return fieldToOptionListName;
		}

		private static string GetListNameFromFileName(string file)
		{
			return Path.GetFileNameWithoutExtension(file);
			//file.Substring(0, file.IndexOf(".xml"));
		}



		private delegate void DelegateThatTouchesLiftFile(string pathToLiftFile);

		private bool DoSomethingToLiftFile(DelegateThatTouchesLiftFile doSomething)
		{
			if (!File.Exists(PathToLiftFile))
				return false;
			try
			{
				doSomething(PathToLiftFile);
				return true;
			}
			catch (Exception error)
			{
				ErrorReport.NotifyUserOfProblem("Another program has WeSay's dictionary file open, so we cannot make the input system change.  Make sure WeSay isn't running.");
				return false;
			}

		}

		public bool MakeFieldNameChange(Field field, string oldName)
		{
			Debug.Assert(!String.IsNullOrEmpty(oldName));
			if (string.IsNullOrEmpty(oldName))
			{
				return false;
			}
			oldName = Regex.Escape(oldName);

			//NB: we're just using regex, here, not xpaths which in this case
			//would be nice (e.g., "name" is a pretty generic thing to be changing)
			return DoSomethingToLiftFile((p) =>
				 {
					 //traits
					 if (field.DataTypeName == Field.BuiltInDataType.Option.ToString() ||
						 field.DataTypeName ==
						 Field.BuiltInDataType.OptionCollection.ToString())
					 {
						 FileUtils.GrepFile(p,
								  string.Format("name\\s*=\\s*[\"']{0}[\"']", oldName),
								  string.Format("name=\"{0}\"", field.FieldName));
					 }
					 else
					 {
						 //<field>s
						 FileUtils.GrepFile(p,
								  string.Format("type\\s*=\\s*[\"']{0}[\"']", oldName),
								  string.Format("type=\"{0}\"", field.FieldName));
					 }
				 });
		}

		private void CommitWritingSystemIdChangesToLiftFile()
		{
			var helper = new WritingSystemsInLiftFileHelper(WritingSystems, PathToLiftFile);
			foreach (var kvp in _changedWritingSystemIds)
			{
				if (String.IsNullOrEmpty(kvp.Value))
				{
					helper.DeleteWritingSystemId(kvp.Key);
				}
				else
				{
					helper.ReplaceWritingSystemId(kvp.Key, kvp.Value);
				}

			}
		}

		public void MakeWritingSystemIdChange(string oldId, string newId)
		{
			//this is the case the first time someone changes a writing system Id
			if (!_changedWritingSystemIds.Any(kvp => kvp.Key.Equals(oldId, StringComparison.OrdinalIgnoreCase)))
			{
				_changedWritingSystemIds.Add(oldId, newId);
			}
			// this is the case if they have changed the writing system id before
			else if (_changedWritingSystemIds.Any(kvp => kvp.Value.Equals(oldId, StringComparison.OrdinalIgnoreCase)))
			{
				var key = _changedWritingSystemIds.First(kvp => kvp.Value.Equals(oldId, StringComparison.OrdinalIgnoreCase)).Key;
				_changedWritingSystemIds[key] = newId;
			}
			DefaultViewTemplate.OnWritingSystemIDChange(oldId, newId);

			foreach (var optionlist in _optionLists.Values)
			{
				ChangeIdInLoadedOptionListIfNecassary(oldId, newId, optionlist);
			}

			if (WritingSystemChanged != null)
			{
				StringPair p = new StringPair();
				p.from = oldId;
				p.to = newId;
				WritingSystemChanged(this, p);
			}
		}

		public void MakeMeaningFieldChange(string oldId, string newId)
		{
			//change meaning field in template
			DefaultViewTemplate.OnMeaningFieldChange(newId);

			// lets tasks know that meaning field has changed
			if (MeaningFieldChanged != null)
			{
				StringPair p = new StringPair();
				p.from = oldId;
				p.to = newId;
				MeaningFieldChanged(this, p);
			}
		}

		private void ChangeIdInLoadedOptionListIfNecassary(string oldId, string newId, OptionsList optionlist)
		{

			var abbreviationMultiText = new List<MultiText>(optionlist.Options.Select(option => option.Abbreviation));
			var nameMultiText = new List<MultiText>(optionlist.Options.Select(option => option.Name));
			var descriptionMultiText = new List<MultiText>(optionlist.Options.Select(option => option.Description));

			var multiTextsToChange = abbreviationMultiText.Concat(nameMultiText).Concat(descriptionMultiText);

			if (multiTextsToChange.Any())
			{
				MarkOptionListAsUpdated(optionlist);
			}

			foreach (var multiText in multiTextsToChange.Where(mt => mt.ContainsAlternative(oldId)))
			{
				var existingLanguageFormWithOldId = multiText.Find(oldId);
				var existingLanguageFormWithNewId = multiText.Find(newId);

				//If a non empty languageForm with the newId already exists, keep it around. Else delete it and change the writing system in the language form with the oldId
				if (existingLanguageFormWithNewId == null)
				{
					existingLanguageFormWithOldId.WritingSystemId = newId;
				}
				else if (String.IsNullOrEmpty(existingLanguageFormWithNewId.Form))
				{
					multiText.RemoveLanguageForm(existingLanguageFormWithNewId);
					existingLanguageFormWithOldId.WritingSystemId = newId;
				}
				else
				{
					multiText.RemoveLanguageForm(existingLanguageFormWithOldId);
				}
			}
		}

		public void DeleteWritingSystemId(string id)
		{
			// Check whether the user has been playing around, unable to make up his (or her) mind.
			if (_changedWritingSystemIds.ContainsKey(id))
				_changedWritingSystemIds[id] = String.Empty;
			else
				_changedWritingSystemIds.Add(id, String.Empty); //adding it to the _changedWritingSystemIds makes sure that all the changes are made in the correct order

			DefaultViewTemplate.DeleteWritingSystem(id);

			if (WritingSystemDeleted != null)
			{
				WritingSystemDeleted(this, new WritingSystemDeletedEventArgs(id));
			}

			foreach (var optionsList in _optionLists.Values)
			{
				DeleteIdInLoadedOptionListsIfNecassary(id, optionsList);
			}
		}

		private void DeleteIdInLoadedOptionListsIfNecassary(string id, OptionsList optionlist)
		{

			var abbreviationMultiText = new List<MultiText>(optionlist.Options.Select(option => option.Abbreviation));
			var nameMultiText = new List<MultiText>(optionlist.Options.Select(option => option.Name));
			var descriptionMultiText = new List<MultiText>(optionlist.Options.Select(option => option.Description));

			var multiTextsToChange = abbreviationMultiText.Concat(nameMultiText).Concat(descriptionMultiText);

			if (multiTextsToChange.Any())
			{
				MarkOptionListAsUpdated(optionlist);
			}

			foreach (var multiText in multiTextsToChange.Where(mt => mt.ContainsAlternative(id)))
			{
				var languageFormWithId = multiText.Find(id);
				if (languageFormWithId != null)
				{
					multiText.RemoveLanguageForm(languageFormWithId);
				}
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="pathToLift"></param>
		/// <returns>true if it displayed an error message</returns>
		//        public static bool CheckLiftAndReportErrors(string pathToLift)
		//        {
		//            try
		//            {
		//                string errors = Validator.GetAnyValidationErrors(Project.PathToLiftFile);
		//                if (!String.IsNullOrEmpty(errors))
		//                {
		//                    ErrorReport.NotifyUserOfProblem(
		//                            "The dictionary file at {0} does not conform to the LIFT format used by this version of WeSay.  The RNG validator said: {1}.",
		//                            pathToLift,
		//                            errors);
		//                    return true;
		//                }
		//            }
		//            catch (Exception e)
		//            {
		//                ErrorReport.ReportNonFatalException(e);
		//                return true;
		//            }
		//            return false;
		//        }

		public bool LiftHasMatchingElement(string element, string attribute, string attributeValue)
		{
			using (XmlReader reader = XmlReader.Create(PathToLiftFile))
			{
				while (reader.ReadToFollowing(element))
				{
					string v = reader.GetAttribute(attribute);
					if (!String.IsNullOrEmpty(v) && v == attributeValue)
					{
						return true; //found it
					}
				}
			}
			return false;
		}

		public bool IsWritingSystemUsedInLiftFile(string id)
		{
			if (!File.Exists(PathToLiftFile))
			{
				return false;
			}
			string regex = string.Format("lang\\s*=\\s*[\"']{0}[\"']", Regex.Escape(id));
			throw new NotImplementedException();
			//todo find grep here
			// return FileUtils.GrepFile(PathToLiftFile, regex);
		}

		/// <summary>
		/// Files to process when backing up or checking in
		/// </summary>
		/// <param name="pathToProjectRoot"></param>
		/// <returns></returns>
		public static string[] GetFilesBelongingToProject(string pathToProjectRoot)
		{
			List<String> files = new List<string>();
			string[] allFiles = Directory.GetFiles(pathToProjectRoot,
												   "*",
												   SearchOption.AllDirectories);
			string[] antipatterns = { "Cache", "cache", ".bak", ".old", ".liftold", ".WeSayUserMemory" };

			foreach (string file in allFiles)
			{
				if (!Matches(file, antipatterns))
				{
					files.Add(file);
				}
			}
			return files.ToArray();
		}

		private static bool Matches(string file, IEnumerable<string> antipatterns)
		{
			foreach (string s in antipatterns)
			{
				if (file.Contains(s))
				{
					return true;
				}
			}
			return false;
		}

		public void BackupNow()
		{
			try
			{
				if (BackupMaker != null)//it will for many tests, which don't need to be slowed down by all this
					BackupMaker.BackupNow(ProjectDirectoryPath, UiOptions.Language, PathToLiftFile);
			}
			catch (Exception error)
			{
				ErrorReport.NotifyUserOfProblem(string.Format("WeSay was not able to do a backup.\r\nReason: {0}", error.Message));
			}
		}

		/// <summary>
		/// Call this when something changed, so we might want to backup or sync
		/// </summary>
		/// <param name="description"></param>
		public void ConsiderSynchingOrBackingUp(string description)
		{
			//nb: we have multiple lengths we could go to eventually, perhaps with different rules:
			//      commit locally, commit to local backup, commit peers on LAN, commit across internet

			//TODO: follow PT and OurWord to a once a day system, which
			//will take saving the last backkup time somewhere else
			//(or querying the repo)

			//this is a temporary hack... we don't want to backup on startup,
			//it slows the startup time too much
			if (BackupMaker.TimeOfLastBackupAttempt == default(DateTime))
			{
				BackupMaker.ResetTimeOfLastBackup();
			}

			TimeSpan diff = DateTime.Now - BackupMaker.TimeOfLastBackupAttempt;
			if (diff.TotalMinutes > 20)
			{
				BackupNow();
			}
		}


		/// <summary>
		/// this is a transition sort of thing during refactoring... the problem is the
		/// many tests that we need to change if/when we change the nature of the Tasks property
		/// </summary>
		public void LoadTasksFromConfigFile()
		{
			ConfigFileReader configReader = _container.Resolve<ConfigFileReader>();
			TaskConfigurations = configReader.GetTasksConfigurations(_container);
			Tasks = ConfigFileTaskBuilder.CreateTasks(_container, TaskConfigurations);
		}

		public delegate void ContainerAdder(ContainerBuilder b);

		public void AddToContainer(ContainerAdder adder)
		{
			var containerBuilder = new ContainerBuilder();
			adder.Invoke(containerBuilder);
			containerBuilder.Update(_container);
		}

		public void SetupUserForChorus()
		{
			//at the moment, all we do here is make sure send/receive is active.
			//Chorus is providing its own user name (as of Jan 2010, by asking the OS)
			var action = new SendReceiveAction();
			_addins.SetDoShowInWeSay(action.ID, true);
		}
		public string WritingSystemIdForNamesAndQuestions
		{
			get
			{
				string ws = "en";
				try
				{
					ws = Project.DefaultViewTemplate.GetField(LexSense.WellKnownProperties.SemanticDomainDdp4).
							WritingSystemIds[0];
				}
				catch (Exception)
				{
					if (!_alreadyReportedWsLookupFailure)
					{
						_alreadyReportedWsLookupFailure = true;
						ErrorReport.NotifyUserOfProblem(
							"WeSay was unable to get an input system to use from the configuration Semantic Domain Field. English will be used.");
					}
				}
				return ws;
			}
		}
	}

	public class MediaNamingHelper
	{
		public MediaNamingHelper(IEnumerable<string> lexicalUnitWritingSystemIds)
		{
			LexicalUnitWritingSystemIds = lexicalUnitWritingSystemIds;
		}

		public IEnumerable<string> LexicalUnitWritingSystemIds
		{
			get;
			set;
		}
	}
}
