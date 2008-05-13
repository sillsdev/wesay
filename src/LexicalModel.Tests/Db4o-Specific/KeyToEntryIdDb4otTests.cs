using System;
using System.Collections.Generic;
using NUnit.Framework;
using Palaso.Text;
using WeSay.Foundation;
using WeSay.LexicalModel.Db4o_Specific;


namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class KeyToEntryIdDb4otTests : BaseDb4oSpecificTests
	{

		[Test, Ignore("not yet")]
		public void GetGlossToEntryIdPairs_GetsKeyFromSimpleGloss()
		{
			CycleDatabase();
//            LexEntry entry = new LexEntry();
//            entry.LexicalForm["en"] = "findme";
//            _entriesList.Add(entry);
//            CycleDatabase();
//            //don't want to find this one
//            _dataSource.Data.Set(new LanguageForm("en", "findme", new MultiText()));
//            List<LexEntry> list = Db4oLexQueryHelper.FindDisconnectedObjectsFromLanguageForm<LexEntry, LexicalFormMultiText>(_dataSource, "findme");
//            Assert.AreEqual(1, list.Count);

			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			sense.Gloss.SetAlternative("en", "onegloss");
			_entriesList.Add(entry);

			CycleDatabase();

			List<LexEntry> list = Db4oLexQueryHelper.FindDisconnectedObjectsFromLanguageForm<LexEntry, SenseGlossMultiText>(_dataSource, "onegloss");
			Assert.AreEqual(1, list.Count);


			List<KeyValuePair<string,long>> keyPairs = KeyToEntryIdInitializer.GetGlossToEntryIdPairs(_dataSource, "en");
			Assert.AreEqual(1, keyPairs.Count);
			Assert.AreEqual("onegloss", keyPairs[0].Key);


		}



	}

}