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
			_dataSource.Data.Set(new LanguageForm("en", "findme", null));
			List<LexEntry> list = Db4oLexQueryHelper.FindObjectsFromLanguageForm<LexEntry, LexicalFormMultiText>(_dataSource, "findme");
			Assert.AreEqual(1, list.Count);
		}

		[Test]
		public void FindEntriesFromGloss()
		{
			CycleDatabase();
			string gloss = "ant";
			AddEntryWithGloss(gloss);
			CycleDatabase();
			//don't want to find this one
			_dataSource.Data.Set(new LanguageForm("en", gloss, null));

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

	}

}