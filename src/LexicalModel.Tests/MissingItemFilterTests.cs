using System;
using NUnit.Framework;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class MissingItemFilterTests
	{
		[Test]
		public void ConstructWithField()
		{
			Field field = new Field("customField", "LexExampleSentence", new string[] {"vernacular"});
			Assert.IsNotNull(new MissingFieldQuery(field));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ConstructWithField_NullField_Throws()
		{
			new MissingFieldQuery(null);
		}

		[Test]
		public void Key_SameFieldParameters_Same()
		{
			MissingFieldQuery filter1 =
					new MissingFieldQuery(new Field("customField",
													"LexExampleSentence",
													new string[] {"vernacular"}));
			MissingFieldQuery filter2 =
					new MissingFieldQuery(new Field("customField",
													"LexExampleSentence",
													new string[] {"vernacular"}));
			Assert.IsTrue(filter1.Key == filter2.Key);
		}

		[Test]
		public void Key_DifferentWS_Different()
		{
			MissingFieldQuery filter1 =
					new MissingFieldQuery(new Field("customField",
													"LexExampleSentence",
													new string[] {"vernacular"}));
			MissingFieldQuery filter2 =
					new MissingFieldQuery(new Field("customField",
													"LexExampleSentence",
													new string[] {"analysis"}));
			Assert.IsFalse(filter1.Key == filter2.Key);
		}

		[Test]
		public void Key_MultipleWSInDifferentOrder_Same()
		{
			MissingFieldQuery filter1 =
					new MissingFieldQuery(new Field("customField",
													"LexExampleSentence",
													new string[] {"vernacular", "analysis"}));
			MissingFieldQuery filter2 =
					new MissingFieldQuery(new Field("customField",
													"LexExampleSentence",
													new string[] {"analysis", "vernacular"}));
			Assert.IsTrue(filter1.Key == filter2.Key);
		}

		[Test]
		public void FilteringPredicate_Null_False()
		{
			Field field = new Field("customField", "LexExampleSentence", new string[] {"vernacular"});
			MissingFieldQuery f = new MissingFieldQuery(field);
			Assert.IsFalse(f.FilteringPredicate(null));
		}
	}
}