using System;
using NUnit.Framework;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class MissingInfoControlTests
	{
		InMemoryRecordListManager _recordListManager;
		IRecordList<LexEntry> _missingTranslationRecordList;
		private IRecordList<LexEntry> _allRecords;
		ViewTemplate _viewTemplate;
		private MissingTranslationFilter _missingTranslation;

		public class MissingTranslationFilter:IFilter<LexEntry>
		{

			static private bool IsMissingTranslation(LexEntry entry)
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
						if (exampleSentence.Translation["analysis"].Length == 0)
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
			WeSayWordsProject.InitializeForTests();
			_recordListManager = new InMemoryRecordListManager();
			_allRecords = _recordListManager.GetListOfType<LexEntry>();
			this._missingTranslation = new MissingTranslationFilter();
			LexEntrySortHelper lexEntrySortHelper = new LexEntrySortHelper("vernacular", true);
			_recordListManager.Register(this._missingTranslation, lexEntrySortHelper);
			_missingTranslationRecordList = _recordListManager.GetListOfTypeFilteredFurther(this._missingTranslation, lexEntrySortHelper);
			_missingTranslationRecordList.Add(CreateTestEntry("apple", "red thing", "An apple a day keeps the doctor away."));
			_missingTranslationRecordList.Add(CreateTestEntry("banana", "yellow food", "Monkeys like to eat bananas."));
			_missingTranslationRecordList.Add(CreateTestEntry("car", "small motorized vehicle", "Watch out for cars when you cross the street."));
			_missingTranslationRecordList.Add(CreateTestEntry("dog", "animal with four legs; man's best friend", "He walked his dog."));


			string[] analysisWritingSystemIds = new string[] { "analysis" };
			string[] vernacularWritingSystemIds = new string[] { "vernacular" };
			this._viewTemplate = new ViewTemplate();
			this._viewTemplate.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(), "LexEntry",vernacularWritingSystemIds));
			this._viewTemplate.Add(new Field(Field.FieldNames.SenseGloss.ToString(), "LexSense",analysisWritingSystemIds));
			this._viewTemplate.Add(new Field(Field.FieldNames.ExampleSentence.ToString(), "LexExampleSentence",vernacularWritingSystemIds));
			this._viewTemplate.Add(new Field(Field.FieldNames.ExampleTranslation.ToString(), "LexExampleSentence",analysisWritingSystemIds));

		}

		private static LexEntry CreateTestEntry(string lexicalForm, string gloss, string exampleSentence)
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm["vernacular"] = lexicalForm;
			LexSense sense = (LexSense)entry.Senses.AddNew();
			sense.Gloss["analysis"] = gloss;
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			example.Sentence["vernacular"] = exampleSentence;
			return entry;
		}

		private static void AddTranslationToEntry(LexEntry entry, string translation)
		{
			LexSense sense = (LexSense)entry.Senses[0];
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences[0];
			example.Translation["analysis"] = translation;
		}

		[TearDown]
		public void TearDown()
		{
			_recordListManager.Dispose();
		}


		[Test]
		public void Create()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate, _allRecords);
			Assert.IsNotNull(missingInfoControl);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_NullRecords_Throws()
		{
			new MissingInfoControl(null, _viewTemplate, _missingTranslation.FilteringPredicate, _allRecords);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_NullviewTemplate_Throws()
		{
			new MissingInfoControl(_missingTranslationRecordList, null, _missingTranslation.FilteringPredicate, _allRecords);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_NullFilter_Throws()
		{
			new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, null, _allRecords);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_NullAllRecords_Throws()
		{
			new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate, null);
		}

		[Test]
		public void CurrentRecord_InitializedToFirst()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate, _allRecords);
			Assert.AreEqual(_missingTranslationRecordList[0], missingInfoControl.CurrentRecord);
		}

		[Test]
		public void SetCurrentRecordToPrevious_AtFirst_StaysAtFirst()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate, _allRecords);
			missingInfoControl.SetCurrentRecordToPrevious();
			Assert.AreEqual(_missingTranslationRecordList[0], missingInfoControl.CurrentRecord);
		}

		[Test]
		public void SetCurrentRecordToNextThenPrevious_SamePlace()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate, _allRecords);
			missingInfoControl.SetCurrentRecordToNext();
			missingInfoControl.SetCurrentRecordToPrevious();
			Assert.AreEqual(_missingTranslationRecordList[0], missingInfoControl.CurrentRecord);
		}

		[Test]
		public void SetCurrentRecordToNext_AtLast_StaysAtLast()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate, _allRecords);
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
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate, _allRecords);
			missingInfoControl.SetCurrentRecordToNext();
			Assert.AreEqual(_missingTranslationRecordList[1], missingInfoControl.CurrentRecord);
		}

		[Test]
		public void SetCurrentRecordToPrevious_AfterChangedSoNoLongerMeetsFilter_StaysAtFirst()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate, _allRecords);
			AddTranslationToEntry(missingInfoControl.CurrentRecord,"a bogus translation of example");
			missingInfoControl.SetCurrentRecordToPrevious();
			Assert.AreEqual(_missingTranslationRecordList[0], missingInfoControl.CurrentRecord);
		}

		[Test]
		public void SetCurrentRecordToPrevious_AfterChangedSoNoLongerMeetsFilter_GoesToEntryBeforeChangedOne()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate, _allRecords);
			missingInfoControl.SetCurrentRecordToNext();
			missingInfoControl.SetCurrentRecordToNext();
			AddTranslationToEntry(missingInfoControl.CurrentRecord, "a bogus translation of example");
			missingInfoControl.SetCurrentRecordToPrevious();
			Assert.AreEqual(_missingTranslationRecordList[1], missingInfoControl.CurrentRecord);
		}

		[Test]
		public void SetCurrentRecordToNextThenPrevious_AfterChangedSoNoLongerMeetsFilter_SamePlace()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate, _allRecords);
			missingInfoControl.SetCurrentRecordToNext();
			AddTranslationToEntry(missingInfoControl.CurrentRecord, "a bogus translation of example");
			missingInfoControl.SetCurrentRecordToPrevious();
			Assert.AreEqual(_missingTranslationRecordList[0], missingInfoControl.CurrentRecord);
		}

		[Test]
		public void SetCurrentRecordToPrevious_AtLast_AfterChangedSoNoLongerMeetsFilter_StaysAtLast()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate, _allRecords);
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
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate, _allRecords);
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
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate, _allRecords);
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
		public void SetCurrentRecordToNext_AfterChangedSoNoLongerMeetsFilter_GoesToNext()
		{
			MissingInfoControl missingInfoControl = new MissingInfoControl(_missingTranslationRecordList, _viewTemplate, _missingTranslation.FilteringPredicate, _allRecords);
			missingInfoControl.SetCurrentRecordToNext();
			AddTranslationToEntry(missingInfoControl.CurrentRecord, "a bogus translation of example");
			missingInfoControl.SetCurrentRecordToNext();
			Assert.AreEqual(_missingTranslationRecordList[1], missingInfoControl.CurrentRecord);
		}


	}
}