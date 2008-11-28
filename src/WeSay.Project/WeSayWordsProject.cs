using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Autofac;
using Autofac.Builder;
using Autofac.Component;
using LiftIO;
using LiftIO.Validation;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.i8n;
using WeSay.AddinLib;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Options;
using WeSay.LexicalModel;

namespace WeSay.Project
{
	public class WeSayWordsProject : BasilProject
	{
		private IList<ITask> _tasks;
		private ViewTemplate _defaultViewTemplate;
		private IList<ViewTemplate> _viewTemplates;
		private readonly Dictionary<string, OptionsList> _optionLists;
		private string _pathToLiftFile;
		private string _cacheLocationOverride;
		private FileStream _liftFileStreamForLocking;
		private LiftUpdateService _liftUpdateService;

		private readonly AddinSet _addins;
		private IList<LexRelationType> _relationTypes;
		private ChorusBackupMaker _backupMaker;
		private Autofac.IContainer _container;

		public const int CurrentWeSayConfigFileVersion = 5; // This variable must be updated with every new vrsion of the WeSay config file

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
			BackupMaker = new ChorusBackupMaker();

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
		public new static void InitializeForTests()
		{
			WeSayWordsProject project = new WeSayWordsProject();

			try
			{
				File.Delete(PathToPretendLiftFile);
			}
			catch (Exception) {}

			Directory.CreateDirectory(Path.GetDirectoryName(PathToPretendLiftFile));
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
			wsc.Write(XmlWriter.Create(PathToPretendWritingSystemPrefs));

			project.SetupProjectDirForTests(PathToPretendLiftFile);


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
			StringCatalogSelector = "en";
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
			ErrorReport.ReportNonFatalMessage(
					"WeSay had a problem. You should quit now and let WeSay try to fix the problem when you run it again.\r\n\r\nIn the release build, the cache would now be invalidated and the user would not see the following crash dialog.");
			throw error;
#else
			//todo: make a way to pass on this error to us
			Palaso.Reporting.ErrorReport.ReportNonFatalMessage(
				"WeSay had a problem. You should quit now and let WeSay try to fix the problem when you run it again.");
#endif
		}

