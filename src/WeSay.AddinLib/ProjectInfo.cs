using System;
using System.ComponentModel.Design;
using Palaso.WritingSystems;
using WeSay.LexicalModel.Foundation;

namespace WeSay.AddinLib
{
	public class ProjectInfo
	{
		private readonly string _pathToTopLevelDirectory;
		private string _pathToLIFT;
		private readonly string _pathToExportDirectory;
		private readonly string _name;
		private readonly string _pathToApplicationRootDirectory;
		private readonly string[] _filesBelongingToProject;

		private readonly FileLocater _locateFile;
		private readonly IWritingSystemRepository _writingSystems;
		private readonly object _project;
		public IServiceProvider ServiceProvider;

		public ProjectInfo(string name,
						   string pathToApplicationRootDirectory,
						   string pathToTopLevelDirectory,
						   string pathToLIFT,
						   string pathToExportDirectory,
						   string[] filesBelongingToProject,
						   FileLocater locateFile,
						   IWritingSystemRepository writingSystems,
						   IServiceProvider serviceProvider,
						   // these signatures were to reduce the dependencies on addins that didn't need this stuff
						   object project)
				// these signatures were to reduce the dependencies on addins that didn't need this stuff

		{
			ServiceProvider = serviceProvider;
			_pathToTopLevelDirectory = pathToTopLevelDirectory;
			_locateFile = locateFile;
			_writingSystems = writingSystems;
			_project = project;
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
			get { return _pathToLIFT; }
			set { _pathToLIFT = value; }
		}

		public string PathToExportDirectory
		{
			get { return _pathToExportDirectory; }
		}

		public string PathToTopLevelDirectory
		{
			get { return _pathToTopLevelDirectory; }
		}

		public string Name
		{
			get { return _name; }
		}

		public string[] FilesBelongingToProject
		{
			get { return _filesBelongingToProject; }
		}

		public IWritingSystemRepository WritingSystems
		{
			get { return _writingSystems; }
		}

		public string PathToApplicationRootDirectory
		{
			get { return _pathToApplicationRootDirectory; }
		}

		public object Project
		{
			get { return _project; }
		}
	}
}