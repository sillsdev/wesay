using System.IO;
using LiftIO.Validation;
using NUnit.Framework;
using WeSay.AddinLib;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace Addin.Transform.Tests
{
	[TestFixture]
	public class HtmlTransformerTests
	{
		public HtmlTransformer _addin;
		private LexEntryRepository _lexEntryRepository;
		private string _filePath;

		[SetUp]
		public void Setup()
		{
			WeSayWordsProject.InitializeForTests();
			_filePath = Path.GetTempFileName();
			_lexEntryRepository = new LexEntryRepository(_filePath);
			Db4oLexModelHelper.Initialize(_lexEntryRepository.Db4oDataSource.Data);

			_addin = new HtmlTransformer();
			_addin.LaunchAfterTransform = false;
		}

		[TearDown]
		public void TearDown()
		{
			if (File.Exists(_addin.PathToOutput))
			{
				File.Delete(_addin.PathToOutput);
			}
			_lexEntryRepository.Dispose();
			File.Delete(_filePath);
		}

		[Test]
		public void LaunchWithDefaultSettings()
		{
			LaunchAddin();
			Assert.IsTrue(File.Exists(_addin.PathToOutput));
		}

		private string LaunchAddin()
		{
			string contents =
					string.Format(
							@"<?xml version='1.0' encoding='utf-8'?>
<lift  version='{0}'><entry id='one'><sense><gloss lang='en'><text>hello</text></gloss></sense></entry><entry id='two'/></lift>",
							Validator.LiftVersion);
			if (WeSayWordsProject.Project.LiftIsLocked)
			{
				WeSayWordsProject.Project.ReleaseLockOnLift();
			}
			File.WriteAllText(WeSayWordsProject.Project.PathToLiftFile, contents);
			_addin.Launch(null, WeSayWordsProject.Project.GetProjectInfoForAddin());
			Assert.IsTrue(File.Exists(_addin.PathToOutput));
			string result = File.ReadAllText(_addin.PathToOutput);
			Assert.Greater(result.Trim().Length, 0);

			return result;
		}

		[Test]
		public void CanGetXsltFromResource()
		{
			ProjectInfo info = WeSayWordsProject.Project.GetProjectInfoForAddin();
			string path = info.LocateFile("plift2html.xsl");
			if (!string.IsNullOrEmpty(path))
			{
				File.Delete(path);
			}
			Stream stream = LiftTransformer.GetXsltStream(info, "plift2html.xsl");
			Assert.IsNotNull(stream);
		}
	}
}