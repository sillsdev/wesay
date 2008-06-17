using System;
using System.Drawing;
using System.IO;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class MissingInfoControlTests
	{
		private LexEntryRepository _lexEntryRepository;
		private string _filePath;
		private ResultSet<LexEntry> _missingTranslationRecordList;
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

			_filePath = Path.GetTempFileName();
			_lexEntryRepository = new LexEntryRepository(_filePath);

			_missingTranslation = new MissingTranslationFilter();

			_writingSystem =
					new WritingSystem("pretendVernacular", new Font(FontFamily.GenericSansSerif, 24));

			CreateTestEntry("apple", "red thing", "An apple a day keeps the doctor away.");
			CreateTestEntry("banana", "yellow food", "Monkeys like to eat bananas.");
			CreateTestEntry("car",
							"small motorized vehicle",
							"Watch out for cars when you cross the street.");
			CreateTestEntry("dog",
							"animal with four legs; man's best friend",
							"He walked his dog.");

			_missingTranslationRecordList =
					_lexEntryRepository.GetEntriesMatchingFilterSortedByLexicalUnit(
							_missingTranslation, _writingSystem);

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
			LexEntry entry = _lexEntryRepository.CreateItem();
			entry.LexicalForm[_writingSystem.Id] = lexicalForm;
			LexSense sense = (LexSense) entry.Senses.AddNew();
			sense.Definition["analysis"] = Definition;
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Sentence[_writingSystem.Id] = exampleSentence;
			_lexEntryRepository.SaveItem(entry);
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
			_lexEntryRepository.Dispose();
			File.Delete(_filePath);
		}

		[Test]
		public void Create()
		{
			using (
					MissingInfoControl missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   _missingTranslation.FilteringPredicate,
												   _lexEntryRepository))
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
										   _lexEntryRepository)) {}
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_NullviewTemplate_Throws()
		{
			using (
					new MissingInfoControl(_missingTranslationRecordList,
										   null,
										   _missingTranslation.FilteringPredicate,
										   _lexEntryRepository)) {}
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_NullFilter_Throws()
		{
			using (
					new MissingInfoControl(_missingTranslationRecordList,
										   _viewTemplate,
										   null,
										   _lexEntryRepository)) {}
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
												   _lexEntryRepository))
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
												   _lexEntryRepository))
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
												   _lexEntryRepository))
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
												   _lexEntryRepository))
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
												   _lexEntryRepository))
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
												   _lexEntryRepository))
			{
				AddTranslationToEntry(missingInfoControl.CurrentEntry,
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
												   _lexEntryRepository))
			{
				missingInfoControl.SetCurrentRecordToNext();
				missingInfoControl.SetCurrentRecordToNext();
				AddTranslationToEntry(missingInfoControl.CurrentEntry,
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
												   _lexEntryRepository))
			{
				missingInfoControl.SetCurrentRecordToNext();
				AddTranslationToEntry(missingInfoControl.CurrentEntry,
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
												   _lexEntryRepository))
			{
				int count = _missingTranslationRecordList.Count;
				for (int i = 0;i < count;i++)
				{
					missingInfoControl.SetCurrentRecordToNext();
				}
				AddTranslationToEntry(missingInfoControl.CurrentEntry,
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
												   _lexEntryRepository))
			{
				int count = _missingTranslationRecordList.Count;
				for (int i = 0;i < count - 2;i++)
				{
					missingInfoControl.SetCurrentRecordToNext();
				}
				AddTranslationToEntry(missingInfoControl.CurrentEntry,
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
												   _lexEntryRepository))
			{
				int count = _missingTranslationRecordList.Count;
				for (int i = 0;i < count;i++)
				{
					missingInfoControl.SetCurrentRecordToNext();
				}
				AddTranslationToEntry(missingInfoControl.CurrentEntry,
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
												   _lexEntryRepository))
			{
				missingInfoControl.SetCurrentRecordToNext();
				AddTranslationToEntry(missingInfoControl.CurrentEntry,
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
												   _lexEntryRepository))
			{
				missingInfoControl.SetCurrentRecordToNext();
				RecordToken<LexEntry> currentRecord = missingInfoControl.CurrentRecord;
				AddTranslationToEntry(missingInfoControl.CurrentEntry, "a bogus translation of example");
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
												   _lexEntryRepository))
			{
				missingInfoControl.SetCurrentRecordToNext();
				RecordToken<LexEntry> currentRecord = missingInfoControl.CurrentRecord;
				AddTranslationToEntry(missingInfoControl.CurrentEntry, "a bogus translation of example");
				AddTranslationToEntry(missingInfoControl.CurrentEntry, string.Empty);
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