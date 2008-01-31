using System.Threading;
using NUnit.Framework;
using WeSay.Foundation.Options;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;
using WeSay.UI;
using System.Windows.Forms;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class EntryViewControlTests
	{
		LexEntry empty;
		LexEntry apple;
		LexEntry banana;
		LexEntry car;
		LexEntry bike;
		private ViewTemplate _viewTemplate;
		private string _primaryMeaningFieldName;

		[SetUp]
		public void SetUp()
		{
			Db4oLexModelHelper.InitializeForNonDbTests();
			WeSayWordsProject.InitializeForTests();


#if GlossMeaning
		   _primaryMeaningFieldName = Field.FieldNames.SenseGloss.ToString();
#else
			_primaryMeaningFieldName = LexSense.WellKnownProperties.Definition;
#endif

			string[] analysisWritingSystemIds = new string[] { BasilProject.Project.WritingSystems.TestWritingSystemAnalId };
			string[] vernacularWritingSystemIds = new string[] { BasilProject.Project.WritingSystems.TestWritingSystemVernId };
			this._viewTemplate = new ViewTemplate();
			this._viewTemplate.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(), "LexEntry", vernacularWritingSystemIds));
			 this._viewTemplate.Add(new Field(_primaryMeaningFieldName, "LexSense", analysisWritingSystemIds));
			this._viewTemplate.Add(new Field(Field.FieldNames.ExampleSentence.ToString(), "LexExampleSentence", vernacularWritingSystemIds));
			this._viewTemplate.Add(new Field(Field.FieldNames.ExampleTranslation.ToString(), "LexExampleSentence", analysisWritingSystemIds));

			empty = CreateTestEntry("", "", "");
			apple = CreateTestEntry("apple", "red thing", "An apple a day keeps the doctor away.");
			banana = CreateTestEntry("banana", "yellow food", "Monkeys like to eat bananas.");
			car = CreateTestEntry("car", "small motorized vehicle", "Watch out for cars when you cross the street.");
			bike = CreateTestEntry("bike", "vehicle with two wheels", "He rides his bike to school.");

		}

		[Test]
		public void CreateWithInventory()
		{
			using (EntryViewControl entryViewControl = new EntryViewControl())
			{
				Assert.IsNotNull(entryViewControl);
			}
		}

		[Test]
		public void NullDataSource_ShowsEmpty()
		{
			using (EntryViewControl entryViewControl = CreateForm(null))
			{
				Assert.AreEqual(string.Empty, entryViewControl.ControlFormattedView.Text);
			}
		}

		[Test]
		public void FormattedView_ShowsCurrentEntry()
		{
		   TestEntryShows(apple);
		   TestEntryShows(banana);
		}

		[Test]
		public void FormattedView_ShowsPartOfSpeech()
		{
			LexSense sense = (LexSense) apple.Senses[0];
			OptionRef o;
			o = sense.GetOrCreateProperty<OptionRef>("POS");
			o.Value = "noun";//nb: this is the key, which for noun happens to be the English display name tested for below
			using (EntryViewControl entryViewControl = CreateForm(apple))
			{
				Assert.IsTrue(entryViewControl.ControlFormattedView.Text.Contains("noun"));
				Assert.IsFalse(entryViewControl.ControlFormattedView.Text.Contains("nombre"));
			}
		}

		private void TestEntryShows(LexEntry entry)
		{
			using (EntryViewControl entryViewControl = CreateForm(entry))
			{
				Assert.IsTrue(entryViewControl.ControlFormattedView.Text.Contains(GetLexicalForm(entry)));
				Assert.IsTrue(entryViewControl.ControlFormattedView.Text.Contains(GetMeaning(entry)));
				Assert.IsTrue(entryViewControl.ControlFormattedView.Text.Contains(GetExampleSentence(entry)));
			}
		}

		[Test, Ignore("For now, we also show the ghost field in this situation.")]
		public void EditField_SingleControl()
		{


			using (EntryViewControl entryViewControl = CreateFilteredForm(apple, _primaryMeaningFieldName, "LexSense", BasilProject.Project.WritingSystems.TestWritingSystemAnalId))
			{
				Assert.AreEqual(1, entryViewControl.ControlEntryDetail.Count);
			}
		}

		[Test]
		public void EditField_SingleControlWithGhost()
		{
			using (EntryViewControl entryViewControl = CreateFilteredForm(apple, _primaryMeaningFieldName, "LexSense", BasilProject.Project.WritingSystems.TestWritingSystemAnalId))
			{
				Assert.AreEqual(2, entryViewControl.ControlEntryDetail.Count);
			}
		}

		[Test]
		public void EditField_Change_UpdatesSenseMeaning()
		{
			EnsureField_Change_UpdatesSenseMeaning(car);
			EnsureField_Change_UpdatesSenseMeaning(bike);
		}

		private  void EnsureField_Change_UpdatesSenseMeaning(LexEntry entry)
		{
			using (EntryViewControl entryViewControl = CreateFilteredForm(entry, _primaryMeaningFieldName, "LexSense", BasilProject.Project.WritingSystems.TestWritingSystemAnalId))
			{
				DetailList entryDetailControl = entryViewControl.ControlEntryDetail;
				Label labelControl = entryDetailControl.GetLabelControlFromRow(0);
				Assert.AreEqual("Meaning 1", labelControl.Text);
				MultiTextControl editControl = (MultiTextControl) entryDetailControl.GetEditControlFromRow(0);
				editControl.TextBoxes[0].Text = "test";
				Assert.IsTrue(editControl.TextBoxes[0].Text.Contains(GetMeaning(entry)));
			}
		}

		[Test]
		public void EditField_Change_DisplayedInFormattedView()
		{
			using (EntryViewControl entryViewControl = CreateFilteredForm(apple, Field.FieldNames.EntryLexicalForm.ToString(), "LexEntry", BasilProject.Project.WritingSystems.TestWritingSystemVernId))
			{
				DetailList entryDetailControl = entryViewControl.ControlEntryDetail;
				MultiTextControl editControl = (MultiTextControl) entryDetailControl.GetEditControlFromRow(0);
				editControl.TextBoxes[0].Text = "test";
				Assert.IsTrue(entryViewControl.ControlFormattedView.Text.Contains("test"));
			}
		}

		[Test]
		public void EditField_RemoveContents_RemovesSense()
		{
			LexEntry meaningOnly = CreateTestEntry("word", "meaning", "");
			using (EntryViewControl entryViewControl = CreateForm(meaningOnly))
			{
				MultiTextControl editControl = GetEditControl(entryViewControl.ControlEntryDetail, "Meaning 1");
				editControl.TextBoxes[0].Text = "";
				Thread.Sleep(1000);
				Application.DoEvents();

				Assert.IsTrue(GetEditControl(entryViewControl.ControlEntryDetail, "Meaning").Name.Contains("ghost"), "Only ghost should remain");
			}
		}

		private static MultiTextControl GetEditControl(DetailList detailList, string labelText) {
			MultiTextControl editControl = null;
			for (int i = 0; i < detailList.Count; i++)
			{
				Label label = detailList.GetLabelControlFromRow(i);
				if(label.Text == labelText)
				{
					editControl = (MultiTextControl)detailList.GetEditControlFromRow(i);
					break;
				}
			}
			return editControl;
		}

		[Test]
		public void FormattedView_FocusInControl_Displayed()
		{
			using (
				EntryViewControl entryViewControl =
					CreateFilteredForm(apple, Field.FieldNames.EntryLexicalForm.ToString(), "LexEntry",
									   BasilProject.Project.WritingSystems.TestWritingSystemVernId))
			{
				entryViewControl.ControlFormattedView.Select();
				string rtfOriginal = entryViewControl.ControlFormattedView.Rtf;

				DetailList entryDetailControl = entryViewControl.ControlEntryDetail;
				Control editControl = entryDetailControl.GetEditControlFromRow(0);

				//JDH added after we added multiple ws's per field. Was: editControl.Select();
				((MultiTextControl) editControl).TextBoxes[0].Select();

				Assert.AreNotEqual(rtfOriginal, entryViewControl.ControlFormattedView.Rtf);
			}
		}

		[Test, Ignore("Not implemented yet.")]
		public void DoSomethingSensibleWhenWSInFieldWasntListedInProjectCollection()
		{
		}

		[Test]
		public void FormattedView_ChangeRecordThenBack_NothingHighlighted()
		{
			using (EntryViewControl entryViewControl = CreateFilteredForm(apple, Field.FieldNames.EntryLexicalForm.ToString(), "LexEntry", BasilProject.Project.WritingSystems.TestWritingSystemVernId))
			{
				entryViewControl.ControlFormattedView.Select();
				string rtfAppleNothingHighlighted = entryViewControl.ControlFormattedView.Rtf;

				DetailList entryDetailControl = entryViewControl.ControlEntryDetail;
				Control editControl = entryDetailControl.GetEditControlFromRow(0);

				//JDH added after we added multiple ws's per field. Was: editControl.Select();
				((MultiTextControl) editControl).TextBoxes[0].Select();

				Assert.AreNotEqual(rtfAppleNothingHighlighted, entryViewControl.ControlFormattedView.Rtf);

				entryViewControl.DataSource = banana;
				entryViewControl.DataSource = apple;
				//            Debug.WriteLine("Expected: "+rtfAppleNothingHighlighted);
				//            Debug.WriteLine("Actual:" + lexFieldControl.ControlFormattedView.Rtf);
				Assert.AreEqual(rtfAppleNothingHighlighted, entryViewControl.ControlFormattedView.Rtf);
			}
		}

		[Test]
		public void FormattedView_EmptyField_StillHighlighted()
		{
			using (EntryViewControl entryViewControl = CreateFilteredForm(empty, Field.FieldNames.EntryLexicalForm.ToString(), "LexEntry", BasilProject.Project.WritingSystems.TestWritingSystemVernId))
			{
				entryViewControl.ControlFormattedView.Select();
				string rtfEmptyNothingHighlighted = entryViewControl.ControlFormattedView.Rtf;

				DetailList entryDetailControl = entryViewControl.ControlEntryDetail;
				Control editControl = entryDetailControl.GetEditControlFromRow(0);

				//JDH added after we added multiple ws's per field. Was: editControl.Select();
				((MultiTextControl) editControl).TextBoxes[0].Select();

				Assert.AreNotEqual(rtfEmptyNothingHighlighted, entryViewControl.ControlFormattedView.Rtf);
			}
		}

		private EntryViewControl CreateForm(LexEntry entry)
		{
			EntryViewControl entryViewControl = new EntryViewControl();
			entryViewControl.ViewTemplate = _viewTemplate;
			entryViewControl.DataSource = entry;

			return entryViewControl;
		}


		private static EntryViewControl CreateFilteredForm(LexEntry entry, string field, string className, params string[] writingSystems)
		{
			ViewTemplate viewTemplate = new ViewTemplate();
			viewTemplate.Add(new Field(field, className, writingSystems));
			EntryViewControl entryViewControl = new EntryViewControl();
			entryViewControl.ViewTemplate = viewTemplate;
			entryViewControl.DataSource = entry;
			return entryViewControl;
		}

		private static LexEntry CreateTestEntry(string lexicalForm, string meaning, string exampleSentence)
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm[GetSomeValidWsIdForField("EntryLexicalForm")] = lexicalForm;
			LexSense sense = (LexSense)entry.Senses.AddNew();
#if GlossMeaning
			sense.Gloss[GetSomeValidWsIdForField("SenseGloss")] = meaning;
#else
			sense.Definition[WeSayWordsProject.Project.WritingSystems.TestWritingSystemAnalId] = meaning;
#endif
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			example.Sentence[GetSomeValidWsIdForField("ExampleSentence")] = exampleSentence;
			return entry;
		}

		private static string GetSomeValidWsIdForField(string fieldName)
		{
			return WeSayWordsProject.Project.DefaultViewTemplate.GetField(fieldName).WritingSystemIds[0];
		}

		private static string GetLexicalForm(LexEntry entry)
		{
			return entry.LexicalForm.GetFirstAlternative();
		}

		private static string GetMeaning(LexEntry entry)
		{
 #if GlossMeaning
			return ((LexSense)entry.Senses[0]).Gloss.GetFirstAlternative();
#else
			return ((LexSense)entry.Senses[0]).Definition.GetFirstAlternative();
#endif
		}

		private static string GetExampleSentence(LexEntry entry)
		{
			return ((LexExampleSentence)((LexSense)entry.Senses[0]).ExampleSentences[0]).Sentence.GetFirstAlternative();
		}
	}
}
