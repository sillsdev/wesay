using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay.LexicalTools
{
	public class GatherWordListTask : WordGatheringTaskBase
	{
		private readonly string _wordListFileName;
		private GatherWordListControl _gatherControl;
		private List<string> _words;
		private int _currentWordIndex = 0;
		private string _writingSystemIdForWordListWords;
	   // private bool _suspendNotificationOfNavigation=false;


		public GatherWordListTask(IRecordListManager recordListManager,
								  string label,
								  string description,
								  string wordListFileName,
								  string writingSystemIdForWordListLanguage,
								  ViewTemplate viewTemplate)
			: base(label, description, false, recordListManager, viewTemplate)
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
			_wordListFileName = wordListFileName;
			_words = null;
			_writingSystemIdForWordListWords = writingSystemIdForWordListLanguage;
		}

		private void LoadWordList()
		{
			string pathLocal = Path.Combine(WeSayWordsProject.Project.PathToWeSaySpecificFilesDirectoryInProject, _wordListFileName);
			string pathToUse = pathLocal;
			if (!File.Exists(pathLocal))
			{
				string pathInProgramDir =
					Path.Combine(WeSayWordsProject.ApplicationCommonDirectory, _wordListFileName);
				pathToUse = pathInProgramDir;
				if (!File.Exists(pathToUse))
				{
					Palaso.Reporting.ErrorReport.ReportNonFatalMessage(
						"WeSay could not find the wordlist.  It expected to find it either at {0} or {1}.", pathLocal,
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
				} while (true);
			}

			NavigateFirstToShow();
		}


		public bool IsTaskComplete
		{
			get {
				if (_words != null)
				{
					return CurrentWordIndex >= _words.Count;
				}
				else
					return true;
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
				if (_gatherControl==null)
			   {
				   _gatherControl = new GatherWordListControl(this, ViewTemplate);
			   }
			   return _gatherControl;
			}
		}

		public string CurrentWord
		{
			get {return _words[CurrentWordIndex]; }
		}

		public bool CanNavigateNext
		{
			get
			{
				if (_words == null)
				{
					return false;
				}
				return _words.Count > CurrentWordIndex;
			}
		}

		public bool CanNavigatePrevious
		{
			get { return CurrentWordIndex > 0; }
		}

		protected int CurrentWordIndex
		{
			get { return _currentWordIndex; }
			set {
				_currentWordIndex = value;
				Debug.Assert(CurrentWordIndex >= 0);

				//nb: (CurrentWordIndex == _words.Count) is used to mark the "all done" state:

//                if (!_suspendNotificationOfNavigation && UpdateSourceWord != null)
//                {
//                    UpdateSourceWord.Invoke(this, null);
//                }
			}
		}

		public override void Activate()
		{
			if (!WeSayWordsProject.Project.WritingSystems.ContainsKey(_writingSystemIdForWordListWords))
			{
				Palaso.Reporting.ErrorReport.ReportNonFatalMessage("The writing system of the words in the word list will be used to add reversals and definitions.  Therefore, it needs to be in the list of writing systems for this project.  Either change the writing system that this task uses for the word list (currently '{0}') or add a writing system with this id to the project.", _writingSystemIdForWordListWords);
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
				m.SetAlternative(_writingSystemIdForWordListWords, CurrentWord);
				return m;
			}
		}

		public void WordCollected(MultiText newVernacularWord)
		{
			LexSense sense = new LexSense();
			sense.Definition.MergeIn(CurrentWordAsMultiText);
			sense.Gloss.MergeIn(CurrentWordAsMultiText);//we use this for matching up, and well, it probably is a good gloss

			Db4oLexQueryHelper.AddSenseToLexicon(RecordListManager, newVernacularWord, sense);
			RecordListManager.GoodTimeToCommit();
		}

		public override void Deactivate()
		{
			base.Deactivate();
			if (_gatherControl != null)
			{
				_gatherControl.Dispose();
			}
			_gatherControl = null;
			RecordListManager.GoodTimeToCommit();
		}

		public void NavigatePrevious()
		{
			--CurrentWordIndex;
		}

		public void NavigateNext()
		{
		   // _suspendNotificationOfNavigation = true;

			CurrentWordIndex++;
			while (CanNavigateNext && GetMatchingRecords(CurrentWordAsMultiText).Count > 0)
			{
				++CurrentWordIndex;
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
			CurrentWordIndex = 0;
		}

		public List<LexEntry> GetMatchingRecords(MultiText gloss)
		{
			return Db4oLexQueryHelper.FindObjectsFromLanguageForm<LexEntry, SenseGlossMultiText>(RecordListManager, gloss.GetFirstAlternative());
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
		public void TryToRemoveAssociationWithListWordFromEntry(LexEntry entry)
		{

			// have to iterate through these in reverse order
			// since they might get modified
			for (int i = entry.Senses.Count - 1; i >= 0; i--)
			{
				LexSense sense = (LexSense)entry.Senses[i];
				if(sense.Gloss !=null)
				{
					if(sense.Gloss.ContainsAlternative(_writingSystemIdForWordListWords))
					{
						if (sense.Gloss[_writingSystemIdForWordListWords] == CurrentListWord)
						{
							//since we copy the gloss into the defniition, too, if that hasn't been
							//modified, then we don't want to let it being non-empty keep us from
							//removing the sense. We're trying to enable typo correcting.
							if (sense.Definition[_writingSystemIdForWordListWords] == CurrentListWord)
							{
								sense.Definition.SetAlternative(_writingSystemIdForWordListWords, null);
								sense.Definition.RemoveEmptyStuff();
							}
							sense.Gloss.SetAlternative(_writingSystemIdForWordListWords, null);
							sense.Gloss.RemoveEmptyStuff();
							if (!sense.IsEmptyForPurposesOfDeletion)
							{
								//removing the gloss didn't make it empty. So repent of removing the gloss.
								sense.Gloss.SetAlternative(_writingSystemIdForWordListWords, CurrentListWord);
							}
						}
					}
				}
			}
			entry.CleanUpAfterEditting();
			if (entry.IsEmptyExceptForLexemeFormForPurposesOfDeletion)
			{
				Lexicon.RemoveEntry(entry);
			}
		}



		private string CurrentListWord
		{
			get { return _words[_currentWordIndex]; }
		}

		public IEnumerable<LexEntry> CurrentEntriesSorted
		{
			get
			{
				List<LexEntry> entries = GetMatchingRecords(CurrentWordAsMultiText);
				entries.Sort(new EntryByBestLexemeFormAlternativeComparer(ViewTemplate.GetField(LexEntry.WellKnownProperties.LexicalUnit).WritingSystems[0]));
				return entries;
			}
		}



	}
}
