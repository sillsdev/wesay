
using System.Windows.Forms;
using NUnit.Framework;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Tests;
using WeSay.UI;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class LayouterTests
	{
		private int _rowCount;

		[SetUp]
		public void Setup()
		{
			BasilProject.InitializeForTests();
		}

		[Test]
		public void RightNumberOfRows()
		{
			MakeDetailList();
			Assert.AreEqual(14, _rowCount);
		}

		[Test]
		public void RowsInRightPlace()
		{
			DetailList dl = MakeDetailList();

			Control rowControl = dl.GetControlOfRow(0);
			Label l = (Label) dl.GetLabelControlFromReferenceControl(rowControl);
			Assert.AreEqual("Word", l.Text);
		}

		[Test]
		public void WordShownInVernacular()
		{
			DetailList dl = MakeDetailList();

			Control rowControl = dl.GetControlOfRow(0);

			WeSayTextBox box = (WeSayTextBox)dl.GetEditControlFromReferenceControl(rowControl);
			Assert.AreEqual("WordInVernacular", box.Text);
	  }

		private DetailList MakeDetailList()
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm[BasilProject.Project.VernacularWritingSystemDefault.Id] = "WordInVernacular";
			entry.LexicalForm[BasilProject.Project.AnalysisWritingSystemDefault.Id] = "WordInAnalysis";
			AddSense(entry);
			AddSense(entry);

			DetailList dl = new DetailList();
			LexEntryLayouter layout = new LexEntryLayouter(dl);
			_rowCount = layout.AddWidgets(entry);
			return dl;
		}

		private static void AddSense(LexEntry entry)
		{
			LexSense sense = (LexSense) entry.Senses.AddNew();
			sense.Gloss[BasilProject.Project.AnalysisWritingSystemDefault.Id] = "GlossInAnalysis";
			AddExample(sense);
			AddExample(sense);
		}

		private static void AddExample(LexSense sense)
		{
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Sentence[BasilProject.Project.VernacularWritingSystemDefault.Id] = "sentence";
		}
	}
}
