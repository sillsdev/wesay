using System;
using System.Windows.Forms;
using Autofac;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using SIL.DictionaryServices.Model;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.TestUtilities;
using WeSay.UI;
using WeSay.UI.TextBoxes;
using SIL.Lift;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class LayouterTests
	{
		private int _rowCount;
		private IServiceLocator Context{ get; set;}

		[OneTimeSetUp]
		public void FixtureSetup()
		{
			SIL.Windows.Forms.Keyboarding.KeyboardController.Initialize();
		}

		[OneTimeTearDown]
		public void FixtureTeardown()
		{
			SIL.Windows.Forms.Keyboarding.KeyboardController.Shutdown();
		}

		[SetUp]
		public void Setup()
		{
			WeSayProjectTestHelper.InitializeForTests();
			var b = new ContainerBuilder();
			b.Register(c => new MediaNamingHelper(new string[] {"en"}));
			b.Register<IWeSayTextBox>(c =>
			{
				var m = new WeSayTextBox();
				return m;
			});
			b.Register<IWeSayComboBox>(c =>
			{
				var m = new WeSayComboBox();
				return m;
			});
			b.Register<IWeSayListView>(c =>
			{
				var m = new WeSayListView();
				return m;
			});
			Context =   new WeSay.Project.ServiceLocatorAdapter(b.Build());
		}

		[TearDown]
		public void TearDown()
		{
			WeSayProjectTestHelper.CleanupForTests();
		}

		[Test]
		public void Create_NullBuilder_Throws()
		{
			Assert.Throws<ArgumentNullException>(() => new LexEntryLayouter(null, 0, new ViewTemplate(), null, Context, new LexEntry(), false, () => new TestConfirmDelete(), false));
		}

		[Test]
		public void Create_NullviewTemplate_Throws()
		{
			using (DetailList detailList = new DetailList())
			{
				Assert.Throws<ArgumentNullException>(() => new LexEntryLayouter(detailList, 0, null, null, Context, new LexEntry(), false, () => new TestConfirmDelete(), false));
			}
		}

		[Test]
		public void RightNumberOfRows()
		{
			using (MakeDetailList(false, false, MakeViewTemplate()))
			{
				Assert.AreEqual(14, _rowCount);
			}
		}

		[Test]
		public void RightNumberOfRowsWithMinorLabel()
		{
			using (MakeDetailList(false, true, MakeViewTemplate()))
			{
				Assert.AreEqual(16, _rowCount);
			}
		}

		//see: WS-1120 Add option to limit "add meanings" task to the ones that have a semantic domain
		//also: WS-639 (jonathan_coombs@sil.org) In Add meanings, don't show extra meaning slots just because a sense was created for the semantic domain
		[Test]
		public void DoWantGhosts_False_RightNumberOfRows()
		{
			var template = MakeViewTemplate();
			template.DoWantGhosts = false;
			using (MakeDetailList(false, false, template))
			{
				Assert.AreEqual(13, _rowCount);
			}
		}

		[Test]
		public void DoWantGhosts_False_RightNumberOfRows_WithMinorMeaningLabel()
		{
			var template = MakeViewTemplate();
			template.DoWantGhosts = false;
			using (MakeDetailList(false, true, template))
			{
				Assert.AreEqual(15, _rowCount);
			}
		}

		[Test]
		public void RightNumberOfRowsWithShowAll()
		{
			using (MakeDetailList(true, false, MakeViewTemplate()))
			{
				Assert.AreEqual(16, _rowCount); //12 + 2 *(1 ghost example + 1 rare multitext)
			}
		}

		[Test]
		public void RightNumberOfRowsWithShowAllWithMinorMeaningLabel()
		{
			using (MakeDetailList(true, true, MakeViewTemplate()))
			{
				Assert.AreEqual(18, _rowCount); //12 + 2 *(1 minor label + 1 ghost example + 1 rare multitext)
			}
		}

		[Test]
		public void RowsInRightPlace()
		{
			using (DetailList dl = MakeDetailList(false, false, MakeViewTemplate()))
			{
				Label l = dl.GetLabelControlFromRow(0);
				Assert.AreEqual("Word", l.Text);
			}
		}

		[Test]
		public void RowsInRightPlaceWithMinorMeaningLabel()
		{
			using (DetailList dl = MakeDetailList(false, true, MakeViewTemplate()))
			{
				Label l = dl.GetLabelControlFromRow(0);
				Assert.AreEqual("Word", l.Text);
			}
		}

		[Test]
		public void WordShownInVernacular()
		{
			using (DetailList dl = MakeDetailList(false, false, MakeViewTemplate()))
			{
				MultiTextControl box = (MultiTextControl) dl.GetEditControlFromRow(0);
				Assert.AreEqual("WordInVernacular", box.TextBoxes[0].Text);
			}
		}

		[Test]
		public void WordShownInVernacularWithMinorMeaningLabel()
		{
			using (DetailList dl = MakeDetailList(false, true, MakeViewTemplate()))
			{
				MultiTextControl box = (MultiTextControl)dl.GetEditControlFromRow(0);
				Assert.AreEqual("WordInVernacular", box.TextBoxes[0].Text);
			}
		}

		[Test]
		public void EmptyViewTemplate()
		{
			LexEntry entry = GetNewEntry();

			using (DetailList dl = new DetailList())
			{
				LexEntryLayouter layout = new LexEntryLayouter(dl, 0, new ViewTemplate(), null, Context, entry, false, () => new TestConfirmDelete(), false);
				layout.AddWidgets();
				Assert.AreEqual(0, dl.RowCount);
			}
		}

		[Test]
		public void EmptyViewTemplateWithMinorLabel()
		{
			LexEntry entry = GetNewEntry();

			using (DetailList dl = new DetailList())
			{
				LexEntryLayouter layout = new LexEntryLayouter(dl, 0, new ViewTemplate(), null, Context, entry, false, () => new TestConfirmDelete(), true);
				layout.AddWidgets();
				Assert.AreEqual(0, dl.RowCount);
			}
		}


		private static LexEntry GetNewEntry()
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm[WritingSystemsIdsForTests.VernacularIdForTest] =
					"WordInVernacular";
			entry.LexicalForm[WritingSystemsIdsForTests.AnalysisIdForTest] =
					"WordInAnalysis";
			AddSense(entry);
			AddSense(entry);
			return entry;
		}

		private DetailList MakeDetailList(bool showNormallyHiddenFields, bool showMinorMeaningLabel, ViewTemplate template)
		{
			//TODO need tests for other data types when made optional
			//TODO need tests for showing non-empty optional tests in non-show-all mode

			LexEntry entry = GetNewEntry();

			DetailList dl = new DetailList();
			LexEntryLayouter layout = new LexEntryLayouter(dl, 0, template, null, Context, entry, false, () => new TestConfirmDelete(), showMinorMeaningLabel);
			layout.ShowNormallyHiddenFields = showNormallyHiddenFields;
			layout.AddWidgets();
			_rowCount = dl.RowCount;
			return dl;
		}

		private ViewTemplate MakeViewTemplate()
		{
			string[] analysisWritingSystemIds = new string[]
													{
															WritingSystemsIdsForTests.AnalysisIdForTest
													};
			string[] vernacularWritingSystemIds = new string[]
													  {
															  WritingSystemsIdsForTests.VernacularIdForTest
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
			example.Sentence[WritingSystemsIdsForTests.VernacularIdForTest] = "sentence";
		}
	}
}
