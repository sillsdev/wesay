using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Palaso.TestUtilities;
using NUnit.Framework;
using System.IO;
using Palaso.WritingSystems;
using WeSay.LexicalModel.Foundation;
using WeSay.Project.ConfigMigration.WritingSystems;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class WritingSystemPrefsToLdmlMigratorTests
	{

		private string GetWritingSystemPrefContent(
			string abbreviation,
			string fontname,
			int fontsize,
			string id,
			bool isAudio,
			bool IsUnicode,
			bool isRightToLeft,
			WritingSystemDefinition.SortRulesType sortUsing,
			string spellCheckingId)
		{
			return String.Format(
@"<?xml version='1.0' encoding='utf-8'?>
<WritingSystemCollection>
  <members>
	<WritingSystem>
	  <Abbreviation>sr</Abbreviation>
	  <FontName>Arial</FontName>
	  <FontSize>12</FontSize>
	  <Id>sr-Latn-RS</Id>
	  <IsAudio>False</IsAudio>
	  <IsUnicode>True</IsUnicode>
	  <RightToLeft>False</RightToLeft>
	  <SortUsing></SortUsing>
	  <SpellCheckingId>ser</SpellCheckingId>
	</WritingSystem>
	<WritingSystem>
	  <Abbreviation>{0}</Abbreviation>
	  <FontName>{1}</FontName>
	  <FontSize>{2}</FontSize>
	  <Id>{3}</Id>
	  <IsAudio>{4}</IsAudio>
	  <IsUnicode>{5}</IsUnicode>
	  <RightToLeft>{6}</RightToLeft>
	  <SortUsing>{7}</SortUsing>
	  <SpellCheckingId>{8}</SpellCheckingId>
	</WritingSystem>
  </members>
</WritingSystemCollection>"
,abbreviation, fontname, fontsize, id, isAudio.ToString(), IsUnicode.ToString(), isRightToLeft.ToString(), sortUsing, spellCheckingId).Replace("'", "\"");
		}

		[Test]
		public void WritingSystemPrefsContainsVariousInfo_Migrate_WritingSystemsAreIdenticalForWeSayPurposes()
		{
			WeSayWordsProject.InitializeForTests();
			WritingSystemCollection _writingSystemsFromWritingSystemPrefs = new WritingSystemCollection();
			WritingSystemCollection _writingSystemsFromLdml = new WritingSystemCollection();
			using (TemporaryFolder tempFolder = new TemporaryFolder())
			{
				string pathToWritingSystemPrefs = Path.Combine(tempFolder.Path, "WritingSystemPrefs.xml");
				string pathToLdmlWritingSystemsFolder = Path.Combine(tempFolder.Path, "WritingSystems");
				File.WriteAllText(pathToWritingSystemPrefs, GetWritingSystemPrefContent(
																 "de", "Ansana New", 14, "de-AU-1911", true, false,
																 false,
																 WritingSystemDefinition.SortRulesType.CustomSimple,
																 "de-AU"));
				_writingSystemsFromWritingSystemPrefs.LoadFromLegacyWeSayFile(pathToWritingSystemPrefs);
				WritingSystemPrefsToLdmlMigrator migrator = new WritingSystemPrefsToLdmlMigrator(tempFolder.Path);
				migrator.MigrateIfNeeded();
				_writingSystemsFromLdml.Load(pathToLdmlWritingSystemsFolder);
				Assert.IsFalse(File.Exists(pathToWritingSystemPrefs));
			}
			AssertThatWritingsystemsAreIdenticalForWeSayPurposes(_writingSystemsFromWritingSystemPrefs, _writingSystemsFromLdml);
		}

		[Test]
		public void WritingSystemPrefs_Migrate_LdmlFilesAreCreatedAndNamedAccordingToRfcTag()
		{
			WeSayWordsProject.InitializeForTests();
			WritingSystemCollection _writingSystemsFromWritingSystemPrefs = new WritingSystemCollection();
			WritingSystemCollection _writingSystemsFromLdml = new WritingSystemCollection();
			using (TemporaryFolder tempFolder = new TemporaryFolder())
			{
				string pathToWritingSystemPrefs = Path.Combine(tempFolder.Path, "WritingSystemPrefs.xml");
				string pathToLdmlWritingSystemsFolder = Path.Combine(tempFolder.Path, "WritingSystems");
				File.WriteAllText(pathToWritingSystemPrefs, GetWritingSystemPrefContent(
																"de", "Ansana New", 14, "de-AU-1911", true, false,
																false,
																WritingSystemDefinition.SortRulesType.CustomSimple,
																"de-AU"));
				_writingSystemsFromWritingSystemPrefs.LoadFromLegacyWeSayFile(pathToWritingSystemPrefs);
				WritingSystemPrefsToLdmlMigrator migrator = new WritingSystemPrefsToLdmlMigrator(tempFolder.Path);
				migrator.MigrateIfNeeded();
				_writingSystemsFromLdml.Load(pathToLdmlWritingSystemsFolder);
				Assert.IsFalse(File.Exists(pathToWritingSystemPrefs));
				AssertThatLdmlFilesWereCreated(_writingSystemsFromWritingSystemPrefs, pathToLdmlWritingSystemsFolder);
			}
		}



		[Test]
		public void WritingSystemPrefs_Migrate_AudioWritingSystemIsTreatedCorrectly()
		{
			WeSayWordsProject.InitializeForTests();
			WritingSystemCollection _writingSystemsFromWritingSystemPrefs = new WritingSystemCollection();
			WritingSystemCollection _writingSystemsFromLdml = new WritingSystemCollection();
			using (TemporaryFolder tempFolder = new TemporaryFolder())
			{
				string pathToWritingSystemPrefs = Path.Combine(tempFolder.Path, "WritingSystemPrefs.xml");
				string pathToLdmlWritingSystemsFolder = Path.Combine(tempFolder.Path, "WritingSystems");
				File.WriteAllText(pathToWritingSystemPrefs, GetWritingSystemPrefContent(
																"de", "Ansana New", 14, "de-AU-1911", true, false,
																false,
																WritingSystemDefinition.SortRulesType.CustomSimple,
																"de-AU"));
				_writingSystemsFromWritingSystemPrefs.LoadFromLegacyWeSayFile(pathToWritingSystemPrefs);
				WritingSystemPrefsToLdmlMigrator migrator = new WritingSystemPrefsToLdmlMigrator(tempFolder.Path);
				migrator.MigrateIfNeeded();
				_writingSystemsFromLdml.Load(pathToLdmlWritingSystemsFolder);
				Assert.IsFalse(File.Exists(pathToWritingSystemPrefs));
				AssertThatLdmlFilesWereCreated(_writingSystemsFromWritingSystemPrefs, pathToLdmlWritingSystemsFolder);
			}
		}

		private void AssertThatLdmlFilesWereCreated(WritingSystemCollection writingSystemsFromWritingSystemPrefs, string pathToLdmlFolder)
		{
			foreach (KeyValuePair<string, WritingSystem> idToWritingSystemPair in writingSystemsFromWritingSystemPrefs)
			{
				Assert.IsTrue(File.Exists(Path.Combine(pathToLdmlFolder, idToWritingSystemPair.Key) + ".ldml"));
			}
		}

		private void AssertThatWritingsystemsAreIdenticalForWeSayPurposes(WritingSystemCollection writingSystemsFromWritingSystemPrefs, WritingSystemCollection writingSystemsFromLdml)
		{
			foreach (KeyValuePair<string, WritingSystem> idToWritingSystemPair in writingSystemsFromWritingSystemPrefs)
			{

				Assert.AreEqual(idToWritingSystemPair.Value.Id, writingSystemsFromLdml[idToWritingSystemPair.Key].Id);
				Assert.AreEqual(idToWritingSystemPair.Value.IsAudio, writingSystemsFromLdml[idToWritingSystemPair.Key].IsAudio);
				Assert.AreEqual(idToWritingSystemPair.Value.IsUnicode, writingSystemsFromLdml[idToWritingSystemPair.Key].IsUnicode);
				Assert.AreEqual(idToWritingSystemPair.Value.KeyboardName, writingSystemsFromLdml[idToWritingSystemPair.Key].KeyboardName);
				Assert.AreEqual(idToWritingSystemPair.Value.RightToLeft, writingSystemsFromLdml[idToWritingSystemPair.Key].RightToLeft);
				Assert.AreEqual(idToWritingSystemPair.Value.SortUsing, writingSystemsFromLdml[idToWritingSystemPair.Key].SortUsing);
				Assert.AreEqual(idToWritingSystemPair.Value.SpellCheckingId, writingSystemsFromLdml[idToWritingSystemPair.Key].SpellCheckingId);
				Assert.AreEqual(idToWritingSystemPair.Value.UsesCustomSortRules, writingSystemsFromLdml[idToWritingSystemPair.Key].UsesCustomSortRules);

			}
		}
	}
}
