using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Foundation;

namespace WeSay.Project
{
	public class BasilProject: IProject, IDisposable
	{
		private static BasilProject _singleton;
		private string _uiFontName;

		protected static BasilProject Singleton
		{
			get { return _singleton; }
			set { _singleton = value; }
		}

		private readonly WritingSystemCollection _writingSystems;
		private string _projectDirectoryPath = string.Empty;
		private string _stringCatalogSelector = string.Empty;
		private float _uiFontSize;

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
			_writingSystems = new WritingSystemCollection();
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

		public virtual void CreateEmptyProjectFiles(string projectDirectoryPath)
		{
			_projectDirectoryPath = projectDirectoryPath;
			//  Directory.CreateDirectory(ProjectCommonDirectory);
			InitStringCatalog();
			InitWritingSystems();
			Save();
		}

		public virtual void Save()
		{
			Save(_projectDirectoryPath);
		}

		public virtual void Save(string projectDirectoryPath)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			XmlWriter writer = XmlWriter.Create(PathToWritingSystemPrefs, settings);
			_writingSystems.Write(writer);
			writer.Close();
		}

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
			BasilProject project = new BasilProject();
			project.LoadFromProjectDirectoryPath(GetPretendProjectDirectory());
			project.StringCatalogSelector = "en";
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
				l.Add(WritingSystems[id]);
			}
			return l;
		}

		public string ProjectDirectoryPath
		{
			get { return _projectDirectoryPath; }
			protected set { _projectDirectoryPath = value; }
		}

		public string PathToWritingSystemPrefs
		{
			get
			{
				return
						GetPathToWritingSystemPrefs(
								PathToDirectoryContaingWritingSystemFilesInProject
								/*ProjectCommonDirectory*/);
			}
		}

		//        public string PathToOptionsLists
		//        {
		//            get
		//            {
		//                return GetPathToWritingSystemPrefs(CommonDirectory);
		//            }
		//        }

		private static string GetPathToWritingSystemPrefs(string parentDir)
		{
			return Path.Combine(parentDir, "WritingSystemPrefs.xml");
		}

		public string PathToDirectoryContaingWritingSystemFilesInProject
		{
			get { return ProjectDirectoryPath; }
		}

		public string LocateStringCatalog()
		{
			if (File.Exists(PathToStringCatalogInProjectDir))
			{
				return PathToStringCatalogInProjectDir;
			}

			//fall back to the program's common directory
			string path = Path.Combine(ApplicationCommonDirectory, _stringCatalogSelector + ".po");
			if (File.Exists(path))
			{
				return path;
			}

			else
			{
				return null;
			}
		}

		public string PathToStringCatalogInProjectDir
		{
			get
			{
				return Path.Combine(ProjectDirectoryPath /*ProjectCommonDirectory*/,
									_stringCatalogSelector + ".po");
			}
		}

		public static string ApplicationCommonDirectory
		{
			get { return Path.Combine(GetTopAppDirectory(), "common"); }
		}

		public static string ApplicationRootDirectory
		{
			get { return DirectoryOfExecutingAssembly; }
		}

		public string ApplicationTestDirectory
		{
			get { return Path.Combine(GetTopAppDirectory(), "test"); }
		}

		protected static string GetTopAppDirectory()
		{
			string path;

			path = DirectoryOfExecutingAssembly;
			char sep = Path.DirectorySeparatorChar;
			int i = path.ToLower().LastIndexOf(sep + "output" + sep);

			if (i > -1)
			{
				path = path.Substring(0, i + 1);
			}
			return path;
		}

		public static string DirectoryOfExecutingAssembly
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
					path = Assembly.GetExecutingAssembly().Location;
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

		private void InitWritingSystems()
		{
			if (File.Exists(PathToWritingSystemPrefs))
			{
				_writingSystems.Load(PathToWritingSystemPrefs);
			}
			else
			{
				//load defaults
				_writingSystems.Load(GetPathToWritingSystemPrefs(ApplicationCommonDirectory));
			}
		}

		//        /// <summary>
		//        /// Get the options lists, e.g. PartsOfSpeech, from files
		//        /// </summary>
		//        private void InitOptionsLists()
		//        {
		//            Directory.
		//        }

		public string StringCatalogSelector
		{
			get { return _stringCatalogSelector; }
			set { _stringCatalogSelector = value; }
		}

		protected string UiFontName
		{
			get { return _uiFontName; }
			set { _uiFontName = value; }
		}

		protected float UiFontSizeInPoints
		{
			get { return _uiFontSize; }
			set { _uiFontSize = value; }
		}

		private void InitStringCatalog()
		{
			try
			{
				if (_stringCatalogSelector == "test")
				{
					new StringCatalog("test", UiFontName, UiFontSizeInPoints);
				}
				string p = LocateStringCatalog();
				if (p == null)
				{
					new StringCatalog(UiFontName, UiFontSizeInPoints);
				}
				else
				{
					new StringCatalog(p, UiFontName, UiFontSizeInPoints);
				}
			}
			catch (FileNotFoundException)
			{
				//todo:when we add logging, this would be a good place to log a problem
				new StringCatalog();
			}
		}
	}
}
