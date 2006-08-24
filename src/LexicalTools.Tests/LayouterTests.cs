
using NUnit.Framework;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Tests;
using WeSay.UI;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class LayouterTests
	{
		[SetUp]
		public void Setup()
		{
			//Application.Init();
		}

		[Test]
		public void CountEntryLayoutRows()
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm["en"] = "12";
			AddSense(entry);
			AddSense(entry);

			DetailList builder = new DetailList();
			LexEntryLayouter layout = new LexEntryLayouter(builder);
			int rowcount = layout.AddWidgets(entry);

			Assert.AreEqual(11, rowcount);
		}

		private static void AddSense(LexEntry entry)
		{
			LexSense sense = entry.Senses.AddNew();
			AddExample(sense);
			AddExample(sense);
		}

		private static void AddExample(LexSense sense)
		{
			LexExampleSentence example = sense.ExampleSentences.AddNew();
			example.Sentence["th"] = "sentence";
		}
	}
}
