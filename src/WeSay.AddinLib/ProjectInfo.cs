using System.Collections;
using System.Collections.Generic;
using WeSay.Language;

namespace WeSay.AddinLib
{
	public class ProjectInfo
	{

		private string _pathToTopLevelDirectory;
		private string _pathToLIFT;
		private string _pathToExportDirectory;
		private string _name;
		private readonly string _pathToApplicationRootDirectory;
		private string[] _filesBelongingToProject;


		private FileLocater _locateFile;
		private readonly WritingSystemCollection _writingSystems;
		private readonly object _project;
		private readonly object _recordListManager;

		public ProjectInfo(string name,
			string pathToApplicationRootDirectory,
			string pathToTopLevelDirectory,
			string pathToLIFT,
			string pathToExportDirectory,
			string[] filesBelongingToProject,
			FileLocater locateFile,
			WritingSystemCollection writingSystems,
			object project)             // these signatures were to reduce the dependencies on addins that didn't need this stuff
//            object recordListManager) // these signatures were to reduce the dependencies on addins that didn't need this stuff
		{
			_pathToTopLevelDirectory = pathToTopLevelDirectory;
			_locateFile = locateFile;
			_writingSystems = writingSystems;
			_project = project;
		 //   _recordListManager = recordListManager;
			_filesBelongingToProject = filesBelongingToProject;
			_name = name;
			_pathToApplicationRootDirectory = pathToApplicationRootDirectory;
			_pathToLIFT = pathToLIFT;
			_pathToExportDirectory = pathToExportDirectory;
		}

		/// <summary>
		/// Find the file, starting with the project dirs and moving to the app dirs.
		/// This allows a user to override an installed file by making thier own.
		/// </summary>
		public string LocateFile(string fileName)
		{
			return _locateFile(fileName);
		}

		public string PathToLIFT
		{
			get
			{
				return _pathToLIFT;
			}
			set { _pathToLIFT = value; }
		}

		public string PathToExportDirectory
		{
			get
			{
				return _pathToExportDirectory;
			}
		}

		public string PathToTopLevelDirectory
		{
			get
			{
				return _pathToTopLevelDirectory;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}


		public string[] FilesBelongingToProject
		{
			get
			{
				return _filesBelongingToProject;
			}
		}

		public WritingSystemCollection WritingSystems
		{
			get
			{
				return _writingSystems;
			}
		}

		public string PathToApplicationRootDirectory
		{
			get { return _pathToApplicationRootDirectory; }
		}

//        public object RecordListManager
//        {
//            get { return _recordListManager; }
//        }

		public object Project
		{
			get { return _project; }
		}
	}
}