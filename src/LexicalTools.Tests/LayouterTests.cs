using System;
using System.Windows.Forms;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.UI;
using WeSay.UI.TextBoxes;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class LayouterTests
	{
		private int _rowCount;

		[SetUp]
		public void Setup()
		{
			WeSayWordsProject.InitializeForTests();
		}
		private IServiceLocator Context
		{
			get { return null;}
		}
		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_NullBuilder_Throws()
		{
			new LexEntryLayouter(null, new ViewTemplate(), null, Context, new LexEntry());
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_NullviewTemplate_Throws()
		{
			using (DetailList detailList = new DetailList())
			{
				new LexEntryLayouter(detailList, null, null, Context, new LexEntry());
			}
		}

		[Test]
		public void RightNumberOfRows()
		{
			using (MakeDetailList(false, MakeViewTemplate()))
			{
				Assert.AreEqual(14, _rowCount);
			}
		}

		//see: WS-1120 Add option to limit "add meanings" task to the ones that have a semantic domain
		//also: WS-639 (jonathan_coombs@sil.org) In Add meanings, don't show extra meaning slots just because a sense was created for the semantic domain
		[Test]
		public void DoWantGhosts_False_RightNumberOfRows()
		{
			var template = MakeViewTemplate();
			template.DoWantGhosts = false;
			using (MakeDetailList(false, template))
			{
				Assert.AreEqual(13, _rowCount);
			}
		}

		[Test]
		public void RightNumberOfRowsWithShowAll()
		{
			using (MakeDetailList(true, MakeViewTemplate()))
			{
				Assert.AreEqual(16, _rowCount); //12 + 2 *(1 ghost example + 1 rare multitext)
			}
		}

		[Test]
		public void RowsInRightPlace()
		{
			using (DetailList dl = MakeDetailList(false, MakeViewTemplate()))
			{
				Label l = dl.GetLabelControlFromRow(0);
				Assert.AreEqual("Word", l.Text);
			}
		}

		[Test]
		public void WordShownInVernacular()
		{
			using (DetailList dl = MakeDetailList(false, MakeViewTemplate()))
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
				LexEntryLayouter layout = new LexEntryLayouter(dl, new ViewTemplate(), null, Context, entry);
				_rowCount = layout.AddWidgets();
				Assert.AreEqual(0, _rowCount);
			}
		}

		private static LexEntry GetNewEntry()
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm[BasilProject.Project.WritingSystems.TestWritingSystemVernId] =
					"WordInVernacular";
			entry.LexicalForm[BasilProject.Project.WritingSystems.TestWritingSystemAnalId] =
					"WordInAnalysis";
			AddSense(entry);
			AddSense(entry);
			return entry;
		}

		private DetailList MakeDetailList(bool showNormallyHiddenFields, ViewTemplate template)
		{
			//TODO need tests for other data types when made optional
			//TODO need tests for showing non-empty optional tests in non-show-all mode

			LexEntry entry = GetNewEntry();

			DetailList dl = new DetailList();
			LexEntryLayouter layout = new LexEntryLayouter(dl, template, null, Context, entry);
			layout.ShowNormallyHiddenFields = showNormallyHiddenFields;
			_rowCount = layout.AddWidgets();
			return dl;
		}

		private ViewTemplate MakeViewTemplate()
		{
			string[] analysisWritingSystemIds = new string[]
													{
															BasilProject.Project.WritingSystems.
																	TestWritingSystemAnalId
													};
			string[] vernacularWritingSystemIds = new string[]
													  {
															  BasilProject.Project.WritingSystems.
																	  TestWritingSystemVernId
													  };
			ViewTemplate viewTemplate = new ViewTemplate();
			Field field = new Field(Field.FieldNames.EntryLexicalForm.ToString(),
									"LexEntry",
									vernacularWritingSystemIds);
			field.DisplayName = "Word";
			viewTemplate.Add(field);
#if GlossMeaning
			string meaningFieldName = Field.FieldNames.SenseGloss.ToString();
#else
			string meaningFieldName = LexSense.WellKnownProperties.Definition;
#endif
			viewTemplate.Add(new Field(meaningFieldName, "LexSense", analysisWritingSystemIds));
			viewTemplate.Add(new Field(Field.FieldNames.ExampleSentence.ToString(),
									   "LexExampleSentence",
									   vernacularWritingSystemIds));
			viewTemplate.Add(new Field(Field.FieldNames.ExampleTranslation.ToString(),
									   "LexExampleSentence",
									   analysisWritingSystemIds));

			Field rare = new Field("rare", "LexSense", analysisWritingSystemIds);
			rare.Visibility = CommonEnumerations.VisibilitySetting.NormallyHidden;
			viewTemplate.Add(rare);
			return viewTemplate;
		}

		private static void AddSense(LexEntry entry)
		{
			LexSense sense = new LexSense();
			entry.Senses.Add(sense);
#if GlossMeaning
			sense.Gloss[WeSayWordsProject.Project.DefaultViewTemplate.GetField("SenseGloss").WritingSystemIds[0]] = "GlossInAnalysis";
#else
			sense.Definition[
					WeSayWordsProject.Project.DefaultViewTemplate.GetField(
							LexSense.WellKnownProperties.Definition).WritingSystemIds[0]] =
					"MeaningInAnalysis";
#endif
			AddExample(sense);
			AddExample(sense);
		}

		private static void AddExample(LexSense sense)
		{
			LexExampleSentence example = new LexExampleSentence();
			sense.ExampleSentences.Add(example);
			example.Sentence[BasilProject.Project.WritingSystems.TestWritingSystemVernId] =
					"sentence";
		}
	}
}