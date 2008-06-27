using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Palaso.Reporting;
using Palaso.Text;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.LexicalTools
{
	public class GatherWordListTask: WordGatheringTaskBase
	{
		private readonly string _wordListFileName;
		private GatherWordListControl _gatherControl;
		private List<string> _words;
		private int _currentWordIndex = 0;
		private readonly string _writingSystemIdForWordListWords;
		private readonly WritingSystem _lexicalUnitWritingSystem;
		// private bool _suspendNotificationOfNavigation=false;

		public GatherWordListTask(LexEntryRepository lexEntryRepository,
								  string label,
								  string description,
								  string wordListFileName,
								  string writingSystemIdForWordListLanguage,
								  ViewTemplate viewTemplate)
				: base(label, description, false, lexEntryRepository, viewTemplate)
		{
			if (wordListFileName == null)
			{
				throw new ArgumentNullException("wordListFileName");
			}
			if (writingSystemIdForWordListLanguage == null)
			{
				throw new ArgumentNullException("writingSystemIdForWordListLanguage");
			}
			if (viewTemplate == null)
			{
				throw new ArgumentNullException("viewTemplate");
			}
			Field lexicalFormField =
					viewTemplate.GetField(Field.FieldNames.EntryLexicalForm.ToString());
			if (lexicalFormField == null || lexicalFormField.WritingSystemIds.Count < 1)
			{
				_lexicalUnitWritingSystem =
						BasilProject.Project.WritingSystems.UnknownVernacularWritingSystem;
			}
			else
			{
				string firstWSid = lexicalFormField.WritingSystemIds[0];
				WritingSystem firstWS = BasilProject.Project.WritingSystems[firstWSid];
				_lexicalUnitWritingSystem = firstWS;
			}

			_wordListFileName = wordListFileName;
			_words = null;
			_writingSystemIdForWordListWords = writingSystemIdForWordListLanguage;
		}

		private void LoadWordList()
		{
			string pathLocal =
					Path.Combine(
							WeSayWordsProject.Project.PathToWeSaySpecificFilesDirectoryInProject,
							_wordListFileName);
			string pathToUse = pathLocal;
			if (!File.Exists(pathLocal))
			{
				string pathInProgramDir =
						Path.Combine(WeSayWordsProject.ApplicationCommonDirectory, _wordListFileName);
				pathToUse = pathInProgramDir;
				if (!File.Exists(pathToUse))
				{
					ErrorReport.ReportNonFatalMessage(
							"WeSay could not find the wordlist.  It expected to find it either at {0} or {1}.",
							pathLocal,
							pathInProgramDir);
					return;
				}
			}

			_words = new List<string>();

			using (TextReader r = File.OpenText(pathToUse))
			{
				do
				{
					string s = r.ReadLine();
					if (s == null)
					{
						break;
					}
					_words.Add(s);
				}
				while (true);
			}

			NavigateFirstToShow();
		}

		public bool IsTaskComplete
		{
			get
			{
				if (_words != null)
				{
					return CurrentIndexIntoWordlist >= _words.Count;
				}
				else
				{
					return true;
				}
			}
		}

		/// <summary>
		/// The GatherWordListControl associated with this task
		/// </summary>
		/// <remarks>Non null only when task is activated</remarks>
		public override Control Control
		{
			get
			{
				if (_gatherControl == null)
				{
					_gatherControl = new GatherWordListControl(this, _lexicalUnitWritingSystem.Id);
				}
				return _gatherControl;
			}
		}

		public string CurrentWordFromWordlist
		{
			get { return _words[CurrentIndexIntoWordlist]; }
		}

		public bool CanNavigateNext
		{
			get
			{
				if (_words == null)
				{
					return false;
				}
				return _words.Count > CurrentIndexIntoWordlist;
			}
		}

		public bool CanNavigatePrevious
		{
			get { return CurrentIndexIntoWordlist > 0; }
		}

		private int CurrentIndexIntoWordlist
		{
			get { return _currentWordIndex; }
			set
			{
				_currentWordIndex = value;
				Debug.Assert(CurrentIndexIntoWordlist >= 0);

				//nb: (CurrentWordIndex == _words.Count) is used to mark the "all done" state:

				//                if (!_suspendNotificationOfNavigation && UpdateSourceWord != null)
				//                {
				//                    UpdateSourceWord.Invoke(this, null);
				//                }
			}
		}

		public override void Activate()
		{
			if (
					!WeSayWordsProject.Project.WritingSystems.ContainsKey(
							 _writingSystemIdForWordListWords))
			{
				ErrorReport.ReportNonFatalMessage(
						"The writing system of the words in the word list will be used to add reversals and definitions.  Therefore, it needs to be in the list of writing systems for this project.  Either change the writing system that this task uses for the word list (currently '{0}') or add a writing system with this id to the project.",
						_writingSystemIdForWordListWords);
			}

			if (_words == null)
			{
				LoadWordList();
			}
			base.Activate();
		}

		/// <summary>
		/// Someday, we may indeed have multi-string foreign words
		/// </summary>
		public MultiText CurrentWordAsMultiText
		{
			get
			{
				MultiText m = new MultiText();
				m.SetAlternative(_writingSystemIdForWordListWords, CurrentWordFromWordlist);
				return m;
			}
		}

		public void WordCollected(MultiText newVernacularWord)
		{
			LexSense sense = new LexSense();
			sense.Definition.MergeIn(CurrentWordAsMultiText);
			sense.Gloss.MergeIn(CurrentWordAsMultiText);
			//we use this for matching up, and well, it probably is a good gloss

			AddSenseToLexicon(newVernacularWord, sense);
		}

		/// <summary>
		/// Try to add the sense to a matching entry. If none found, make a new entry with the sense
		/// </summary>
		private void AddSenseToLexicon(MultiTextBase lexemeForm, LexSense sense)
		{
			//review: the desired semantics of this find are unclear, if we have more than one ws
			ResultSet<LexEntry> entriesWithSameForm =
					LexEntryRepository.GetEntriesWithMatchingLexicalForm(
							lexemeForm[_lexicalUnitWritingSystem.Id], _lexicalUnitWritingSystem);
			if (entriesWithSameForm.Count == 0)
			{
				LexEntry entry = LexEntryRepository.CreateItem();
				entry.LexicalForm.MergeIn(lexemeForm);
				entry.Senses.Add(sense);
				LexEntryRepository.SaveItem(entry);
			}
			else
			{
				LexEntry entry = entriesWithSameForm[0].RealObject;

				foreach (LexSense s in entry.Senses)
				{
					if (sense.Gloss.Forms.Length > 0)
					{
						LanguageForm glossWeAreAdding = sense.Gloss.Forms[0];
						string glossInThisWritingSystem =
								s.Gloss.GetExactAlternative(glossWeAreAdding.WritingSystemId);
						if (glossInThisWritingSystem == glossWeAreAdding.Form)
						{
							return; //don't add it again
						}
					}
				}
				entry.Senses.Add(sense);
			}
		}

		public override void Deactivate()
		{
			base.Deactivate();
			if (_gatherControl != null)
			{
				_gatherControl.Dispose();
			}
			_gatherControl = null;
		}

		public void NavigatePrevious()
		{
			--CurrentIndexIntoWordlist;
		}

		public void NavigateNext()
		{
			// _suspendNotificationOfNavigation = true;

			CurrentIndexIntoWordlist++;
			while (CanNavigateNext && GetMatchingRecords().Count > 0)
			{
				++CurrentIndexIntoWordlist;
			}
			//  _suspendNotificationOfNavigation = false;
			//            if (UpdateSourceWord != null)
			//            {
			//                UpdateSourceWord.Invoke(this, null);
			//            }
		}

		public void NavigateFirstToShow()
		{
			_currentWordIndex = -1;
			NavigateNext();
		}

		public void NavigateAbsoluteFirst()
		{
			CurrentIndexIntoWordlist = 0;
		}

		public ResultSet<LexEntry> GetMatchingRecords()
		{
			return
					LexEntryRepository.GetEntriesWithMatchingGlossSortedByLexicalForm(
							CurrentWordAsMultiText.Find(_writingSystemIdForWordListWords),
							_lexicalUnitWritingSystem);
		}

		protected override int ComputeCount(bool returnResultEvenIfExpensive)
		{
			return CountNotRelevant;
		}

		protected override int ComputeReferenceCount()
		{
			return CountNotRelevant; //Todo
		}

		/// <summary>
		/// Removes the sense (if otherwise empty) and deletes the entry if it has no reason left to live
		/// </summary>
		public void TryToRemoveAssociationWithListWordFromEntry(RecordToken<LexEntry> recordToken)
		{
			// have to iterate through these in reverse order
			// since they might get modified
			LexEntry entry = recordToken.RealObject;
			for (int i = entry.Senses.Count - 1;i >= 0;i--)
			{
				LexSense sense = (LexSense) entry.Senses[i];
				if (sense.Gloss != null)
				{
					if (sense.Gloss.ContainsAlternative(_writingSystemIdForWordListWords))
					{
						if (sense.Gloss[_writingSystemIdForWordListWords] == CurrentListWord)
						{
							//since we copy the gloss into the defniition, too, if that hasn't been
							//modified, then we don't want to let it being non-empty keep us from
							//removing the sense. We're trying to enable typo correcting.
							if (sense.Definition[_writingSystemIdForWordListWords] ==
								CurrentListWord)
							{
								sense.Definition.SetAlternative(_writingSystemIdForWordListWords,
																null);
								sense.Definition.RemoveEmptyStuff();
							}
							sense.Gloss.SetAlternative(_writingSystemIdForWordListWords, null);
							sense.Gloss.RemoveEmptyStuff();
							if (!sense.IsEmptyForPurposesOfDeletion)
							{
								//removing the gloss didn't make it empty. So repent of removing the gloss.
								sense.Gloss.SetAlternative(_writingSystemIdForWordListWords,
														   CurrentListWord);
							}
						}
					}
				}
			}
			entry.CleanUpAfterEditting();
			if (entry.IsEmptyExceptForLexemeFormForPurposesOfDeletion)
			{
				LexEntryRepository.DeleteItem(entry);
			}
			else
			{
				LexEntryRepository.SaveItem(entry);
			}
		}

		private string CurrentListWord
		{
			get { return _words[_currentWordIndex]; }
		}
	}
}