		public bool LoadFromLiftLexiconPath(string liftPath)
		{
			try
			{

				if (!File.Exists(liftPath))
				{
					ErrorReport.ReportNonFatalMessage(
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
					ErrorReport.ReportNonFatalMessage(
							String.Format(
									"WeSay was unable to open the file at '{0}' for writing, because the system won't allow it. Check that 'ReadOnly' is cleared, otherwise investigate your user permissions to write to this file.",
									liftPath));
					return false;
				}
				catch (IOException)
				{
					ErrorReport.ReportNonFatalMessage(
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
					ErrorReport.ReportNonFatalMessage(
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
					ErrorReport.ReportNonFatalMessage(
							String.Format(
									"WeSay was unable to open the file at '{0}' for reading, because the system won't allow it. Investigate your user permissions to write to this file.",
									PathToConfigFile));
					return false;
				}
				catch (IOException e)
				{
					ErrorReport.ReportNonFatalMessage(
							String.Format(
									"WeSay was unable to open the file at '{0}' for reading. \n Further information: {1}",
									PathToConfigFile,
									e.Message));
					return false;
				}

				//ProjectDirectoryPath = Directory.GetParent(Directory.GetParent(liftPath).FullName).FullName;
				ProjectDirectoryPath = Directory.GetParent(liftPath).FullName;

				LoadFromProjectDirectoryPath(ProjectDirectoryPath);
				return true;
			}
			catch (ConfigurationException e)
			{
				ErrorReport.ReportNonFatalMessage(e.Message);
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
				XPathNavigator nav = configDoc.CreateNavigator().SelectSingleNode("//uiOptions");
				if (nav != null)
				{
					string ui = nav.GetAttribute("uiLanguage", "");
					if (!string.IsNullOrEmpty(ui))
					{
						StringCatalogSelector = ui;
					}
					UiFontName = nav.GetAttribute("uiFont", "");
					string s = nav.GetAttribute("uiFontSize", string.Empty);
					float f;
					if (!float.TryParse(s, out f) || f == 0)
					{
						f = 12;
					}
					UiFontSizeInPoints = f;
				}
				CheckIfConfigFileVersionIsToNew(configDoc);
				MigrateConfigurationXmlIfNeeded(configDoc, PathToConfigFile);
			}
			base.LoadFromProjectDirectoryPath(projectDirectoryPath);

			//container change InitializeViewTemplatesFromProjectFiles();

			//review: is this the right place for this?
			PopulateDIContainer();

			LoadBackupPlan();
		}

		public static void CheckIfConfigFileVersionIsToNew(XPathDocument configurationDoc)
		{
			if (configurationDoc.CreateNavigator().SelectSingleNode("configuration") != null)
			{
				string versionNumberAsString =
					configurationDoc.CreateNavigator().SelectSingleNode("configuration").GetAttribute("version", "");
				if(int.Parse(versionNumberAsString) > CurrentWeSayConfigFileVersion)
				{
					throw new ApplicationException("The config file is to new for this version of wesay. Please download a newer version of wesay from www.wesay.org");
				}
			}
		}

		[Serializable]
	//    [ComVisible(true)]
		public delegate object ServiceCreatorCallback(
		   IServiceContainer container,
		   Type serviceType
		);

		private void PopulateDIContainer()
		{
			var builder = new ContainerBuilder();

			//builder.Register<UserSettingsRepository>(new UserSettingsRepository());
			builder.Register(new WordListCatalog()).SingletonScoped();

			builder.Register<IProgressNotificationProvider>(new DialogProgressNotificationProvider());

			builder.Register<LexEntryRepository>(
				c => c.Resolve<IProgressNotificationProvider>().Go<LexEntryRepository>("Loading Dictionary",
						progressState => new LexEntryRepository(_pathToLiftFile, progressState)));

			//builder.Register<ViewTemplate>(DefaultPrintingTemplate).Named("PrintingTemplate");

			var catalog = new TaskTypeCatalog();
			catalog.RegisterAllTypes(builder);

			builder.Register<TaskTypeCatalog>(catalog).SingletonScoped();

			//this is a bit weird, did it to get around a strange problem where it was left open,
			//never found out by whom.  But note, it does affect behavior.  It means that
			//the first time the reader is asked for, it will be reading the value as it was
			//back when we did this assignment.
			string configFileText = File.ReadAllText(PathToConfigFile);

			builder.Register<ConfigFileReader>(c => new ConfigFileReader(configFileText, catalog)).SingletonScoped();

			builder.Register<TaskCollection>().SingletonScoped();

			foreach (var viewTemplate in ConfigFileReader.CreateViewTemplates(configFileText, WritingSystems))
			{
				//todo: this isn't going to work if we start using multiple tempates.
				//will have to go to a naming system.
				builder.Register(viewTemplate).SingletonScoped();
			}

		  //  builder.Register<ViewTemplate>(DefaultViewTemplate);

		  //  builder.Register(DefaultViewTemplate);
			// can't currently get at the instance
			//someday: builder.Register<StringCatalog>(new StringCatalog()).ExternallyOwned();

			_container = builder.Build();
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

		private void LoadBackupPlan()
		{
			//what a mess. I hate .net new fangled xml stuff...
			XPathDocument projectDoc = GetConfigurationDoc();
			XPathNavigator backupPlanNav = projectDoc.CreateNavigator();
			backupPlanNav = backupPlanNav.SelectSingleNode("configuration/" + ChorusBackupMaker.ElementName);
			if (backupPlanNav == null)
			{
				//make sure we have a fresh copy with any defaults
				BackupMaker = new ChorusBackupMaker();
				return;
			}

			XmlReader r = XmlReader.Create(new StringReader(backupPlanNav.OuterXml));
			BackupMaker = ChorusBackupMaker.LoadFromReader(r);
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
				ErrorNotificationDialog.ReportException(e);
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
				ErrorNotificationDialog.ReportException(e);
			}
		}

		public bool MigrateConfigurationXmlIfNeeded()
		{
			return MigrateConfigurationXmlIfNeeded(new XPathDocument(PathToConfigFile),
												   PathToConfigFile);
		}

		public static bool MigrateConfigurationXmlIfNeeded(XPathDocument configurationDoc,
														   string targetPath)
		{
			Logger.WriteEvent("Checking if migration of configuration is needed.");

			bool didMigrate = false;

			if (configurationDoc.CreateNavigator().SelectSingleNode("configuration") == null)
			{
				MigrateUsingXSLT(configurationDoc, "MigrateConfig0To1.xsl", targetPath);
				configurationDoc = new XPathDocument(targetPath);
				didMigrate = true;
			}
			if (
					configurationDoc.CreateNavigator().SelectSingleNode(
							"configuration[@version='1']") != null)
			{
				MigrateUsingXSLT(configurationDoc, "MigrateConfig1To2.xsl", targetPath);
				configurationDoc = new XPathDocument(targetPath);
				didMigrate = true;
			}
			if (configurationDoc.CreateNavigator().SelectSingleNode("configuration[@version='2']") != null)
			{
				MigrateUsingXSLT(configurationDoc, "MigrateConfig2To3.xsl", targetPath);
				configurationDoc = new XPathDocument(targetPath);
				didMigrate = true;
			}
			if (configurationDoc.CreateNavigator().SelectSingleNode("configuration[@version='3']") != null)
			{
				MigrateUsingXSLT(configurationDoc, "MigrateConfig3To4.xsl", targetPath);
				configurationDoc = new XPathDocument(targetPath);
				didMigrate = true;
			}
			if (configurationDoc.CreateNavigator().SelectSingleNode("configuration[@version='4']") != null)
			{
				MigrateUsingXSLT(configurationDoc, "MigrateConfig4To5.xsl", targetPath);
				configurationDoc = new XPathDocument(targetPath);
				didMigrate = true;
			}
			return didMigrate;
		}

		private static void MigrateUsingXSLT(IXPathNavigable configurationDoc,
											 string xsltName,
											 string targetPath)
		{
			Logger.WriteEvent("Migrating Configuration File {0}", xsltName);
			using (
					Stream stream =
							Assembly.GetExecutingAssembly().GetManifestResourceStream(
									typeof (WeSayWordsProject), xsltName))
			{
				XslCompiledTransform transform = new XslCompiledTransform();
				using (XmlReader reader = XmlReader.Create(stream))
				{
					transform.Load(reader);
					string tempPath = Path.GetTempFileName();
					XmlWriterSettings settings = new XmlWriterSettings();
					settings.Indent = true;
					using (XmlWriter writer = XmlWriter.Create(tempPath, settings))
					{
						transform.Transform(configurationDoc, writer);
						TempFileCollection tempfiles = transform.TemporaryFiles;
						if (tempfiles != null)
								// tempfiles will be null when debugging is not enabled
						{
							tempfiles.Delete();
						}
						writer.Close();
					}
					string s = targetPath + ".tmp";
					if (File.Exists(s))
					{
						File.Delete(s);
					}
					if (File.Exists(targetPath)) //review: JDH added this because of a failing test, and from my reading, the target shouldn't need to pre-exist
					{
						File.Move(targetPath, s);
					}
					File.Move(tempPath, targetPath);
					File.Delete(s);
				}
			}
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
//                    ErrorReport.ReportNonFatalMessage(
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
				ErrorReport.ReportNonFatalMessage(
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
								   new WeSay.Foundation.ServiceLocatorAdapter(_container),
								   this);
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
					ErrorReport.ReportNonFatalMessage("There was a problem reading the task xml. " +
													  e.Message);
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
			string pathToWritingSystemPrefs = GetPathToWritingSystemPrefs(projectDirectoryPath);
			File.Copy(GetPathToWritingSystemPrefs(ApplicationCommonDirectory), pathToWritingSystemPrefs);


			string pathToConfigFile = GetPathToConfigFile(projectDirectoryPath, projectName);
			File.Copy(PathToDefaultConfig, pathToConfigFile, true);

			//hack
			StickDefaultViewTemplateInNewConfigFile(pathToWritingSystemPrefs, pathToConfigFile);

			MigrateConfigurationXmlIfNeeded(new XPathDocument(pathToConfigFile),pathToConfigFile) ;

			var pathToLiftFile = Path.Combine(projectDirectoryPath, projectName + ".lift");
			if (!File.Exists(pathToLiftFile))
			{
				Utilities.CreateEmptyLiftFile(pathToLiftFile, LiftExporter.ProducerString, false);
			}
		}

		/// <summary>
		/// this is something of a hack, because we currently create the default viewtemplate from
		/// code, but everything else from template xml files.  So this opens up the default config
		/// and sticks a nice new code-computed default view template into it.
		/// </summary>
		/// <param name="pathToWritingSystemPrefs"></param>
		/// <param name="pathToConfigFile"></param>
		private static void StickDefaultViewTemplateInNewConfigFile(string pathToWritingSystemPrefs, string pathToConfigFile)
		{
			WritingSystemCollection writingSystemCollection = new WritingSystemCollection();
			writingSystemCollection.Load(pathToWritingSystemPrefs);

			var template = ViewTemplate.MakeMasterTemplate(writingSystemCollection);
			StringBuilder builder = new StringBuilder();
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.OmitXmlDeclaration = true;
			using (var writer = XmlWriter.Create(builder, settings))
			{
				template.Write(writer);
			}

			XmlDocument doc = new XmlDocument();
			doc.Load(pathToConfigFile);
			var e = doc.SelectSingleNode("configuration").AppendChild(doc.CreateElement("components"));
			e.InnerXml = builder.ToString();
			doc.Save(pathToConfigFile);
		}

		public string PathToConfigFile
		{
			get
			{
				string name = Path.GetFileNameWithoutExtension(PathToLiftFile);
				string directoryInProject = PathToWeSaySpecificFilesDirectoryInProject;
				return GetPathToConfigFile(directoryInProject, name);
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
			if (LiftIsLocked)
			{
				ReleaseLockOnLift();
			}
			if(_container !=null)
			{
				_container.Dispose();//this will dispose of objects in the container (at least those with the normal "lifetype" setting)
			}
		}

		/// <remark>
		/// The protection provided by this simple opproach is obviously limitted;
		/// it will keep the lift file safe normally... but could lead to non-data-loosing crashes
		/// if some automated process was sitting out there, just waiting to open as soon as we realease
		/// </summary>
		private void ReleaseLockOnLift()
		{
			//Debug.Assert(_liftFileStreamForLocking != null);
			//_liftFileStreamForLocking.Close();
			//_liftFileStreamForLocking.Dispose();
			//_liftFileStreamForLocking = null;
		}

		public bool LiftIsLocked
		{
			get { return _liftFileStreamForLocking != null; }
		}

		private void LockLift()
		{
			//Debug.Assert(_liftFileStreamForLocking == null);
			//_liftFileStreamForLocking = File.OpenRead(PathToLiftFile);
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
					_pathToLiftFile = Path.Combine(PathToWeSaySpecificFilesDirectoryInProject,
												   Path.GetFileName(ProjectDirectoryPath) + ".lift");
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

		private static string GetPathToCacheFromPathToLift(string pathToLift)
		{
			return Path.Combine(Path.GetDirectoryName(pathToLift), "Cache");
		}

		public string PathToRepository
		{
			get { return PathToLiftFile; }
			//get { return GetPathToDb4oLexicalModelDBFromPathToLift(PathToLiftFile); }
		}

		public string GetPathToDb4oLexicalModelDBFromPathToLift(string pathToLift)
		{
			return Path.Combine(GetPathToCacheFromPathToLift(pathToLift),
								Path.GetFileNameWithoutExtension(pathToLift) + ".words");
		}

		/// <summary>
		/// Find the file, starting with the project dirs and moving to the app dirs.
		/// This allows a user to override an installed file by making thier own.
		/// </summary>
		/// <returns></returns>
		public string LocateFile(string fileName)
		{
			string path = Path.Combine(PathToWeSaySpecificFilesDirectoryInProject, fileName);
			if (File.Exists(path))
			{
				return path;
			}

			//            path = Path.Combine(ProjectCommonDirectory, fileName);
			//            if (File.Exists(path))
			//            {
			//                return path;
			//            }

			path = Path.Combine(ApplicationCommonDirectory, fileName);
			if (File.Exists(path))
			{
				return path;
			}

			path = Path.Combine(DirectoryOfExecutingAssembly, fileName);
			if (File.Exists(path))
			{
				return path;
			}

			path = Path.Combine(GetTopAppDirectory(), fileName);
			if (File.Exists(path))
			{
				return path;
			}

			return null;
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

		public LiftUpdateService LiftUpdateService
		{
			get { return _liftUpdateService; }
			set { _liftUpdateService = value; }
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

		public ChorusBackupMaker BackupMaker
		{
			get { return _backupMaker; }
			set { _backupMaker = value; }
		}

		public IContainer Container
		{
			get { return _container; }
		}

		public override void Save()
		{
			_addins.InitializeIfNeeded(); // must be done before locking file for writing

			var pendingConfigFile = new TempFileForSafeWriting(Project.PathToConfigFile);
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;

			XmlWriter writer = XmlWriter.Create(pendingConfigFile.TempFilePath, settings);
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

			BackupMaker.Save(writer);

			_addins.Save(writer);

			writer.WriteEndDocument();
			writer.Close();

			pendingConfigFile.WriteWasSuccessful();

			base.Save();

			BackupNow();

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
				LoadOptionsList(pathInProject);
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
				LoadOptionsList(pathInProgramDir);
			}

			return _optionLists[field.OptionsListFile];
		}

		private void LoadOptionsList(string pathToOptionsList)
		{
			string name = Path.GetFileName(pathToOptionsList);
			OptionsList list = OptionsList.LoadFromFile(pathToOptionsList);
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
				ErrorReport.ReportNonFatalMessage("Another program has WeSay's dictionary file open, so we cannot make the writing system change.  Make sure WeSay isn't running.");
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
						 GrepFile(p,
								  string.Format("name\\s*=\\s*[\"']{0}[\"']", oldName),
								  string.Format("name=\"{0}\"", field.FieldName));
					 }
					 else
					 {
						 //<field>s
						 GrepFile(p,
								  string.Format("type\\s*=\\s*[\"']{0}[\"']", oldName),
								  string.Format("type=\"{0}\"", field.FieldName));
					 }
				 });
			return true;
		}

		public bool MakeWritingSystemIdChange(WritingSystem ws, string oldId)
		{
			if (DoSomethingToLiftFile((p) =>
					 //todo: expand the regular expression here to account for all reasonable patterns
					 GrepFile(PathToLiftFile,
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
					ErrorReport.ReportNonFatalMessage(
							"The dictionary file at {0} does not conform to the LIFT format used by this version of WeSay.  The RNG validator said: {1}.",
							pathToLift,
							errors);
					return true;
				}
			}
			catch (Exception e)
			{
				ErrorNotificationDialog.ReportException(e);
				return true;
			}
			return false;
		}

		private static void GrepFile(string inputPath, string pattern, string replaceWith)
		{
			Regex regex = new Regex(pattern, RegexOptions.Compiled);
			string tempPath = inputPath + ".tmp";

			using (StreamReader reader = File.OpenText(inputPath))
			{
				using (StreamWriter writer = new StreamWriter(tempPath))
				{
					while (!reader.EndOfStream)
					{
						writer.WriteLine(regex.Replace(reader.ReadLine(), replaceWith));
					}
					writer.Close();
				}
				reader.Close();
			}
			//string backupPath = GetUniqueFileName(inputPath);
			string backupPath = inputPath + ".bak";

			ReplaceFileWithUserInteractionIfNeeded(tempPath, inputPath, backupPath);
		}

		private static void ReplaceFileWithUserInteractionIfNeeded(string tempPath,
																   string inputPath,
																   string backupPath)
		{
			bool succeeded = false;
			do
			{
				try
				{
					File.Replace(tempPath, inputPath, backupPath);
					succeeded = true;
				}

				catch (IOException)
				{
					//nb: we don't want to provide an option to cancel.  Better to crash than cancel.
					ErrorReport.ReportNonFatalMessage(Application.ProductName +
													  " was unable to get at the dictionary file to update it.  Please ensure that WeSay isn't running with it open, then click the 'OK' button below. If you cannot figure out what program has the LIFT file open, the best choice is to kill WeSay Configuration Tool using the Task Manager (ctrl+alt+del), so that the configuration does not fall out of sync with the LIFT file.");
				}
			}
			while (!succeeded);
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
			string[] antipatterns = {"Cache", "cache", ".bak", ".old", ".liftold"};

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
				BackupMaker.BackupNow(ProjectDirectoryPath, StringCatalogSelector);
			}
			catch (Exception error)
			{
				ErrorReport.ReportNonFatalMessage(string.Format("WeSay was not able to do a backup.\r\nReason: {0}", error.Message));
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
			TimeSpan diff = DateTime.Now - BackupMaker.TimeOfLastBackupAttempt;
		   // if(diff.TotalSeconds  > 30)
			if(diff.TotalMinutes > 5)
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
	}
}