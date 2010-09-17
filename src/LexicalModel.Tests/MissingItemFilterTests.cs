using System;
using System.Collections.Generic;
using NUnit.Framework;
using Palaso.DictionaryServices.Model;
using Palaso.Lift.Options;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class MissingItemFilterTests
	{
		[Test]
		public void ConstructWithField()
		{
			Field field = new Field("customField", "LexExampleSentence", new string[] {"vernacular"});
			Assert.IsNotNull(new MissingFieldQuery(field, null, null));
		}

		[Test]
		public void ConstructWithField_NullField_Throws()
		{
			Assert.Throws<ArgumentNullException>(() => new MissingFieldQuery(null, null, null));
		}

		[Test]
		public void Key_SameFieldParameters_Same()
		{
			MissingFieldQuery filter1 =
					new MissingFieldQuery(new Field("customField",
													"LexExampleSentence",
													new string[] { "vernacular" }), null, null);
			MissingFieldQuery filter2 =
					new MissingFieldQuery(new Field("customField",
													"LexExampleSentence",
													new string[] { "vernacular" }), null, null);
			Assert.IsTrue(filter1.Key == filter2.Key);
		}

		[Test]
		public void Key_DifferentWS_Different()
		{
			MissingFieldQuery filter1 =
					new MissingFieldQuery(new Field("customField",
													"LexExampleSentence",
													new string[] { "vernacular" }), null, null);
			MissingFieldQuery filter2 =
					new MissingFieldQuery(new Field("customField",
													"LexExampleSentence",
													new string[] {"analysis"}), null, null);
			Assert.IsFalse(filter1.Key == filter2.Key);
		}

		[Test]
		public void Key_MultipleWSInDifferentOrder_Same()
		{
			MissingFieldQuery filter1 =
					new MissingFieldQuery(new Field("customField",
													"LexExampleSentence",
													new string[] {"vernacular", "analysis"}), null, null);
			MissingFieldQuery filter2 =
					new MissingFieldQuery(new Field("customField",
													"LexExampleSentence",
													new string[] {"analysis", "vernacular"}), null, null);
			Assert.IsTrue(filter1.Key == filter2.Key);
		}

		[Test]
		public void FilteringPredicate_Null_False()
		{
			Field field = new Field("customField", "LexExampleSentence", new string[] {"vernacular"});
			MissingFieldQuery f = new MissingFieldQuery(field, null, null);
			Assert.IsFalse(f.FilteringPredicate(null));
		}

		[Test]
		public void FilteringPredicate_PartOfSpeechIsNull_True()
		{
			LexEntry entryWithUnknownPos = new LexEntry();
			entryWithUnknownPos.LexicalForm.SetAlternative("de", "LexicalForm");
			entryWithUnknownPos.Senses.Add(new LexSense());
			Field field = new Field("POS", "LexSense", new string[] { "en" }, Field.MultiplicityType.ZeroOr1, "Option");
			MissingFieldQuery f = new MissingFieldQuery(field, null, null);
			Assert.IsTrue(f.FilteringPredicate(entryWithUnknownPos));
		}

		[Test]
		public void FilteringPredicate_PartOfSpeechIsUnknown_True()
		{
			LexEntry entryWithUnknownPos = new LexEntry();
			entryWithUnknownPos.LexicalForm.SetAlternative("de", "LexicalForm");
			entryWithUnknownPos.Senses.Add(new LexSense());
			entryWithUnknownPos.Senses[0].Properties.Add(new KeyValuePair<string, object>("POS", new OptionRef()));
			((OptionRef) entryWithUnknownPos.Senses[0].Properties[0].Value).Key = "unknown";
			Field field = new Field("POS", "LexSense", new string[] { "en" }, Field.MultiplicityType.ZeroOr1, "Option");
			MissingFieldQuery f = new MissingFieldQuery(field, null, null);
			Assert.IsTrue(f.FilteringPredicate(entryWithUnknownPos));
		}

		[Test]
		public void FilteringPredicate_PartOfSpeechIsNotNullOrUnknown_False()
		{
			LexEntry entryWithUnknownPos = new LexEntry();
			entryWithUnknownPos.LexicalForm.SetAlternative("de", "LexicalForm");
			entryWithUnknownPos.Senses.Add(new LexSense());
			entryWithUnknownPos.Senses[0].Properties.Add(new KeyValuePair<string, object>("POS", new OptionRef()));
			((OptionRef)entryWithUnknownPos.Senses[0].Properties[0].Value).Key = "notUnknown";
			Field field = new Field("POS", "LexSense", new string[] { "en" }, Field.MultiplicityType.ZeroOr1, "Option");
			MissingFieldQuery f = new MissingFieldQuery(field, null, null);
			Assert.IsFalse(f.FilteringPredicate(entryWithUnknownPos));
		}


		[Test]
		public void FilteringPredicate_RelationTargetIsEmtpyString_True()
		{
			CheckRelationFilter("uncle", string.Empty, true);
		}

		[Test]
		public void FilteringPredicate_RelationTargetIsNull_True()
		{
			CheckRelationFilter("uncle", null, true);
		}

		[Test]
		public void FilteringPredicate_RelationTargetIsFilled_False()
		{
			CheckRelationFilter("uncle","ken", false);
		}

		private static void CheckRelationFilter(string relationname,string targetId, bool shouldMatch)
		{
			LexEntry entry = new LexEntry();
			entry.AddRelationTarget(relationname, targetId);
			Field field = new Field(relationname, "LexEntry",
									new string[] { "vernacular" },
									Field.MultiplicityType.ZeroOr1,
									"RelationToOneEntry");
			MissingFieldQuery f = new MissingFieldQuery(field, null, null);
			Assert.AreEqual(shouldMatch, f.FilteringPredicate(entry));
		}

	}
}