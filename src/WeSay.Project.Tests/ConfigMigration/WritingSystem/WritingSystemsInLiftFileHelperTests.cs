using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using Palaso.IO;
using Palaso.TestUtilities;
using Palaso.WritingSystems;
using Palaso.WritingSystems.Migration.WritingSystemsLdmlV0To1Migration;
using WeSay.Project.ConfigMigration.WritingSystem;

namespace WeSay.Project.Tests.ConfigMigration.WritingSystem
{
	[TestFixture]
	public class WritingSystemsInLiftFileHelperTests
	{
		private class TestEnvironment : IDisposable {
			private readonly TemporaryFolder _folder;
			private readonly TempFile _liftFile1;

			public TestEnvironment(string rfctag):this(rfctag, "x-dontcare")
			{
			}

			public TestEnvironment(string rfctag, string rfctag2)
			{
				_folder = new TemporaryFolder("WritingSystemsInLiftFileHelper");
				var pathtoLiftFile1 = Path.Combine(_folder.Path, "test1.lift");
				_liftFile1 = new TempFile(String.Format(_liftFile1Content, rfctag, rfctag2));
				_liftFile1.MoveTo(pathtoLiftFile1);

				Helper = new WritingSystemsInLiftFileHelper(WritingSystemsPath, _liftFile1.Path);
			}

#region LongFileContent
			private readonly string _liftFile1Content =
 @"<?xml version='1.0' encoding='utf-8'?>
<lift
	version='0.13'
	producer='WeSay 1.0.0.0'>
	<entry
		id='chùuchìi mǔu rɔ̂ɔp_dd15cbc4-9085-4d66-af3d-8428f078a7da'
		dateCreated='2008-11-03T06:17:24Z'
		dateModified='2009-10-12T04:05:40Z'
		guid='dd15cbc4-9085-4d66-af3d-8428f078a7da'>
		<lexical-unit>
			<form
				lang='{0}'>
				<text>chùuchìi mǔu krɔ̂ɔp</text>
			</form>
			<form
				lang='{1}'>
				<text>ฉู่ฉี่หมูรอบ</text>
			</form>
		</lexical-unit>
		<sense
			id='df801833-d55b-4492-b501-650da7bc7b73'>
			<definition>
				<form
					lang='{0}'>
					<text>A kind of curry fried with crispy pork</text>
				</form>
			<form
				lang='{1}'>
				<text>ฉู่ฉี่หมูรอบ</text>
			</form>
			</definition>
		</sense>
	</entry>
</lift>".Replace("'", "\"");
#endregion

			public string ProjectPath
			{
				get { return _folder.Path; }
			}

			public WritingSystemsInLiftFileHelper Helper { get; private set; }

			public void Dispose()
			{
				_liftFile1.Dispose();
				_folder.Dispose();
			}

			public string WritingSystemsPath
			{
				get { return Path.Combine(ProjectPath, "WritingSystems"); }
			}

			public string PathToLiftFile
			{
				get { return _liftFile1.Path; }
			}

			public string GetLdmlFileforWs(string id)
			{
				return Path.Combine(WritingSystemsPath, String.Format("{0}.ldml", id));
			}

