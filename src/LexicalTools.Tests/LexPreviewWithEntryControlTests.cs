using NUnit.Framework;
using WeSay.LexicalModel;
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
		private FieldInventory _fieldInventory;

		[SetUp]
		public void SetUp()
		{
			BasilProject.InitializeForTests();

			empty = CreateTestEntry("", "", "");
			apple = CreateTestEntry("apple", "red thing", "An apple a day keeps the doctor away.");
			banana = CreateTestEntry("banana", "yellow food", "Monkeys like to eat bananas.");
			car = CreateTestEntry("car", "small motorized vehicle", "Watch out for cars when you cross the street.");
			bike = CreateTestEntry("bike", "vehicle with two wheels", "He rides his bike to school.");

			string[] analysisWritingSystemIds = new string[] { BasilProject.Project.WritingSystems.AnalysisWritingSystemDefaultId };
			string[] vernacularWritingSystemIds = new string[] { BasilProject.Project.WritingSystems.VernacularWritingSystemDefaultId };
			this._fieldInventory = new FieldInventory();
			this._fieldInventory.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(), vernacularWritingSystemIds));
			this._fieldInventory.Add(new Field(Field.FieldNames.SenseGloss.ToString(), analysisWritingSystemIds));
			this._fieldInventory.Add(new Field(Field.FieldNames.ExampleSentence.ToString(), vernacularWritingSystemIds));
			this._fieldInventory.Add(new Field(Field.FieldNames.ExampleTranslation.ToString(), analysisWritingSystemIds));

		}

		[Test]
		public void CreateWithInventory()
		{
			LexPreviewWithEntryControl lexFieldControl = new LexPreviewWithEntryControl(_fieldInventory);
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
			LexPreviewWithEntryControl lexFieldControl = CreateFilteredForm(apple, Field.FieldNames.SenseGloss.ToString(), BasilProject.Project.WritingSystems.AnalysisWritingSystemDefaultId);
			Assert.AreEqual(1, lexFieldControl.ControlEntryDetail.Count);
		}

		[Test]
		public void EditField_SingleControlWithGhost()
		{
			LexPreviewWithEntryControl lexFieldControl = CreateFilteredForm(apple, Field.FieldNames.SenseGloss.ToString(), BasilProject.Project.WritingSystems.AnalysisWritingSystemDefaultId);
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
			LexPreviewWithEntryControl lexFieldControl = CreateFilteredForm(entry, Field.FieldNames.SenseGloss.ToString(), BasilProject.Project.WritingSystems.AnalysisWritingSystemDefaultId);
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
			LexPreviewWithEntryControl lexFieldControl = CreateFilteredForm(apple, Field.FieldNames.EntryLexicalForm .ToString(), BasilProject.Project.WritingSystems.VernacularWritingSystemDefaultId);
			DetailList entryDetailControl = lexFieldControl.ControlEntryDetail;
			Control referenceControl = entryDetailControl.GetControlOfRow(0);
			MultiTextControl editControl = (MultiTextControl)entryDetailControl.GetEditControlFromReferenceControl(referenceControl);
			editControl.TextBoxes[0].Text = "test";
			Assert.IsTrue(lexFieldControl.ControlFormattedView.Text.Contains("test"));
	   }

		[Test]
		public void FormattedView_FocusInControl_Displayed()
		{
			LexPreviewWithEntryControl lexFieldControl = CreateFilteredForm(apple, Field.FieldNames.EntryLexicalForm.ToString(), BasilProject.Project.WritingSystems.VernacularWritingSystemDefaultId);
			lexFieldControl.ControlFormattedView.Select();
			string rtfOriginal = lexFieldControl.ControlFormattedView.Rtf;

			DetailList entryDetailControl = lexFieldControl.ControlEntryDetail;
			Control referenceControl = entryDetailControl.GetControlOfRow(0);
			Control editControl = entryDetailControl.GetEditControlFromReferenceControl(referenceControl);

			//JDH added after we added multiple ws's per field. Was: editControl.Select();
			((MultiTextControl)editControl).TextBoxes[0].Select();

			Assert.AreNotEqual(rtfOriginal, lexFieldControl.ControlFormattedView.Rtf);
		}

		[Test]
		public void FormattedView_ChangeRecordThenBack_NothingHighlighted()
		{
			LexPreviewWithEntryControl lexFieldControl = CreateFilteredForm(apple, Field.FieldNames.EntryLexicalForm.ToString(), BasilProject.Project.WritingSystems.VernacularWritingSystemDefaultId);
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
			Assert.AreEqual(rtfAppleNothingHighlighted, lexFieldControl.ControlFormattedView.Rtf);
		}

		[Test]
		public void FormattedView_EmptyField_StillHighlighted()
		{
			LexPreviewWithEntryControl lexFieldControl = CreateFilteredForm(empty, Field.FieldNames.EntryLexicalForm.ToString(), BasilProject.Project.WritingSystems.VernacularWritingSystemDefaultId);
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
			LexPreviewWithEntryControl lexFieldControl = new LexPreviewWithEntryControl(_fieldInventory);
			lexFieldControl.DataSource = entry;

			return lexFieldControl;
		}


		private static LexPreviewWithEntryControl CreateFilteredForm(LexEntry entry, string field, params string[] writingSystems)
		{
			FieldInventory fieldInventory = new FieldInventory();
			fieldInventory.Add(new Field(field, writingSystems));
			LexPreviewWithEntryControl lexFieldControl = new LexPreviewWithEntryControl(fieldInventory);
			lexFieldControl.DataSource = entry;
			return lexFieldControl;
		}

		private static LexEntry CreateTestEntry(string lexicalForm, string gloss, string exampleSentence)
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm[BasilProject.Project.WritingSystems.VernacularWritingSystemDefault.Id] = lexicalForm;
			LexSense sense = (LexSense)entry.Senses.AddNew();
			sense.Gloss[BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault.Id] = gloss;
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			example.Sentence[BasilProject.Project.WritingSystems.VernacularWritingSystemDefault.Id] = exampleSentence;
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
