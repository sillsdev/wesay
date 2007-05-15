namespace WeSay.AddinLib
{
	public class ProjectInfo
	{

		private string _pathToTopLevelDirectory;
		private string _pathToLIFT;
		private string _name;
		private string[] _filesBelongingToProject;

		public ProjectInfo(string name, string pathToTopLevelDirectory, string pathToLIFT, string[] filesBelongingToProject)
		{
			_pathToTopLevelDirectory = pathToTopLevelDirectory;
			_filesBelongingToProject = filesBelongingToProject;
			_name = name;
			_pathToLIFT = pathToLIFT;
		}

		public string PathToLIFT
		{
			get
			{
				return _pathToLIFT;
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
	}
}