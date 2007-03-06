
using System;
using System.Windows.Forms;
using NUnit.Framework;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;
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
			Db4oLexModelHelper.InitializeForNonDbTests();
			WeSay.Project.WeSayWordsProject.InitializeForTests();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_NullBuilder_Throws()
		{
			new LexEntryLayouter(null, new ViewTemplate());

		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_NullviewTemplate_Throws()
		{
			new LexEntryLayouter(new DetailList(), null);
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
			Label l = dl.GetLabelControlFromReferenceControl(rowControl);
			Assert.AreEqual("Word", l.Text);
		}

		[Test]
		public void WordShownInVernacular()
		{
			DetailList dl = MakeDetailList();

			Control rowControl = dl.GetControlOfRow(0);

			MultiTextControl box = (MultiTextControl)dl.GetEditControlFromReferenceControl(rowControl);
			Assert.AreEqual("WordInVernacular", box.TextBoxes[0].Text);
	  }

		[Test]
		public void EmptyViewTemplate()
		{
			LexEntry entry = GetNewEntry();

			DetailList dl = new DetailList();
			LexEntryLayouter layout = new LexEntryLayouter(dl, new ViewTemplate());
			_rowCount = layout.AddWidgets(entry);
			Assert.AreEqual(0, _rowCount);
		}

		private static LexEntry GetNewEntry() {
			LexEntry entry = new LexEntry();
			entry.LexicalForm[BasilProject.Project.WritingSystems.TestWritingSystemVernId] = "WordInVernacular";
			entry.LexicalForm[BasilProject.Project.WritingSystems.TestWritingSystemAnalId] = "WordInAnalysis";
			AddSense(entry);
			AddSense(entry);
			return entry;
		}

		private DetailList MakeDetailList()
		{
			string[] analysisWritingSystemIds = new string[] { BasilProject.Project.WritingSystems.TestWritingSystemAnalId };
			string[] vernacularWritingSystemIds = new string[] { BasilProject.Project.WritingSystems.TestWritingSystemVernId };
			ViewTemplate viewTemplate = new ViewTemplate();
			Field field = new Field(Field.FieldNames.EntryLexicalForm.ToString(), vernacularWritingSystemIds);
			field.DisplayName = "Word";
			viewTemplate.Add(field);
			viewTemplate.Add(new Field(Field.FieldNames.SenseGloss.ToString(), analysisWritingSystemIds));
			viewTemplate.Add(new Field(Field.FieldNames.ExampleSentence.ToString(), vernacularWritingSystemIds));
			viewTemplate.Add(new Field(Field.FieldNames.ExampleTranslation.ToString(), analysisWritingSystemIds));

			LexEntry entry = GetNewEntry();

			DetailList dl = new DetailList();
			LexEntryLayouter layout = new LexEntryLayouter(dl, viewTemplate);
			_rowCount = layout.AddWidgets(entry);
			return dl;
		}

		private static void AddSense(LexEntry entry)
		{
			LexSense sense = (LexSense) entry.Senses.AddNew();
			sense.Gloss[WeSay.Project.WeSayWordsProject.Project.ViewTemplate.GetField("SenseGloss").WritingSystemIds[0]] = "GlossInAnalysis";
			AddExample(sense);
			AddExample(sense);
		}

		private static void AddExample(LexSense sense)
		{
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Sentence[BasilProject.Project.WritingSystems.TestGetWritingSystemVern.Id] = "sentence";
		}
	}
}
