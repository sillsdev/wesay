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
	public class WritingSystemsFromLiftCreatorTests
	{
		private class TestEnvironment : IDisposable {
			private readonly TemporaryFolder _folder;
			private readonly TempFile _liftFile1;
			private readonly TempFile _liftFile2;

			public TestEnvironment()
			{
				_folder = new TemporaryFolder("WritingSystemsFromLiftCreator");
				var pathtoLiftFile1 = Path.Combine(_folder.Path, "test1.lift");
				_liftFile1 = new TempFile(_liftFile1Content);
				_liftFile1.MoveTo(pathtoLiftFile1);

				var pathtoLiftFile2 = Path.Combine(_folder.Path, "test2.lift");
				_liftFile2 = new TempFile(_liftFile2Content);
				_liftFile2.MoveTo(pathtoLiftFile2);
				Creator = new WritingSystemsFromLiftCreator(ProjectPath);

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
				lang='bogusws1'>
				<text>chùuchìi mǔu krɔ̂ɔp</text>
			</form>
			<form
				lang='audio'>
				<text>ฉู่ฉี่หมูรอบ</text>
			</form>
			<form
				lang='de'>
				<text>ฉู่ฉี่หมูรอบ</text>
			</form>
		</lexical-unit>
		<sense
			id='df801833-d55b-4492-b501-650da7bc7b73'>
			<definition>
				<form
					lang='en'>
					<text>A kind of curry fried with crispy pork</text>
				</form>
			</definition>
		</sense>
	</entry>
</lift>".Replace("'", "\"");

			private readonly string _liftFile2Content =
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
				lang='wee'>
				<text>chùuchìi mǔu krɔ̂ɔp</text>
			</form>
			<form
				lang='x-wee'>
				<text>ฉู่ฉี่หมูรอบ</text>
			</form>
		</lexical-unit>
	</entry>
</lift>".Replace("'", "\"");
#endregion

			public string ProjectPath
			{
				get { return _folder.Path; }
			}

			public WritingSystemsFromLiftCreator Creator { get; private set; }

			public void Dispose()
			{
				_liftFile1.Dispose();
				_liftFile2.Dispose();
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

			public string PathToLiftFileWithDuplicates
			{
				get { return _liftFile2.Path; }
			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInLift_LiftFileContainsNonConformantRfcTag_CreatesConformingWritingSystem()
		{
			using (var environment = new TestEnvironment())
			{
				environment.Creator.CreateNonExistentWritingSystemsFoundInLift(environment.PathToLiftFile);
				string writingSystem1FilePath = Path.Combine(environment.WritingSystemsPath, "x-bogusws1.ldml");
				string writingSystem2FilePath = Path.Combine(environment.WritingSystemsPath, "qaa-Zxxx-x-audio.ldml");
				Assert.That(File.Exists(writingSystem1FilePath));
				Assert.That(File.Exists(writingSystem2FilePath));
				AssertThatXmlIn.File(writingSystem1FilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='']");
				AssertThatXmlIn.File(writingSystem1FilePath).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-bogusws1']");
				AssertThatXmlIn.File(writingSystem2FilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='qaa']");
				AssertThatXmlIn.File(writingSystem2FilePath).HasAtLeastOneMatchForXpath("/ldml/identity/script[@type='Zxxx']");
				AssertThatXmlIn.File(writingSystem2FilePath).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-audio']");

			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInLift_LiftFileContainsNonConformantRfcTag_UpdatesRfcTagInLiftFile()
		{
			using (var environment = new TestEnvironment())
			{
				environment.Creator.CreateNonExistentWritingSystemsFoundInLift(environment.PathToLiftFile);
				AssertThatXmlIn.File(environment.PathToLiftFile).HasAtLeastOneMatchForXpath("/lift/entry/lexical-unit/form[@lang='x-bogusws1']");
				AssertThatXmlIn.File(environment.PathToLiftFile).HasAtLeastOneMatchForXpath("/lift/entry/lexical-unit/form[@lang='qaa-Zxxx-x-audio']");
			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInLift_LiftFileContainsNonConformantRfcTagWithDuplicates_UpdatesRfcTagInLiftFile()
		{
			using (var environment = new TestEnvironment())
			{
				environment.Creator.CreateNonExistentWritingSystemsFoundInLift(environment.PathToLiftFileWithDuplicates);
				AssertThatXmlIn.File(environment.PathToLiftFileWithDuplicates).HasAtLeastOneMatchForXpath("/lift/entry/lexical-unit/form[@lang='x-wee-dupl1']");
				AssertThatXmlIn.File(environment.PathToLiftFileWithDuplicates).HasAtLeastOneMatchForXpath("/lift/entry/lexical-unit/form[@lang='x-wee']");
			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInLift_LiftFileContainsConformantRfcTagWithNoCorrespondingLdml_CreatesLdml()
		{
			using (var environment = new TestEnvironment())
			{
				environment.Creator.CreateNonExistentWritingSystemsFoundInLift(environment.PathToLiftFile);
				AssertThatXmlIn.File(environment.PathToLiftFile).HasAtLeastOneMatchForXpath("/lift/entry/lexical-unit/form[@lang='de']");
				string writingSystem1FilePath = Path.Combine(environment.WritingSystemsPath, "de.ldml");
				Assert.That(File.Exists(writingSystem1FilePath), Is.True);
				AssertThatXmlIn.File(writingSystem1FilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='de']");
				AssertThatXmlIn.File(writingSystem1FilePath).HasNoMatchForXpath("/ldml/identity/script");
				AssertThatXmlIn.File(writingSystem1FilePath).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(writingSystem1FilePath).HasNoMatchForXpath("/ldml/identity/variant");
			}
		}
	}
}
