using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Reporting;
using WeSay.Foundation;
using WeSay.Foundation.Progress;
using WeSay.LexicalModel;

namespace WeSay.Project
{
	public class WeSayWordsProject : BasilProject
	{
		//private string _lexiconDatabaseFileName = null;
		private IList<ITask> _tasks;
		private ViewTemplate _viewTemplate;
		private Dictionary<string, OptionsList> _optionLists;
		private string _pathToLiftFile;
		private string _cacheLocationOverride;
		private FileStream _liftFileStreamForLocking;

		public WeSayWordsProject()
		{
			_optionLists = new Dictionary<string, OptionsList>();
		}

		public IList<ITask> Tasks
		{
			get
			{
				return this._tasks;
			}
			set
			{
				this._tasks = value;
			}
		}

		public static new WeSayWordsProject Project
		{
			get
			{
				if (Singleton == null)
				{
				  throw new InvalidOperationException("WeSayWordsProject Not initialized. For tests, call BasilProject.InitializeForTests().");
				}
				return (WeSayWordsProject) Singleton;
			}
		}


		/// <summary>
		/// See comment on BasilProject.InitializeForTests()
		/// </summary>
		public static new void InitializeForTests()
		{
			WeSayWordsProject project = new WeSayWordsProject();
			string s = Path.Combine(GetPretendProjectDirectory(),"WeSay");
			s = Path.Combine(s,"pretend.lift");
			project.SetupProjectDirForTests(s);
		}

		public void SetupProjectDirForTests(string pathToLift)
		{
			_projectDirectoryPath = Directory.GetParent(pathToLift).Parent.FullName;
			PathToLiftFile = pathToLift;
//            if (!Directory.Exists(pathToLift))
//            {
//                Directory.CreateDirectory(pathToLift);
//            }
			if (File.Exists(PathToProjectTaskInventory))
			{
				File.Delete(PathToProjectTaskInventory);
			}
			File.Copy(Path.Combine(ApplicationTestDirectory, "tasks.xml"), WeSayWordsProject.Project.PathToProjectTaskInventory, true);

			Reporting.ErrorReporter.OkToInteractWithUser = false;
			LoadFromProjectDirectoryPath(_projectDirectoryPath);
			StringCatalogSelector = "en";
		}

		public bool LoadFromLiftLexiconPath(string liftPath)
		{
			try
			{
				PathToLiftFile = liftPath;
				if (!File.Exists(liftPath))
				{
					throw new ApplicationException("WeSay cannot find a lexicon where it was looking, which is at " +
												   liftPath);
				}

				//walk up from file to /wesay to /<project>
				_projectDirectoryPath = Directory.GetParent(Directory.GetParent(liftPath).FullName).FullName;

				if (CheckLexiconIsInValidProjectDirectory(liftPath))
				{
					this._projectDirectoryPath = ProjectDirectoryPath;

					/*if (GetCacheIsOutOfDate(liftPath))
					{
						throw new ApplicationException(
							"Possible programming error. The cache should be up-to-date before calling this method.");
					}
					*/
					base.LoadFromProjectDirectoryPath(
						ProjectDirectoryPath);
					return true;
				}
				else
				{
					PathToLiftFile = null;
					_projectDirectoryPath = null;
					return false;
				}
			}
			catch (Exception e)
			{
				ErrorReporter.ReportNonFatalMessage(e.Message);
				return false;
			}
		}



		private ViewTemplate GetViewTemplateFromProjectFiles()
		{
			ViewTemplate template = new ViewTemplate();
			try
			{
				XmlDocument projectDoc = GetProjectDoc();
				if (projectDoc != null)
				{
					XmlNode node = projectDoc.SelectSingleNode("tasks/components/viewTemplate");
					template.LoadFromString(node.OuterXml);
				}
			}
			catch (Exception error)
			{
				MessageBox.Show("There may have been a problem reading the field template xml. A default template will be created." + error.Message);
			}
			return template;
		}


		private XmlDocument GetProjectDoc()
		{
			XmlDocument projectDoc = null;
			if (File.Exists(PathToProjectTaskInventory))
			{
				try
				{
					projectDoc = new XmlDocument();
					projectDoc.Load(WeSayWordsProject.Project.PathToProjectTaskInventory);
				}
				catch (Exception e)
				{
					Reporting.ErrorReporter.ReportNonFatalMessage("There was a problem reading the task xml. " + e.Message);
					projectDoc = null;
				}
			}
			return projectDoc;
		}


		private bool CheckLexiconIsInValidProjectDirectory(string liftPath)
		{
			DirectoryInfo lexiconDirectoryInfo = Directory.GetParent(liftPath);
			DirectoryInfo projectRootDirectoryInfo = lexiconDirectoryInfo.Parent;
			string lexiconDirectoryName = lexiconDirectoryInfo.Name;
			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				//windows
				lexiconDirectoryName = lexiconDirectoryName.ToLowerInvariant();
			}

			if (projectRootDirectoryInfo == null ||
				lexiconDirectoryName != "wesay" ||
				(!IsValidProjectDirectory(projectRootDirectoryInfo.FullName)))
			{
				string message = "WeSay cannot open the lexicon, because it is not in a proper WeSay/Basil project structure.";
				ErrorReporter.ReportNonFatalMessage(message);
				return false;
			}
			return true;
		}

