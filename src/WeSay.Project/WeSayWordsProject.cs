using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using LiftIO;
using Mono.Addins;
using Palaso.Reporting;
using WeSay.AddinLib;
using WeSay.Data;
using WeSay.Foundation.Options;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.LexicalModel.Tests;

namespace WeSay.Project
{
	public class WeSayWordsProject : BasilProject
	{
		private IList<ITask> _tasks;
		private ViewTemplate _defaultViewTemplate;
		private IList<ViewTemplate> _viewTemplates;
		private Dictionary<string, OptionsList> _optionLists;
		private string _pathToLiftFile;
		private string _cacheLocationOverride;
		private FileStream _liftFileStreamForLocking;
		private LiftUpdateService _liftUpdateService;

		private AddinSet _addins;
		private IList<LexRelationType> _relationTypes;

		public event EventHandler EditorsSaveNow;

		public class StringPair :EventArgs {
			public string from;
			public string to;
		}
		public event EventHandler<StringPair> WritingSystemChanged;


		public WeSayWordsProject()
		{
			_addins = AddinSet.Create(GetAddinNodes, LocateFile);
			_optionLists = new Dictionary<string, OptionsList>();
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

			//jdh added, amidst some confusion about why it was suddenly needed, on april 17,2007
			Utilities.CreateEmptyLiftFile(PathToPretendLiftFile, "InitializeForTests()", true);

			project.SetupProjectDirForTests(PathToPretendLiftFile);
		}

		/// <summary>
		/// See comment on BasilProject.InitializeForTests()
		/// </summary>
//        public static void InitializeForTests(string pathToLift)
//        {
//            WeSayWordsProject project = new WeSayWordsProject();
//            project.ProjectDirectoryPath = Directory.GetParent(pathToLift).Parent.FullName;
//            project.PathToLiftFile = pathToLift;
//            ErrorReport.IsOkToInteractWithUser = false;
//            project.LoadFromProjectDirectoryPath(project.ProjectDirectoryPath);
//            project.StringCatalogSelector = "en";
//        }
//

//        public static void InitializeForTests(WeSayWordsProject project)
//        {
//            Project = project;
//        }


		public static string PathToPretendLiftFile
		{
			get { return Path.Combine(GetPretendProjectDirectory(), "PRETEND.lift"); }
		}

		public void SetupProjectDirForTests(string pathToLift)
		{
			ProjectDirectoryPath = Directory.GetParent(pathToLift).Parent.FullName;
			PathToLiftFile = pathToLift;
//            if (!Directory.Exists(pathToLift))
//            {
//                Directory.CreateDirectory(pathToLift);
//            }
			if (File.Exists(PathToConfigFile))
			{
				File.Delete(PathToConfigFile);
			}
			string configName = Path.GetFileName(Project.PathToConfigFile);
			File.Copy(Path.Combine(ApplicationTestDirectory, configName), Project.PathToConfigFile, true);

			ErrorReport.IsOkToInteractWithUser = false;
			LoadFromProjectDirectoryPath(ProjectDirectoryPath);
			StringCatalogSelector = "en";
		}

		/// <summary>
		/// configuration tools should call this when they modify something (e.g. writing systems) that
		/// calls for rebuilding the caches.
		/// </summary>
		public void InvalidateCacheSilently()
		{
			DateTime liftLastWriteTimeUtc = File.GetLastWriteTimeUtc(WeSayWordsProject.Project.PathToLiftFile);
			if (File.Exists(WeSayWordsProject.Project.PathToDb4oLexicalModelDB))
			{
				try // don't crash if we can't update
				{
					using (
							Db4oDataSource ds =
									new Db4oDataSource(
											WeSayWordsProject.Project.PathToDb4oLexicalModelDB))
					{
						//this should be different now so the cache should be updated
						//but it shouldn't be off by enough to make it so we lose
						//records if a crash occured and updates hadn't been written
						//out yet and so are still pending in the db.
						CacheManager.UpdateSyncPointInCache(ds.Data,
															liftLastWriteTimeUtc.AddMilliseconds(10));
					}
				}
				catch
				{
					try
					{
						File.Delete(WeSayWordsProject.Project.PathToDb4oLexicalModelDB);
					}
					catch(Exception )
					{
						Palaso.Reporting.ErrorReport.ReportNonFatalMessage(
							"Please exit WeSay and manually delete this cache file: {0}.",
							WeSayWordsProject.Project.PathToDb4oLexicalModelDB);
					}
				}

			}
		}
		/// <summary>
		/// exception handlers should call this when the database or other caches seem broken or out of sync
		/// </summary>
		/// <param name="error"></param>
		public void HandleProbableCacheProblem(Exception error)
		{
#if DEBUG
			Palaso.Reporting.ErrorReport.ReportNonFatalMessage(
				"WeSay had a problem. You should quit now and let WeSay try to fix the problem when you run it again.\r\n\r\nIn the release build, the cache would now be invalidated and the user would not see the following crash dialog.");
			throw error;
#else
			InvalidateCacheSilently();
			//todo: make a way to pass on this error to us
			Palaso.Reporting.ErrorReport.ReportNonFatalMessage(
				"WeSay had a problem. You should quit now and let WeSay try to fix the problem when you run it again.");
#endif
		}