			public void WriteContentToLdmlFileInWritingSystemFolderWithName(string name, string content)
			{
				if(!Directory.Exists(WritingSystemsPath))
				{
					Directory.CreateDirectory(WritingSystemsPath);
				}
				File.WriteAllText(Path.Combine(WritingSystemsPath, name+".ldml"), content);
			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInLift_LiftFileContainsNonConformantRfcTag_CreatesConformingWritingSystem()
		{
			using (var e = new TestEnvironment("x-bogusws1", "audio"))
			{
				e.Helper.CreateNonExistentWritingSystemsFoundInFile();
				Assert.That(File.Exists(e.GetLdmlFileforWs("qaa-x-bogusws1")));
				Assert.That(File.Exists(e.GetLdmlFileforWs("qaa-Zxxx-x-audio")));
				AssertThatXmlIn.File(e.GetLdmlFileforWs("qaa-x-bogusws1")).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='qaa']");
				AssertThatXmlIn.File(e.GetLdmlFileforWs("qaa-x-bogusws1")).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-bogusws1']");
				AssertThatXmlIn.File(e.GetLdmlFileforWs("qaa-Zxxx-x-audio")).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='qaa']");
				AssertThatXmlIn.File(e.GetLdmlFileforWs("qaa-Zxxx-x-audio")).HasAtLeastOneMatchForXpath("/ldml/identity/script[@type='Zxxx']");
				AssertThatXmlIn.File(e.GetLdmlFileforWs("qaa-Zxxx-x-audio")).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-audio']");

			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInLift_LiftFileContainsNonConformantRfcTag_WSIdChangeLogUpdated()
		{
			using (var e = new TestEnvironment("x-bogusws1", "audio"))
			{
				e.Helper.CreateNonExistentWritingSystemsFoundInFile();
				Assert.That(File.Exists(e.GetLdmlFileforWs("qaa-x-bogusws1")));
				Assert.That(File.Exists(e.GetLdmlFileforWs("qaa-Zxxx-x-audio")));
				string idChangeLogFilePath = Path.Combine(e.WritingSystemsPath, "idchangelog.xml");
				AssertThatXmlIn.File(idChangeLogFilePath).HasAtLeastOneMatchForXpath("/WritingSystemChangeLog/Changes[Add/Id/text()='qaa-x-bogusws1' and Add/Id/text()='qaa-Zxxx-x-audio']");
			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInLift_LiftFileContainsNonConformantRfcTag_UpdatesRfcTagInLiftFile()
		{
			using (var environment = new TestEnvironment("Zxxx-x-bogusws1", "audio"))
			{
				environment.Helper.CreateNonExistentWritingSystemsFoundInFile();
				AssertThatXmlIn.File(environment.PathToLiftFile).HasAtLeastOneMatchForXpath("/lift/entry/lexical-unit/form[@lang='qaa-x-Zxxx-bogusws1']");
				AssertThatXmlIn.File(environment.PathToLiftFile).HasAtLeastOneMatchForXpath("/lift/entry/lexical-unit/form[@lang='qaa-Zxxx-x-audio']");
			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInLift_LiftFileContainsNonConformantRfcTagWithDuplicates_UpdatesRfcTagInLiftFile()
		{
			using (var environment = new TestEnvironment("wee", "x-wee"))
			{
				environment.Helper.CreateNonExistentWritingSystemsFoundInFile();
				AssertThatXmlIn.File(environment.PathToLiftFile).HasAtLeastOneMatchForXpath("/lift/entry/lexical-unit/form[@lang='qaa-x-wee-dupl0']");
				AssertThatXmlIn.File(environment.PathToLiftFile).HasAtLeastOneMatchForXpath("/lift/entry/lexical-unit/form[@lang='qaa-x-wee']");
			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInLift_LiftFileContainsConformantRfcTagWithNoCorrespondingLdml_CreatesLdml()
		{
			using (var e = new TestEnvironment("de", "x-dontcare"))
			{
				e.Helper.CreateNonExistentWritingSystemsFoundInFile();
				AssertThatXmlIn.File(e.PathToLiftFile).HasAtLeastOneMatchForXpath("/lift/entry/lexical-unit/form[@lang='de']");
				Assert.That(File.Exists(e.GetLdmlFileforWs("de")), Is.True);
				AssertThatXmlIn.File(e.GetLdmlFileforWs("de")).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='de']");
				AssertThatXmlIn.File(e.GetLdmlFileforWs("de")).HasNoMatchForXpath("/ldml/identity/script");
				AssertThatXmlIn.File(e.GetLdmlFileforWs("de")).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(e.GetLdmlFileforWs("de")).HasNoMatchForXpath("/ldml/identity/variant");
			}
		}

		[Test]
		//This test makes sure that existing Flex private use tags are not changed
		public void CreateNonExistentWritingSystemsFoundInLift_LiftFileContainsEntirelyPrivateUseRfcTagThatExistsInRepo_RfcTagIsNotMigrated()
		{
			using (var e = new TestEnvironment("x-en-Zxxx-x-audio"))
			{
				e.WriteContentToLdmlFileInWritingSystemFolderWithName("x-en-Zxxx-x-audio", LdmlContentForTests.Version0("x-en", "Zxxx", "", "x-audio"));
				e.Helper.CreateNonExistentWritingSystemsFoundInFile();
				Assert.That(File.Exists(e.GetLdmlFileforWs("x-en-Zxxx-x-audio")), Is.True);
				Assert.That(File.Exists(e.GetLdmlFileforWs("en-Zxxx-x-audio")), Is.False);
				AssertThatXmlIn.File(e.PathToLiftFile).HasAtLeastOneMatchForXpath("/lift/entry/lexical-unit/form[@lang='x-en-Zxxx-x-audio']");
				AssertThatXmlIn.File(e.PathToLiftFile).HasNoMatchForXpath("/lift/entry/lexical-unit/form[@lang='en-Zxxx-x-audio']");
			}
		}

		[Test]

		public void CreateNonExistentWritingSystemsFoundInLift_LiftFileContainsEntirelyPrivateUseRfcTagThatDoesNotExistInRepo_RfcTagIsMigrated()
		{
			using (var e = new TestEnvironment("x-blah"))
			{
				e.Helper.CreateNonExistentWritingSystemsFoundInFile();
				Assert.That(File.Exists(e.GetLdmlFileforWs("qaa-x-blah")), Is.True);
				AssertThatXmlIn.File(e.PathToLiftFile).HasAtLeastOneMatchForXpath("/lift/entry/lexical-unit/form[@lang='qaa-x-blah']");
			}
		}

		[Test]
		//This test makes sure that nonexisting private use tags are migrated if necessary
		public void CreateNonExistentWritingSystemsFoundInLift_LiftFileContainsAudioTagThatDoesNotExistInRepo_RfcTagIsMigrated()
		{
			using (var e = new TestEnvironment("x-audio"))
			{
				e.Helper.CreateNonExistentWritingSystemsFoundInFile();
				Assert.That(File.Exists(e.GetLdmlFileforWs("x-audio")), Is.False);
				Assert.That(File.Exists(e.GetLdmlFileforWs("qaa-Zxxx-x-audio")), Is.True);
				AssertThatXmlIn.File(e.PathToLiftFile).HasNoMatchForXpath("/lift/entry/lexical-unit/form[@lang='x-audio']");
				AssertThatXmlIn.File(e.PathToLiftFile).HasAtLeastOneMatchForXpath("/lift/entry/lexical-unit/form[@lang='qaa-Zxxx-x-audio']");
			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInLift_LiftFileContainsNonConformantRfcTagWithDuplicatesContainingduplicateMarker_UpdatesRfcTagInLiftFile()
		{
			using (var environment = new TestEnvironment("wee-dupl1", "x-wee-dupl1"))
			{
				environment.Helper.CreateNonExistentWritingSystemsFoundInFile();
				AssertThatXmlIn.File(environment.PathToLiftFile).HasAtLeastOneMatchForXpath("/lift/entry/lexical-unit/form[@lang='qaa-x-wee-dupl1']");
				AssertThatXmlIn.File(environment.PathToLiftFile).HasAtLeastOneMatchForXpath("/lift/entry/lexical-unit/form[@lang='qaa-x-wee-dupl1-dupl0']");
			}
		}
	}
}
