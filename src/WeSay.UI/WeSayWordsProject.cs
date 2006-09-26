using System;
using System.IO;

namespace WeSay.UI
{
	public class WeSayWordsProject : BasilProject
	{
		private string _pathToLexiconDatabase = "lexicon.yap";

		public WeSayWordsProject()//string pathToLexiconDatabase)
			: base()
		{
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

		public void LoadFromLexiconPath(string lexiconPath)
		{
			_pathToLexiconDatabase = lexiconPath;
			CheckLexiconIsInValidProjectDirectory();
			//walk up from file to /wesay to /<project>
			base.LoadFromProjectDirectoryPath(Directory.GetParent(Directory.GetParent(lexiconPath).FullName).FullName);
		}

		private void CheckLexiconIsInValidProjectDirectory()
		{

		}

		public override  void Create(string projectDirectoryPath)
		{
			base.Create(projectDirectoryPath);
			Directory.CreateDirectory(this.PathToWeSaySpecificFilesDirectory);
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
				return System.IO.Path.Combine(this.PathToWeSaySpecificFilesDirectory, "tasks.xml");
			}
		}

		public string PathToLexicalModelDB
		{
			get
			{
				return System.IO.Path.Combine(ProjectDirectoryPath, this._pathToLexiconDatabase);
			}
		}

		private string ApplicationDirectory
		{
			get
			{
				return System.IO.Path.Combine(ProjectDirectoryPath, "WeSay");
			}
		}

		public string PathToWeSaySpecificFilesDirectory
		{
			get
			{
				return Path.Combine(ProjectDirectoryPath, "WeSay");
			}
		}
	}
}