using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using Exortech.NetReflector;
using NUnit.Framework;
using Palaso.Xml;
using WeSay.LexicalModel.Foundation;
using Palaso.TestUtilities;
using Palaso.WritingSystems;

namespace WeSay.LexicalModel.Tests.Foundation
{
	[TestFixture]
	public class LdmlInFolderWritingSystemCollectionTests
	{
		private class TestEnvironment:IDisposable
		{
			private IWritingSystemRepository _collection;
			private TemporaryFolder _wesayProjectFolder;
			private TemporaryFolder _ldmlWsFolder;
			private TempFile _wsPrefsFile;

			public TestEnvironment()
			{
				_wesayProjectFolder = new TemporaryFolder("WesayProject");
				_ldmlWsFolder = new TemporaryFolder(_wesayProjectFolder, "WritingSystems");
				_wsPrefsFile = new TempFile(_ldmlWsFolder);
			}

			public string PathToWritingSystemsFolder
			{
				get { return _ldmlWsFolder.Path; }
			}

			public string PathToWsPrefsFile
			{
				get { return _wsPrefsFile.Path; }
			}

			public void Dispose()
			{
				_wsPrefsFile.Dispose();
				_ldmlWsFolder.Dispose();
				_wesayProjectFolder.Dispose();
			}
		}

