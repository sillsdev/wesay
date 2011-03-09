using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using Autofac;
using Autofac.Builder;
using Chorus;
using Chorus.UI.Notes.Bar;
using Chorus.Utilities;
using LiftIO;
using LiftIO.Validation;
using Microsoft.Practices.ServiceLocation;
using Palaso.DictionaryServices.Lift;
using Palaso.DictionaryServices.Model;
using Palaso.IO;
using Palaso.Lift.Options;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.Progress;
using Palaso.UiBindings;
using Palaso.Xml;
using WeSay.AddinLib;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation;
using WeSay.LexicalModel.Foundation.Options;
using WeSay.Project.ConfigMigration.UserConfig;
using WeSay.Project.ConfigMigration.WeSayConfig;
using WeSay.Project.Synchronize;
using WeSay.UI;

namespace WeSay.Project
{
	public class WeSayWordsProject : BasilProject, IFileLocator
	{
		private IList<ITask> _tasks;
		private ViewTemplate _defaultViewTemplate;
		private IList<ViewTemplate> _viewTemplates;
		private readonly Dictionary<string, OptionsList> _optionLists;
		private string _pathToLiftFile;
		private string _cacheLocationOverride;

		private readonly AddinSet _addins;
		private IList<LexRelationType> _relationTypes;
		private ChorusBackupMaker _backupMaker;
		private Autofac.IContainer _container;

		public const int CurrentWeSayConfigFileVersion = 8; // This variable must be updated with every new vrsion of the WeSayConfig file
		public const int CurrentWeSayUserSpecificConfigFileVersion = 2; // This variable must be updated with every new vrsion of the WeSayUserConfig file

		public event EventHandler EditorsSaveNow;

		public class StringPair: EventArgs
		{
			public string from;
			public string to;
		}

		public event EventHandler<StringPair> WritingSystemChanged;

		public WeSayWordsProject()
		{
			_addins = AddinSet.Create(GetAddinNodes, LocateFile);
			_optionLists = new Dictionary<string, OptionsList>();
//            BackupMaker = new ChorusBackupMaker();

		}

		public IList<ITask> Tasks
		{
			get { return _tasks; }
			set { _tasks = value; }
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
				return (WeSayWordsProject) Singleton;
			}
		}

		/// <summary>
		/// See comment on BasilProject.InitializeForTests()
		/// </summary>
		public new static WeSayWordsProject InitializeForTests()
		{
			WeSayWordsProject project = new WeSayWordsProject();

			try
			{
				File.Delete(PathToPretendLiftFile);
			}
			catch (Exception) {}

			DirectoryInfo projectDirectory = Directory.CreateDirectory(Path.GetDirectoryName(PathToPretendLiftFile));
			Utilities.CreateEmptyLiftFile(PathToPretendLiftFile, "InitializeForTests()", true);

			//setup writing systems
			WritingSystemCollection wsc = new WritingSystemCollection();
			wsc.Add(wsc.TestWritingSystemVernId,
					new WritingSystem(wsc.TestWritingSystemVernId, new Font("Courier", 10)));
			wsc.Add(wsc.TestWritingSystemAnalId,
					new WritingSystem(wsc.TestWritingSystemAnalId, new Font("Arial", 15)));
			if (File.Exists(PathToPretendWritingSystemPrefs))
			{
				File.Delete(PathToPretendWritingSystemPrefs);
			}
		   string pathToLdmlWsFolder = GetPathToLdmlWritingSystemsFolder(projectDirectory.FullName);
			if (Directory.Exists(pathToLdmlWsFolder))
			{
				Directory.Delete(pathToLdmlWsFolder, true);
			}
			wsc.Write(pathToLdmlWsFolder);

			project.SetupProjectDirForTests(PathToPretendLiftFile);
			project.BackupMaker = null;//don't bother. Modern tests which might want to check backup won't be using this old approach anyways.
			return project;
		}

		public static string PathToPretendLiftFile
		{
			get { return Path.Combine(GetPretendProjectDirectory(), "PRETEND.lift"); }
		}

		public static string PathToPretendWritingSystemPrefs
		{
			get { return Path.Combine(GetPretendProjectDirectory(), "WritingSystemPrefs.xml"); }
		}