		public override void CreateEmptyProjectFiles(string projectDirectoryPath)
		{
			base.CreateEmptyProjectFiles(projectDirectoryPath);
			Directory.CreateDirectory(PathToWeSaySpecificFilesDirectoryInProject);
			_viewTemplate = ViewTemplate.MakeMasterTemplate(WritingSystems);
		   // this._lexiconDatabaseFileName = this.Name+".words";


			if (!File.Exists(PathToLiftFile))
			{
				LiftIO.Utilities.CreateEmptyLiftFile(PathToLiftFile, LiftExporter.ProducerString, false);
			}

		}

		public static bool IsValidProjectDirectory(string dir)
		{
			string[] requiredDirectories = new string[] { "common", "wesay" };
			foreach (string s in requiredDirectories)
			{
				if (!Directory.Exists(Path.Combine(dir, s)))
					return false;
			}
			return true;
		}

		public string PathToProjectTaskInventory
		{
			get
			{
				return System.IO.Path.Combine(PathToWeSaySpecificFilesDirectoryInProject, "tasks.xml");
			}
		}

		public override void Dispose()
		{
			base.Dispose();
			if (_liftFileStreamForLocking != null)
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


		public void LockLift()
		{
			Debug.Assert(_liftFileStreamForLocking == null);
			 _liftFileStreamForLocking = File.OpenRead(PathToLiftFile);
		}

		public string PathToLiftFile
		{
			get
			{
				if (String.IsNullOrEmpty(_pathToLiftFile))
				{
					_pathToLiftFile =
						Path.Combine(PathToWeSaySpecificFilesDirectoryInProject, Path.GetFileName(ProjectDirectoryPath ) + ".lift");
				}
				return _pathToLiftFile;
			}

			set
			{
				_pathToLiftFile = value;
				if (value == null)
				{
					_projectDirectoryPath = null;
				}
				else
				{
					_projectDirectoryPath = Directory.GetParent(value).Parent.FullName;
				}
			}
		}

		public string PathToLiftBackupDir
		{
			get
			{
				return PathToLiftFile + " incremental xml backup";
			}
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

		private static string GetPathToCacheFromPathToLift(string pathToLift)
		{
			return Path.Combine(Path.GetDirectoryName(pathToLift), "Cache");
		}

		public string PathToDb4oLexicalModelDB
		{
			get
			{
				return GetPathToDb4oLexicalModelDBFromPathToLift(PathToLiftFile);
			}
		}

		public string GetPathToDb4oLexicalModelDBFromPathToLift(string pathToLift)
		{
				return System.IO.Path.Combine(GetPathToCacheFromPathToLift(pathToLift), Path.GetFileNameWithoutExtension(pathToLift) + ".words");
		}




		public string PathToWeSaySpecificFilesDirectoryInProject
		{
			get
			{
				return Path.Combine(ProjectDirectoryPath, "wesay");
			}
		}

		public ViewTemplate ViewTemplate
		{
			get
			{
				if(_viewTemplate==null)
				{
					ViewTemplate templateAsFoundInProjectFiles = GetViewTemplateFromProjectFiles();
					ViewTemplate fullUpToDateTemplate = ViewTemplate.MakeMasterTemplate(WritingSystems);
					ViewTemplate.SynchronizeInventories(fullUpToDateTemplate, templateAsFoundInProjectFiles);
					_viewTemplate = fullUpToDateTemplate;
				}
				return _viewTemplate;
			}
		}

		public IEnumerable OptionFieldNames
		{
			get
			{
				List<string> names = new List<string>();

				foreach (Field field in ViewTemplate.Fields)
				{
					if(field.DataTypeName=="Option")
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

				foreach (Field field in ViewTemplate.Fields)
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
			set
			{
				_cacheLocationOverride = value;
			}
		}


		public OptionsList GetOptionsList(string name)
		{
			OptionsList list;
			if(_optionLists.TryGetValue(name, out list))
			{
				return list;
			}

			string pathInProject = Path.Combine(this.PathToWeSaySpecificFilesDirectoryInProject, name);
			if (File.Exists(pathInProject))
			{
				LoadOptionsList(pathInProject);
			}
			else
			{
				string pathInProgramDir = Path.Combine(ApplicationCommonDirectory, name);
				if (!File.Exists(pathInProgramDir))
				{
					throw new ApplicationException(
						string.Format("Could not find the optionsList file {0}. Expected to find it at: {1} or {2}", name, pathInProject, pathInProgramDir));
				}
				LoadOptionsList(pathInProgramDir);
			}

			return _optionLists[name];
	   }

		private void LoadOptionsList(string pathToOptionsList)
		{
			string name = Path.GetFileName(pathToOptionsList);
			OptionsList list = new OptionsList();
			list.LoadFromFile(pathToOptionsList);
			_optionLists.Add(name, list);
		}

		/// <summary>
		/// Used with xml export, e.g. with LIFT to set the proper "range" for option fields
		/// </summary>
		/// <returns></returns>
		public  Dictionary<string, string> GetFieldToOptionListNameDictionary()
		{
			Dictionary<string, string> fieldToOptionListName = new Dictionary<string, string>();
			foreach (Field field in this.ViewTemplate.Fields)
			{
				if (field.OptionsListFile != null && field.OptionsListFile.Trim() != "")
				{
					fieldToOptionListName.Add(field.FieldName, GetListNameFromFileName(field.OptionsListFile));
				}
			}
			return fieldToOptionListName;
		}

		private string GetListNameFromFileName(string file)
		{
			return file.Substring(0, file.IndexOf(".xml"));
		}


	}
}