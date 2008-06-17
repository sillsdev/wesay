using System;
using NUnit.Framework;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class MissingItemFilterTests
	{
		[Test]
		public void ConstructWithField()
		{
			Field field =
					new Field("customField", "LexExampleSentence", new string[] {"vernacular"});
			Assert.IsNotNull(new MissingItemFilter(field));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ConstructWithField_NullField_Throws()
		{
			new MissingItemFilter(null);
		}

		[Test]
		public void ConstructWithFieldTemplate()
		{
			ViewTemplate f = new ViewTemplate();
			f.Add(new Field("field1", "LexEntry", new string[] {"en", "br", "th"}));
			Assert.IsNotNull(new MissingItemFilter(f, "field1"));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ConstructWithFieldTemplate_NullFieldTemplate_Throws()
		{
			new MissingItemFilter(null, "field1");
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ConstructWithFieldTemplate_NullFieldName_Throws()
		{
			ViewTemplate f = new ViewTemplate();
			f.Add(new Field("field1", "LexEntry", new string[] {"en", "br", "th"}));
			new MissingItemFilter(f, null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void ConstructWithFieldTemplate_FieldNameNotInFieldTemplate_Throws()
		{
			ViewTemplate f = new ViewTemplate();
			f.Add(new Field("field1", "LexEntry", new string[] {"en", "br", "th"}));
			new MissingItemFilter(f, "field");
		}

		[Test]
		public void Key_SameFieldParameters_Same()
		{
			MissingItemFilter filter1 =
					new MissingItemFilter(
							new Field("customField",
									  "LexExampleSentence",
									  new string[] {"vernacular"}));
			MissingItemFilter filter2 =
					new MissingItemFilter(
							new Field("customField",
									  "LexExampleSentence",
									  new string[] {"vernacular"}));
			Assert.IsTrue(filter1.Key == filter2.Key);
		}

		[Test]
		public void Key_DifferentWS_Different()
		{
			MissingItemFilter filter1 =
					new MissingItemFilter(
							new Field("customField",
									  "LexExampleSentence",
									  new string[] {"vernacular"}));
			MissingItemFilter filter2 =
					new MissingItemFilter(
							new Field("customField", "LexExampleSentence", new string[] {"analysis"}));
			Assert.IsFalse(filter1.Key == filter2.Key);
		}

		[Test]
		public void Key_MultipleWSInDifferentOrder_Same()
		{
			MissingItemFilter filter1 =
					new MissingItemFilter(
							new Field("customField",
									  "LexExampleSentence",
									  new string[] {"vernacular", "analysis"}));
			MissingItemFilter filter2 =
					new MissingItemFilter(
							new Field("customField",
									  "LexExampleSentence",
									  new string[] {"analysis", "vernacular"}));
			Assert.IsTrue(filter1.Key == filter2.Key);
		}

		[Test]
		public void FilteringPredicate_Null_False()
		{
			Field field =
					new Field("customField", "LexExampleSentence", new string[] {"vernacular"});
			MissingItemFilter f = new MissingItemFilter(field);
			Assert.IsFalse(f.FilteringPredicate(null));
		}
	}
}