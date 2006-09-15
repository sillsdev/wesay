using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using WeSay.Language;

namespace WeSay.UI
{
	public class WeSayWordsProject : BasilProject
	{
		public WeSayWordsProject(string projectDirectoryPath) : base(projectDirectoryPath){}

		public string PathToTaskConfig
		{
			get
			{
				return System.IO.Path.Combine(ApplicationDirectory, "tasks.xml");
			}
		}

		public string PathToLexicalModelDB
		{
			get
			{
				return System.IO.Path.Combine(ProjectDirectoryPath, "lexicon.yap");
			}
		}

		private string ApplicationDirectory
		{
			get
			{
				return System.IO.Path.Combine(ProjectDirectoryPath, "WeSay");
			}
		}

	}

	public class BasilProject : WeSay.UI.IProject, IDisposable
	{
		private static BasilProject _singleton;

		private System.Collections.Generic.Dictionary<string, WritingSystem> _writingSystems;

		public System.Collections.Generic.Dictionary<string, WritingSystem> WritingSystems
		{
			get
			{
				return _writingSystems;
			}
		}
		private string _projectDirectoryPath;

		protected string ProjectDirectoryPath
		{
			get
			{
				return _projectDirectoryPath;
			}
		}

		private XmlDocument _fontPrefsDoc;
		private  WritingSystem _vernacularWritingSystemDefault;
		private  WritingSystem _analysisWritingSystemDefault;
		private string _stringCatalogSelector;

		public static BasilProject Project
		{
			get
			{
				if (_singleton == null)
				{
					throw new ApplicationException("BasilProject Not initialized. For tests, call BasilProject.InitializeForTests().");
				}
				return _singleton;
			}
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
			BasilProject project = new BasilProject(@"../../SampleProjects/PRETEND");
			project.InitWritingSystems();
			project.StringCatalogSelector = "en";
			project.InitStringCatalog();
		}

		public BasilProject(string projectDirectoryPath)
		{
			 _projectDirectoryPath = projectDirectoryPath;
			 _singleton = this;
			_writingSystems = new Dictionary<string, WritingSystem>();
		}

		public string PathToWritingSystemPrefs
		{
			get
			{
				return System.IO.Path.Combine(CommonDirectory, "writingSystemPrefs.xml");
			}
		}

		public string LocateStringCatalog()
		{
			if (File.Exists(PathToStringCatalogInProjectDir))
			{
				return PathToStringCatalogInProjectDir;
			}

			//fall back to the program's common directory
			string path = System.IO.Path.Combine(ApplicationCommonDirectory, _stringCatalogSelector + ".po");
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
				return System.IO.Path.Combine(CommonDirectory, _stringCatalogSelector+".po");
			}
		}

		public string ApplicationCommonDirectory
		{
			get
			{
				//GetEntryAssembly is null from nunit
				//string path = Assembly.GetEntryAssembly().CodeBase.Replace("file:///", "");
				string path = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "");
				//go up to dir containing the executable
				path = Directory.GetParent(path).FullName;
			   if (path.ToLower().IndexOf("output") > -1)
				{
					//go up to output
					path = Directory.GetParent(path).FullName;
				   //go up to directory containing output
					path = Directory.GetParent(path).FullName;
				}
				 path =System.IO.Path.Combine(path, "common");
				 return path;
			}
		}

		private string CommonDirectory
		{
			get
			{
				return System.IO.Path.Combine(_projectDirectoryPath, "common");
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

		}

		/// <summary>
		/// NB: this is currently not part of normal construction in order to facilitate
		/// tests which need to access paths provided by the project in order to create
		/// the files which this method will read.
		/// </summary>
		public void InitWritingSystems()
		{
			_fontPrefsDoc = new XmlDocument();
			_fontPrefsDoc.Load(PathToWritingSystemPrefs);
			foreach(XmlNode node in _fontPrefsDoc.SelectNodes("prefs/writingSystem"))
			{
				 WritingSystem ws = new WritingSystem(node);
				 _writingSystems.Add(ws.Id, ws);
			}

			string id = GetIdOfLabelledWritingSystem("analysisWritingSystem");
			_analysisWritingSystemDefault = _writingSystems[id];
			id = GetIdOfLabelledWritingSystem("vernacularWritingSystem");
			_vernacularWritingSystemDefault = _writingSystems[id];
		}

		private string GetIdOfLabelledWritingSystem(string label)
		{
			return _fontPrefsDoc.SelectSingleNode("prefs").Attributes[label].Value;
		}


//        public static void LoadVernacularWritingSystem(string filePath)
//        {
//            _vernacularWritingSystemDefault = new WritingSystem(filePath);
//            _writingSystems.Add(WritingSystem._vernacularWritingSystemDefault.Id, WritingSystem._vernacularWritingSystemDefault);
//        }
//        public static void LoadAnalysisWritingSystem(string filePath)
//        {
//            _analysisWritingSystemDefault = new WritingSystem(filePath);
//            _writingSystems.Add(WritingSystem._analysisWritingSystemDefault.Id, WritingSystem._analysisWritingSystemDefault);
//        }

		public  WritingSystem AnalysisWritingSystemDefault
		{
			get
			{
				 return _analysisWritingSystemDefault;
			}
		}

		public  WritingSystem VernacularWritingSystemDefault
		{
			get
			{
				return _vernacularWritingSystemDefault;
			}
		}

		public string StringCatalogSelector
		{
			get { return _stringCatalogSelector; }
			set { _stringCatalogSelector = value; }
		}


		private  WritingSystem GetWritingSystem(string id)
		{
			if (!_writingSystems.ContainsKey(id))
			{
				System.Drawing.Font font = new System.Drawing.Font("Arial", 12, FontStyle.Italic);//italic 'cause something's wrong
				_writingSystems.Add(id, new WritingSystem(id, font));
			}
			return _writingSystems[id];
		}

		public void InitStringCatalog()
		{
			try
			{
				string p = this.LocateStringCatalog();
				if (p == null)
				{
					new StringCatalog();
				}
				else
				{
					new StringCatalog(p);
				}
			}
			catch(System.IO.FileNotFoundException )
			{
				//todo: get some ui message up
				new StringCatalog();
			}
		}
	}
}
