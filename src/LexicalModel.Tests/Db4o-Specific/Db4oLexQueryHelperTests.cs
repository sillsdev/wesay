using System;
using System.Collections.Generic;
using NUnit.Framework;
using WeSay.Language;
using WeSay.LexicalModel.Db4o_Specific;


namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class Db4oLexQueryHelperTests : BaseDb4oSpecificTests
	{
		[Test]
		public void FindEntriesFromLexemeForm()
		{
			CycleDatabase();
			LexEntry entry = new LexEntry();
			entry.LexicalForm["en"] = "findme";
			_entriesList.Add(entry);
			CycleDatabase();
			//don't want to find this one
			_dataSource.Data.Set(new LanguageForm("en", "findme", new MultiText()));
			List<LexEntry> list = Db4oLexQueryHelper.FindObjectsFromLanguageForm<LexEntry, LexicalFormMultiText>(_dataSource, "findme");
			Assert.AreEqual(1, list.Count);
		}

		[Test]
		public void FindEntryFromGuid()
		{
			CycleDatabase();
			Guid g = Guid.NewGuid();
			LexEntry entry = new LexEntry(null, g);
			entry.LexicalForm["en"] = "hello";
			_entriesList.Add(new LexEntry());
			_entriesList.Add(entry);
			_entriesList.Add(new LexEntry());
			CycleDatabase();
			LexEntry found = Db4oLexQueryHelper.FindObjectFromGuid<LexEntry>(_dataSource, g);
			Assert.AreEqual("hello", found.LexicalForm["en"]);
		}

		[Test]
		public void CannotFindEntryFromGuid()
		{
			CycleDatabase();
			Guid g = Guid.NewGuid();
			_entriesList.Add(new LexEntry());
			_entriesList.Add(new LexEntry(null, g));
			_entriesList.Add(new LexEntry());
			CycleDatabase();
			LexEntry found = Db4oLexQueryHelper.FindObjectFromGuid<LexEntry>(_dataSource, Guid.NewGuid());
			Assert.IsNull(found);
		}
		[Test,ExpectedException(typeof(ApplicationException))]
		public void MultipleGuidMatchesThrows()
		{
			CycleDatabase();
			Guid g = Guid.NewGuid();
			_entriesList.Add(new LexEntry(null, g));
			_entriesList.Add(new LexEntry(null, g));
			CycleDatabase();
			Db4oLexQueryHelper.FindObjectFromGuid<LexEntry>(_dataSource, g);
		}
		[Test]
		public void FindEntriesFromGloss()
		{
			CycleDatabase();
			string gloss = "ant";
			AddEntryWithGloss(gloss);
			CycleDatabase();
			//don't want to find this one
			_dataSource.Data.Set(new LanguageForm("en", gloss, new MultiText()));

			List<LexEntry> list = Db4oLexQueryHelper.FindObjectsFromLanguageForm<LexEntry, SenseGlossMultiText>(_dataSource, gloss);
			Assert.AreEqual(1, list.Count);
		}

		private void AddEntryWithGloss(string gloss)
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			sense.Gloss["en"] = gloss;
			_entriesList.Add(entry);
		}



		[Test]
		public void AddSenseAppendsToExistingLexEntry()
		{
			CycleDatabase();
			string formToFind = "Bank";
			AddEntryWithLexemeForm("ignore1");
			LexEntry entryToMatch =AddEntryWithLexemeForm(formToFind);
			AddEntryWithLexemeForm("ignore2");

			MultiText lexemeForm = new MultiText();
			lexemeForm["en"] = formToFind;
			Assert.AreEqual(0, entryToMatch.Senses.Count);

			//add sense to empty entry
			LexSense sense = new LexSense();
			sense.Gloss["en"] = "money place";
			Db4oLexQueryHelper.AddSenseToLexicon(_recordListManager, lexemeForm, sense);
			Assert.AreEqual(1, entryToMatch.Senses.Count);

			//add sense to  entry which already has one sense
			LexSense sense2 = new LexSense();
			Db4oLexQueryHelper.AddSenseToLexicon(_recordListManager, lexemeForm, sense2);
			Assert.AreEqual(2, entryToMatch.Senses.Count);

			sense.Gloss["en"] = "side of river";
		}

		private LexEntry AddEntryWithLexemeForm(string lexemeForm)
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm["en"] = lexemeForm;
			_entriesList.Add(entry);
			return entry;
		}

	}

}