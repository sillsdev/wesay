using System;
using System.IO;
using System.Reflection;
using System.Xml;
using WeSay.Language;

namespace WeSay.Project
{
	public class BasilProject : IProject, IDisposable
	{
	  private static BasilProject _singleton;

	  protected static BasilProject Singleton
	  {
		get
		{
		  return BasilProject._singleton;
		}
		set
		{
		  BasilProject._singleton = value;
		}
	  }
		private WritingSystemCollection _writingSystems;
		protected  string _projectDirectoryPath;
		private string _stringCatalogSelector;

		public event EventHandler HackedEditorsSaveNow;

		public static BasilProject Project
		{
			get
			{
				if (_singleton == null)
				{
				  throw new InvalidOperationException("BasilProject Not initialized. For tests, call BasilProject.InitializeForTests().");
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
			get
			{
				return _singleton != null;
			}
		}

		public BasilProject()
		{
			Project =this;
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
		public virtual void LoadFromProjectDirectoryPath(string projectDirectoryPath, bool dontInitialize)
		{
			this._projectDirectoryPath = projectDirectoryPath;
			if (!dontInitialize)
			{
				InitStringCatalog();
				InitWritingSystems();
			}
		}

		public virtual void CreateEmptyProjectFiles(string projectDirectoryPath)
		{
			this._projectDirectoryPath = projectDirectoryPath;
			Directory.CreateDirectory(CommonDirectory);
			InitStringCatalog();
			InitWritingSystems();
			Save();
		}

		public virtual void Save()
		{
		   Save(_projectDirectoryPath);
		   if (HackedEditorsSaveNow != null)
		   {
			   HackedEditorsSaveNow.Invoke(this, null);
		   }
		}

	   public virtual void Save(string projectDirectoryPath)
		{
			XmlWriter writer = XmlWriter.Create(PathToWritingSystemPrefs);
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
			Reporting.ErrorReporter.OkToInteractWithUser = false;
			BasilProject project = new BasilProject();
			project.LoadFromProjectDirectoryPath(GetPretendProjectDirectory());
			project.StringCatalogSelector = "en";
		}

		public static string GetPretendProjectDirectory()
		{
			return Path.Combine(GetTopAppDirectory(), Path.Combine("SampleProjects","PRETEND"));
		}

		public WritingSystemCollection WritingSystems
		{
			get
			{
				return _writingSystems;
			}
		}

		public string ProjectDirectoryPath
		{
			get
			{
				return _projectDirectoryPath;
			}
		}

		public string PathToWritingSystemPrefs
		{
			get
			{
				return GetPathToWritingSystemPrefs(CommonDirectory);
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
			return Path.Combine(parentDir, "writingSystemPrefs.xml");
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

			else return null;
		}


		public string PathToStringCatalogInProjectDir
		{
			get
			{
				return Path.Combine(CommonDirectory, _stringCatalogSelector+".po");
			}
		}
		public string ApplicationCommonDirectory
		{
			get
			{
				return Path.Combine(GetTopAppDirectory(), "common");
			}
		}

		public string ApplicationTestDirectory
		{
			get
			{
				return Path.Combine(GetTopAppDirectory(), "test");
			}
		}

		protected static string GetTopAppDirectory()
		{
			string path;

			bool unitTesting = Assembly.GetEntryAssembly() == null;
			if (unitTesting)
			{
				path = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
			}
			else
			{
				path = Assembly.GetExecutingAssembly().Location;
			}

			//go up to dir containing the executable
			path = Directory.GetParent(path).FullName;
			if (path.ToLower().IndexOf("output") > -1)
			{
				//go up to output
				path = Directory.GetParent(path).FullName;
				//go up to directory containing output
				path = Directory.GetParent(path).FullName;
			}
			return path;
		}

		private string CommonDirectory
		{
			get
			{
				return Path.Combine(_projectDirectoryPath, "common");
			}
		}

		public string Name
		{
			get
			{
				//we don't really want to give this directory out... this is just for a test
				return Path.GetFileName(this._projectDirectoryPath);
			}
		}


		public void Dispose()
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
			{   //load defaults
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


		private void InitStringCatalog()
		{
			try
			{
				string p = LocateStringCatalog();
				if (p == null)
				{
					new StringCatalog();
				}
				else
				{
					new StringCatalog(p);
				}
			}
			catch(FileNotFoundException )
			{
				//todo:when we add logging, this would be a good place to log a problem
				new StringCatalog();
			}
		}


	}
}
