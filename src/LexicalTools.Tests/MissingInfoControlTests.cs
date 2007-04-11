using System;
using NUnit.Framework;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class MissingInfoControlTests
	{
		InMemoryRecordListManager _recordListManager;
		IRecordList<LexEntry> _missingTranslationRecordList;
		ViewTemplate _viewTemplate;
		private MissingTranslationFilter _missingTranslation;

		public class MissingTranslationFilter:IFilter<LexEntry>
		{

			private bool IsMissingTranslation(LexEntry entry)
			{
				if (entry == null)
				{
					return false;
				}
				bool hasSense = false;
				bool hasExample = false;
				foreach (LexSense sense in entry.Senses)
				{
					hasSense = true;
					foreach (LexExampleSentence exampleSentence in sense.ExampleSentences)
					{
						hasExample = true;
						if (exampleSentence.Translation[BasilProject.Project.WritingSystems.TestWritingSystemAnalId].Length == 0)
						{
							return true;
						}
					}
				}
				return !(hasSense && hasExample);
			}

			public Predicate<LexEntry> FilteringPredicate
			{
				get
				{
					return IsMissingTranslation;
				}
			}

			public string Key
			{
				get
				{
					return "MissingTranslationFilter";
				}
			}
		}
		[SetUp]
		public void SetUp()
		{
			Db4oLexModelHelper.InitializeForNonDbTests();
			BasilProject.InitializeForTests();
			_recordListManager = new InMemoryRecordListManager();
			this._missingTranslation = new MissingTranslationFilter();
			_recordListManager.Register(this._missingTranslation);
			_missingTranslationRecordList = _recordListManager.GetListOfTypeFilteredFurther<LexEntry>(this._missingTranslation);
			_missingTranslationRecordList.Add(CreateTestEntry("apple", "red thing", "An apple a day keeps the doctor away."));
			_missingTranslationRecordList.Add(CreateTestEntry("banana", "yellow food", "Monkeys like to eat bananas."));
			_missingTranslationRecordList.Add(CreateTestEntry("car", "small motorized vehicle", "Watch out for cars when you cross the street."));
			_missingTranslationRecordList.Add(CreateTestEntry("bike", "vehicle with two wheels", "He rides his bike to school."));

			string[] analysisWritingSystemIds = new string[] { BasilProject.Project.WritingSystems.TestWritingSystemAnalId };
			string[] vernacularWritingSystemIds = new string[] { BasilProject.Project.WritingSystems.TestWritingSystemVernId };
			this._viewTemplate = new ViewTemplate();
			this._viewTemplate.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(), "LexEntry",vernacularWritingSystemIds));
			this._viewTemplate.Add(new Field(Field.FieldNames.SenseGloss.ToString(), "LexSense",analysisWritingSystemIds));
			this._viewTemplate.Add(new Field(Field.FieldNames.ExampleSentence.ToString(), "LexExampleSentence",vernacularWritingSystemIds));
			this._viewTemplate.Add(new Field(Field.FieldNames.ExampleTranslation.ToString(), "LexExampleSentence",analysisWritingSystemIds));

		}

		private static LexEntry CreateTestEntry(string lexicalForm, string gloss, string exampleSentence)
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm[BasilProject.Project.WritingSystems.TestWritingSystemVernId] = lexicalForm;
			LexSense sense = (LexSense)entry.Senses.AddNew();
			sense.Gloss[BasilProject.Project.WritingSystems.TestWritingSystemAnalId] = gloss;
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			example.Sentence[BasilProject.Project.WritingSystems.TestWritingSystemVernId] = exampleSentence;
			return entry;
		}

		private static void AddTranslationToEntry(LexEntry entry, string translation)
		{
			LexSense sense = (LexSense)entry.Senses[0];
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences[0];
			example.Translation[BasilProject.Project.WritingSystems.TestWritingSystemAnalId] = translation;
		}

		[TearDown]
		public void TearDown()
		{
			_recordListManager.Dispose();
		}


		[Test]
		public void Create()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate);
			Assert.IsNotNull(missingInfoControl);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_NullRecords_Throws()
		{
			new MissingInfoControl(null, _viewTemplate, _missingTranslation.FilteringPredicate);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_NullviewTemplate_Throws()
		{
			new MissingInfoControl(_missingTranslationRecordList, null, _missingTranslation.FilteringPredicate);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_NullFilter_Throws()
		{
			new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, null);
		}

		[Test]
		public void CurrentRecord_InitializedToFirst()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate);
			Assert.AreEqual(_missingTranslationRecordList[0], missingInfoControl.CurrentRecord);
		}

		[Test]
		public void SetCurrentRecordToPrevious_AtFirst_StaysAtFirst()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate);
			missingInfoControl.SetCurrentRecordToPrevious();
			Assert.AreEqual(_missingTranslationRecordList[0], missingInfoControl.CurrentRecord);
		}

		[Test]
		public void SetCurrentRecordToNextThenPrevious_SamePlace()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate);
			missingInfoControl.SetCurrentRecordToNext();
			missingInfoControl.SetCurrentRecordToPrevious();
			Assert.AreEqual(_missingTranslationRecordList[0], missingInfoControl.CurrentRecord);
		}

		[Test]
		public void SetCurrentRecordToNext_AtLast_StaysAtLast()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate);
			int count = _missingTranslationRecordList.Count;
			for (int i = 0; i <= count; i++)
			{
				missingInfoControl.SetCurrentRecordToNext();
			}
			Assert.AreEqual(_missingTranslationRecordList[count - 1], missingInfoControl.CurrentRecord);
		}

		[Test]
		public void SetCurrentRecordToNext_GoesToNext()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate);
			missingInfoControl.SetCurrentRecordToNext();
			Assert.AreEqual(_missingTranslationRecordList[1], missingInfoControl.CurrentRecord);
		}

		[Test]
		public void SetCurrentRecordToPrevious_AfterChangedSoNoLongerMeetsFilter_StaysAtFirst()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate);
			AddTranslationToEntry(missingInfoControl.CurrentRecord,"a bogus translation of example");
			missingInfoControl.SetCurrentRecordToPrevious();
			Assert.AreEqual(_missingTranslationRecordList[0], missingInfoControl.CurrentRecord);
		}

		[Test]
		public void SetCurrentRecordToPrevious_AfterChangedSoNoLongerMeetsFilter_GoesToEntryBeforeChangedOne()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate);
			missingInfoControl.SetCurrentRecordToNext();
			missingInfoControl.SetCurrentRecordToNext();
			AddTranslationToEntry(missingInfoControl.CurrentRecord, "a bogus translation of example");
			missingInfoControl.SetCurrentRecordToPrevious();
			Assert.AreEqual(_missingTranslationRecordList[1], missingInfoControl.CurrentRecord);
		}

		[Test]
		public void SetCurrentRecordToNextThenPrevious_AfterChangedSoNoLongerMeetsFilter_SamePlace()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate);
			missingInfoControl.SetCurrentRecordToNext();
			AddTranslationToEntry(missingInfoControl.CurrentRecord, "a bogus translation of example");
			missingInfoControl.SetCurrentRecordToPrevious();
			Assert.AreEqual(_missingTranslationRecordList[0], missingInfoControl.CurrentRecord);
		}

		[Test]
		public void SetCurrentRecordToPrevious_AtLast_AfterChangedSoNoLongerMeetsFilter_StaysAtLast()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate);
			int count = _missingTranslationRecordList.Count;
			for (int i = 0; i < count; i++)
			{
				missingInfoControl.SetCurrentRecordToNext();
			}
			AddTranslationToEntry(missingInfoControl.CurrentRecord, "a bogus translation of example");
			missingInfoControl.SetCurrentRecordToPrevious();
			Assert.AreEqual(_missingTranslationRecordList[count - 2], missingInfoControl.CurrentRecord);
		}

		[Test]
		public void SetCurrentRecordToPrevious_AtSecondToLast_AfterChangedSoNoLongerMeetsFilter_GoesToEntryBeforeChangedOne()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate);
			int count = _missingTranslationRecordList.Count;
			for (int i = 0; i < count-2; i++)
			{
				missingInfoControl.SetCurrentRecordToNext();
			}
			AddTranslationToEntry(missingInfoControl.CurrentRecord, "a bogus translation of example");
			missingInfoControl.SetCurrentRecordToPrevious();
			Assert.AreEqual(_missingTranslationRecordList[count - 3], missingInfoControl.CurrentRecord);
		}


		[Test]
		public void SetCurrentRecordToNext_AtLast_AfterChangedSoNoLongerMeetsFilter_StaysAtLast()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate);
			int count = _missingTranslationRecordList.Count;
			for (int i = 0; i < count; i++)
			{
				missingInfoControl.SetCurrentRecordToNext();
			}
			AddTranslationToEntry(missingInfoControl.CurrentRecord, "a bogus translation of example");
			missingInfoControl.SetCurrentRecordToNext();
			Assert.AreEqual(_missingTranslationRecordList[count - 2], missingInfoControl.CurrentRecord);
		}

		[Test]
		public void SetCurrentRecordToNext__AfterChangedSoNoLongerMeetsFilter_GoesToNext()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate);
			missingInfoControl.SetCurrentRecordToNext();
			AddTranslationToEntry(missingInfoControl.CurrentRecord, "a bogus translation of example");
			missingInfoControl.SetCurrentRecordToNext();
			Assert.AreEqual(_missingTranslationRecordList[1], missingInfoControl.CurrentRecord);
		}


	}
}