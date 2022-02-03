using SIL.i18n;
using SIL.Lexicon;
using SIL.Reporting;
using SIL.Windows.Forms.Progress;
using SIL.WritingSystems;
using SIL.WritingSystems.Migration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using WeSay.UI;

namespace WeSay.Project
{
	public class BasilProjectTestHelper
	{
		/// <summary>
		/// Many tests throughout the system will not care at all about project related things,
		/// but they will break if there is no project initialized, since many things
		/// will reach the project through a static property.
		/// Those tests can just call this before doing anything else, so
		/// that other things don't break.
		/// </summary>
		public static void InitializeForTests()
		{
			ErrorReport.IsOkToInteractWithUser = false;
			var project = new BasilProject();
			project.LoadFromProjectDirectoryPath(BasilProject.GetPretendProjectDirectory());
			project.UiOptions.Language = "en";
		}
	}

	public class BasilProject : IDisposable
	{
		private static BasilProject _singleton;

		protected static BasilProject Singleton
		{
			get { return _singleton; }
		}
		public UiConfigurationOptions UiOptions { get; set; }

		private IWritingSystemRepository _writingSystems;
		private static string _projectDirectoryPath = string.Empty;

		public static BasilProject Project
		{
			get
			{
				if (_singleton == null)
				{
					throw new InvalidOperationException(
							"BasilProject Not initialized. For tests, call BasilProject.InitializeForTests().");
				}
				return _singleton;
			}
			set
			{
				if (_singleton != null)
				{
					_singleton.Dispose();
				}
				_singleton = value;
			}
		}

		public static bool IsInitialized
		{
			get { return _singleton != null; }
		}

		public BasilProject()
		{
			Project = this;
			_writingSystems = null;
			UiOptions = new UiConfigurationOptions();
		}

		public virtual void LoadFromProjectDirectoryPath(string projectDirectoryPath, IProgressNotificationProvider progressProvider = null)
		{
			LoadFromProjectDirectoryPath(projectDirectoryPath, false, progressProvider);
		}

		/// <summary>
		/// Tests can use this version
		/// </summary>
		/// <param name="projectDirectoryPath"></param>
		/// <param name="dontInitialize"></param>
		public virtual void LoadFromProjectDirectoryPath(string projectDirectoryPath,
														 bool dontInitialize,
														 IProgressNotificationProvider progressProvider = null)
		{
			_projectDirectoryPath = projectDirectoryPath;
			if (!dontInitialize)
			{
				InitStringCatalog();
				InitWritingSystems();
			}
		}

		protected static void OnWritingSystemLoadProblem(IEnumerable<WritingSystemRepositoryProblem> problems)
		{
			string message = @"There were some problems while loading the writing systems definitions in this project.
You can continue to work, but do let us know about the problem.
There are problems in:
";
			foreach (var problem in problems)
			{
				message += String.Format("  {0}", problem.Exception.Message);
			}

			ErrorReport.NotifyUserOfProblem(message);

		}
		protected static void OnWritingSystemMigration(int toVersion, IEnumerable<LdmlMigrationInfo> migrationinfo)
		{
			throw new ApplicationException("Input system migration should have been done by now, but it seems it hasn't.");
		}
		//        public virtual void CreateEmptyProjectFiles(string projectDirectoryPath)
		//      {
		//            _projectDirectoryPath = projectDirectoryPath;
		//            //  Directory.CreateDirectory(ProjectCommonDirectory);
		//            InitStringCatalog();
		//            InitWritingSystems();
		//            Save();
		//    }

		public virtual void Save()
		{
			_writingSystems.Save();
		}

		public static string GetPretendProjectDirectory()
		{
			return Path.Combine(ApplicationTestDirectory, Path.Combine("SampleProjects", "PRETEND"));
		}

		public IWritingSystemRepository WritingSystems
		{
			get { return _writingSystems; }
		}

		public IList<WritingSystemDefinition> WritingSystemsFromIds(IEnumerable<string> writingSystemIds)
		{
			return writingSystemIds.Select(id => WritingSystems.Get(id)).ToList();
		}

		/// <summary>
		/// When the user is playing around with writing system setup, he may delete a
		/// writing system that has been assigned to fields.  In that case, to avoid
		/// crashing (see https://jira.sil.org/browse/WS-233) we need to weed out any
		/// writing systems that no longer exist.
		/// </summary>
		public IList<string> FilterOutBadWritingSystems(IList<string> writingSystemIds)
		{
			List<string> validIds = new List<string>();
			foreach (var id in writingSystemIds)
			{
				if (WritingSystems.Contains(id))
					validIds.Add(id);
			}
			return validIds.ToList();
		}

