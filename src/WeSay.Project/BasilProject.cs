using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using Palaso.i18n;
using Palaso.Reporting;
using Palaso.Xml;
using WeSay.LexicalModel.Foundation;

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

	public class BasilProject: IDisposable
	{
		private static BasilProject _singleton;

		protected static BasilProject Singleton
		{
			get { return _singleton; }
		}
		public UiConfigurationOptions UiOptions { get; set; }

		private WritingSystemCollection _writingSystems;
		private string _projectDirectoryPath = string.Empty;

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

		public virtual void LoadFromProjectDirectoryPath(string projectDirectoryPath)
		{
			LoadFromProjectDirectoryPath(projectDirectoryPath, false);
		}

		/// <summary>
		/// Tests can use this version
		/// </summary>
		/// <param name="projectDirectoryPath"></param>
		/// <param name="dontInitialize"></param>
		public virtual void LoadFromProjectDirectoryPath(string projectDirectoryPath,
														 bool dontInitialize)
		{
			_projectDirectoryPath = projectDirectoryPath;
			if (!dontInitialize)
			{
				InitStringCatalog();
				InitWritingSystems();
			}
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
			return Path.Combine(GetTopAppDirectory(), Path.Combine("SampleProjects", "PRETEND"));
		}

		public WritingSystemCollection WritingSystems
		{
			get { return _writingSystems; }
		}

		public IList<WritingSystem> WritingSystemsFromIds(IEnumerable<string> writingSystemIds)
		{
			List<WritingSystem> l = new List<WritingSystem>();
			foreach (string id in writingSystemIds)
			{
				l.Add(WritingSystems.Get(id));
			}
			return l;
		}

		public string ProjectDirectoryPath
		{
			get { return _projectDirectoryPath; }
			protected set { _projectDirectoryPath = value; }
		}

		public static string GetPathToWritingSystemPrefs(string parentDir)
		{
				return Path.Combine(parentDir, "WritingSystemPrefs.xml");
		}

		public static string GetPathToLdmlWritingSystemsFolder(string parentDir)
		{
				return Path.Combine(parentDir, "WritingSystems");
		}

		//        public string PathToOptionsLists
		//        {
		//            get
		//            {
		//                return GetPathToWritingSystemPrefs(CommonDirectory);
		//            }
		//        }


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

		public static string ApplicationCommonDirectory
		{
			get { return Path.Combine(GetTopAppDirectory(), "common"); }
		}

		public static string ApplicationRootDirectory
		{
			get { return DirectoryOfTheApplicationExecutable; }
		}

		public string ApplicationTestDirectory
		{
			get { return Path.Combine(GetTopAppDirectory(), "test"); }
		}

		protected static string GetTopAppDirectory()
		{
			string path = DirectoryOfTheApplicationExecutable;
			char sep = Path.DirectorySeparatorChar;
			int i = path.ToLower().LastIndexOf(sep + "output" + sep);

			if (i > -1)
			{
				path = path.Substring(0, i + 1);
			}
			return path;
		}

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
				File.Copy(path, Path.Combine(pathProjectToWritingSystemsFolder, Path.GetFileName(path)));
			}
		}

		protected void InitWritingSystems()
		{
			if (!Directory.Exists(GetPathToLdmlWritingSystemsFolder(ProjectDirectoryPath)))
			{
				CopyWritingSystemsFromApplicationCommonDirectoryToNewProject(ProjectDirectoryPath);
			}
			_writingSystems = new WritingSystemCollection(
				GetPathToLdmlWritingSystemsFolder(ProjectDirectoryPath)
			);

			// TODO Chris move this to the migrator
			//if (_writingSystems.Count == 0)
			//{
			//    _writingSystems.LoadFromLegacyWeSayFile(GetPathToWritingSystemPrefs(ProjectDirectoryPath));
			//    _writingSystems.Write(GetPathToLdmlWritingSystemsFolder(ProjectDirectoryPath));
			//    File.Delete(GetPathToWritingSystemPrefs(ProjectDirectoryPath));
			//}
		}

		//        /// <summary>
		//        /// Get the options lists, e.g. PartsOfSpeech, from files
		//        /// </summary>
		//        private void InitOptionsLists()
		//        {
		//            Directory.
		//        }


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
			get { return Application.ProductVersion; }
		}

	}
}
