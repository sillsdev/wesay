using System.Diagnostics;
using NUnit.Framework;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;
using WeSay.UI;
using System.Windows.Forms;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class LexPreviewWithEntryControlTests
	{
		LexEntry empty;
		LexEntry apple;
		LexEntry banana;
		LexEntry car;
		LexEntry bike;
		private ViewTemplate _viewTemplate;

		[SetUp]
		public void SetUp()
		{
			Db4oLexModelHelper.InitializeForNonDbTests();
			WeSayWordsProject.InitializeForTests();

			empty = CreateTestEntry("", "", "");
			apple = CreateTestEntry("apple", "red thing", "An apple a day keeps the doctor away.");
			banana = CreateTestEntry("banana", "yellow food", "Monkeys like to eat bananas.");
			car = CreateTestEntry("car", "small motorized vehicle", "Watch out for cars when you cross the street.");
			bike = CreateTestEntry("bike", "vehicle with two wheels", "He rides his bike to school.");

			string[] analysisWritingSystemIds = new string[] { BasilProject.Project.WritingSystems.TestWritingSystemAnalId };
			string[] vernacularWritingSystemIds = new string[] { BasilProject.Project.WritingSystems.TestWritingSystemVernId };
			this._viewTemplate = new ViewTemplate();
			this._viewTemplate.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(), vernacularWritingSystemIds));
			this._viewTemplate.Add(new Field(Field.FieldNames.SenseGloss.ToString(), analysisWritingSystemIds));
			this._viewTemplate.Add(new Field(Field.FieldNames.ExampleSentence.ToString(), vernacularWritingSystemIds));
			this._viewTemplate.Add(new Field(Field.FieldNames.ExampleTranslation.ToString(), analysisWritingSystemIds));

		}

		[Test]
		public void CreateWithInventory()
		{
			LexPreviewWithEntryControl lexFieldControl = new LexPreviewWithEntryControl();
			Assert.IsNotNull(lexFieldControl);
		}

		[Test]
		public void NullDataSource_ShowsEmpty()
		{
			LexPreviewWithEntryControl lexFieldControl = CreateForm(null);
			Assert.AreEqual(string.Empty, lexFieldControl.ControlFormattedView.Text);
		}

		[Test]
		public void FormattedView_ShowsCurrentEntry()
		{
		   TestEntryShows(apple);
		   TestEntryShows(banana);
		}

		private void TestEntryShows(LexEntry entry)
		{
			LexPreviewWithEntryControl lexFieldControl = CreateForm(entry);
			Assert.IsTrue(lexFieldControl.ControlFormattedView.Text.Contains(GetLexicalForm(entry)));
			Assert.IsTrue(lexFieldControl.ControlFormattedView.Text.Contains(GetGloss(entry)));
			Assert.IsTrue(lexFieldControl.ControlFormattedView.Text.Contains(GetExampleSentence(entry)));
		}

		[Test, Ignore("For now, we also show the ghost field in this situation.")]
		public void EditField_SingleControl()
		{
			LexPreviewWithEntryControl lexFieldControl = CreateFilteredForm(apple, Field.FieldNames.SenseGloss.ToString(), BasilProject.Project.WritingSystems.TestWritingSystemAnalId);
			Assert.AreEqual(1, lexFieldControl.ControlEntryDetail.Count);
		}

		[Test]
		public void EditField_SingleControlWithGhost()
		{
			LexPreviewWithEntryControl lexFieldControl = CreateFilteredForm(apple, Field.FieldNames.SenseGloss.ToString(), BasilProject.Project.WritingSystems.TestWritingSystemAnalId);
			Assert.AreEqual(2, lexFieldControl.ControlEntryDetail.Count);
		}

		[Test]
		public void EditField_MapsToLexicalForm()
		{
			TestEditFieldMapsToLexicalForm(car);
			TestEditFieldMapsToLexicalForm(bike);
		}

		private static void TestEditFieldMapsToLexicalForm(LexEntry entry)
		{
			LexPreviewWithEntryControl lexFieldControl = CreateFilteredForm(entry, Field.FieldNames.SenseGloss.ToString(), BasilProject.Project.WritingSystems.TestWritingSystemAnalId);
			DetailList entryDetailControl = lexFieldControl.ControlEntryDetail;
			Control referenceControl = entryDetailControl.GetControlOfRow(0);
			Label labelControl = entryDetailControl.GetLabelControlFromReferenceControl(referenceControl);
			Assert.AreEqual("Meaning", labelControl.Text);
			MultiTextControl editControl = (MultiTextControl)entryDetailControl.GetEditControlFromReferenceControl(referenceControl);
			editControl.TextBoxes[0].Text = "test";
			Assert.IsTrue(editControl.TextBoxes[0].Text.Contains(GetGloss(entry)));
		}

		[Test]
		public void EditField_Change_DisplayedInFormattedView()
		{
			LexPreviewWithEntryControl lexFieldControl = CreateFilteredForm(apple, Field.FieldNames.EntryLexicalForm .ToString(), BasilProject.Project.WritingSystems.TestWritingSystemVernId);
			DetailList entryDetailControl = lexFieldControl.ControlEntryDetail;
			Control referenceControl = entryDetailControl.GetControlOfRow(0);
			MultiTextControl editControl = (MultiTextControl)entryDetailControl.GetEditControlFromReferenceControl(referenceControl);
			editControl.TextBoxes[0].Text = "test";
			Assert.IsTrue(lexFieldControl.ControlFormattedView.Text.Contains("test"));
	   }

		[Test]
		public void EditField_RemoveContents_RemovesSense()
		{
			LexEntry meaningOnly = CreateTestEntry("word", "meaning", "");
			LexPreviewWithEntryControl lexFieldControl = CreateForm(meaningOnly);
			DetailList detailList = lexFieldControl.ControlEntryDetail;
			MultiTextControl editControl = GetEditControl(detailList, "Meaning");
			editControl.TextBoxes[0].Text = "";

			Assert.IsNull(GetEditControl(detailList, "Meaning"));
		}

		private static MultiTextControl GetEditControl(DetailList detailList, string labelText) {
			MultiTextControl editControl = null;
			for (int i = 0; i < detailList.Count; i++)
			{
				Control referenceControl = detailList.GetControlOfRow(i);
				Label label = detailList.GetLabelControlFromReferenceControl(referenceControl);
				if(label.Text == labelText)
				{
					editControl = (MultiTextControl)detailList.GetEditControlFromReferenceControl(referenceControl);
					break;
				}
			}
			return editControl;
		}

		[Test]
		public void FormattedView_FocusInControl_Displayed()
		{
			LexPreviewWithEntryControl lexFieldControl = CreateFilteredForm(apple, Field.FieldNames.EntryLexicalForm.ToString(), BasilProject.Project.WritingSystems.TestWritingSystemVernId);
			lexFieldControl.ControlFormattedView.Select();
			string rtfOriginal = lexFieldControl.ControlFormattedView.Rtf;

			DetailList entryDetailControl = lexFieldControl.ControlEntryDetail;
			Control referenceControl = entryDetailControl.GetControlOfRow(0);
			Control editControl = entryDetailControl.GetEditControlFromReferenceControl(referenceControl);

			//JDH added after we added multiple ws's per field. Was: editControl.Select();
			((MultiTextControl)editControl).TextBoxes[0].Select();

			Assert.AreNotEqual(rtfOriginal, lexFieldControl.ControlFormattedView.Rtf);
		}

		[Test, Ignore("Not implemented yet.")]
		public void DoSomethingSensibleWhenWSInFieldWasntListedInProjectCollection()
		{
		}

		[Test]
		public void FormattedView_ChangeRecordThenBack_NothingHighlighted()
		{
			LexPreviewWithEntryControl lexFieldControl = CreateFilteredForm(apple, Field.FieldNames.EntryLexicalForm.ToString(), BasilProject.Project.WritingSystems.TestWritingSystemVernId);
			lexFieldControl.ControlFormattedView.Select();
			string rtfAppleNothingHighlighted = lexFieldControl.ControlFormattedView.Rtf;

			DetailList entryDetailControl = lexFieldControl.ControlEntryDetail;
			Control referenceControl = entryDetailControl.GetControlOfRow(0);
			Control editControl = entryDetailControl.GetEditControlFromReferenceControl(referenceControl);

			//JDH added after we added multiple ws's per field. Was: editControl.Select();
			((MultiTextControl)editControl).TextBoxes[0].Select();

			Assert.AreNotEqual(rtfAppleNothingHighlighted, lexFieldControl.ControlFormattedView.Rtf);

			lexFieldControl.DataSource = banana;
			lexFieldControl.DataSource = apple;
//            Debug.WriteLine("Expected: "+rtfAppleNothingHighlighted);
//            Debug.WriteLine("Actual:" + lexFieldControl.ControlFormattedView.Rtf);
			Assert.AreEqual(rtfAppleNothingHighlighted, lexFieldControl.ControlFormattedView.Rtf);
		}

		[Test]
		public void FormattedView_EmptyField_StillHighlighted()
		{
			LexPreviewWithEntryControl lexFieldControl = CreateFilteredForm(empty, Field.FieldNames.EntryLexicalForm.ToString(), BasilProject.Project.WritingSystems.TestWritingSystemVernId);
			lexFieldControl.ControlFormattedView.Select();
			string rtfEmptyNothingHighlighted = lexFieldControl.ControlFormattedView.Rtf;

			DetailList entryDetailControl = lexFieldControl.ControlEntryDetail;
			Control referenceControl = entryDetailControl.GetControlOfRow(0);
			Control editControl = entryDetailControl.GetEditControlFromReferenceControl(referenceControl);

			//JDH added after we added multiple ws's per field. Was: editControl.Select();
			((MultiTextControl)editControl).TextBoxes[0].Select();

			Assert.AreNotEqual(rtfEmptyNothingHighlighted, lexFieldControl.ControlFormattedView.Rtf);
		}

		private LexPreviewWithEntryControl CreateForm(LexEntry entry)
		{
			LexPreviewWithEntryControl lexFieldControl = new LexPreviewWithEntryControl();
			lexFieldControl.ViewTemplate = _viewTemplate;
			lexFieldControl.DataSource = entry;

			return lexFieldControl;
		}


		private static LexPreviewWithEntryControl CreateFilteredForm(LexEntry entry, string field, params string[] writingSystems)
		{
			ViewTemplate viewTemplate = new ViewTemplate();
			viewTemplate.Add(new Field(field, writingSystems));
			LexPreviewWithEntryControl lexFieldControl = new LexPreviewWithEntryControl();
			lexFieldControl.ViewTemplate = viewTemplate;
			lexFieldControl.DataSource = entry;
			return lexFieldControl;
		}

		private static LexEntry CreateTestEntry(string lexicalForm, string gloss, string exampleSentence)
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm[GetSomeValidWsIdForField("EntryLexicalForm")] = lexicalForm;
			LexSense sense = (LexSense)entry.Senses.AddNew();
			sense.Gloss[GetSomeValidWsIdForField("SenseGloss")] = gloss;
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			example.Sentence[GetSomeValidWsIdForField("ExampleSentence")] = exampleSentence;
			return entry;
		}

		private static string GetSomeValidWsIdForField(string fieldName)
		{
			return WeSay.Project.WeSayWordsProject.Project.ViewTemplate.GetField(fieldName).WritingSystemIds[0];
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
