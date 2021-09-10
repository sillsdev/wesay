using NUnit.Framework;
using SIL.WritingSystems;
using System.IO;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.TestUtilities;

namespace Addin.Transform.Tests
{
	[TestFixture]
	public class HtmlTransformerTests
	{
		public HtmlTransformer _addin;
		private LexEntryRepository _lexEntryRepository;
		private string _filePath;
		private string _liftFilePath;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			SIL.Reporting.ErrorReport.IsOkToInteractWithUser = false;
			Sldr.Initialize(true);
		}

		[OneTimeTearDown]
		public void OneTimeTeardown()
		{
			Sldr.Cleanup();
		}

		[SetUp]
		public void Setup()
		{
			WeSayProjectTestHelper.InitializeForTests();
			_filePath = Path.GetTempFileName();
			_liftFilePath = Path.GetTempFileName();
			_lexEntryRepository = new LexEntryRepository(_filePath);

			_addin = new HtmlTransformer();
			_addin.LaunchAfterTransform = false;
		}

		[TearDown]
		public void TearDown()
		{
			if (_addin != null && File.Exists(_addin.PathToOutput))
			{
				File.Delete(_addin.PathToOutput);
			}
			if (_lexEntryRepository != null)
			{
				_lexEntryRepository.Dispose();
			}
			File.Delete(_filePath);
			File.Delete(_liftFilePath);
			WeSayProjectTestHelper.CleanupForTests();
		}

		[Test]
		public void LaunchWithDefaultSettings()
		{
			LaunchAddin();
			Assert.IsTrue(File.Exists(_addin.PathToOutput));
		}

		private void LaunchAddin()
		{
			// cjh: what's the point of this file... it doesn't seem to be used anywhere...
			//            string contents =
			//                    string.Format(
			//                            @"<?xml version='1.0' encoding='utf-8'?>
			//<lift  version='{0}'><entry id='one'><sense><gloss lang='en'><text>hello</text></gloss></sense></entry><entry id='two'/></lift>",
			//                            Validator.LiftVersion);
			//            File.WriteAllText(_liftFilePath, contents);
			_addin.Launch(null,
						  WeSayWordsProject.Project.GetProjectInfoForAddin());
			Assert.IsTrue(File.Exists(_addin.PathToOutput));
			string result = File.ReadAllText(_addin.PathToOutput);
			Assert.Greater(result.Trim().Length, 0);
		}

		//        [Test]
		//        public void CanGetXsltFromResource()
		//        {
		//
		//            ProjectInfo info = WeSayWordsProject.Project.GetProjectInfoForAddin();
		//            string path = info.LocateFile("plift2html.xsl");
		//            if (!string.IsNullOrEmpty(path))
		//            {
		//                File.Delete(path);
		//            }
		//            Stream stream = LiftTransformer.GetXsltStream(info, "plift2html.xsl");
		//            Assert.IsNotNull(stream);
		//        }
	}
}