		public void SetupProjectDirForTests(string pathToLift)
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
			LoadFromProjectDirectoryPath(ProjectDirectoryPath);
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
			Palaso.Reporting.ErrorReport.NotifyUserOfProblem(
				"WeSay had a problem. You should quit now and let WeSay try to fix the problem when you run it again.");
#endif
		}

		public bool LoadFromLiftLexiconPath(string liftPath)
		{
			try
			{

				if (!File.Exists(liftPath))
				{
					ErrorReport.NotifyUserOfProblem(
							String.Format(
									"WeSay tried to find the lexicon at '{0}', but could not find it.\r\n\r\nTry opening the LIFT file by double clicking on it.",
									liftPath));
					return false;
				}
				try
				{
					using (FileStream fs = File.OpenWrite(liftPath))
					{
						fs.Close();
					}
				}
				catch (UnauthorizedAccessException)
				{
					ErrorReport.NotifyUserOfProblem(
							String.Format(
									"WeSay was unable to open the file at '{0}' for writing, because the system won't allow it. Check that 'ReadOnly' is cleared, otherwise investigate your user permissions to write to this file.",
									liftPath));
					return false;
				}
				catch (IOException)
				{
					ErrorReport.NotifyUserOfProblem(
							String.Format(
									"WeSay was unable to open the file at '{0}' for writing, probably because it is locked by some other process on your computer. Maybe you need to quit WeSay? If you can't figure out what has it locked, restart your computer.",
									liftPath));
					return false;
				}

				if (!liftPath.Contains(Path.DirectorySeparatorChar.ToString()))
				{
					Logger.WriteEvent("Converting filename only liftPath {0} to full path {1}", liftPath, Path.GetFullPath(liftPath));
					liftPath = Path.GetFullPath(liftPath);
				}

				PathToLiftFile = liftPath;

				if (!File.Exists(liftPath))
				{
					ErrorReport.NotifyUserOfProblem(
							String.Format(
									"WeSay tried to find the WeSay configuration file at '{0}', but could not find it.\r\n\r\nTry using the configuration Tool to create one.",
									PathToConfigFile));
					return false;
				}
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

				//ProjectDirectoryPath = Directory.GetParent(Directory.GetParent(liftPath).FullName).FullName;
				ProjectDirectoryPath = Directory.GetParent(liftPath).FullName;

				try
				{
					LoadFromProjectDirectoryPath(ProjectDirectoryPath);
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
			if(parent==null)//like if the file was at c:\foo.lift
			{
				return false;
			}
			projectDirectory = parent.FullName;
			string commonDir = Path.Combine(projectDirectory, "common");
			string dirHoldingLift = Path.GetFileName(Path.GetDirectoryName(liftPath));
			return dirHoldingLift == "wesay" && Directory.Exists(commonDir);
		}

		public override void LoadFromProjectDirectoryPath(string projectDirectoryPath)
		{
			ProjectDirectoryPath = projectDirectoryPath;

			//may have already been done, but maybe not
			MoveFilesFromOldDirLayout(projectDirectoryPath);

			XPathDocument configDoc = GetConfigurationDoc();
			if (configDoc != null) // will be null if we're creating a new project
			{
//                XPathNavigator nav = configDoc.CreateNavigator().SelectSingleNode("//uiOptions");
//                if (nav != null)
//                {
//                    string ui = nav.GetAttribute("uiLanguage", "");
//                    if (!string.IsNullOrEmpty(ui))
//                    {
//                        UiOptions.Language = ui;
//                    }
//                    UiOptions.LabelFontName = nav.GetAttribute("uiFont", "");
//                    string s = nav.GetAttribute("uiFontSize", string.Empty);
//                    float f;
//                    if (!float.TryParse(s, out f) || f == 0)
//                    {
//                        f = 12;
//                    }
//                    UiOptions.LabelFontSizeInPoints = f;
//                }
				CheckIfConfigFileVersionIsTooNew(configDoc);
				var m = new ConfigurationMigrator();
				Console.WriteLine("{0}",PathToConfigFile);
				m.MigrateConfigurationXmlIfNeeded(configDoc, PathToConfigFile);
			}
			base.LoadFromProjectDirectoryPath(projectDirectoryPath);

			//container change InitializeViewTemplatesFromProjectFiles();

			//review: is this the right place for this?
			PopulateDIContainer();

			var userConfigMigrator = new WeSayUserConfigMigrator(PathToUserSpecificConfigFile);
			userConfigMigrator.MigrateIfNeeded();

			LoadUserConfig();
			InitStringCatalog();

		}

		public static void CheckIfConfigFileVersionIsTooNew(XPathDocument configurationDoc)
		{
			if (configurationDoc.CreateNavigator().SelectSingleNode("configuration") != null)
			{
				string versionNumberAsString =
					configurationDoc.CreateNavigator().SelectSingleNode("configuration").GetAttribute("version", "");
				if(int.Parse(versionNumberAsString) > CurrentWeSayConfigFileVersion)
				{
					throw new ApplicationException("The config file is too new for this version of wesay. Please download a newer version of wesay from www.wesay.org");
				}
			}
		}

		[Serializable]
	//    [ComVisible(true)]
		public delegate object ServiceCreatorCallback(
		   IServiceContainer container,
		   Type serviceType
		);

		//TOdo: figure out how to move this to wher it belongs... should be doable if/when we
		//can do container building from within the lexical assemblies.
		public static string GetUrlFromLexEntry(LexEntry entry)
		{
			var filename = Path.GetFileName(Project.PathToLiftFile);
			//review: see also: HttpUtility.UrlEncode
			filename = Uri.EscapeDataString(filename);
			string url = string.Format("lift://{0}?type=entry&", filename);
			url += "label=" + entry.GetSimpleFormForLogging() + "&";
			url += "id=" + entry.Guid;
			url = url.Trim('&');
			return url;
		}

		private void PopulateDIContainer()
		{
			var builder = new ContainerBuilder();

			builder.Register(new WordListCatalog()).SingletonScoped();

			builder.Register<IProgressNotificationProvider>(new DialogProgressNotificationProvider());

			//NB: these are delegates because the viewtemplate is not yet avaialbe when were're building the container
			builder.Register<OptionsList>(c => GetSemanticDomainsList());//todo: figure out how to limit this with a name... currently, it's for any OptionList

			// I (CP) don't think this is needed
			builder.Register<IEnumerable<string>>(c => GetIdsOfSingleOptionFields());//todo: figure out how to limit this with a name... currently, it's for any IEnumerable<string>

			builder.Register<LiftDataMapper>( c =>
			  {
				  try
				  {
					  return c.Resolve<IProgressNotificationProvider>().Go
						  <LiftDataMapper>(
							  "Loading Dictionary",
							  progressState =>
						  new LiftDataMapper(
								  _pathToLiftFile,
								  GetSemanticDomainsList(),
								  GetIdsOfSingleOptionFields(),
								  progressState
							  )
						  );
				  }
				  catch (LiftFormatException error)
				  {
					  ErrorReport.NotifyUserOfProblem(error.Message);
					  throw;
				  }
			  });

			builder.Register<LexEntryRepository>();
//            builder.Register<LexEntryRepository>(
//                 c => c.Resolve<IProgressNotificationProvider>().Go<LexEntryRepository>("Loading Dictionary",
//                         progressState => new LexEntryRepository(_pathToLiftFile, progressState)));


			builder.Register<ICountGiver>(c => c.Resolve<LexEntryRepository>());



			var catalog = new TaskTypeCatalog();
			catalog.RegisterAllTypes(builder);

			builder.Register<TaskTypeCatalog>(catalog).SingletonScoped();

			//this is a bit weird, did it to get around a strange problem where it was left open,
			//never found out by whom.  But note, it does affect behavior.  It means that
			//the first time the reader is asked for, it will be reading the value as it was
			//back when we did this assignment.
			string configFileText = File.ReadAllText(PathToConfigFile);

			string defaultXmlConfigText = File.ReadAllText(PathToDefaultConfig);

			builder.Register<ConfigFileReader>(c => new ConfigFileReader(configFileText, defaultXmlConfigText,  catalog)).SingletonScoped();

			builder.Register<TaskCollection>().SingletonScoped();

			var viewTemplates = ConfigFileReader.CreateViewTemplates(configFileText, WritingSystems);
			foreach (var viewTemplate in viewTemplates)
			{
				//todo: this isn't going to work if we start using multiple tempates.
				//will have to go to a naming system.
				builder.Register(viewTemplate).SingletonScoped();
			}

			builder.Register<ViewTemplate>(c => DefaultPrintingTemplate).Named("PrintingTemplate");
			builder.Register<WritingSystemCollection>(c => DefaultViewTemplate.WritingSystems).ExternallyOwned();

			RegisterChorusStuff(builder, viewTemplates.First().CreateListForChorus());


			builder.Register<PublicationFontStyleProvider>(c=> new PublicationFontStyleProvider(c.Resolve<ViewTemplate>("PrintingTemplate")));

			builder.Register<IOptionListReader>(c => new DdpListReader()).Named(LexSense.WellKnownProperties.SemanticDomainDdp4);
			builder.Register<IOptionListReader>(c => new GenericOptionListReader());


			builder.Register<PictureControl>(c=> new PictureControl(Path.GetDirectoryName(PathToLiftFile), PathToPictures, GetFileLocator())).FactoryScoped();

		  //  builder.Register<ViewTemplate>(DefaultViewTemplate);

		  //  builder.Register(DefaultViewTemplate);
			// can't currently get at the instance
			//someday: builder.Register<StringCatalog>(new StringCatalog()).ExternallyOwned();

			builder.Register<CheckinDescriptionBuilder>().SingletonScoped();
			builder.Register<Chorus.sync.ProjectFolderConfiguration>(new WeSayChorusProjectConfiguration(Path.GetDirectoryName(PathToConfigFile))).SingletonScoped();
			builder.Register<ChorusBackupMaker>().SingletonScoped();
			builder.Register<UiConfigurationOptions>().SingletonScoped();


			//it is sad that we initially used a static for logger, and that hasn't been completely undone yet.
			//but by registering it here, we at least make it possible for components to get access to it this
			//"proper" way.
			builder.Register<Logger>(c => Logger.Singleton);
			builder.Register<ILogger>(c =>
										  {
											  var m = new MultiLogger();
											  Logger.Init();//it's ok if this was already done
											  m.Add(Logger.Singleton);
											  m.Add(c.Resolve<CheckinDescriptionBuilder>());
											  return m;
										  });

			//            var ap = new AudioPathProvider(Project.WeSayWordsProject.Project.PathToAudio,
//                        () => entry.LexicalForm.GetBestAlternativeString(lexicalUnitField.WritingSystemIds));

//            var x = _defaultViewTemplate.GetField(
//            builder.Register<AudioPathProvider>(c=>new AudioPathProvider(PathToAudio, )));

			builder.Register(c=>
				new MediaNamingHelper(c.Resolve<ViewTemplate>().GetField(LexEntry.WellKnownProperties.LexicalUnit).WritingSystemIds)).ContainerScoped();


			_container = builder.Build();
		}

		private void RegisterChorusStuff(ContainerBuilder builder, IEnumerable<IWritingSystem> writingSystemsForChorus)
		{
			//NB: currently, the ctor for ChorusSystem requires hg, since it gets or creates a repo in the path.
			if (!string.IsNullOrEmpty(Chorus.VcsDrivers.Mercurial.HgRepository.GetEnvironmentReadinessMessage("en")))
				return;

			//TODO: move all this stuff to ChorusSystem
			ChorusUIComponentsInjector.Inject(builder, Path.GetDirectoryName(PathToConfigFile));
			var chorusSystem = new ChorusSystem(Path.GetDirectoryName(PathToConfigFile));
			chorusSystem.WritingSystems = writingSystemsForChorus;
			builder.Register(writingSystemsForChorus);
			builder.Register<Chorus.UI.Review.NavigateToRecordEvent>(chorusSystem.NavigateToRecordEvent);
			builder.Register<ChorusSystem>(chorusSystem);

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
			});

			var mapping = new NotesToRecordMapping();
			mapping.FunctionToGetCurrentUrlForNewNotes = (entry, id) => GetUrlFromLexEntry(entry as LexEntry);
			mapping.FunctionToGoFromObjectToItsId = (entry) => (entry as LexEntry).Guid.ToString();
			builder.Register<NotesToRecordMapping>(mapping);

			builder.Register<NotesBarView>(c => c.Resolve<ChorusSystem>().WinForms.CreateNotesBar(PathToLiftFile, c.Resolve<NotesToRecordMapping>(), new NullProgress())).FactoryScoped();

		}

		public IEnumerable<string> GetIdsOfSingleOptionFields()
		{
			foreach (Field field in DefaultViewTemplate.Fields)
			{
				if (field.DataTypeName == "Option")
					yield return field.FieldName;
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
			if (File.Exists(PathToUserSpecificConfigFile))
			{
				dom.Load(PathToUserSpecificConfigFile);
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
			return m.MigrateConfigurationXmlIfNeeded(new XPathDocument(PathToConfigFile),
												   PathToConfigFile);
		}



		//        public void LoadFromConfigFilePath(string path)
		//        {
		//            DirectoryInfo weSayDirectoryInfo = Directory.GetParent(path);
		//
		//        }

//        private void InitializeViewTemplatesFromProjectFiles()
//        {
//            if (_viewTemplates == null)
//            {
//                List<ViewTemplate> viewTemplates = new List<ViewTemplate>();
//                ViewTemplate factoryTemplate = ViewTemplate.MakeMasterTemplate(WritingSystems);
//
//                try
//                {
//                    XPathDocument projectDoc = GetConfigurationDoc();
//                    if (projectDoc != null)
//                    {
//                        XPathNodeIterator nodes =
//                                projectDoc.CreateNavigator().Select(
//                                        "configuration/components/viewTemplate");
//                        foreach (XPathNavigator node in nodes)
//                        {
//                            ViewTemplate userTemplate = new ViewTemplate();
//                            userTemplate.LoadFromString(node.OuterXml);
//                            ViewTemplate.UpdateUserViewTemplate(factoryTemplate, userTemplate);
//                            if (userTemplate.Id == "Default View Template")
//                            {
//                                _defaultViewTemplate = userTemplate;
//                            }
//                            viewTemplates.Add(userTemplate);
//                        }
//                    }
//                }
//                catch (Exception error)
//                {
//                    ErrorReport.NotifyUserOfProblem(
//                            "There may have been a problem reading the view template xml of the configuration file. A default template will be created." +
//                            error.Message);
//                }
//                if (_defaultViewTemplate == null)
//                {
//                    _defaultViewTemplate = factoryTemplate;
//                }
//                _viewTemplates = viewTemplates;
//            }
//        }

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
								   ApplicationRootDirectory,
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


		public static void CreateEmptyProjectFiles(string projectDirectoryPath)
		{
			string name = Path.GetFileName(projectDirectoryPath);
			CreateEmptyProjectFiles(projectDirectoryPath, name);
		}
		public static void CreateEmptyProjectFiles(string projectDirectoryPath, string projectName)
		{

			Directory.CreateDirectory(projectDirectoryPath);
			CopyWritingSystemsFromApplicationCommonDirectoryToNewProject(projectDirectoryPath);
			string pathToConfigFile = GetPathToConfigFile(projectDirectoryPath, projectName);
			File.Copy(PathToDefaultConfig, pathToConfigFile, true);

			//hack
			StickDefaultViewTemplateInNewConfigFile(projectDirectoryPath, pathToConfigFile);

			var m = new ConfigurationMigrator();
			m.MigrateConfigurationXmlIfNeeded(new XPathDocument(pathToConfigFile), pathToConfigFile);

			var pathToLiftFile = Path.Combine(projectDirectoryPath, projectName + ".lift");
			if (!File.Exists(pathToLiftFile))
			{
				Utilities.CreateEmptyLiftFile(pathToLiftFile, LiftWriter.ProducerString, false);
			}
		}

		/// <summary>
		/// this is something of a hack, because we currently create the default viewtemplate from
		/// code, but everything else from template xml files.  So this opens up the default config
		/// and sticks a nice new code-computed default view template into it.
		/// </summary>
		/// <param name="pathToWritingSystemPrefs"></param>
		/// <param name="pathToConfigFile"></param>
		private static void StickDefaultViewTemplateInNewConfigFile(string projectPath, string pathToConfigFile)
		{
			var writingSystemCollection = new WritingSystemCollection();
			writingSystemCollection.Load(GetPathToLdmlWritingSystemsFolder(projectPath));

			var template = ViewTemplate.MakeMasterTemplate(writingSystemCollection);
			var builder = new StringBuilder();
			using (var writer = XmlWriter.Create(builder, CanonicalXmlSettings.CreateXmlWriterSettings(ConformanceLevel.Fragment)))
			{
				template.Write(writer);
			}

			var doc = new XmlDocument();
			doc.Load(pathToConfigFile);
			var e = doc.SelectSingleNode("configuration").AppendChild(doc.CreateElement("components"));
			e.InnerXml = builder.ToString();
			doc.Save(pathToConfigFile);
		}

		public string PathToConfigFile
		{
			get
			{
				return GetPathToConfigFile(PathToWeSaySpecificFilesDirectoryInProject,
					Path.GetFileNameWithoutExtension(PathToLiftFile));
			}
		}

		public string PathToUserSpecificConfigFile
		{
			get
			{
				return Path.Combine(PathToWeSaySpecificFilesDirectoryInProject,
									System.Environment.UserName + ".WeSayUserConfig");
			}
		}

		private static string GetPathToConfigFile(string directoryInProject, string name)
		{
			return Path.Combine(directoryInProject,
								name + ".WeSayConfig");
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
			if(_container !=null)
			{
				_container.Dispose();//this will dispose of objects in the container (at least those with the normal "lifetype" setting)
			}
		}

		public override string Name
		{
			get { return Path.GetFileNameWithoutExtension(PathToLiftFile); }
		}

		public string PathToLiftFile
		{
			get
			{
				if (String.IsNullOrEmpty(_pathToLiftFile))
				{
					_pathToLiftFile = GetPathToLiftFileGivenProjectDirectory();
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

		private string GetPathToLiftFileGivenProjectDirectory()
		{
			//first, we assume it's based on the name of the directory
			var path = Path.Combine(PathToWeSaySpecificFilesDirectoryInProject,
										   Path.GetFileName(ProjectDirectoryPath) + ".lift");

			//if that doesn't give us one, then we find one which has a matching wesayconfig file
			if (!File.Exists(path))
			{
				foreach (var liftPath in Directory.GetFiles(ProjectDirectoryPath, "*.lift"))
				{
					if (File.Exists(liftPath.Replace(".lift", ".WeSayConfig")))
					{
						return liftPath;
					}
				}
#if mono    //try this too(probably not needed...)
				//anyhow remember case is sensitive, and a simpe "tolower"
				//doens't cut it because the exists will fail if it's the wrong case (WS-14982)
				foreach (var liftPath in Directory.GetFiles(ProjectDirectoryPath, "*.Lift"))
				{
					if (File.Exists(liftPath.Replace(".Lift", ".WeSayConfig")))
					{
						return liftPath;
					}
				}
#endif
			}
			return path;
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
		/// <summary>
		/// Find the file, starting with the project dirs and moving to the app dirs.
		/// This allows a user to override an installed file by making thier own.
		/// </summary>
		/// <returns></returns>
		private FileLocator GetFileLocator()
		{
			return new FileLocator(new string[] { PathToWeSaySpecificFilesDirectoryInProject,
												  ApplicationCommonDirectory, DirectoryOfTheApplicationExecutable, GetTopAppDirectory()});
		}



		public string PathToWeSaySpecificFilesDirectoryInProject
		{
			get { return ProjectDirectoryPath; }
		}

		public string PathOldToWeSaySpecificFilesDirectoryInProject
		{
			get { return Path.Combine(ProjectDirectoryPath, "wesay"); }
		}

		public ViewTemplate DefaultViewTemplate
		{
			get
			{
				if (_defaultViewTemplate == null)
				{
					//container change
//                    InitializeViewTemplatesFromProjectFiles();
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

		public WritingSystem HeadWordWritingSystem
		{
			get
			{
				Field f = DefaultViewTemplate.GetField(LexEntry.WellKnownProperties.LexicalUnit);
				if (f.WritingSystemIds.Count == 0)
				{
					return WritingSystems.UnknownVernacularWritingSystem;
				}
				return WritingSystems[f.WritingSystemIds[0]];
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
				if(!PreventBackupForTests)
						_backupMaker = value;
			}
		}

		public IContainer Container
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

			var pendingConfigFile = new TempFileForSafeWriting(Project.PathToConfigFile);

			var writer = XmlWriter.Create(pendingConfigFile.TempFilePath, CanonicalXmlSettings.CreateXmlWriterSettings());
			writer.WriteStartDocument();
			writer.WriteStartElement("configuration");
			writer.WriteAttributeString("version", CurrentWeSayConfigFileVersion.ToString());

			writer.WriteStartElement("components");
			foreach (ViewTemplate template in ViewTemplates)
			{
				template.Write(writer);
			}
			writer.WriteEndElement();

			TaskCollection tasks;
			if(_container.TryResolve<TaskCollection>(out tasks))
				tasks.Write(writer);


			if (EditorsSaveNow != null)
			{
				EditorsSaveNow.Invoke(writer, null);
			}

			_addins.Save(writer);

			writer.WriteEndDocument();
			writer.Close();

			pendingConfigFile.WriteWasSuccessful();

			base.Save();

			SaveUserSpecificConfiguration();
			BackupNow();

		}

		private void SaveUserSpecificConfiguration()
		{
			var pendingConfigFile = new TempFileForSafeWriting(Project.PathToUserSpecificConfigFile);

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
			if (File.Exists(pathInProject))
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

		private void LoadOptionsList(string fieldName,string pathToOptionsList)
		{
			string name = Path.GetFileName(pathToOptionsList);
			IOptionListReader reader;
			object r;
			//first, try for a reader named after the field
			if(_container.TryResolve(fieldName, out r))
			{
				reader = r as IOptionListReader;
			}
			else
			{
				reader = _container.Resolve<IOptionListReader>();
			}
			OptionsList list = reader.LoadFromFile(pathToOptionsList);
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
			if(!File.Exists(PathToLiftFile))
				return false;
			try
			{
				doSomething(PathToLiftFile);
				return true;
			}
			catch (Exception error)
			{
				ErrorReport.NotifyUserOfProblem("Another program has WeSay's dictionary file open, so we cannot make the writing system change.  Make sure WeSay isn't running.");
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

		public bool MakeWritingSystemIdChange(WritingSystem ws, string oldId)
		{
			if (DoSomethingToLiftFile((p) =>
					 //todo: expand the regular expression here to account for all reasonable patterns
					 FileUtils.GrepFile(PathToLiftFile,
							  string.Format("lang\\s*=\\s*[\"']{0}[\"']",
											Regex.Escape(oldId)),
							  string.Format("lang=\"{0}\"", ws.Id))))
			{
				WritingSystems.IdOfWritingSystemChanged(ws, oldId);
				DefaultViewTemplate.ChangeWritingSystemId(oldId, ws.Id);

				if (WritingSystemChanged != null)
				{
					StringPair p = new StringPair();
					p.from = oldId;
					p.to = ws.Id;
					WritingSystemChanged.Invoke(this, p);
				}
				return true;
			}

			return false;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="pathToLift"></param>
		/// <returns>true if it displayed an error message</returns>
		public static bool CheckLiftAndReportErrors(string pathToLift)
		{
			try
			{
				string errors = Validator.GetAnyValidationErrors(Project.PathToLiftFile);
				if (!String.IsNullOrEmpty(errors))
				{
					ErrorReport.NotifyUserOfProblem(
							"The dictionary file at {0} does not conform to the LIFT format used by this version of WeSay.  The RNG validator said: {1}.",
							pathToLift,
							errors);
					return true;
				}
			}
			catch (Exception e)
			{
				ErrorReport.ReportNonFatalException(e);
				return true;
			}
			return false;
		}

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
				if(BackupMaker!=null)//it will for many tests, which don't need to be slowed down by all this
					BackupMaker.BackupNow(ProjectDirectoryPath, UiOptions.Language);
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
			if(BackupMaker.TimeOfLastBackupAttempt == default(DateTime))
			{
				BackupMaker.ResetTimeOfLastBackup();
			}

			TimeSpan diff = DateTime.Now - BackupMaker.TimeOfLastBackupAttempt;
			if(diff.TotalMinutes > 20)
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
			Tasks = ConfigFileTaskBuilder.CreateTasks(_container, configReader.GetTasksConfigurations(_container));
		}

		public delegate void ContainerAdder(Autofac.Builder.ContainerBuilder b);

		public void AddToContainer(ContainerAdder adder)
		{
			var containerBuilder = new Autofac.Builder.ContainerBuilder();
			adder.Invoke(containerBuilder);
			containerBuilder.Build(_container);
		}

		public void SetupUserForChorus()
		{
			//at the moment, all we do here is make sure send/receive is active.
			//Chorus is providing its own user name (as of Jan 2010, by asking the OS)
			var action = new SendReceiveAction();
			_addins.SetDoShowInWeSay(action.ID, true);
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
