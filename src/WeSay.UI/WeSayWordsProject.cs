using System;
using System.Diagnostics;
using System.IO;

namespace WeSay.UI
{
	public class WeSayWordsProject : BasilProject
	{
		private string _lexiconDatabaseFileName = null;

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
			Debug.Assert(File.Exists(lexiconPath));

			_lexiconDatabaseFileName = Path.GetFileName(lexiconPath);
			CheckLexiconIsInValidProjectDirectory(lexiconPath);
			//walk up from file to /wesay to /<project>
			base.LoadFromProjectDirectoryPath(Directory.GetParent(Directory.GetParent(lexiconPath).FullName).FullName);
			Debug.Assert(PathToLexicalModelDB == lexiconPath);
	   }

		public override  void LoadFromProjectDirectoryPath(string projectDirectoryPath)
		{
			base.LoadFromProjectDirectoryPath(projectDirectoryPath);

			DetermineWordsFile();
		}

		private void DetermineWordsFile()
		{
			//try to use the one implied by the project name (e.g. thai.words)
			if (File.Exists(this.PathToLexicalModelDB))
			{
				return;
			}

			//use the first words file we do find
			string[] p = Directory.GetFiles(this.PathToWeSaySpecificFilesDirectory, "*.words");
			if (p.Length > 0)
			{
				this._lexiconDatabaseFileName = Path.GetFileName(p[0]);
			}
			else
			{
				//just leave as is, couldn't find one.
			}
		}

		private void CheckLexiconIsInValidProjectDirectory(string p)
		{
			string[] dirs = p.Split('\\');
			string projectRoot=Directory.GetParent( Directory.GetParent(p).FullName).FullName;
			if(dirs[dirs.Length-1] != "WeSay"
				|| (!IsValidProjectDirectory(projectRoot)))
			{
				throw new ApplicationException("WeSay cannot open the lexicon, because it is not in a proper WeSay/Basil project structure.");
			}
		}

		public override void Create(string projectDirectoryPath)
		{
			base.Create(projectDirectoryPath);
			Directory.CreateDirectory(this.PathToWeSaySpecificFilesDirectory);
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
				return System.IO.Path.Combine(this.PathToWeSaySpecificFilesDirectory, "tasks.xml");
			}
		}

		public string PathToLexicalModelDB
		{
			get
			{
				if (_lexiconDatabaseFileName != null)
				{
					return System.IO.Path.Combine(PathToWeSaySpecificFilesDirectory, this._lexiconDatabaseFileName);
				}
				else
				{
					return System.IO.Path.Combine(PathToWeSaySpecificFilesDirectory, this.Name+".words");
				}
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