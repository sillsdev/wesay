using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Palaso.TestUtilities;
using WeSay.LexicalModel.Foundation.WritingSystemMigration;

namespace WeSay.LexicalModel.Tests.Foundation.Migration
{
	[TestFixture]
	public class WritingSystemMigratorTests
	{
		private class TestEnvironment:IDisposable
		{
			private Dictionary<string, string> _oldToNewRfcTagMap;
			private string _pathToWsPrefsFile;
			private TemporaryFolder _pretendProjectDirectory;
			private string _pathToLdmlWsRepo;

			public TestEnvironment()
			{
				_pretendProjectDirectory = new TemporaryFolder("PretendWeSayProject");
				_pathToWsPrefsFile = Path.Combine(_pretendProjectDirectory.Path, "WritingSystemPrefs.xml");
				_pathToLdmlWsRepo = Path.Combine(_pretendProjectDirectory.Path, "WritingSystems");
			}

			public string PathToWsPrefsFile
			{
				get { return _pathToWsPrefsFile; }
			}

			public string PathToLdmlWsRepo
			{
				get { return _pathToLdmlWsRepo; }
			}

			public Dictionary<string, string> OldToNewRfcTagMap
			{
				get { return _oldToNewRfcTagMap; }
			}

			public void WriteContentToWsPrefsFile(string content)
			{
				File.WriteAllText(_pathToWsPrefsFile, content);
			}

			public void ChangeRfcTags(Dictionary<string, string> oldToNewRfcTagMap)
			{
				_oldToNewRfcTagMap = oldToNewRfcTagMap;
			}

			public void Dispose()
			{
				if (Directory.Exists(_pathToLdmlWsRepo))
				{
					foreach (var ldmlFile in Directory.GetFiles(_pathToLdmlWsRepo))
					{
						File.Delete(ldmlFile);
					}
					Directory.Delete(_pathToLdmlWsRepo);
				}
				File.Delete(_pathToWsPrefsFile);
				if (Directory.Exists(_pathToLdmlWsRepo))
				{
					Directory.Delete(_pretendProjectDirectory.Path);
				}
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainingOnlyLanguageTagInId_LanguageInLdmlIsSetToThatlanguageTag()
		{
			using(var environment = new TestEnvironment())
			{
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en"));
				var migrator = new WritingSystemMigrator();
				migrator.MigrateIfNecassary(environment.PathToWsPrefsFile, environment.PathToLdmlWsRepo, environment.ChangeRfcTags);
				AssertThatXmlIn.File("en.ldml").HasAtLeastOneMatchForXpath("/ldml/identity/language/[@type = 'en']");
			}
		}
	}

	public class WritingSystemPrefsFileContent
	{
		public static string SingleWritingSystem(string id)
		{
				return String.Format(
@"<?xml version='1.0' encoding='utf-8'?>
<WritingSystemCollection>
  <members>
	<WritingSystem>
	  <Id>{0}</Id>
	  <IsAudio>False</IsAudio>
	  <IsUnicode>True</IsUnicode>
	  <RightToLeft>False</RightToLeft>
	  <SortUsing>v</SortUsing>
	  <SpellCheckingId>{0}</SpellCheckingId>
	  <Abbreviation>v</Abbreviation>
	  <FontName>Arial</FontName>
	  <FontSize>12</FontSize>
	</WritingSystem>
  </members>
</WritingSystemCollection>".Replace("'", "\""), id);
		}
	}
}
