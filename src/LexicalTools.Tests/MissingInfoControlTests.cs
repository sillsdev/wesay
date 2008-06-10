using System;
using System.Drawing;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class MissingInfoControlTests
	{
		private IRecordListManager _recordListManager;
		private IRecordList<LexEntry> _missingTranslationRecordList;
		private ViewTemplate _viewTemplate;
		private MissingTranslationFilter _missingTranslation;
		private WritingSystem _writingSystem;

		public class MissingTranslationFilter: IFilter<LexEntry>
		{
			private static bool IsMissingTranslation(LexEntry entry)
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
				get { return IsMissingTranslation; }
			}

			public string Key
			{
				get { return "MissingTranslationFilter"; }
			}
		}

		[SetUp]
		public void SetUp()
		{
			Db4oLexModelHelper.InitializeForNonDbTests();
			WeSayWordsProject.InitializeForTests();
			_recordListManager = new InMemoryRecordListManager();
			_missingTranslation = new MissingTranslationFilter();

			_writingSystem =
					new WritingSystem("pretendVernacular", new Font(FontFamily.GenericSansSerif, 24));

			LexEntrySortHelper lexEntrySortHelper = new LexEntrySortHelper(_writingSystem, true);
			_recordListManager.Register(_missingTranslation, lexEntrySortHelper);
			_missingTranslationRecordList =
					_recordListManager.GetListOfTypeFilteredFurther(_missingTranslation,
																	lexEntrySortHelper);
			_missingTranslationRecordList.Add(
					CreateTestEntry("apple", "red thing", "An apple a day keeps the doctor away."));
			_missingTranslationRecordList.Add(
					CreateTestEntry("banana", "yellow food", "Monkeys like to eat bananas."));
			_missingTranslationRecordList.Add(
					CreateTestEntry("car",
									"small motorized vehicle",
									"Watch out for cars when you cross the street."));
			_missingTranslationRecordList.Add(
					CreateTestEntry("dog",
									"animal with four legs; man's best friend",
									"He walked his dog."));

			string[] analysisWritingSystemIds = new string[] {"analysis"};
			string[] vernacularWritingSystemIds = new string[] {_writingSystem.Id};
			RtfRenderer.HeadWordWritingSystemId = vernacularWritingSystemIds[0];

			_viewTemplate = new ViewTemplate();
			_viewTemplate.Add(
					new Field(Field.FieldNames.EntryLexicalForm.ToString(),
							  "LexEntry",
							  vernacularWritingSystemIds));
			_viewTemplate.Add(
					new Field(LexSense.WellKnownProperties.Definition,
							  "LexSense",
							  analysisWritingSystemIds));



			_viewTemplate.Add(
					new Field(Field.FieldNames.ExampleSentence.ToString(),
							  "LexExampleSentence",
							  vernacularWritingSystemIds));
			_viewTemplate.Add(
					new Field(Field.FieldNames.ExampleTranslation.ToString(),
							  "LexExampleSentence",
							  analysisWritingSystemIds));
		}

		private LexEntry CreateTestEntry(string lexicalForm, string Definition, string exampleSentence)
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm[_writingSystem.Id] = lexicalForm;
			LexSense sense = (LexSense) entry.Senses.AddNew();
			sense.Definition["analysis"] = Definition;
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Sentence[_writingSystem.Id] = exampleSentence;
			return entry;
		}

		private static void AddTranslationToEntry(LexEntry entry, string translation)
		{
			LexSense sense = (LexSense) entry.Senses[0];
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences[0];
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
			using (
					MissingInfoControl missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   _missingTranslation.FilteringPredicate,
												   _recordListManager))
			{
				Assert.IsNotNull(missingInfoControl);
			}
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_NullRecords_Throws()
		{
			using (
					new MissingInfoControl(null,
										   _viewTemplate,
										   _missingTranslation.FilteringPredicate,
										   _recordListManager)) {}
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_NullviewTemplate_Throws()
		{
			using (
					new MissingInfoControl(_missingTranslationRecordList,
										   null,
										   _missingTranslation.FilteringPredicate,
										   _recordListManager)) {}
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_NullFilter_Throws()
		{
			using (
					new MissingInfoControl(_missingTranslationRecordList,
										   _viewTemplate,
										   null,
										   _recordListManager)) {}
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_NullAllRecords_Throws()
		{
			using (
					new MissingInfoControl(_missingTranslationRecordList,
										   _viewTemplate,
										   _missingTranslation.FilteringPredicate,
										   null)) {}
		}

		[Test]
		public void CurrentRecord_InitializedToFirst()
		{
			using (
					MissingInfoControl missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   _missingTranslation.FilteringPredicate,
												   _recordListManager))
			{
				Assert.AreEqual(_missingTranslationRecordList[0], missingInfoControl.CurrentRecord);
			}
		}

		[Test]
		public void SetCurrentRecordToPrevious_AtFirst_StaysAtFirst()
		{
			using (
					MissingInfoControl missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   _missingTranslation.FilteringPredicate,
												   _recordListManager))
			{
				missingInfoControl.SetCurrentRecordToPrevious();
				Assert.AreEqual(_missingTranslationRecordList[0], missingInfoControl.CurrentRecord);
			}
		}

		[Test]
		public void SetCurrentRecordToNextThenPrevious_SamePlace()
		{
			using (
					MissingInfoControl missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   _missingTranslation.FilteringPredicate,
												   _recordListManager))
			{
				missingInfoControl.SetCurrentRecordToNext();
				missingInfoControl.SetCurrentRecordToPrevious();
				Assert.AreEqual(_missingTranslationRecordList[0], missingInfoControl.CurrentRecord);
			}
		}

		[Test]
		public void SetCurrentRecordToNext_AtLast_StaysAtLast()
		{
			using (
					MissingInfoControl missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   _missingTranslation.FilteringPredicate,
												   _recordListManager))
			{
				int count = _missingTranslationRecordList.Count;
				for (int i = 0;i <= count;i++)
				{
					missingInfoControl.SetCurrentRecordToNext();
				}
				Assert.AreEqual(_missingTranslationRecordList[count - 1],
								missingInfoControl.CurrentRecord);
			}
		}

		[Test]
		public void SetCurrentRecordToNext_GoesToNext()
		{
			using (
					MissingInfoControl missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   _missingTranslation.FilteringPredicate,
												   _recordListManager))
			{
				missingInfoControl.SetCurrentRecordToNext();
				Assert.AreEqual(_missingTranslationRecordList[1], missingInfoControl.CurrentRecord);
			}
		}

		[Test]
		public void SetCurrentRecordToPrevious_AfterChangedSoNoLongerMeetsFilter_StaysAtFirst()
		{
			using (
					MissingInfoControl missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   _missingTranslation.FilteringPredicate,
												   _recordListManager))
			{
				AddTranslationToEntry(missingInfoControl.CurrentRecord,
									  "a bogus translation of example");
				missingInfoControl.SetCurrentRecordToPrevious();
				Assert.AreEqual(_missingTranslationRecordList[0], missingInfoControl.CurrentRecord);
			}
		}

		[Test]
		public void
				SetCurrentRecordToPrevious_AfterChangedSoNoLongerMeetsFilter_GoesToEntryBeforeChangedOne
				()
		{
			using (
					MissingInfoControl missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   _missingTranslation.FilteringPredicate,
												   _recordListManager))
			{
				missingInfoControl.SetCurrentRecordToNext();
				missingInfoControl.SetCurrentRecordToNext();
				AddTranslationToEntry(missingInfoControl.CurrentRecord,
									  "a bogus translation of example");
				missingInfoControl.SetCurrentRecordToPrevious();
				Assert.AreEqual(_missingTranslationRecordList[1], missingInfoControl.CurrentRecord);
			}
		}

		[Test]
		public void SetCurrentRecordToNextThenPrevious_AfterChangedSoNoLongerMeetsFilter_SamePlace()
		{
			using (
					MissingInfoControl missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   _missingTranslation.FilteringPredicate,
												   _recordListManager))
			{
				missingInfoControl.SetCurrentRecordToNext();
				AddTranslationToEntry(missingInfoControl.CurrentRecord,
									  "a bogus translation of example");
				missingInfoControl.SetCurrentRecordToPrevious();
				Assert.AreEqual(_missingTranslationRecordList[0], missingInfoControl.CurrentRecord);
			}
		}

		[Test]
		public void SetCurrentRecordToPrevious_AtLast_AfterChangedSoNoLongerMeetsFilter_StaysAtLast()
		{
			using (
					MissingInfoControl missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   _missingTranslation.FilteringPredicate,
												   _recordListManager))
			{
				int count = _missingTranslationRecordList.Count;
				for (int i = 0;i < count;i++)
				{
					missingInfoControl.SetCurrentRecordToNext();
				}
				AddTranslationToEntry(missingInfoControl.CurrentRecord,
									  "a bogus translation of example");
				missingInfoControl.SetCurrentRecordToPrevious();
				Assert.AreEqual(_missingTranslationRecordList[count - 2],
								missingInfoControl.CurrentRecord);
			}
		}

		[Test]
		public void
				SetCurrentRecordToPrevious_AtSecondToLast_AfterChangedSoNoLongerMeetsFilter_GoesToEntryBeforeChangedOne
				()
		{
			using (
					MissingInfoControl missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   _missingTranslation.FilteringPredicate,
												   _recordListManager))
			{
				int count = _missingTranslationRecordList.Count;
				for (int i = 0;i < count - 2;i++)
				{
					missingInfoControl.SetCurrentRecordToNext();
				}
				AddTranslationToEntry(missingInfoControl.CurrentRecord,
									  "a bogus translation of example");
				missingInfoControl.SetCurrentRecordToPrevious();
				Assert.AreEqual(_missingTranslationRecordList[count - 3],
								missingInfoControl.CurrentRecord);
			}
		}

		[Test]
		public void SetCurrentRecordToNext_AtLast_AfterChangedSoNoLongerMeetsFilter_StaysAtLast()
		{
			using (
					MissingInfoControl missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   _missingTranslation.FilteringPredicate,
												   _recordListManager))
			{
				int count = _missingTranslationRecordList.Count;
				for (int i = 0;i < count;i++)
				{
					missingInfoControl.SetCurrentRecordToNext();
				}
				AddTranslationToEntry(missingInfoControl.CurrentRecord,
									  "a bogus translation of example");
				missingInfoControl.SetCurrentRecordToNext();
				Assert.AreEqual(_missingTranslationRecordList[count - 2],
								missingInfoControl.CurrentRecord);
			}
		}

		[Test]
		public void SetCurrentRecordToNext_AfterChangedSoNoLongerMeetsFilter_GoesToNext()
		{
			using (
					MissingInfoControl missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   _missingTranslation.FilteringPredicate,
												   _recordListManager))
			{
				missingInfoControl.SetCurrentRecordToNext();
				AddTranslationToEntry(missingInfoControl.CurrentRecord,
									  "a bogus translation of example");
				missingInfoControl.SetCurrentRecordToNext();
				Assert.AreEqual(_missingTranslationRecordList[1], missingInfoControl.CurrentRecord);
			}
		}

		[Test]
		public void ChangedSoNoLongerMeetsFilter_RemovedFromTodoAndAddedToCompleteList()
		{
			using (
					MissingInfoControl missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   _missingTranslation.FilteringPredicate,
												   _recordListManager))
			{
				missingInfoControl.SetCurrentRecordToNext();
				LexEntry currentRecord = missingInfoControl.CurrentRecord;
				AddTranslationToEntry(currentRecord, "a bogus translation of example");
				Assert.AreEqual(missingInfoControl._completedRecordsListBox.SelectedItem,
								currentRecord);
				Assert.IsFalse(missingInfoControl._recordsListBox.DataSource.Contains(currentRecord));
#if Visual
				DebugShowState(missingInfoControl, currentRecord);
#endif
			}
		}

		[Test]
		public void ChangeSoMeetsFilter_AfterChangedSoNoLongerMeetsFilter_StaysHighlighted()
		{
			using (
					MissingInfoControl missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   _missingTranslation.FilteringPredicate,
												   _recordListManager))
			{
				missingInfoControl.SetCurrentRecordToNext();
				LexEntry currentRecord = missingInfoControl.CurrentRecord;
				AddTranslationToEntry(currentRecord, "a bogus translation of example");
				AddTranslationToEntry(currentRecord, string.Empty);
				Assert.AreEqual(missingInfoControl._recordsListBox.SelectedItem, currentRecord);
				Assert.IsFalse(
						missingInfoControl._completedRecordsListBox.DataSource.Contains(currentRecord));
#if Visual
				DebugShowState(missingInfoControl, currentRecord);
#endif
			}
		}
#if Visual
		private static void DebugShowState(MissingInfoControl missingInfoControl,
										   LexEntry currentRecord)
		{
			Console.WriteLine("Current:");
			Console.WriteLine(currentRecord);

			Console.WriteLine("ToDo:");
			foreach (LexEntry item in missingInfoControl._recordsListBox.Items)
			{
				Console.WriteLine(item);
			}

			Console.WriteLine("Completed:");
			foreach (LexEntry item in missingInfoControl._completedRecordsListBox.Items)
			{
				Console.WriteLine(item);
			}
		}
#endif
	}
}