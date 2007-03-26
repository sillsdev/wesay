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
		private string _wordListWritingSystemId;
		/// <summary>
		/// Fires when the user navigates to a new word from the wordlist
		/// </summary>
		public event EventHandler UpdateSourceWord;


		public GatherWordListTask(IRecordListManager recordListManager,
								  string label,
								  string description,
								  string wordListFileName,
								  string wordListWritingSystemId,
								  ViewTemplate viewTemplate)
			: base(label, description, false, recordListManager)
		{
			if (wordListFileName == null)
			{
				throw new ArgumentNullException("wordListFileName");
			}
			if (wordListWritingSystemId == null)
			{
				throw new ArgumentNullException("wordListWritingSystemId");
			}
			if (viewTemplate == null)
			{
				throw new ArgumentNullException("viewTemplate");
			}
			_wordListFileName = wordListFileName;
			_words = null;
			_wordListWritingSystemId = wordListWritingSystemId;
			_viewTemplate = viewTemplate;
		}

		private void LoadWordList()
		{
			_words = new List<string>();
			string path = Path.Combine(WeSayWordsProject.Project.PathToWeSaySpecificFilesDirectoryInProject, _wordListFileName);
			if (!File.Exists(path))
			{
				path = Path.Combine(WeSayWordsProject.Project.ApplicationCommonDirectory, _wordListFileName);
			}
			using (TextReader r = File.OpenText(path))
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
			get { return CurrentWordIndex >= _words.Count; }
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
			get { return _words.Count > CurrentWordIndex ; }
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
				m.SetAlternative(_wordListWritingSystemId, CurrentWord);
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
