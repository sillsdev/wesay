
using System;
using System.Windows.Forms;
using NUnit.Framework;
using WeSay.Foundation;
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
			WeSayWordsProject.InitializeForTests();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_NullBuilder_Throws()
		{
			new LexEntryLayouter(null, new ViewTemplate(), null);

		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_NullviewTemplate_Throws()
		{
			using (DetailList detailList = new DetailList())
			{
				new LexEntryLayouter(detailList, null, null);
			}
		}

		[Test]
		public void RightNumberOfRows()
		{
			using (DetailList dl = MakeDetailList(false))
			{
				Assert.AreEqual(14, _rowCount);
			}
		}

		[Test]
		public void RightNumberOfRowsWithShowAll()
		{
			using (DetailList dl = MakeDetailList(true))
			{
				Assert.AreEqual(16, _rowCount); //12 + 2 *(1 ghost example + 1 rare multitext)
			}
		}

		[Test]
		public void RowsInRightPlace()
		{
			using (DetailList dl = MakeDetailList(false))
			{
				Label l = dl.GetLabelControlFromRow(0);
				Assert.AreEqual("Word", l.Text);
			}
		}

		[Test]
		public void WordShownInVernacular()
		{
			using (DetailList dl = MakeDetailList(false))
			{
				MultiTextControl box = (MultiTextControl) dl.GetEditControlFromRow(0);
				Assert.AreEqual("WordInVernacular", box.TextBoxes[0].Text);
			}
		}

		[Test]
		public void EmptyViewTemplate()
		{
			LexEntry entry = GetNewEntry();

			using (DetailList dl = new DetailList())
			{
				LexEntryLayouter layout = new LexEntryLayouter(dl, new ViewTemplate(), null);
				_rowCount = layout.AddWidgets(entry);
				Assert.AreEqual(0, _rowCount);
			}
		}

		private static LexEntry GetNewEntry() {
			LexEntry entry = new LexEntry();
			entry.LexicalForm[BasilProject.Project.WritingSystems.TestWritingSystemVernId] = "WordInVernacular";
			entry.LexicalForm[BasilProject.Project.WritingSystems.TestWritingSystemAnalId] = "WordInAnalysis";
			AddSense(entry);
			AddSense(entry);
			return entry;
		}

		private DetailList MakeDetailList(bool showNormallyHiddenFields)
		{
			string[] analysisWritingSystemIds = new string[] { BasilProject.Project.WritingSystems.TestWritingSystemAnalId };
			string[] vernacularWritingSystemIds = new string[] { BasilProject.Project.WritingSystems.TestWritingSystemVernId };
			ViewTemplate viewTemplate = new ViewTemplate();
			Field field = new Field(Field.FieldNames.EntryLexicalForm.ToString(), "LexEntry", vernacularWritingSystemIds);
			field.DisplayName = "Word";
			viewTemplate.Add(field);
			viewTemplate.Add(new Field(Field.FieldNames.SenseGloss.ToString(), "LexSense", analysisWritingSystemIds));
			viewTemplate.Add(new Field(Field.FieldNames.ExampleSentence.ToString(), "LexExampleSentence", vernacularWritingSystemIds));
			viewTemplate.Add(new Field(Field.FieldNames.ExampleTranslation.ToString(), "LexExampleSentence", analysisWritingSystemIds));

			Field rare = new Field("rare", "LexSense", analysisWritingSystemIds);
			rare.Visibility = CommonEnumerations.VisibilitySetting.NormallyHidden;
			viewTemplate.Add(rare);

			//TODO need tests for other data types when made optional
			//TODO need tests for showing non-empty optional tests in non-show-all mode

			LexEntry entry = GetNewEntry();

			DetailList dl = new DetailList();
			LexEntryLayouter layout = new LexEntryLayouter(dl, viewTemplate, null);
			layout.ShowNormallyHiddenFields = showNormallyHiddenFields;
			_rowCount = layout.AddWidgets(entry);
			return dl;
		}

		private static void AddSense(LexEntry entry)
		{
			LexSense sense = (LexSense) entry.Senses.AddNew();
			sense.Gloss[WeSayWordsProject.Project.DefaultViewTemplate.GetField("SenseGloss").WritingSystemIds[0]] = "GlossInAnalysis";
			AddExample(sense);
			AddExample(sense);
		}

		private static void AddExample(LexSense sense)
		{
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Sentence[BasilProject.Project.WritingSystems.TestWritingSystemVernId] = "sentence";
		}
	}
}