		public string ProjectDirectoryPath
		{
			get { return _projectDirectoryPath; }
			protected set { _projectDirectoryPath = value; }
		}

		//public static string GetPathToWritingSystemPrefs(string parentDir)
		//{
		//        return Path.Combine(parentDir, "WritingSystemPrefs.xml");
		//}

		public static string GetPathToLdmlWritingSystemsFolder(string parentDir)
		{
			return Path.Combine(parentDir, "WritingSystems");
		}

		public static string GetPathToSharedSettingsFolder(string parentDir)
		{
			return Path.Combine(parentDir, "SharedSettings");
		}

		public static string GetPathToPictures(string parentDir)
		{
			return Path.Combine(parentDir, "pictures");
		}

		// <summary>
		// Locates the StringCatalog file, matching any file ending in <language>.po first in the Project folder,
		// then in the Application Common folder.
		// </summary>
		public string LocateStringCatalog()
		{
			string filePattern = "*" + UiOptions.Language + ".po";
			var matchingFiles = Directory.GetFiles(ProjectDirectoryPath, filePattern);
			if (matchingFiles.Length > 0)
			{
				return matchingFiles[0];
			}

			//fall back to the program's common directory
			matchingFiles = Directory.GetFiles(ApplicationCommonDirectory, filePattern);
			if (matchingFiles.Length > 0)
			{
				return matchingFiles[0];
			}

			return null;
		}

		/// <summary>
		/// Name and release stage of app, for usage as a directory name.
		/// </summary>
		public static string WeSaySharedDirectory
		{
			get
			{
				string dir = "wesay";
#if ALPHA
				dir += "-alpha";
#elif BETA
				dir += "-beta";
#elif CANDIDATE
				dir += "-beta";
#endif
				return dir;
			}
		}

		/// <summary>
		/// Path to directory where the app's data is installed, such as
		/// `/usr/share/wesay` or `/app/share/wesay` or CommonApplicationData/wesay.
		/// </summary>
		private static string AppInstalledDataDir
		{
			get
			{
				if (Platform.IsLinux)
				{
					return Path.Combine(InstallationPrefixDir, "share", WeSaySharedDirectory);
				} else {
					string dir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
					return Path.Combine(dir, WeSaySharedDirectory);
				}
			}
		}

		/// <summary>
		/// Linux installation prefix directory, such as `/usr` or `/app`.
		/// </summary>
		private static string InstallationPrefixDir
		{
			get
			{
				// If WeSay.App.exe is installed to /app/lib/wesay/WeSay.App.exe,
				// The prefix (/app) is two levels up.
				return Directory.GetParent(DirectoryOfTheApplicationExecutable).Parent.ToString();
			}
		}

		/// <summary>
		/// Path to the directory containing files from git repo directory named "common".
		/// </summary>
		public static string ApplicationCommonDirectory
		{
			get
			{
				var appCommonDir = Path.Combine(DirectoryOfTheApplicationExecutable, "common");
				if (!Directory.Exists(appCommonDir))
				{
					appCommonDir = AppInstalledDataDir;
				}
				if (!Directory.Exists(appCommonDir))
				{
					appCommonDir = Path.Combine(TopDevDirectory, "common");
				}
				return appCommonDir;
			}
		}

		public static string ApplicationRootDirectory
		{
			get { return DirectoryOfTheApplicationExecutable; }
		}

		public static string ApplicationSharedDirectory
		{
			get
			{
				string shareddir;
				bool unitTesting = Assembly.GetEntryAssembly() == null;
				if (unitTesting)
				{
					shareddir = DirectoryOfTheApplicationExecutable;
				}
				else
				{
					shareddir = AppInstalledDataDir;
				}
				return shareddir;
			}
		}

		public static string ApplicationTestDirectory => Path.Combine(TopDevDirectory ?? throw new InvalidOperationException(), "test");

		// On development machines we run in src\output\Debug so get the parent directory of src to access other directories
		protected static string TopDevDirectory => Path.GetDirectoryName(Path.GetDirectoryName(DirectoryOfTheApplicationExecutable));

