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
				if (_singleton == null)
				{
					throw new ApplicationException("WeSayWordsProject Not initialized. For tests, call BasilProject.InitializeForTests().");
				}
				return (WeSayWordsProject) _singleton;
			}
		}

		public override void Load(string projectDirectoryPath)
		{
			base.Load(projectDirectoryPath);
		}

		public override  void Create(string projectDirectoryPath)
		{
			base.Create(projectDirectoryPath);
			Directory.CreateDirectory(Path.GetDirectoryName(this.PathToLexicalModelDB));
		}


		public string PathToTaskConfig
		{
			get
			{
				return System.IO.Path.Combine(this.ApplicationDirectory, "tasks.xml");
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
}