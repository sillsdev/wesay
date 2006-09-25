using System;
using System.IO;

namespace WeSay.UI
{
	public class WeSayWordsProject : BasilProject
	{
		public WeSayWordsProject()
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

		public override void Load(string projectDirectoryPath)
		{
			base.Load(projectDirectoryPath);
		}

		public override  void Create(string projectDirectoryPath)
		{
			base.Create(projectDirectoryPath);
			Directory.CreateDirectory(this.PathToWeSaySpecificFilesDirectory);
			Directory.CreateDirectory(Path.GetDirectoryName(this.PathToLexicalModelDB));
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

		private string PathToWeSaySpecificFilesDirectory
		{
			get
			{
				return Path.Combine(ProjectDirectoryPath, "WeSay");
			}
		}
	}
}