		public static string DirectoryOfTheApplicationExecutable
		{
			get
			{
				string path;
				bool unitTesting = Assembly.GetEntryAssembly() == null;
				if (unitTesting)
				{
					path = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
					path = Uri.UnescapeDataString(path);
				}
				else
				{
					//was suspect in WS1156, where it seemed to start looking in the,
					//outlook express program folder after sending an email from wesay...
					//so maybe it doesn't always mean *this* executing assembly?
					//  path = Assembly.GetExecutingAssembly().Location;
					path = Application.ExecutablePath;
				}
				return Directory.GetParent(path).FullName;
			}
		}

		protected string OldProjectCommonDirectory
		{
			get { return Path.Combine(_projectDirectoryPath, "common"); }
		}

		public virtual string Name
		{
			get { return "Need to override"; }
		}

		public virtual void Dispose()
		{
			if (_singleton == this)
			{
				_singleton = null;
			}
		}

		public static void CopyWritingSystemsFromApplicationCommonDirectoryToNewProject(string projectDirectoryPath)
		{
			string pathProjectToWritingSystemsFolder = GetPathToLdmlWritingSystemsFolder(projectDirectoryPath);
			string pathCommonToWritingSystemsFolder = GetPathToLdmlWritingSystemsFolder(ApplicationCommonDirectory);
			Directory.CreateDirectory(pathProjectToWritingSystemsFolder);
			foreach (string path in Directory.GetFiles(pathCommonToWritingSystemsFolder, "*.ldml"))
			{
				var destPath = Path.Combine(pathProjectToWritingSystemsFolder, Path.GetFileName(path));
				if (!File.Exists(destPath))
				{
					File.Copy(path, destPath);
				}
			}
		}

		protected void InitWritingSystems()
		{
			if (!Directory.Exists(GetPathToLdmlWritingSystemsFolder(ProjectDirectoryPath)))
			{
				CopyWritingSystemsFromApplicationCommonDirectoryToNewProject(ProjectDirectoryPath);
			}
			if (!Directory.Exists(GetPathToSharedSettingsFolder(ProjectDirectoryPath)))
			{
				Directory.CreateDirectory(GetPathToSharedSettingsFolder(ProjectDirectoryPath));
			}
			if (_writingSystems == null)
			{
				var userSettingsDataMapper =
					new UserLexiconSettingsWritingSystemDataMapper(new FileSettingsStore(
						LexiconSettingsFileHelper.GetUserLexiconSettingsPath(ProjectDirectoryPath)));
				var projectSettingsDataMapper =
					new ProjectLexiconSettingsWritingSystemDataMapper(new FileSettingsStore(
						LexiconSettingsFileHelper.GetProjectLexiconSettingsPath(ProjectDirectoryPath)));
				ICustomDataMapper<WritingSystemDefinition>[] customDataMapper =
				{
					userSettingsDataMapper,
					projectSettingsDataMapper
				};

				_writingSystems = LdmlInFolderWritingSystemRepository.Initialize(
					GetPathToLdmlWritingSystemsFolder(ProjectDirectoryPath),
					customDataMapper,
					null,
					OnWritingSystemMigration,
					OnWritingSystemLoadProblem);

				// Set default configurations
				foreach (string id in _writingSystems.AllWritingSystems.Select(ws => ws.LanguageTag).ToArray())
				{
					var ws = _writingSystems.Get(id);
					if (id == "en")
						ws.Abbreviation = "eng";
					if (ws.DefaultCollation == null)
						ws.DefaultCollation = new IcuRulesCollationDefinition("standard");
					if (ws.DefaultFont == null)
						ws.DefaultFont = new FontDefinition("Arial");
					if (ws.DefaultFontSize == 0)
						ws.DefaultFontSize = 12;
					_writingSystems.Set(ws);
				}
				_writingSystems.Save();
			}
		}

		protected void InitStringCatalog()
		{
			try
			{
				if (UiOptions.Language == "test")
				{
					new StringCatalog("test", UiOptions.LabelFontName, UiOptions.LabelFontSizeInPoints);
				}
				string p = LocateStringCatalog();
				if (p == null)
				{
					new StringCatalog(UiOptions.LabelFontName, UiOptions.LabelFontSizeInPoints);
				}
				else
				{
					new StringCatalog(p, UiOptions.LabelFontName, UiOptions.LabelFontSizeInPoints);
				}
			}
			catch (FileNotFoundException)
			{
				//todo:when we add logging, this would be a good place to log a problem
				new StringCatalog();
			}
		}

		public static string VersionString
		{
			get
			{
				string versionString = Application.ProductVersion;
#if ALPHA
				versionString += " ALPHA";
#elif BETA
				versionString += " BETA";
#elif CANDIDATE
				versionString += " RC";
#endif
				return versionString;
			}
		}

	}
}