		public bool LoadFromLiftLexiconPath(string liftPath)
		{
			try
			{
				PathToLiftFile = liftPath;

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

				if (!File.Exists(PathToConfigFile))
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

				if (CheckLexiconIsInValidProjectDirectory(liftPath))
				{
					ProjectDirectoryPath = ProjectDirectoryPath;

					LoadFromProjectDirectoryPath(ProjectDirectoryPath);
					return true;
				}
				else
				{
					PathToLiftFile = null;
					ProjectDirectoryPath = null;
					return false;
				}
			}
			catch (Exception e)
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
			Debug.Assert(File.Exists(liftPath));
			projectDirectory = Directory.GetParent(Directory.GetParent(liftPath).FullName).FullName;
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
					if(!float.TryParse(s, out f) || f==0)
					{
						f = 12;
					}
					UiFontSizeInPoints = f;
				}
				MigrateConfigurationXmlIfNeeded(configDoc, PathToConfigFile);
			}
			base.LoadFromProjectDirectoryPath(projectDirectoryPath);
			InitializeViewTemplatesFromProjectFiles();
		}

		private void MoveFilesFromOldDirLayout(string projectDir)
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
				if(projectDir.Contains("biatah"))
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
					catch(Exception)
					{
						//no big deal if other files prevent deleting it
					}
				}
			}
			catch(Exception err)
			{
				ApplicationException e = new ApplicationException("Error while trying to migrate to new file structure. ", err);
				Palaso.Reporting.ErrorNotificationDialog.ReportException(e);
			}
		}

		private static void MoveSubDirectory(string targetParentDir, string subDirName, string directoryToMoveName)
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
				foreach (string source in Directory.GetFiles(projectDir, presumedExportName + "-sfm*.*"))
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
				ApplicationException e = new ApplicationException("Error while trying to move export files to new structure. ", err);
				Palaso.Reporting.ErrorNotificationDialog.ReportException(e);
			}
		}



		public static bool MigrateConfigurationXmlIfNeeded(XPathDocument configurationDoc, string targetPath)
		{
			Logger.WriteEvent("Checking if migration of configuration is needed.");

			bool didMigrate = false;

			if (configurationDoc.CreateNavigator().SelectSingleNode("configuration") == null)
			{
				MigrateUsingXSLT(configurationDoc, "MigrateConfig0To1.xsl", targetPath);
				configurationDoc = new XPathDocument(targetPath);
				didMigrate = true;
			}
			if (configurationDoc.CreateNavigator().SelectSingleNode("configuration[@version='1']") != null)
			{
				MigrateUsingXSLT(configurationDoc, "MigrateConfig1To2.xsl", targetPath) ;
				configurationDoc = new XPathDocument(targetPath);
				didMigrate = true;
			}
			return didMigrate;
		}

		private static void MigrateUsingXSLT(XPathDocument configurationDoc, string xsltName, string targetPath)
		{
			Logger.WriteEvent("Migrating Configuration File {0}", xsltName);
			using (
				Stream stream =
					Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof (WeSayWordsProject),
																			  xsltName))
			{
				XslCompiledTransform transform = new XslCompiledTransform();
				using (XmlReader reader = XmlReader.Create(stream))
				{
					transform.Load(reader);
					string tempPath = Path.GetTempFileName();
					using (XmlWriter writer = XmlWriter.Create(tempPath))
					{
						transform.Transform(configurationDoc, writer);
						TempFileCollection tempfiles = transform.TemporaryFiles;
						if (tempfiles != null)  // tempfiles will be null when debugging is not enabled
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
					File.Move(targetPath, s);
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

		private void InitializeViewTemplatesFromProjectFiles()
		{
			if (_viewTemplates == null)
			{
				List<ViewTemplate> viewTemplates = new List<ViewTemplate>();
				ViewTemplate factoryTemplate = ViewTemplate.MakeMasterTemplate(WritingSystems);

				try
				{
					XPathDocument projectDoc = GetConfigurationDoc();
					if (projectDoc != null)
					{
						XPathNodeIterator nodes =
								projectDoc.CreateNavigator().Select("configuration/components/viewTemplate");
						foreach (XPathNavigator node in nodes)
						{
							ViewTemplate userTemplate = new ViewTemplate();
							userTemplate.LoadFromString(node.OuterXml);
							ViewTemplate.UpdateUserViewTemplate(factoryTemplate, userTemplate);
							if (userTemplate.Id == "Default View Template")
							{
								_defaultViewTemplate = userTemplate;
							}
							viewTemplates.Add(userTemplate);
						}
					}
				}
				catch (Exception error)
				{
					ErrorReport.ReportNonFatalMessage(
							"There may have been a problem reading the view template xml of the configuration file. A default template will be created." +
							error.Message);
				}
				if (_defaultViewTemplate == null)
				{
					_defaultViewTemplate = factoryTemplate;
				}
				_viewTemplates = viewTemplates;
			}
		}

		public XPathNodeIterator GetAddinNodes()
		{
			return GetAddinNodes(GetConfigurationDoc());
		}

		private static XPathNodeIterator GetAddinNodes(XPathDocument configDoc)
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
								   BasilProject.ApplicationRootDirectory,
								   ProjectDirectoryPath,
								   PathToLiftFile,
								   PathToExportDirectory,
								   GetFilesBelongingToProject(ProjectDirectoryPath),
								   AddinSet.Singleton.LocateFile,
								   WritingSystems,
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
					ErrorReport.ReportNonFatalMessage("There was a problem reading the task xml. " + e.Message);
					projectDoc = null;
				}
			}
			return projectDoc;
		}

		private static bool CheckLexiconIsInValidProjectDirectory(string liftPath)
		{
			DirectoryInfo lexiconDirectoryInfo = Directory.GetParent(liftPath);
			DirectoryInfo projectRootDirectoryInfo = lexiconDirectoryInfo;
			string lexiconDirectoryName = lexiconDirectoryInfo.Name;
			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				//windows
				lexiconDirectoryName = lexiconDirectoryName.ToLowerInvariant();
			}

			if (projectRootDirectoryInfo == null ||
			  //  lexiconDirectoryName != "wesay" ||
				(!IsValidProjectDirectory(projectRootDirectoryInfo.FullName)))
			{
				string message =
						"WeSay cannot open the lexicon, because it is not in a proper WeSay/Basil project structure.";
				ErrorReport.ReportNonFatalMessage(message);
				return false;
			}
			return true;
		}

		public string PathToDefaultConfig
		{
			get { return Path.Combine(BasilProject.ApplicationCommonDirectory, "default.WeSayConfig"); }
		}

		public override void CreateEmptyProjectFiles(string projectDirectoryPath)
		{
			//enhance: some of this would be a lot cleaner if we just copied the silly
			//  default.WeSayConfig over and used that.

			ProjectDirectoryPath = projectDirectoryPath;
			Directory.CreateDirectory(PathToWeSaySpecificFilesDirectoryInProject);

			base.CreateEmptyProjectFiles(projectDirectoryPath);

			//hack
			File.Copy(PathToDefaultConfig, PathToConfigFile, true);

			_defaultViewTemplate = ViewTemplate.MakeMasterTemplate(WritingSystems);
			_viewTemplates = new List<ViewTemplate>();
			_viewTemplates.Add(_defaultViewTemplate);

			XPathDocument doc = new XPathDocument(PathToDefaultConfig);
			_addins.Load(GetAddinNodes(doc));

			if (!File.Exists(PathToLiftFile))
			{
				Utilities.CreateEmptyLiftFile(PathToLiftFile, LiftExporter.ProducerString, false);
			}
		}

		public static bool IsValidProjectDirectory(string dir)
		{
			string[] requiredDirectories = new string[] {};
			foreach (string s in requiredDirectories)
			{
				if (!Directory.Exists(Path.Combine(dir, s)))
				{
					return false;
				}
			}
			return true;
		}

		public string PathToConfigFile
		{
			get
			{
				string name = Path.GetFileNameWithoutExtension(PathToLiftFile);
				return Path.Combine(PathToWeSaySpecificFilesDirectoryInProject, name + ".WeSayConfig");
			}
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
		}

		/// <remark>
		/// The protection provided by this simple opproach is obviously limitted;
		/// it will keep the lift file safe normally... but could lead to non-data-loosing crashes
		/// if some automated process was sitting out there, just waiting to open as soon as we realease
		/// </summary>
		public void ReleaseLockOnLift()
		{
			Debug.Assert(_liftFileStreamForLocking != null);
			_liftFileStreamForLocking.Close();
			_liftFileStreamForLocking.Dispose();
			_liftFileStreamForLocking = null;
		}

		public bool LiftIsLocked
		{
			get { return _liftFileStreamForLocking != null; }
		}

		public void LockLift()
		{
			Debug.Assert(_liftFileStreamForLocking == null);
			_liftFileStreamForLocking = File.OpenRead(PathToLiftFile);
		}

		public override string Name
		{
			get
			{
				return Path.GetFileNameWithoutExtension(PathToLiftFile);
			}
		}

		public string PathToLiftFile
		{
			get
			{
				if (String.IsNullOrEmpty(_pathToLiftFile))
				{
					_pathToLiftFile =
							Path.Combine(PathToWeSaySpecificFilesDirectoryInProject,
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
					ProjectDirectoryPath = Path.GetDirectoryName(value);// Directory.GetParent(value).Parent.FullName;
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
			get
			{
				return Path.Combine(PathToWeSaySpecificFilesDirectoryInProject,
									"pictures");
			}
		}

		private static string GetPathToCacheFromPathToLift(string pathToLift)
		{
			return Path.Combine(Path.GetDirectoryName(pathToLift), "Cache");
		}

		public string PathToDb4oLexicalModelDB
		{
			get { return GetPathToDb4oLexicalModelDBFromPathToLift(PathToLiftFile); }
		}

		public string GetPathToDb4oLexicalModelDBFromPathToLift(string pathToLift)
		{
			return
					Path.Combine(GetPathToCacheFromPathToLift(pathToLift),
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
					InitializeViewTemplatesFromProjectFiles();
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
					InitializeViewTemplatesFromProjectFiles();
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
					_relationTypes.Add(
							new LexRelationType("RelationToOneEntry",
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
				if(f.WritingSystemIds.Count == 0)
				{
					return this.WritingSystems.UnknownVernacularWritingSystem;
				}
				return WritingSystems[f.WritingSystemIds[0]];
			}
		}

		public ViewTemplate DefaultPrintingTemplate
		{
			get { return DefaultViewTemplate; }
		}


		public override void Save()
		{
			_addins.InitializeIfNeeded(); // must be done before locking file for writing

			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;

			XmlWriter writer = XmlWriter.Create(Project.PathToConfigFile, settings);
			writer.WriteStartDocument();
			writer.WriteStartElement("configuration");
			writer.WriteAttributeString("version", "1");

			if (EditorsSaveNow != null)
			{
				EditorsSaveNow.Invoke(writer, null);
			}

			_addins.Save(writer);

			writer.WriteEndDocument();
			writer.Close();

			base.Save(); // nb: this saves the writing system stuff, so if it is called before EditorsSaveNow, we won't get the latest stuff from editors working on them.
		}

		public Field GetFieldFromDefaultViewTemplate(string fieldName)
		{
			foreach (Field field in DefaultViewTemplate.Fields)
			{
				if (field.FieldName  == fieldName)
				{
					return field;
				}
			}
			return null;
		}

		public OptionsList GetOptionsList(string fieldName)
		{
			Field field = GetFieldFromDefaultViewTemplate(fieldName);
			if(field==null)
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

			string pathInProject = Path.Combine(PathToWeSaySpecificFilesDirectoryInProject, field.OptionsListFile);
			if (File.Exists(pathInProject))
			{
				LoadOptionsList(pathInProject);
			}
			else
			{
				string pathInProgramDir = Path.Combine(ApplicationCommonDirectory, field.OptionsListFile);
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
					fieldToOptionListName.Add(field.FieldName, GetListNameFromFileName(field.OptionsListFile));
				}
			}
			return fieldToOptionListName;
		}

		private static string GetListNameFromFileName(string file)
		{
			return Path.GetFileNameWithoutExtension(file);//file.Substring(0, file.IndexOf(".xml"));
		}

		public void MakeFieldNameChange(Field field, string oldName)
		{
			Debug.Assert(!String.IsNullOrEmpty(oldName));
			if (string.IsNullOrEmpty(oldName))
			{
				return;
			}
			oldName = Regex.Escape(oldName);

			//NB: we're just using regex, here, not xpaths which in this case
			//would be nice (e.g., "name" is a pretty generic thing to be changing)
			if (File.Exists(PathToLiftFile))
			{
				//traits
				if (field.DataTypeName == Field.BuiltInDataType.Option.ToString()
					|| field.DataTypeName == Field.BuiltInDataType.OptionCollection.ToString())
				{
					GrepFile(PathToLiftFile,
							 string.Format("name\\s*=\\s*[\"']{0}[\"']", oldName),
							 string.Format("name=\"{0}\"", field.FieldName));
				}
				else
				{
					//<field>s
					GrepFile(PathToLiftFile,
							 string.Format("tag\\s*=\\s*[\"']{0}[\"']", oldName),
							 string.Format("tag=\"{0}\"", field.FieldName));
				}
			}
		}

		public void MakeWritingSystemIdChange(WritingSystem ws, string oldId)
		{
			//Todo: WS-227 Before changing a ws id in a lift file, ensure that it isn't already in use

			WritingSystems.IdOfWritingSystemChanged(ws, oldId);
			DefaultViewTemplate.ChangeWritingSystemId(oldId, ws.Id);


			if (File.Exists(PathToLiftFile))
			{
				//todo: expand the regular expression here to account for all reasonable patterns
				GrepFile(PathToLiftFile,
						 string.Format("lang\\s*=\\s*[\"']{0}[\"']", Regex.Escape(oldId)),
						 string.Format("lang=\"{0}\"", ws.Id));
			}

			if(WritingSystemChanged!=null)
			{
				StringPair p = new StringPair();
				p.from = oldId;
				p.to = ws.Id;
				WritingSystemChanged.Invoke(this, p);
			}

			//this worked but it just gets overwritten when Setup closes
			/*if (File.Exists(PathToConfigFile))
			{
				GrepFile(PathToConfigFile,
						 string.Format("wordListWritingSystemId>\\s*{0}\\s*<", oldId),
						 string.Format("wordListWritingSystemId>{0}<", ws.Id));
			}
			*/
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

		private static void ReplaceFileWithUserInteractionIfNeeded(string tempPath, string inputPath, string backupPath)
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
					Palaso.Reporting.ErrorReport.ReportNonFatalMessage(Application.ProductName+" was unable to get at the dictionary file to update it.  Please ensure that WeSay isn't running with it open, then click the 'OK' button below. If you cannot figure out what program has the LIFT file open, the best choice is to kill WeSay Configuration Tool using the Task Manager (ctrl+alt+del), so that the configuration does not fall out of sync with the LIFT file.");
				}
			} while (!succeeded);
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

		private static string GetUniqueFileName(string path)
		{
			int i = 1;
			while (File.Exists(path + "old" + i))
			{
				++i;
			}
			return path + "old" + i;
		}



		/// <summary>
		/// Files to process when backing up or checking in
		/// </summary>
		/// <param name="pathToProjectRoot"></param>
		/// <returns></returns>
		public static string[] GetFilesBelongingToProject(string pathToProjectRoot)
		{
			List<String> files = new List<string>();
			string[] allFiles = Directory.GetFiles(pathToProjectRoot, "*", SearchOption.AllDirectories);
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

		private static bool Matches(string file, string[] antipatterns)
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


		public IRecordListManager MakeRecordListManager()
		{
			IRecordListManager recordListManager;

			if (PathToWeSaySpecificFilesDirectoryInProject.IndexOf("PRETEND") > -1)
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
				recordListManager = new Db4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), PathToDb4oLexicalModelDB);
				Db4oLexModelHelper.Initialize(((Db4oRecordListManager)recordListManager).DataSource.Data);
				Lexicon.Init(recordListManager as Db4oRecordListManager);
			}
			return recordListManager;
		}
	}
}
