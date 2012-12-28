using System;
using System.Collections.Generic;
using System.Drawing;
using NUnit.Framework;
using Palaso.IO;
using Palaso.TestUtilities;
using Palaso.WritingSystems;
using Palaso.WritingSystems.Migration.WritingSystemsLdmlV0To1Migration;

namespace WeSay.LexicalModel.Tests.Foundation
{
	[TestFixture]
	public class LdmlInFolderWritingSystemRepositoryTests
	{
		private class TestEnvironment:IDisposable
		{
			private readonly TemporaryFolder _wesayProjectFolder;
			private readonly TemporaryFolder _ldmlWsFolder;
			private readonly TempFile _wsPrefsFile;

			public TestEnvironment()
			{
				_wesayProjectFolder = new TemporaryFolder("WesayProject");
				_ldmlWsFolder = new TemporaryFolder(_wesayProjectFolder, "WritingSystems");
				_wsPrefsFile = new TempFile(_ldmlWsFolder.Path);
			}

			public string PathToWritingSystemsFolder
			{
				get { return _ldmlWsFolder.Path; }
			}

			public void CreateLdmlWritingSystemDefinitionFile()
			{
				IWritingSystemRepository wsCollectionToBeWritten = GetWritingSystemRepository(PathToWritingSystemsFolder);
				WritingSystemDefinition ws = CreateDetailedWritingSystem("en");
				wsCollectionToBeWritten.Set(ws);
				wsCollectionToBeWritten.Save();
			}

			public IWritingSystemRepository MakeSampleCollection()
			{
				var writingSystemStore = GetWritingSystemRepository(PathToWritingSystemsFolder);
				writingSystemStore.Set(WritingSystemDefinition.Parse("en"));
				writingSystemStore.Set(WritingSystemDefinition.Parse("de"));
				return writingSystemStore;
			}

			public void Dispose()
			{
				_wsPrefsFile.Dispose();
				_ldmlWsFolder.Dispose();
				_wesayProjectFolder.Dispose();
			}

			public static void AssertWritingSystemCollectionsAreEqual(IWritingSystemRepository lhs, IWritingSystemRepository rhs)
			{
				foreach (var lhsWritingSystem in lhs.AllWritingSystems)
				{
					var rhsWritingSystem = rhs.Get(lhsWritingSystem.Id);
					Assert.IsTrue(rhs.Contains(lhsWritingSystem.Id));
					Assert.AreEqual(lhsWritingSystem.Id, rhsWritingSystem.Id);
					Assert.AreEqual(lhsWritingSystem.Abbreviation, rhsWritingSystem.Abbreviation);
					Assert.AreEqual(lhsWritingSystem.DefaultFontName, rhsWritingSystem.DefaultFontName);
					Assert.AreEqual(lhsWritingSystem.DefaultFontSize, rhsWritingSystem.DefaultFontSize);
					Assert.AreEqual(lhsWritingSystem.IsVoice, rhsWritingSystem.IsVoice);
					Assert.AreEqual(lhsWritingSystem.IsUnicodeEncoded, rhsWritingSystem.IsUnicodeEncoded);
					Assert.AreEqual(lhsWritingSystem.Keyboard, rhsWritingSystem.Keyboard);
					Assert.AreEqual(lhsWritingSystem.RightToLeftScript, rhsWritingSystem.RightToLeftScript);
					Assert.AreEqual(lhsWritingSystem.SortUsing, rhsWritingSystem.SortUsing);
					Assert.AreEqual(lhsWritingSystem.SortRules, rhsWritingSystem.SortRules);
					Assert.AreEqual(lhsWritingSystem.SpellCheckingId, rhsWritingSystem.SpellCheckingId);
				}
			}

			public static WritingSystemDefinition CreateDetailedWritingSystem(string languageCode)
			{
				var ws = new WritingSystemDefinition
					{
						Language = languageCode,
						Abbreviation = languageCode,
						IsVoice = false,
						DefaultFontName = new Font(FontFamily.GenericSansSerif, 12).Name,
						DefaultFontSize = new Font(FontFamily.GenericSansSerif, 12).Size
					};
				ws.IsVoice = false;
				ws.IsUnicodeEncoded = true;
				ws.Keyboard = "Bogus ivories!";
				ws.RightToLeftScript = false;
				ws.SortUsingCustomSimple("");
				ws.SpellCheckingId = languageCode;
				return ws;
			}
		}

		[Test]
		public void Load_OnlyLdmlWritingSystemFilesExist_WritingSystemsAreLoadedFromThoseFiles()
		{
			using (var e = new TestEnvironment())
			{
				IWritingSystemRepository wsCollectionToBeWritten =
					GetWritingSystemRepository(e.PathToWritingSystemsFolder);
				WritingSystemDefinition ws = TestEnvironment.CreateDetailedWritingSystem("en");
				wsCollectionToBeWritten.Set(ws);
				WritingSystemDefinition ws2 = TestEnvironment.CreateDetailedWritingSystem("de");
				wsCollectionToBeWritten.Set(ws2);
				wsCollectionToBeWritten.Save();

				IWritingSystemRepository loadedWsCollection = GetWritingSystemRepository(e.PathToWritingSystemsFolder);
				TestEnvironment.AssertWritingSystemCollectionsAreEqual(wsCollectionToBeWritten, loadedWsCollection);
			}
		}

