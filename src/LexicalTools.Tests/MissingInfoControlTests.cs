using System;
using System.Drawing;
using Palaso.Data;
using Palaso.DictionaryServices.Model;
using Palaso.TestUtilities;
using Palaso.WritingSystems;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation;
using WeSay.LexicalTools.AddMissingInfo;
using WeSay.Project;

using NUnit.Framework;
using WeSay.TestUtilities;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class MissingInfoControlTests
	{
		private LexEntryRepository _lexEntryRepository;
		private TemporaryFolder _tempFolder;
		private string _filePath;
		private ResultSet<LexEntry> _missingTranslationRecordList;
		private ViewTemplate _viewTemplate;
		private IWritingSystemDefinition _writingSystem;

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
					if (exampleSentence.Translation[WritingSystemsIdsForTests.AnalysisIdForTest].Length == 0)
					{
						return true;
					}
				}
			}
			return !(hasSense && hasExample);
		}

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			Palaso.UI.WindowsForms.Keyboarding.KeyboardController.Initialize();
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			Palaso.UI.WindowsForms.Keyboarding.KeyboardController.Shutdown();
		}

		[SetUp]
		public void SetUp()
		{
			WeSayProjectTestHelper.InitializeForTests();

			_tempFolder = new TemporaryFolder();
			_filePath = _tempFolder.GetTemporaryFile();
			_lexEntryRepository = new LexEntryRepository(_filePath);

			_writingSystem = WritingSystemDefinition.Parse(WritingSystemsIdsForTests.OtherIdForTest);

			CreateTestEntry("apple", "red thing", "An apple a day keeps the doctor away.");
			CreateTestEntry("banana", "yellow food", "Monkeys like to eat bananas.");
			CreateTestEntry("car",
							"small motorized vehicle",
							"Watch out for cars when you cross the street.");
			CreateTestEntry("dog", "animal with four legs; man's best friend", "He walked his dog.");

			string[] analysisWritingSystemIds = new[] { WritingSystemsIdsForTests.AnalysisIdForTest };
			string[] vernacularWritingSystemIds = new[] {_writingSystem.Id};
			RtfRenderer.HeadWordWritingSystemId = vernacularWritingSystemIds[0];

			_viewTemplate = new ViewTemplate
			{
				new Field(Field.FieldNames.EntryLexicalForm.ToString(),
						  "LexEntry",
						  vernacularWritingSystemIds),
				new Field(LexSense.WellKnownProperties.Definition,
						  "LexSense",
						  analysisWritingSystemIds),
				new Field(Field.FieldNames.ExampleSentence.ToString(),
						  "LexExampleSentence",
						  vernacularWritingSystemIds)
			};

			var exampleTranslationField = new Field(
				Field.FieldNames.ExampleTranslation.ToString(),
				"LexExampleSentence",
				analysisWritingSystemIds
			);
			_viewTemplate.Add(exampleTranslationField);

			_missingTranslationRecordList =
					_lexEntryRepository.GetEntriesWithMissingFieldSortedByLexicalUnit(
							exampleTranslationField, null, _writingSystem);
		}

		private void CreateTestEntry(string lexicalForm, string Definition, string exampleSentence)
		{
			var entry = _lexEntryRepository.CreateItem();
			entry.LexicalForm[_writingSystem.Id] = lexicalForm;
			var sense = new LexSense();
			entry.Senses.Add(sense);
			sense.Definition[WritingSystemsIdsForTests.AnalysisIdForTest] = Definition;
			var example = new LexExampleSentence();
			sense.ExampleSentences.Add(example);
			example.Sentence[_writingSystem.Id] = exampleSentence;
			_lexEntryRepository.SaveItem(entry);
			return;
		}

		private static void AddTranslationToEntry(LexEntry entry, string translation)
		{
			LexSense sense = entry.Senses[0];
			LexExampleSentence example = sense.ExampleSentences[0];
			example.Translation[WritingSystemsIdsForTests.AnalysisIdForTest] = translation;
		}

		[TearDown]
		public void TearDown()
		{
			_lexEntryRepository.Dispose();
			_tempFolder.Delete();
		}

		[Test]
		public void Create()
		{
			using (
					var missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   IsMissingTranslation,
												   _lexEntryRepository, new TaskMemory()))
			{
				Assert.IsNotNull(missingInfoControl);
			}
		}

		[Test]
		public void Create_NullRecords_Throws()
		{
			Assert.Throws<ArgumentNullException>(() => new MissingInfoControl(null,
																				  _viewTemplate,
																				  IsMissingTranslation,
																				  _lexEntryRepository, new TaskMemory()));
		}

		[Test]
		public void Create_NullviewTemplate_Throws()
		{
			Assert.Throws<ArgumentNullException>(() =>
													 new MissingInfoControl(_missingTranslationRecordList,
																			null,
																			IsMissingTranslation,
																			_lexEntryRepository, new TaskMemory()));
		}

		[Test]
		public void Create_NullFilter_Throws()
		{
			Assert.Throws<ArgumentNullException>(() =>
													 new MissingInfoControl(_missingTranslationRecordList,
																			_viewTemplate,
																			null,
																			_lexEntryRepository, new TaskMemory()));
		}

		[Test]
		public void Create_NullAllRecords_Throws()
		{
			Assert.Throws<ArgumentNullException>(() =>
					new MissingInfoControl(_missingTranslationRecordList,
										   _viewTemplate,
										   IsMissingTranslation,
										   null, null));
		}

		[Test]
		public void CurrentRecord_InitializedToFirst()
		{
			using (
					var missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   IsMissingTranslation,
												   _lexEntryRepository, new TaskMemory()))
			{
				Assert.AreEqual(_missingTranslationRecordList[0], missingInfoControl.CurrentRecord);
			}
		}

		[Test]
		public void SetCurrentRecordToPrevious_AtFirst_StaysAtFirst()
		{
			using (
					var missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   IsMissingTranslation,
												   _lexEntryRepository, new TaskMemory()))
			{
				missingInfoControl.SetCurrentRecordToPrevious();
				Assert.AreEqual(_missingTranslationRecordList[0], missingInfoControl.CurrentRecord);
			}
		}

		[Test]
		public void SetCurrentRecordToNextThenPrevious_SamePlace()
		{
			using (
					var missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   IsMissingTranslation,
												   _lexEntryRepository, new TaskMemory()))
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
					var missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   IsMissingTranslation,
												   _lexEntryRepository, new TaskMemory()))
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
					var missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   IsMissingTranslation,
												   _lexEntryRepository, new TaskMemory()))
			{
				missingInfoControl.SetCurrentRecordToNext();
				Assert.AreEqual(_missingTranslationRecordList[1], missingInfoControl.CurrentRecord);
			}
		}

		[Test]
		public void SetCurrentRecordToPrevious_AtFirst_AfterChangedSoNoLongerMeetsFilter_StaysAtFirst()
		{
			using (
					var missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   IsMissingTranslation,
												   _lexEntryRepository, new TaskMemory()))
			{
				AddTranslationToEntry(missingInfoControl.CurrentEntry,
									  "a bogus translation of example");
				missingInfoControl.SetCurrentRecordToPrevious();
				Assert.AreEqual(_missingTranslationRecordList[1], missingInfoControl.CurrentRecord);
			}
		}

		[Test]
		public void
				SetCurrentRecordToPrevious_AfterChangedSoNoLongerMeetsFilter_GoesToEntryBeforeChangedOne
				()
		{
			using (
					var missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   IsMissingTranslation,
												   _lexEntryRepository, new TaskMemory()))
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
					var missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   IsMissingTranslation,
												   _lexEntryRepository, new TaskMemory()))
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
					var missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   IsMissingTranslation,
												   _lexEntryRepository, new TaskMemory()))
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
					var missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   IsMissingTranslation,
												   _lexEntryRepository, new TaskMemory()))
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
		public void SetCurrentRecordToNext_AtLast_AfterChangedSoNoLongerMeetsFilter_StaysAtLastWhichWasFormerlyTheSecondToLast()
		{
			using (
					var missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   IsMissingTranslation,
												   _lexEntryRepository, new TaskMemory()))
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
					var missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   IsMissingTranslation,
												   _lexEntryRepository, new TaskMemory()))
			{
				missingInfoControl.SetCurrentRecordToNext();
				AddTranslationToEntry(missingInfoControl.CurrentEntry,
									  "a bogus translation of example");
				missingInfoControl.SetCurrentRecordToNext();
				Assert.AreEqual(_missingTranslationRecordList[2], missingInfoControl.CurrentRecord);
			}
		}

		[Test]
		public void SetCurrentRecordToNext_ChangedSoNoLongerMeetsFilter_RemovedFromTodoAndAddedToCompleteList()
		{
			using (
					var missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   IsMissingTranslation,
												   _lexEntryRepository, new TaskMemory()))
			{
				missingInfoControl.SetCurrentRecordToNext();
				RecordToken<LexEntry> recordToMove = missingInfoControl.CurrentRecord;
				AddTranslationToEntry(missingInfoControl.CurrentEntry,
									  "a bogus translation of example");
				missingInfoControl.SetCurrentRecordToNext();
				Assert.IsFalse(missingInfoControl._todoRecordsListBox.DataSource.Contains(recordToMove));
				Assert.IsTrue(missingInfoControl._completedRecordsListBox.DataSource.Contains(recordToMove));
#if Visual
				DebugShowState(missingInfoControl, currentRecord);
#endif
			}
		}

		[Test]
		public void SetCurrentRecordToPrevious_ChangedSoNoLongerMeetsFilter_RemovedFromTodoAndAddedToCompleteList()
		{
			using (
					var missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   IsMissingTranslation,
												   _lexEntryRepository, new TaskMemory()))
			{
				missingInfoControl.SetCurrentRecordToNext();
				RecordToken<LexEntry> recordToMove = missingInfoControl.CurrentRecord;
				AddTranslationToEntry(missingInfoControl.CurrentEntry,
									  "a bogus translation of example");
				missingInfoControl.SetCurrentRecordToPrevious();
				Assert.IsFalse(missingInfoControl._todoRecordsListBox.DataSource.Contains(recordToMove));
				Assert.IsTrue(missingInfoControl._completedRecordsListBox.DataSource.Contains(recordToMove));
#if Visual
				DebugShowState(missingInfoControl, currentRecord);
#endif
			}
		}


		/// <summary>
		/// regression for ws-1259 "meanings added but not saved"
		/// </summary>
		[Test]
		public void MakeChange_TaskToldToSaveCorrectRecord()
		{
			using (
					var missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   IsMissingTranslation,
												   _lexEntryRepository, new TaskMemory()))
			{
				missingInfoControl.SetCurrentRecordToNext();

				LexEntry toldToSave=null;
				missingInfoControl.TimeToSaveRecord += ((sender, e) => toldToSave = missingInfoControl.CurrentEntry);
				LexEntry guyThatNeedsToBeSaved=missingInfoControl.CurrentEntry;

				AddTranslationToEntry(missingInfoControl.CurrentEntry,
								  "a bogus translation of example");

				Assert.AreEqual(guyThatNeedsToBeSaved, toldToSave);
			}
		}




		[Test]
		public void ChangeSoMeetsFilter_AfterChangedSoNoLongerMeetsFilter_StaysHighlighted()
		{
			using (
					var missingInfoControl =
							new MissingInfoControl(_missingTranslationRecordList,
												   _viewTemplate,
												   IsMissingTranslation,
												   _lexEntryRepository, new TaskMemory()))
			{
				missingInfoControl.SetCurrentRecordToNext();
				RecordToken<LexEntry> currentRecord = missingInfoControl.CurrentRecord;
				AddTranslationToEntry(missingInfoControl.CurrentEntry,
									  "a bogus translation of example");
				AddTranslationToEntry(missingInfoControl.CurrentEntry, string.Empty);
				Assert.AreEqual(missingInfoControl._todoRecordsListBox.SelectedItem, currentRecord);
				Assert.IsFalse(
						missingInfoControl._completedRecordsListBox.DataSource.Contains(
								currentRecord));
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
			foreach (LexEntry item in missingInfoControl._todoRecordsListBox.Items)
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