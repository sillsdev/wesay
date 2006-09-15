using System.IO;
using NUnit.Framework;
using WeSay.Language;

namespace WeSay.UI.Tests
{
	[TestFixture]
	public class WeSayWordsProjectTests
	{
		private string _projectDirectory;

		[SetUp]
		public void Setup()
		{
			DirectoryInfo dirProject = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
			this._projectDirectory = dirProject.FullName;

		}

		[TearDown]
		public void TearDown()
		{
			Directory.Delete(this._projectDirectory, true);
		}


		[Test, Ignore()]
		public void MakeProjectFiles()
		{
			string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			try
			{
				Directory.CreateDirectory(path);
				WeSayWordsProject p = new WeSayWordsProject();
				p.Create(path);
				Assert.IsTrue(Directory.Exists(path));
				Assert.IsTrue(Directory.Exists(p.PathToLexicalModelDB));
			}
			finally
			{
				Directory.Delete(path, true);
			}
		}
	}
}