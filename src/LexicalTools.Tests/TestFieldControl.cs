using NUnit.Framework;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Tests;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class TestFieldControl
	{

		[Test]
		public void Create()
		{
			LexFieldControl lexFieldControl = new LexFieldControl();
			Assert.IsNotNull(lexFieldControl);
		}

		[Test]
		public void DataSource_ShowsAppleEntry()
		{
		   TestEntryShows("apple", "red thing", "An apple a day keeps the doctor away.");
		   TestEntryShows("banana", "yellow food", "Monkeys like to eat bananas.");
		}

		private static void TestEntryShows(string lexicalForm, string gloss, string exampleSentence)
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm["th"] = lexicalForm;
			LexSense sense = (LexSense)entry.Senses.AddNew();
			sense.Gloss["en"] = gloss;
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			example.Sentence["th"] = exampleSentence;

			LexFieldControl lexFieldControl = new LexFieldControl();
			lexFieldControl.DataSource = entry;
			Assert.IsTrue(lexFieldControl.Controls[0].Text.Contains(lexicalForm));
			Assert.IsTrue(lexFieldControl.Controls[0].Text.Contains(gloss));
			Assert.IsTrue(lexFieldControl.Controls[0].Text.Contains(exampleSentence));
		}

	}
}
