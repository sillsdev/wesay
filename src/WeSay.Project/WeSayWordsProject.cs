using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Reporting;
using WeSay.Foundation;

namespace WeSay.Project
{
	public class WeSayWordsProject : BasilProject
	{
		private string _lexiconDatabaseFileName = null;
		private IList<ITask> _tasks;
		private ViewTemplate _viewTemplate;
		private Dictionary<string, OptionsList> _optionLists;

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
			Reporting.ErrorReporter.OkToInteractWithUser = false;
			BasilProject project = new WeSayWordsProject();
			project.LoadFromProjectDirectoryPath(GetPretendProjectDirectory());
			project.StringCatalogSelector = "en";
		}

		public bool LoadFromLexiconPath(string lexiconPath)
		{
			if (!File.Exists(lexiconPath))
			{
			   throw new ApplicationException("WeSay cannot find a lexicon where it was looking, which is at "+lexiconPath);
			}
			lexiconPath = Path.GetFullPath(lexiconPath);

			_lexiconDatabaseFileName = Path.GetFileName(lexiconPath);
			if (CheckLexiconIsInValidProjectDirectory(lexiconPath))
			{
				//walk up from file to /wesay to /<project>
				base.LoadFromProjectDirectoryPath(
					Directory.GetParent(Directory.GetParent(lexiconPath).FullName).FullName);
				Debug.Assert(PathToLexicalModelDB.ToLower() == Path.GetFullPath(lexiconPath).ToLower());
				return true;
			}
			else
			{
				return false;
			}
		}

		public override  void LoadFromProjectDirectoryPath(string projectDirectoryPath)
		{
			base.LoadFromProjectDirectoryPath(projectDirectoryPath);
			DetermineWordsFile();

//            ViewTemplate templateAsFoundInProjectFiles = GetInventoryFromProjectFiles();
//            ViewTemplate fullUpToDateTemplate = ViewTemplate.MakeMasterTemplate(WritingSystems);
//            ViewTemplate.SynchronizeInventories(fullUpToDateTemplate, templateAsFoundInProjectFiles);
//            _viewTemplate = fullUpToDateTemplate;
		}


		private ViewTemplate GetInventoryFromProjectFiles()
		{
			ViewTemplate template = new ViewTemplate();
			try
			{
				XmlDocument projectDoc = GetProjectDoc();
				if (projectDoc != null)
				{
					XmlNode inventoryNode = projectDoc.SelectSingleNode("tasks/components/viewTemplate");
					template.LoadFromString(inventoryNode.OuterXml);
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

		private void DetermineWordsFile()
		{
			//try to use the one implied by the project name (e.g. thai.words)
			if (File.Exists(PathToLexicalModelDB))
			{
				return;
			}
	   }

		private bool CheckLexiconIsInValidProjectDirectory(string p)
		{
			DirectoryInfo lexiconDirectoryInfo = Directory.GetParent(p);
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

		public override void Create(string projectDirectoryPath)
		{
			base.Create(projectDirectoryPath);
			Directory.CreateDirectory(PathToWeSaySpecificFilesDirectoryInProject);
			_viewTemplate = ViewTemplate.MakeMasterTemplate(WritingSystems);
		   // this._lexiconDatabaseFileName = this.Name+".words";
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

		public string PathToLiftBackupDir
		{
			get
			{
				return PathToLexicalModelDB + " incremental xml backup";
			}
		}

		public string PathToCache
		{
			get
			{
				return PathToLexicalModelDB + " Cache";
			}
		}

		public string PathToLexicalModelDB
		{
			get
			{
				if (_lexiconDatabaseFileName != null)
				{
					return System.IO.Path.Combine(PathToWeSaySpecificFilesDirectoryInProject, this._lexiconDatabaseFileName);
				}
				else
				{
					return System.IO.Path.Combine(PathToWeSaySpecificFilesDirectoryInProject, Name+".words");
				}
			}
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
					ViewTemplate templateAsFoundInProjectFiles = GetInventoryFromProjectFiles();
					ViewTemplate fullUpToDateTemplate = ViewTemplate.MakeMasterTemplate(WritingSystems);
					ViewTemplate.SynchronizeInventories(fullUpToDateTemplate, templateAsFoundInProjectFiles);
					_viewTemplate = fullUpToDateTemplate;
				}
				return _viewTemplate;
			}
		   // set { _viewTemplate = value; }
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