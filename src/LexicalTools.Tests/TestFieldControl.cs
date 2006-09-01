using NUnit.Framework;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Tests;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class TestFieldControl
	{
		LexEntry apple;
		LexEntry banana;
		LexEntry car;
		LexEntry bike;

		[SetUp]
		public void SetUp()
		{
			apple = CreateTestEntry("apple", "red thing", "An apple a day keeps the doctor away.");
			banana = CreateTestEntry("banana", "yellow food", "Monkeys like to eat bananas.");
			car = CreateTestEntry("car", "small motorized vehicle", "Watch out for cars when you cross the street.");
			bike = CreateTestEntry("bike", "vehicle with two wheels", "He rides his bike to school.");
		}

		[Test]
		public void Create()
		{
			LexFieldControl lexFieldControl = new LexFieldControl();
			Assert.IsNotNull(lexFieldControl);
		}

		[Test]
		public void DataSource_ShowsCurrentEntry()
		{
		   TestEntryShows(apple);
		   TestEntryShows(banana);
		}

		[Test]
		public void EditField_SingleControl()
		{
			TestSingleControl(apple);
			TestSingleControl(banana);
		}

		private void TestSingleControl(LexEntry entry)
		{
			LexFieldControl lexFieldControl = new LexFieldControl();
			lexFieldControl.DataSource = entry;

			Assert.AreEqual(1, lexFieldControl.Control_EntryDetail.Count);
		}


		[Test]
		public void DataSource_EditField_MapsToLexicalForm()
		{
			TestEditFieldMapsToLexicalForm(car);
			TestEditFieldMapsToLexicalForm(bike);
		}

		private static void TestEditFieldMapsToLexicalForm(LexEntry entry)
		{
			LexFieldControl lexFieldControl = CreateForm(entry);
			//Assert.IsTrue(lexFieldControl.Control_EntryDetail.Contains(GetLexicalForm(entry)));
		}

		private static void TestEntryShows(LexEntry entry)
		{
			LexFieldControl lexFieldControl = CreateForm(entry);
			Assert.IsTrue(lexFieldControl.Control_DataView.Contains(GetLexicalForm(entry)));
			Assert.IsTrue(lexFieldControl.Control_DataView.Contains(GetGloss(entry)));
			Assert.IsTrue(lexFieldControl.Control_DataView.Contains(GetExampleSentence(entry)));
		}

		private static LexFieldControl CreateForm(LexEntry entry)
		{
			LexFieldControl lexFieldControl = new LexFieldControl();
			lexFieldControl.DataSource = entry;

			return lexFieldControl;
		}

		private static LexEntry CreateTestEntry(string lexicalForm, string gloss, string exampleSentence)
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm["th"] = lexicalForm;
			LexSense sense = (LexSense)entry.Senses.AddNew();
			sense.Gloss["en"] = gloss;
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			example.Sentence["th"] = exampleSentence;
			return entry;
		}

		private static string GetLexicalForm(LexEntry entry)
		{
			return entry.LexicalForm.GetFirstAlternative();
		}

		private static string GetGloss(LexEntry entry)
		{
			return ((LexSense)entry.Senses[0]).Gloss.GetFirstAlternative();
		}

		private static string GetExampleSentence(LexEntry entry)
		{
			return ((LexExampleSentence)((LexSense)entry.Senses[0]).ExampleSentences[0]).Sentence.GetFirstAlternative();
		}

	}
}
