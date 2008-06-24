using NUnit.Framework;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class MissingLexicalFormFilterTests
	{
		private MissingItemFilter _missingLexicalFormFilter;

		[SetUp]
		public void Setup()
		{
			Field field =
					new Field(Field.FieldNames.EntryLexicalForm.ToString(),
							  "LexEntry",
							  new string[] {"vernacular"});
			_missingLexicalFormFilter = new MissingItemFilter(field);
		}

		[Test]
		public void LexicalFormHasAnalysisWritingSystem()
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm["vernacular"] = "filler";

			Assert.AreEqual(false, _missingLexicalFormFilter.FilteringPredicate(entry));
		}

		[Test]
		public void LexicalFormNoWritingSystems()
		{
			LexEntry entry = new LexEntry();
			Assert.AreEqual(true, _missingLexicalFormFilter.FilteringPredicate(entry));
		}

		[Test]
		public void LexicalFormWritingSystemNoVernacular()
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm["analysis"] = "filler";
			Assert.AreEqual(true, _missingLexicalFormFilter.FilteringPredicate(entry));
		}
	}
}