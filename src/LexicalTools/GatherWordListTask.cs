using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using WeSay.Data;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay.LexicalTools
{
	public class GatherWordListTask : TaskBase
	{
		private readonly string _wordListFileName;
		private GatherWordListControl _gatherControl;
		private List<string> _words;
		private int _currentWordIndex = 0;
		private ViewTemplate _viewTemplate;
		private string _writingSystemIdForGlossingLanguage;
		/// <summary>
		/// Fires when the user navigates to a new word from the wordlist
		/// </summary>
		public event EventHandler UpdateSourceWord;


		public GatherWordListTask(IRecordListManager recordListManager,
								  string label,
								  string description,
								  string wordListFileName,
								  string writingSystemIdForGlossingLanguage,
								  ViewTemplate viewTemplate)
			: base(label, description, false, recordListManager)
		{
			if (wordListFileName == null)
			{
				throw new ArgumentNullException("wordListFileName");
			}
			if (writingSystemIdForGlossingLanguage == null)
			{
				throw new ArgumentNullException("wordListWritingSystemId");
			}
			if (viewTemplate == null)
			{
				throw new ArgumentNullException("viewTemplate");
			}
			_wordListFileName = wordListFileName;
			_words = null;
			_writingSystemIdForGlossingLanguage = writingSystemIdForGlossingLanguage;
			_viewTemplate = viewTemplate;
		}

		private void LoadWordList()
		{
			string pathLocal = Path.Combine(WeSayWordsProject.Project.PathToWeSaySpecificFilesDirectoryInProject, _wordListFileName);
			string pathToUse = pathLocal;
			if (!File.Exists(pathLocal))
			{
				string pathInProgramDir =
					Path.Combine(WeSayWordsProject.Project.ApplicationCommonDirectory, _wordListFileName);
				pathToUse = pathInProgramDir;
				if (!File.Exists(pathToUse))
				{
					Reporting.ErrorReporter.ReportNonFatalMessage(
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

			NavigateFirst();
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
				   _gatherControl = new GatherWordListControl(this, _viewTemplate);
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

				if (this.UpdateSourceWord != null)
				{
					UpdateSourceWord.Invoke(this, null);
				}
			}
		}

		public override void Activate()
		{
			if (!Project.WeSayWordsProject.Project.WritingSystems.ContainsKey(_writingSystemIdForGlossingLanguage))
			{
				Reporting.ErrorReporter.ReportNonFatalMessage("The writing system of the words in the word list will be used to add glosses.  Therefore, it needs to be in the list of writing systems for this project.  Either change the writing system that this task uses for the word list (currently '{0}') or add a writing system with this id to the project.", _writingSystemIdForGlossingLanguage);
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
				m.SetAlternative(_writingSystemIdForGlossingLanguage, CurrentWord);
				return m;
			}
		}

		public void WordCollected(MultiText newVernacularWord)
		{
			LexSense sense = new LexSense();
			sense.Gloss.MergeIn(CurrentWordAsMultiText);

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
			CurrentWordIndex++;
			while (CanNavigateNext && GetMatchingRecords(CurrentWordAsMultiText).Count > 0)
			{
				++CurrentWordIndex;
			}
		}

		public void NavigateFirst()
		{
			_currentWordIndex = -1;
			NavigateNext();
		}

		public IList<LexEntry> GetMatchingRecords(MultiText gloss)
		{
			return Db4oLexQueryHelper.FindObjectsFromLanguageForm<LexEntry, SenseGlossMultiText>(this.RecordListManager, gloss.GetFirstAlternative());
		}
	}
}
