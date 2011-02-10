using System.IO;
using LiftIO.Validation;
using NUnit.Framework;
using WeSay.Project;

namespace Addin.LiftReports.Tests
{
	[TestFixture]
	public class LiftReportsTests
	{
		public ReportMaker _addin;

		[SetUp]
		public void Setup()
		{
			WeSayWordsProject.InitializeForTests();
			_addin = new ReportMaker();
			_addin.LaunchAfterTransform = false;
		}

		[TearDown]
		public void TearDown()
		{
			if (File.Exists(_addin.PathToOutput))
			{
				File.Delete(_addin.PathToOutput);
			}
		}

		[Test, Ignore("Breaking on Team City as of 8d7a9b2d8b22, don't have time to figure it out.")]
		public void AlwaysHappy() {}

		[Test]
		[Ignore("Unexplicably fails in nvelocity, sometimes")]
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
			File.WriteAllText(WeSayWordsProject.Project.PathToLiftFile, contents);
			_addin.Launch(null, WeSayWordsProject.Project.GetProjectInfoForAddin());
			Assert.IsTrue(File.Exists(_addin.PathToOutput));
			string result = File.ReadAllText(_addin.PathToOutput);
			Assert.Greater(result.Trim().Length, 0);

			return result;
		}
	}
}