		[Test]
		public void Load_LdmlWritingSystemsHaveSameIsoCodeButDifferentRegionInfo_DoesNotCrash()
		{
			using (var e = new TestEnvironment())
			{
				var wsCollectionToBeWritten = GetWritingSystemRepository(e.PathToWritingSystemsFolder);
				WritingSystemDefinition ws = TestEnvironment.CreateDetailedWritingSystem("th");
				ws.Region = "BR";
				wsCollectionToBeWritten.Set(ws);
				WritingSystemDefinition ws2 = TestEnvironment.CreateDetailedWritingSystem("th");
				ws2.Region = "AQ";
				wsCollectionToBeWritten.Set(ws2);
				wsCollectionToBeWritten.Save();
				var loadedWsCollection = GetWritingSystemRepository(e.PathToWritingSystemsFolder);
				TestEnvironment.AssertWritingSystemCollectionsAreEqual(wsCollectionToBeWritten, loadedWsCollection);
			}
		}

		[Test]
		public void Write_LoadedWritingSystemIsDeleted_DeletionIsRoundTripped()
		{
			using (var e = new TestEnvironment())
			{
				//Write out two writing systems
				IWritingSystemRepository wsCollectionToBeWritten =
					GetWritingSystemRepository(e.PathToWritingSystemsFolder);
				WritingSystemDefinition ws = TestEnvironment.CreateDetailedWritingSystem("en");
				wsCollectionToBeWritten.Set(ws);
				WritingSystemDefinition ws2 = TestEnvironment.CreateDetailedWritingSystem("th");
				wsCollectionToBeWritten.Set(ws2);
				wsCollectionToBeWritten.Save();

				//load them up again
				IWritingSystemRepository loadedWsCollection = GetWritingSystemRepository(e.PathToWritingSystemsFolder);
				loadedWsCollection.Remove(ws.Id); //remove one
				loadedWsCollection.Save();

				//Now check that it hasn't come back!
				IWritingSystemRepository loadedWsCollection2 =
					GetWritingSystemRepository(e.PathToWritingSystemsFolder);
				Assert.IsFalse(loadedWsCollection2.Contains(ws.Id));
			}
		}

		[Test]
		public void Roundtripping_Works()
		{
			using (var e = new TestEnvironment())
			{
				IWritingSystemRepository wsCollectionToBeWritten = GetWritingSystemRepository(e.PathToWritingSystemsFolder);
				WritingSystemDefinition ws = TestEnvironment.CreateDetailedWritingSystem("th");
				wsCollectionToBeWritten.Set(ws);
				WritingSystemDefinition ws2 = TestEnvironment.CreateDetailedWritingSystem("en");
				wsCollectionToBeWritten.Set(ws2);
				wsCollectionToBeWritten.Save();
				IWritingSystemRepository loadedWsCollection = GetWritingSystemRepository(e.PathToWritingSystemsFolder);
				TestEnvironment.AssertWritingSystemCollectionsAreEqual(wsCollectionToBeWritten, loadedWsCollection);
			}
		}

		private static IWritingSystemRepository GetWritingSystemRepository(string writingSystemdPath)
		{
			return LdmlInFolderWritingSystemRepository.Initialize(
				writingSystemdPath,
				OnWritingSystemMigration,
				OnWritingSystemLoadProblem,
				WritingSystemCompatibility.Flex7V0Compatible
			);
		}

		private static void OnWritingSystemLoadProblem(IEnumerable<WritingSystemRepositoryProblem> problems)
		{
			throw new ApplicationException("Unexpected Input System load problem during test.");
		}

		private static void OnWritingSystemMigration(IEnumerable<LdmlVersion0MigrationStrategy.MigrationInfo> migrationinfo)
		{
			throw new ApplicationException("Unexpected Input System migration during test.");
		}

		[Test]
		public void Save_WritingSystemReadFromLdmlAndChanged_ChangesSaved()
		{
			using (var e = new TestEnvironment())
			{
				e.CreateLdmlWritingSystemDefinitionFile();
				IWritingSystemRepository loadedWsCollection = GetWritingSystemRepository(e.PathToWritingSystemsFolder);
				loadedWsCollection.Get("en").Keyboard = "changed";
				loadedWsCollection.Save();
				IWritingSystemRepository reloadedWsCollection =
					GetWritingSystemRepository(e.PathToWritingSystemsFolder);
				TestEnvironment.AssertWritingSystemCollectionsAreEqual(loadedWsCollection, reloadedWsCollection);
			}
		}

		[Test]
		public void DeserializeCollectionViaLoad()
		{
			using (var e = new TestEnvironment())
			{
				var store = e.MakeSampleCollection();

				store.Save();

				var c = GetWritingSystemRepository(e.PathToWritingSystemsFolder);
				Assert.IsNotNull(c);
				Assert.AreEqual(2, c.Count);
			}
		}

	}
}