		private void CreateSampleWritingSystemFile(string path)
		{
			using (StreamWriter writer = File.CreateText(path))
			{
				writer.Write(@"<?xml version='1.0' encoding='utf-8'?>
					<WritingSystemCollection>
					  <members>
						<WritingSystem>
						  <FontName>Courier New</FontName>
						  <FontSize>10</FontSize>
						  <Id>PretendAnalysis</Id>
						</WritingSystem>
						<WritingSystem>
						  <FontName>Courier New</FontName>
						  <FontSize>20</FontSize>
						  <Id>PretendVernacular</Id>
						</WritingSystem>
					  </members>
					</WritingSystemCollection>");
				writer.Close();
			}
		}

		private void WriteOldWeSayWritingSystemsFile(string path, IWritingSystemRepository wsCollection)
		{
			XmlWriter writer = XmlWriter.Create(path);
			try
			{
				writer.WriteStartDocument();
				NetReflector.Write(writer, wsCollection);
			}
			finally
			{
				writer.Close();
			}
		}

		[Test]
		public void LoadFromLegacyWeSayFile_WritingSystemsAreLoadedFromThatFile()
		{
			using(var e = new TestEnvironment())
			{
				IWritingSystemRepository wsCollectionToBeWritten =
					new LdmlInFolderWritingSystemRepository(e.PathToWritingSystemsFolder);
				WritingSystemDefinition ws = CreateDetailedWritingSystem("en");
				wsCollectionToBeWritten.Set(ws);
				WritingSystemDefinition ws2 = CreateDetailedWritingSystem("de");
				wsCollectionToBeWritten.Set(ws2);
				wsCollectionToBeWritten.Save();
				IWritingSystemRepository loadedWsCollection = new LdmlInFolderWritingSystemRepository(e.PathToWritingSystemsFolder);
				AssertWritingSystemCollectionsAreEqual(wsCollectionToBeWritten, loadedWsCollection);
			}
		}

		private void AssertWritingSystemCollectionsAreEqual(IWritingSystemRepository lhs, IWritingSystemRepository rhs)
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

		private static WritingSystemDefinition CreateDetailedWritingSystem(string languageCode)
		{
			WritingSystemDefinition ws = new WritingSystemDefinition();
			ws.ISO = languageCode;
			ws.Abbreviation = languageCode;
			ws.IsVoice = false;
			ws.DefaultFontName = new Font(FontFamily.GenericSansSerif, 12).Name;
			ws.DefaultFontSize = new Font(FontFamily.GenericSansSerif, 12).Size;
			ws.IsVoice = false;
			ws.IsUnicodeEncoded = true;
			ws.Keyboard = "Bogus ivories!";
			ws.RightToLeftScript = false;
			ws.SortUsingCustomSimple("");
			ws.SpellCheckingId = languageCode;
			return ws;
		}

		//private WritingSystem CreateDetailedWritingSystemThatCantBeRepresentedByPalaso(string languageCode)
		//{
		//    WritingSystem ws = new WritingSystem();
		//    ws.ISO = languageCode;
		//    ws.Abbreviation = languageCode;
		//    ws.DefaultFontName = new Font(FontFamily.GenericSansSerif, 12).Name;
		//    ws.DefaultFontSize = new Font(FontFamily.GenericSansSerif, 12).Size;
		//    ws.IsVoice = false;
		//    ws.IsUnicodeEncoded = false;
		//    ws.Keyboard = "Bogus ivories!";
		//    ws.RightToLeftScript = false;
		//    ws.SortUsingCustomSimple("bogus rules");
		//    ws.SpellCheckingId = languageCode;
		//    return ws;
		//}


		[Test]
		public void Load_OnlyLdmlWritingSystemFilesExist_WritingSystemsAreLoadedFromThoseFiles()
		{
			using (var e = new TestEnvironment())
			{
				IWritingSystemRepository wsCollectionToBeWritten =
					new LdmlInFolderWritingSystemRepository(e.PathToWritingSystemsFolder);
				WritingSystemDefinition ws = CreateDetailedWritingSystem("th");
				wsCollectionToBeWritten.Set(ws);
				WritingSystemDefinition ws2 = CreateDetailedWritingSystem("en");
				wsCollectionToBeWritten.Set(ws2);
				WriteLdmlWritingSystemFiles(e.PathToWritingSystemsFolder, wsCollectionToBeWritten);
				IWritingSystemRepository loadedWsCollection = new LdmlInFolderWritingSystemRepository(e.PathToWritingSystemsFolder);
				AssertWritingSystemCollectionsAreEqual(wsCollectionToBeWritten, loadedWsCollection);
			}
		}

		[Test]
		public void Load_LdmlWritingSystemsHaveSameIsoCodeButDifferentVariantRegionInfo_DoesNotCrash()
		{
			using (var e = new TestEnvironment())
			{
				var wsCollectionToBeWritten = new LdmlInFolderWritingSystemRepository(e.PathToWritingSystemsFolder);
				WritingSystemDefinition ws = CreateDetailedWritingSystem("th");
				ws.Region = "BR";
				wsCollectionToBeWritten.Set(ws);
				WritingSystemDefinition ws2 = CreateDetailedWritingSystem("th");
				ws2.Region = "AQ";
				wsCollectionToBeWritten.Set(ws2);
				WriteLdmlWritingSystemFiles(e.PathToWritingSystemsFolder, wsCollectionToBeWritten);
				var loadedWsCollection = new LdmlInFolderWritingSystemRepository(e.PathToWritingSystemsFolder);
				AssertWritingSystemCollectionsAreEqual(wsCollectionToBeWritten, loadedWsCollection);
			}
		}

		private static void WriteLdmlWritingSystemFiles(string pathToStore, IWritingSystemRepository wsCollectionToBeWritten)
		{
			var store = new LdmlInFolderWritingSystemRepository(pathToStore);
			foreach (var writingSystem in wsCollectionToBeWritten.AllWritingSystems)
			{
				store.Set(writingSystem);
			}
			store.Save();
		}

		[Test]
		public void Write_LoadedWritingSystemIsDeleted_DeletionIsRoundTripped()
		{
			using (var e = new TestEnvironment())
			{
				//Write out two writing systems
				IWritingSystemRepository wsCollectionToBeWritten =
					new LdmlInFolderWritingSystemRepository(e.PathToWritingSystemsFolder);
				//WritingSystem ws = CreateDetailedWritingSystemThatCantBeRepresentedByPalaso("test");
				WritingSystemDefinition ws = CreateDetailedWritingSystem("en");
				wsCollectionToBeWritten.Set(ws);
				//WritingSystem ws2 = CreateDetailedWritingSystemThatCantBeRepresentedByPalaso("test2");
				WritingSystemDefinition ws2 = CreateDetailedWritingSystem("th");
				wsCollectionToBeWritten.Set(ws2);
				wsCollectionToBeWritten.Save();
				//load them up again
				IWritingSystemRepository loadedWsCollection = new LdmlInFolderWritingSystemRepository(e.PathToWritingSystemsFolder);
				loadedWsCollection.Remove(ws.Id); //remove one
				loadedWsCollection.Save();
				//Now check that it hasn't come back!
				IWritingSystemRepository loadedWsCollection2 =
					new LdmlInFolderWritingSystemRepository(e.PathToWritingSystemsFolder);
				Assert.IsFalse(loadedWsCollection2.Contains(ws.Id));
			}
		}

		[Test]
		public void Roundtripping_Works()
		{
			using (var e = new TestEnvironment())
			{
				IWritingSystemRepository wsCollectionToBeWritten =
					new LdmlInFolderWritingSystemRepository(e.PathToWritingSystemsFolder);
				//WritingSystem ws = CreateDetailedWritingSystemThatCantBeRepresentedByPalaso("test");
				WritingSystemDefinition ws = CreateDetailedWritingSystem("th");
				wsCollectionToBeWritten.Set(ws);
				//WritingSystem ws2 = CreateDetailedWritingSystemThatCantBeRepresentedByPalaso("test2");
				WritingSystemDefinition ws2 = CreateDetailedWritingSystem("en");
				wsCollectionToBeWritten.Set(ws2);
				wsCollectionToBeWritten.Save();
				IWritingSystemRepository loadedWsCollection = new LdmlInFolderWritingSystemRepository(e.PathToWritingSystemsFolder);
				AssertWritingSystemCollectionsAreEqual(wsCollectionToBeWritten, loadedWsCollection);
			}
		}

		[Test]
		public void Save_WritingSystemReadFromLdmlAndChanged_ChangesSaved()
		{
			using (var e = new TestEnvironment())
			{
				CreateLdmlWritingsystemDefinitionFile(e.PathToWritingSystemsFolder);
				IWritingSystemRepository loadedWsCollection = new LdmlInFolderWritingSystemRepository(e.PathToWritingSystemsFolder);
				loadedWsCollection.Get("en").Keyboard = "changed";
				loadedWsCollection.Save();
				IWritingSystemRepository reloadedWsCollection =
					new LdmlInFolderWritingSystemRepository(e.PathToWritingSystemsFolder);
				AssertWritingSystemCollectionsAreEqual(loadedWsCollection, reloadedWsCollection);
			}
		}

		private void CreateLdmlWritingsystemDefinitionFile(string pathToLdmlFolder)
		{
			IWritingSystemRepository wsCollectionToBeWritten = new LdmlInFolderWritingSystemRepository(pathToLdmlFolder);
			WritingSystemDefinition ws = CreateDetailedWritingSystem("en");
			wsCollectionToBeWritten.Set(ws);
			wsCollectionToBeWritten.Save();
	}

		[Test]
		[Category("WritingSystemRefactor")]
		public void MissingIdIsHandledOk()
		{
			using (var e = new TestEnvironment())
			{
				Assert.Fail("Seems really low value to me CH 2011-03");
				IWritingSystemRepository x = new LdmlInFolderWritingSystemRepository(e.PathToWritingSystemsFolder);
				WritingSystemDefinition ws = x.Get("unheardof");
				Assert.IsNotNull(ws);
				Assert.AreSame(ws, x.Get("unheardof"), "Expected to get exactly the same one each time");
			}
		}

		[Test]
		public void RightFont()
		{
			using (var e = new TestEnvironment())
			{
				Assert.Fail("Move to migrator");

				CreateSampleWritingSystemFile(e.PathToWsPrefsFile);
				IWritingSystemRepository repo = new LdmlInFolderWritingSystemRepository(e.PathToWritingSystemsFolder);
				//_collection.LoadFromLegacyWeSayFile(_wsPrefsFile.Path);
				WritingSystemDefinition ws = repo.Get("PretendAnalysis");
				Assert.AreEqual("PretendAnalysis", ws.Id);
				// since Linux may not have CourierNew, we
				// need to test against the font mapping
				Font expectedFont = new Font("Courier New", 10);
				Assert.AreEqual(expectedFont.Name, WritingSystemInfo.CreateFont(ws).Name);
				Assert.AreEqual(expectedFont.Size, WritingSystemInfo.CreateFont(ws).Size);
			}
		}

		// Tim move this to the migrator test
		[Test]
		public void SerializeCollection()
		{
			using (var e = new TestEnvironment())
			{
				Assert.Fail("Move to migrator");
				/*
				string s = MakeXmlFromCollection();

				XmlDocument doc = new XmlDocument();
				doc.LoadXml(s);

				//            Assert.AreEqual(1, doc.SelectNodes("WritingSystemCollection/AnalysisWritingSystemDefaultId").Count);
				//            Assert.AreEqual(1, doc.SelectNodes("WritingSystemCollection/VernacularWritingSystemDefaultId").Count);
				Assert.AreEqual(2,
								doc.SelectNodes("WritingSystemCollection/members/WritingSystem").Count);
				 */
			}
		}

		// TODO Move to migrator tests
		//private static string MakeXmlFromCollection()
		//{
		//    WritingSystemCollection c = MakeSampleCollection();

		//    var builder = new StringBuilder();
		//    using (var writer = XmlWriter.Create(builder, CanonicalXmlSettings.CreateXmlWriterSettings()))
		//    {
		//        writer.WriteStartDocument();
		//        NetReflector.Write(writer, c);
		//        writer.Close();
		//    }

		//    return builder.ToString();
		//}

		private static IWritingSystemRepository MakeSampleCollection(IWritingSystemRepository writingSystemStore)
		{
			writingSystemStore.Set(WritingSystemDefinition.FromLanguage("one"));
			writingSystemStore.Set(WritingSystemDefinition.FromLanguage("two"));
			return writingSystemStore;
		}

		[Test]
		[Category("WritingSystemRefactor")]
		public void WritingSystemCollection_HasUnknownVernacular()
		{
			using (var e = new TestEnvironment())
			{
				Assert.Fail("WeSayProject Tests should perhaps implement this... cjh");

				IWritingSystemRepository c = new LdmlInFolderWritingSystemRepository(e.PathToWritingSystemsFolder);
				Assert.IsNotNull(c.Get(WritingSystemInfo.IdForUnknownVernacular));
			}
		}

		[Test]
		[Category("WritingSystemRefactor")]
		public void WritingSystemCollection_HasUnknownAnalysis()
		{
			using (var e = new TestEnvironment())
			{
				Assert.Fail("WeSayProject Tests should perhaps implement this... cjh");
				IWritingSystemRepository c = new LdmlInFolderWritingSystemRepository(e.PathToWritingSystemsFolder);
				Assert.IsNotNull(c.Get(WritingSystemInfo.IdForUnknownAnalysis));
			}
		}

		[Test]
		public void DeserializeCollectionViaLoad()
		{
			using (var e = new TestEnvironment())
			{
				var store = new LdmlInFolderWritingSystemRepository(e.PathToWritingSystemsFolder);
				MakeSampleCollection(store);
				store.Save();

				var c = new LdmlInFolderWritingSystemRepository(e.PathToWritingSystemsFolder);
				Assert.IsNotNull(c);
				Assert.AreEqual(2, c.Count);
			}
		}

	}
}