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
	public class WritingSystemCollectionTests
	{
		private WritingSystemCollection _collection;
		private TemporaryFolder _wesayProjectFolder;
		private TemporaryFolder _ldmlWsFolder;
		private TempFile _wsPrefsFile;

		[SetUp]
		public void Setup()
		{
			_wesayProjectFolder = new TemporaryFolder("WesayProject");
			_ldmlWsFolder = new TemporaryFolder(_wesayProjectFolder, "WritingSystems");
			_wsPrefsFile = new TempFile(_ldmlWsFolder);
			_collection = new WritingSystemCollection();
		}

		[TearDown]
		public void TearDown()
		{
			_wsPrefsFile.Dispose();
			_ldmlWsFolder.Dispose();
			_wesayProjectFolder.Dispose();
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

		private void WriteOldWeSayWritingSystemsFile(string path, WritingSystemCollection wsCollection)
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
			Assert.Fail("Move to migrator");
			using (TemporaryFolder pretendProjectFolder = new TemporaryFolder("pretendWeSayProjectFolder"))
			{
				WritingSystemCollection wsCollectionToBeWritten = new WritingSystemCollection();
				WritingSystem ws = CreateDetailedWritingSystem("test");
				wsCollectionToBeWritten.Add(ws.Id,ws);
				WritingSystem ws2 = CreateDetailedWritingSystem("test2");
				wsCollectionToBeWritten.Add(ws2.Id, ws2);
				WriteOldWeSayWritingSystemsFile(_wsPrefsFile.Path, wsCollectionToBeWritten);
				WritingSystemCollection loadedWsCollection = new WritingSystemCollection();
				//loadedWsCollection.LoadFromLegacyWeSayFile(_wsPrefsFile.Path);
				AssertWritingSystemCollectionsAreEqual(wsCollectionToBeWritten, loadedWsCollection);
			}
		}

		private void AssertWritingSystemCollectionsAreEqual(WritingSystemCollection lhs, WritingSystemCollection rhs)
		{
			foreach (var lhsWritingSystem in lhs.WritingSystemDefinitions)
			{
				var rhsWritingSystem = rhs.Get(lhsWritingSystem.Id);
				Assert.IsTrue(rhs.Contains(lhsWritingSystem.Id));
				Assert.AreEqual(lhsWritingSystem.Id, rhsWritingSystem.Id);
				Assert.AreEqual(lhsWritingSystem.Abbreviation, rhsWritingSystem.Abbreviation);
				Assert.AreEqual(WritingSystemInfo.CreateFont(lhsWritingSystem).ToString(), WritingSystemInfo.CreateFont(rhsWritingSystem).ToString());
				Assert.AreEqual(lhsWritingSystem.DefaultFontName, rhsWritingSystem.DefaultFontName);
				Assert.AreEqual(lhsWritingSystem.DefaultFontSize, rhsWritingSystem.DefaultFontSize);
				Assert.AreEqual(lhsWritingSystem.IsVoice, rhsWritingSystem.IsVoice);
				Assert.AreEqual(lhsWritingSystem.IsUnicodeEncoded, rhsWritingSystem.IsUnicodeEncoded);
				Assert.AreEqual(lhsWritingSystem.Keyboard, rhsWritingSystem.Keyboard);
				Assert.AreEqual(lhsWritingSystem.RightToLeftScript, rhsWritingSystem.RightToLeftScript);
				Assert.AreEqual(lhsWritingSystem.SortUsing, rhsWritingSystem.SortUsing);
				Assert.AreEqual(lhsWritingSystem.CustomSortRules, rhsWritingSystem.CustomSortRules);
				Assert.AreEqual(lhsWritingSystem.SpellCheckingId, rhsWritingSystem.SpellCheckingId);
			}
		}

		private WritingSystem CreateDetailedWritingSystem(string languageCode)
		{
			WritingSystem ws = new WritingSystem();
			ws.ISO = languageCode;
			ws.Abbreviation = languageCode;
			ws.CustomSortRules = "";
			ws.IsVoice = false;
			ws.DefaultFontName = new Font(FontFamily.GenericSansSerif, 12).Name;
			ws.DefaultFontSize = new Font(FontFamily.GenericSansSerif, 12).Size; ws.IsVoice = false;
			ws.IsUnicodeEncoded = true;
			ws.Keyboard = "Bogus ivories!";
			ws.RightToLeftScript = false;
			ws.SortUsing = CustomSortRulesType.CustomSimple.ToString();
			ws.SpellCheckingId = languageCode;
			return ws;
		}

		private WritingSystem CreateDetailedWritingSystemThatCantBeRepresentedByPalaso(string languageCode)
		{
			WritingSystem ws = new WritingSystem();
			ws.ISO = languageCode;
			ws.Abbreviation = languageCode;
			ws.CustomSortRules = "Bogus roolz!";
			ws.DefaultFontName = new Font(FontFamily.GenericSansSerif, 12).Name;
			ws.DefaultFontSize = new Font(FontFamily.GenericSansSerif, 12).Size;
			ws.IsVoice = false;
			ws.IsUnicodeEncoded = false;
			ws.Keyboard = "Bogus ivories!";
			ws.RightToLeftScript = false;
			ws.SortUsing = CustomSortRulesType.CustomSimple.ToString();
			ws.SpellCheckingId = languageCode;
			return ws;
		}

		[Test]
		public void Load_OnlyLdmlWritingSystemFilesExist_WritingSystemsAreLoadedFromThoseFiles()
		{
				WritingSystemCollection wsCollectionToBeWritten = new WritingSystemCollection();
				WritingSystem ws = CreateDetailedWritingSystem("test");
				wsCollectionToBeWritten.Add(ws.Id, ws);
				WritingSystem ws2 = CreateDetailedWritingSystem("test2");
				wsCollectionToBeWritten.Add(ws2.Id, ws2);
				WriteLdmlWritingSystemFiles(_ldmlWsFolder.FolderPath, wsCollectionToBeWritten);
				WritingSystemCollection loadedWsCollection = new WritingSystemCollection();
				loadedWsCollection.Load(_ldmlWsFolder.FolderPath);
				AssertWritingSystemCollectionsAreEqual(wsCollectionToBeWritten, loadedWsCollection);
		}

		[Test]
		public void Load_LdmlWritingSystemsHaveSameIsoCodeButDifferentVariantRegionInfo_DoesNotCrash()
		{
				var wsCollectionToBeWritten = new WritingSystemCollection();
				WritingSystem ws = CreateDetailedWritingSystem("test");
				ws.Region = "Region1";
				wsCollectionToBeWritten.Add(ws.Id, ws);
				WritingSystem ws2 = CreateDetailedWritingSystem("test");
				ws2.Region = "Region2";
				wsCollectionToBeWritten.Add(ws2.Id, ws2);
				WriteLdmlWritingSystemFiles(_ldmlWsFolder.Path, wsCollectionToBeWritten);
				var loadedWsCollection = new WritingSystemCollection();
				loadedWsCollection.Load(_ldmlWsFolder.Path);
				AssertWritingSystemCollectionsAreEqual(wsCollectionToBeWritten, loadedWsCollection);
		}

		private static void WriteLdmlWritingSystemFiles(string pathToStore, WritingSystemCollection wsCollectionToBeWritten)
		{
			var store = new LdmlInFolderWritingSystemStore(pathToStore);
			foreach (var writingSystem in wsCollectionToBeWritten.WritingSystemDefinitions)
			{
				store.Set(writingSystem);
			}
			store.Save();
		}

		[Test]
		public void Write_LoadedWritingSystemIsDeleted_DeletionIsRoundTripped()
		{
			//Write out two writing systems
			WritingSystemCollection wsCollectionToBeWritten = new WritingSystemCollection();
			WritingSystem ws = CreateDetailedWritingSystemThatCantBeRepresentedByPalaso("test");
			wsCollectionToBeWritten.Add(ws.Id, ws);
			WritingSystem ws2 = CreateDetailedWritingSystemThatCantBeRepresentedByPalaso("test2");
			wsCollectionToBeWritten.Add(ws2.Id, ws2);
			wsCollectionToBeWritten.Write(_ldmlWsFolder.FolderPath);
			//load them up again
			WritingSystemCollection loadedWsCollection = new WritingSystemCollection();
			loadedWsCollection.Load(_ldmlWsFolder.FolderPath);
			loadedWsCollection.Remove(ws.Id);   //remove one
			loadedWsCollection.Write(_ldmlWsFolder.FolderPath); //write out the remaining writing system
			//Now check that it hasn't come back!
			WritingSystemCollection loadedWsCollection2 = new WritingSystemCollection();
			loadedWsCollection2.Load(_ldmlWsFolder.FolderPath);
			Assert.IsFalse(loadedWsCollection2.Contains(ws.Id));
		}

		[Test]
		public void Roundtripping_Works()
		{
			WritingSystemCollection wsCollectionToBeWritten = new WritingSystemCollection();
			WritingSystem ws = CreateDetailedWritingSystemThatCantBeRepresentedByPalaso("test");
			wsCollectionToBeWritten.Add(ws.Id, ws);
			WritingSystem ws2 = CreateDetailedWritingSystemThatCantBeRepresentedByPalaso("test2");
			wsCollectionToBeWritten.Add(ws2.Id, ws2);
			wsCollectionToBeWritten.Write(_ldmlWsFolder.FolderPath);
			WritingSystemCollection loadedWsCollection = new WritingSystemCollection();
			loadedWsCollection.Load(_ldmlWsFolder.FolderPath);
			AssertWritingSystemCollectionsAreEqual(wsCollectionToBeWritten, loadedWsCollection);
		}

		[Test]
		public void Save_WritingSystemReadFromLdmlAndChanged_ChangesSaved()
		{
				CreateLdmlWritingsystemDefinitionFile();
				WritingSystemCollection loadedWsCollection = new WritingSystemCollection();
				loadedWsCollection.Load(_ldmlWsFolder.FolderPath);
				loadedWsCollection.Get("test").Keyboard = "changed";
				loadedWsCollection.Write(_ldmlWsFolder.FolderPath);
				WritingSystemCollection reloadedWsCollection = new WritingSystemCollection();
				reloadedWsCollection.Load(_ldmlWsFolder.FolderPath);
				AssertWritingSystemCollectionsAreEqual(loadedWsCollection, reloadedWsCollection);
		}

		private void CreateLdmlWritingsystemDefinitionFile()
		{
			WritingSystemCollection wsCollectionToBeWritten = new WritingSystemCollection();
			WritingSystem ws = CreateDetailedWritingSystemThatCantBeRepresentedByPalaso("test");
			wsCollectionToBeWritten.Add(ws.Id, ws);
			wsCollectionToBeWritten.Write(_ldmlWsFolder.FolderPath);
		}

		[Test]
		public void TrimToActualTextWritingSystemIds_RemovesAudio()
		{
			var writingSystemCollection = new WritingSystemCollection();
			writingSystemCollection.Add("en", new WritingSystem("en"));
			var audio = new WritingSystem("en");
			audio.IsVoice = true;
			writingSystemCollection.Add("voice", audio);

			var ids = writingSystemCollection.TrimToActualTextWritingSystemIds(new List<string>() { "en", "voice" });
			Assert.AreEqual(1, ids.Count);
			Assert.AreEqual("en", ids[0]);
		}

		[Test]
		public void MissingIdIsHandledOk()
		{
			WritingSystemCollection x = new WritingSystemCollection();
			WritingSystem ws = x.Get("unheardof");
			Assert.IsNotNull(ws);
			Assert.AreSame(ws, x.Get("unheardof"), "Expected to get exactly the same one each time");
		}

		[Test]
		public void RightFont()
		{
			Assert.Fail("Move to migrator");

				CreateSampleWritingSystemFile(_wsPrefsFile.Path);
				//_collection.LoadFromLegacyWeSayFile(_wsPrefsFile.Path);
				WritingSystem ws = _collection.Get("PretendAnalysis");
				Assert.AreEqual("PretendAnalysis", ws.Id);
				// since Linux may not have CourierNew, we
				// need to test against the font mapping
				Font expectedFont = new Font("Courier New", 10);
				Assert.AreEqual(expectedFont.Name, WritingSystemInfo.CreateFont(ws).Name);
				Assert.AreEqual(expectedFont.Size, WritingSystemInfo.CreateFont(ws).Size);
		}


		[Test]
		public void SerializeCollection()
		{
			string s = MakeXmlFromCollection();

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(s);

			//            Assert.AreEqual(1, doc.SelectNodes("WritingSystemCollection/AnalysisWritingSystemDefaultId").Count);
			//            Assert.AreEqual(1, doc.SelectNodes("WritingSystemCollection/VernacularWritingSystemDefaultId").Count);
			Assert.AreEqual(2,
							doc.SelectNodes("WritingSystemCollection/members/WritingSystem").Count);
		}

		private static string MakeXmlFromCollection()
		{
			WritingSystemCollection c = MakeSampleCollection();

			var builder = new StringBuilder();
			using (var writer = XmlWriter.Create(builder, CanonicalXmlSettings.CreateXmlWriterSettings()))
			{
				writer.WriteStartDocument();
				NetReflector.Write(writer, c);
				writer.Close();
			}

			return builder.ToString();
		}

		private static WritingSystemCollection MakeSampleCollection()
		{
			WritingSystemCollection c = new WritingSystemCollection();
			c.Add("one", new WritingSystem("one"));
			c.Add("two", new WritingSystem("two"));
			return c;
		}

		[Test]
		public void WritingSystemCollection_HasUnknownVernacular()
		{
			WritingSystemCollection c = new WritingSystemCollection();
			Assert.IsNotNull(c.UnknownVernacularWritingSystem);
		}

		[Test]
		public void WritingSystemCollection_HasUnknownAnalysis()
		{
			WritingSystemCollection c = new WritingSystemCollection();
			Assert.IsNotNull(c.UnknownAnalysisWritingSystem);
		}

		[Test]
		public void DeserializeCollection()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof(WritingSystemCollection));
			t.Add(typeof(WritingSystem));

			NetReflectorReader r = new NetReflectorReader(t);
			WritingSystemCollection c = r.Read(MakeXmlFromCollection()) as WritingSystemCollection;
			Assert.IsNotNull(c);
			Assert.AreEqual(2, c.Count);
		}

		[Test]
		public void DeserializeCollectionViaLoad()
		{
			MakeSampleCollection().Write(_ldmlWsFolder.Path);

			var c = new WritingSystemCollection();
			c.Load(_ldmlWsFolder.Path);
			Assert.IsNotNull(c);
			Assert.AreEqual(2, c.Count);
		}

	}
}