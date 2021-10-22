using NUnit.Framework;
using SIL.IO;
using SIL.Reporting;
using SIL.TestUtilities;
using System.IO;
using WeSay.ConfigTool.NewProjectCreation;

namespace WeSay.ConfigTool.Tests.NewProjectCreation
{
	[TestFixture]
	public class ProjectFromRawFLExLiftFilesCreatorTests
	{
		[Test]
		public void Create_JustLiftFound_CreatesProjectAndCopiesLIFT()
		{
			using (var testDir = new TemporaryFolder("NormalSituation_CreatesProject"))
			{
				using (var lift = new TempLiftFile(testDir, "<entry id='foo'></entry>", "0.12"))
				{
					var targetDir = testDir.Combine("target");

					Assert.IsTrue(ProjectFromRawFLExLiftFilesCreator.Create(targetDir, lift.Path));
					var projectName = Path.GetFileNameWithoutExtension(targetDir);
					Assert.IsTrue(Directory.Exists(targetDir));
					var liftName = projectName + ".lift";
					AssertFileExistsInTargetDir(targetDir, liftName);

					var liftPath = Path.Combine(targetDir, liftName);
					AssertThatXmlIn.File(liftPath).HasAtLeastOneMatchForXpath("//entry[@id='foo']");
				}
			}
		}

		[Test]
		public void Create_LiftAndRangesFound_CopiesRanges()
		{
			using (var testDir = new TemporaryFolder("NormalSituation_CreatesProject"))
			{
				using (var lift = new TempLiftFile(testDir, "", "0.12"))
				{
					File.WriteAllText(lift.Path + "-ranges", "hello");
					var targetDir = testDir.Combine("target");
					Assert.IsTrue(ProjectFromRawFLExLiftFilesCreator.Create(targetDir, lift.Path));

					var projectName = Path.GetFileNameWithoutExtension(targetDir);
					AssertFileExistsInTargetDir(targetDir, projectName + ".lift-ranges");
				}
			}
		}

		[Test]
		public void Create_LdmlWritingSystemsFound_CopiesWritingSystems()
		{
			string ldmlText =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<ldml>
	<identity>
		<version number="""" />
		<generation date=""2011-06-15T19:35:32"" />
		<language type=""am"" />
		<script type=""Ethi"" />
	</identity>
	<collations />
	<special xmlns:palaso=""urn://palaso.org/ldmlExtensions/v1"">
		<palaso:abbreviation value=""test"" />
		<palaso:defaultFontFamily value=""Arial"" />
		<palaso:defaultFontSize value=""12"" />
	</special>
</ldml>";

			using (var testDir = new TemporaryFolder("NormalSituation_CreatesProject"))
			{
				var lift = new TempLiftFile(testDir, "<entry id='foo'><lexical-unit>\r\n<form lang=\"am\"><text>am</text></form>\r\n</lexical-unit></entry>", "0.12");
				using (var ldmlFile = new TempFile(ldmlText))
				{
					ldmlFile.MoveTo(Path.Combine(testDir.Path, "am.ldml"));
					var targetDir = testDir.Combine("target");
					Assert.IsTrue(ProjectFromRawFLExLiftFilesCreator.Create(targetDir, lift.Path));
					AssertFileExistsInTargetDir(Path.Combine(targetDir, "WritingSystems"), "am.ldml");
				}
			}
		}

		private static void AssertFileExistsInTargetDir(string targetDir, string fileName)
		{
			Assert.IsTrue(File.Exists(Path.Combine(targetDir, fileName)));
		}


		[Test]
		public void Create_SourceLiftLocked_GivesMessageReturnsFalse()
		{
			using (var lift = new TempLiftFile(""))
			{
				using (var stream = File.OpenWrite(lift.Path))
				{
					var targetDir = Path.Combine(Path.GetTempPath(), "ProjectFromFLExCreatorTests");

					using (var x = new ErrorReport.NonFatalErrorReportExpected())
					{
						Assert.IsFalse(ProjectFromRawFLExLiftFilesCreator.Create(targetDir, lift.Path));
					}
				}
			}
		}

	}
}
