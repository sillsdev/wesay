using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Palaso.Data;
using Palaso.Code;
using Palaso.DictionaryServices.Model;
using Palaso.i18n;
using Palaso.UI.WindowsForms.Miscellaneous;
using Palaso.Reporting;
using Palaso.WritingSystems;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.LexicalTools.AddMissingInfo
{
	public partial class MissingInfoControl: UserControl
	{
		private readonly List<RecordToken<LexEntry>> _completedRecords;
		private readonly List<RecordToken<LexEntry>> _todoRecords;
		private RecordToken<LexEntry> _currentRecord;
		private RecordToken<LexEntry> _previousRecord;
		private RecordToken<LexEntry> _nextRecord;
		private Timer _focusFirstFieldTimer;

		private readonly ViewTemplate _viewTemplate;
		private readonly Predicate<LexEntry> _isNotComplete;
		public event EventHandler TimeToSaveRecord;

		public MissingInfoControl(ResultSet<LexEntry> records, ViewTemplate viewTemplate, Predicate<LexEntry> isNotComplete, LexEntryRepository lexEntryRepository, IUserInterfaceMemory memory)
		{
			if (!DesignMode)
			{
				Guard.AgainstNull(records, "records");
				Guard.AgainstNull(viewTemplate, "viewTemplate");
				Guard.AgainstNull(isNotComplete, "isNotComplete");
				Guard.AgainstNull(lexEntryRepository, "lexEntryRepository");
			}

			InitializeComponent();
			PreviewKeyDown += OnPreviewKeyDown;

			_btnNextWord.ReallySetSize(50, 50);
			// _btnPreviousWord.ReallySetSize(30, 30);
			if (DesignMode)
			{
				return;
			}

			memory.TrackSplitContainer(splitContainer1, "betweenListsAndContents");
			memory.TrackSplitContainer(splitContainer2, "betweenToDoAndDoneLists");
			_entryViewControl.SetMemory(memory.CreateNewSection("entryView"));

			_viewTemplate = viewTemplate;
			_isNotComplete = isNotComplete;
			InitializeDisplaySettings();

			_entryViewControl.KeyDown += OnKeyDown;
			_entryViewControl.ViewTemplate = _viewTemplate;
			_entryViewControl.LexEntryRepository = lexEntryRepository;
			_entryViewControl.SenseDeletionEnabled = false;

			WritingSystemDefinition listWritingSystem = GetListWritingSystem();

			_todoRecords = records.ToList<RecordToken<LexEntry>>();
			_todoRecordsListBox.DataSource = _todoRecords;
			_todoRecordsListBox.BorderStyle = BorderStyle.None;
			_todoRecordsListBox.ItemSelectionChanged += OnTodoRecordSelectionChanged;
			_todoRecordsListBox.RetrieveVirtualItem += OnRetrieveVirtualItemEvent;
			_todoRecordsListBox.WritingSystem = listWritingSystem;

			_completedRecords = new List<RecordToken<LexEntry>>();
			_completedRecordsListBox.DataSource = _completedRecords;
			_completedRecordsListBox.BorderStyle = BorderStyle.None;
			_completedRecordsListBox.ItemSelectionChanged += OnCompletedRecordSelectionChanged;
			_completedRecordsListBox.RetrieveVirtualItem += OnRetrieveVirtualItemEvent;
			_completedRecordsListBox.WritingSystem = listWritingSystem;

			labelNextHotKey.BringToFront();
			_btnNextWord.BringToFront();
			_btnPreviousWord.BringToFront();
			SetCurrentRecordFromRecordList();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			/*
			 * cjh 2013-03-22
			 * Trying various attempts to set the focus on the first text box within the missing info control (this one),
			 * I could not figure out a point in execution time within this control that the first text box control was in existence such that I could set the focus.
			 * Winforms Control publishes the Load event, but does not publish other useful events like Shown or Activated, which Winforms Form does.  If we had a Shown event then I believe
			 * we could avoid the timer hack below.  I could not find such a suitable event.
			 *
			 * the Load event would be the natural event to handle for such a thing, but in my tests the text box control was still not focus-able.
			 * I discovered that it unfortunately must be a timing issue, as I could get the focus code to work if I set a breakpoint on the focus code and then immediately continued (F5).
			 * Running the code normally without the breakpoint does not yield the desired set focus.
			 * I reluctantly settled on the timer hack below to ensure that the control is focus-able before setting the focus
			 */
			if (_focusFirstFieldTimer == null)
			{
				_focusFirstFieldTimer = new Timer();
				_focusFirstFieldTimer.Tick += OnFocusFirstFieldTimer_Tick;
				_focusFirstFieldTimer.Interval = 300;
			}
			_focusFirstFieldTimer.Stop();//reset it
			_focusFirstFieldTimer.Start();
		}

		private void OnFocusFirstFieldTimer_Tick(object sender, EventArgs e)
		{
			_focusFirstFieldTimer.Stop();
			_entryViewControl.FocusFirstField();
		}

		private void OnRetrieveVirtualItemEvent(object sender, RetrieveVirtualItemEventArgs e)
		{
			RecordToken<LexEntry> recordToken;
			if (sender == _todoRecordsListBox)
			{
				recordToken = _todoRecords[e.ItemIndex];
			}
			else
			{
				Debug.Assert(sender == _completedRecordsListBox);
				recordToken = _completedRecords[e.ItemIndex];
			}
			var displayString = (string) recordToken["Form"];
			e.Item = new ListViewItem(displayString);
			if (!string.IsNullOrEmpty(displayString))
			{
				return;
			}

			displayString =
					recordToken.RealObject.LexicalForm.GetBestAlternative(
							_todoRecordsListBox.WritingSystem.Id, string.Empty);
			e.Item.Font = new Font(e.Item.Font, FontStyle.Italic);

			if (string.IsNullOrEmpty(displayString))
			{
				displayString = "(" +
								StringCatalog.Get("~Empty",
												  "This is what shows for a word in a list when the user hasn't yet typed anything in for the word.  Like if you click the 'New Word' button repeatedly.") +
								")";
			}
			e.Item.Text = displayString;
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			if (keyData == Keys.Enter || keyData == Keys.PageDown)
			{
				SetCurrentRecordToNext();
				return true;
			}
			if (keyData == Keys.PageUp)
			{
				SetCurrentRecordToPrevious();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}

		/// <summary>
		/// needed because the pos combo box blocks our access to enter, PageDown, etc.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.PageDown)
			{
				e.IsInputKey = false;
				SetCurrentRecordToNext();
			}
			if (e.KeyCode == Keys.PageUp)
			{
				e.IsInputKey = false;
				SetCurrentRecordToPrevious();
			}
		}

		private static WritingSystemDefinition GetListWritingSystem()
		{
			return WeSayWordsProject.Project.DefaultViewTemplate.GetDefaultWritingSystemForField(
				Field.FieldNames.EntryLexicalForm.ToString());
		}

		private void InitializeDisplaySettings()
		{
			BackColor = DisplaySettings.Default.BackgroundColor;
			_entryViewControl.BackColor = DisplaySettings.Default.BackgroundColor;
			//we like it to stand out at design time, but not runtime
		}

		private void OnTodoRecordSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if (e.ItemIndex != -1)
			{
				var recordForWhichSelectionIsChanging = _todoRecords[e.ItemIndex];
				if (CurrentRecord != null)
				{
					MoveRecordToAppropriateListBox(CurrentRecord);
					//reset the index as it may have changed
					_todoRecordsListBox.SelectedIndex =
						_todoRecords.FindIndex(x => x == recordForWhichSelectionIsChanging);
				}

				//This is the case if we previously had a record selected in the completedListBox and now are selecting a record in the todoListBox
				if (e.IsSelected && _completedRecordsListBox.SelectedIndex != -1)
				{
					_completedRecordsListBox.SelectedIndex = -1;
				}
				CurrentRecord = recordForWhichSelectionIsChanging;
				UpdatePreviousAndNextRecords();
			}
			else
			{
				if (CurrentRecord != null)
				{
					MoveRecordToAppropriateListBox(CurrentRecord);
				}
				CurrentRecord = null;
			}

			if (_todoRecords.Count == 0)
			{
			   ShowCompletedMessage();
			}
		}

		private void MoveRecordToAppropriateListBox(RecordToken<LexEntry> record)
		{
			if (_isNotComplete(record.RealObject))
			{
				if (_completedRecords.Contains(record))
				{
					_completedRecords.Remove(record);
				}
				if (!_todoRecords.Contains(record))
				{
					_todoRecords.Add(record);
				}
			}
			else
			{
				if (_todoRecords.Contains(record))
				{
					_todoRecords.Remove(record);
				}
				if (!_completedRecords.Contains(record))
				{
					_completedRecords.Add(record);
				}
			}
			_todoRecordsListBox.VirtualListSize = _todoRecords.Count;
			_completedRecordsListBox.VirtualListSize = _completedRecords.Count;
		}

		private void SaveNow()
		{

					if (TimeToSaveRecord != null)
					{
						TimeToSaveRecord.Invoke(this, null);
					}

		}

		public void SetCurrentRecordToNext()
		{
			if (!_btnNextWord.Focused)
			{
				// we need to make sure that any ghosts have lost their focus and triggered updates before
				// we do anything else
				_btnNextWord.Focus();
			}

			if (_todoRecords.Count > 0)
			{
				if (_nextRecord != null)
				{
					_todoRecordsListBox.SelectedIndex = _todoRecords.FindIndex(r => r == _nextRecord);
				}
				else if(!_isNotComplete(CurrentEntry))
				{
					_todoRecordsListBox.SelectedIndex = _todoRecords.Count - 2;
				}
				_entryViewControl.FocusFirstField();
			}
			else
			{
				ShowCompletedMessage();
			}
		}

		private void ShowCompletedMessage()
		{
			CurrentRecord = null;
			_congratulationsControl.Show(
				StringCatalog.Get("~Congratulations. You have completed this task."));
		}

		public void SetCurrentRecordToPrevious()
		{
			if (_todoRecords.Count > 0)
			{
				if (!_btnPreviousWord.Focused)
				{
					// we need to make sure that any ghosts have lost their focus and triggered updates before
					// we do anything else
					_btnPreviousWord.Focus();
				}

				if (_previousRecord != null)
				{
					_todoRecordsListBox.SelectedIndex = _todoRecords.FindIndex(r => r == _previousRecord);
				}
				else if (!_isNotComplete(CurrentEntry))
				{
					_todoRecordsListBox.SelectedIndex = 1;
				}
				_todoRecordsListBox.Focus();
				// change the focus so that the next focus event will for sure work
				_entryViewControl.Focus();
			}
		}

		private void UpdatePreviousAndNextRecords()
		{
			int currentIndex = RecordListCurrentIndex;
			_previousRecord = (currentIndex > 0) ? _todoRecords[currentIndex - 1] : null;
			_nextRecord = (currentIndex < _todoRecords.Count - 1)
								  ? _todoRecords[currentIndex + 1]
								  : null;
		}

		private void SetCurrentRecordFromRecordList()
		{
			if (_todoRecords.Count == 0)
			{
				CurrentRecord = null;
				_congratulationsControl.Show(
						StringCatalog.Get("~There is no work left to be done on this task."));
			}
			else
			{
				if (RecordListCurrentIndex == -1)
				{
					if (_todoRecordsListBox.Items.Count > 0)
					{
						_todoRecordsListBox.SelectedIndex = 0;
					}
				}
				if (RecordListCurrentIndex != -1)
				{
					CurrentRecord = _todoRecords[RecordListCurrentIndex];
				}
			}
		}

		private void OnCompletedRecordSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if (e.ItemIndex != -1)
			{
				var recordForWhichSelectionIsChanging = _completedRecords[e.ItemIndex];
				if (CurrentRecord != null)
				{
					MoveRecordToAppropriateListBox(CurrentRecord);
					//reset the index as it may have changed
					_completedRecordsListBox.SelectedIndex =
						_completedRecords.FindIndex(x => x == recordForWhichSelectionIsChanging);
				}

				//This is the case if we previously had a record selected in the todoListBox and now are selecting a record in the completedListBox
				if (e.IsSelected && _todoRecordsListBox.SelectedIndex != -1)
				{
					_todoRecordsListBox.SelectedIndex = -1;
				}

				CurrentRecord = recordForWhichSelectionIsChanging;
			}
			else
			{
				if (CurrentRecord != null)
				{
					MoveRecordToAppropriateListBox(CurrentRecord);
				}
				CurrentRecord = null;
			}
		}

		protected int RecordListCurrentIndex
		{
			get { return _todoRecordsListBox.SelectedIndex; }
		}

		protected int CompletedRecordListCurrentIndex
		{
			get { return _completedRecordsListBox.SelectedIndex; }
		}

		public EntryViewControl EntryViewControl
		{
			get { return _entryViewControl; }
		}

		/// <summary>
		/// Sets current record as selected in record list or completed record list
		/// </summary>
		/// <value>null if record list is empty</value>
		public RecordToken<LexEntry> CurrentRecord
		{
			get { return _currentRecord; }
			private set
			{
				if (_currentRecord != value)
				{
					if (CurrentEntry != null)
					{
						CurrentEntry.PropertyChanged -= OnCurrentRecordPropertyChanged;
					}

					_currentRecord = value;
					if (_currentRecord == null)
					{
						_entryViewControl.DataSource = null;
					}
					else
					{
						CurrentEntry = _currentRecord.RealObject;
						CurrentEntry.PropertyChanged += OnCurrentRecordPropertyChanged;
						_entryViewControl.DataSource = CurrentEntry;
						_congratulationsControl.Hide();
					}
				}
			}
		}

		public LexEntry CurrentEntry { get; private set; }

		private void SelectCurrentRecordInTodoRecordList()
		{
			int index = _todoRecords.IndexOf(CurrentRecord);
			Debug.Assert(index != -1);
			_todoRecordsListBox.SelectedIndex = index;
		}

		private void OnCurrentRecordPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			SaveNow();
			Debug.Assert(sender == CurrentEntry);
		}

		private void OnBtnPreviousWordClick(object sender, EventArgs e)
		{
			SetCurrentRecordToPrevious();
		}

		private void OnBtnNextWordClick(object sender, EventArgs e)
		{
			SetCurrentRecordToNext();
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = true;
			e.SuppressKeyPress = true;
			switch (e.KeyCode)
			{
				case Keys.PageUp:
					SetCurrentRecordToPrevious();
					break;
				case Keys.Enter:
				case Keys.PageDown:
					SetCurrentRecordToNext();
					break;

				default:
					e.Handled = false;
					e.SuppressKeyPress = false;
					break;
			}
		}

		// hack to get around the fact that SplitContainer takes over the
		// tab order and doesn't allow you to specify that the controls in the
		// right pane should get the highest tab order.
		// this means the RTF view looks bad. Still haven't figured out how to make
		// cursor go to right position.
		private bool monoOnEnterFix;


		protected override void OnEnter(EventArgs e)
		{
			if (monoOnEnterFix)
			{
				return;
			}
			try
			{
				monoOnEnterFix = true;
				base.OnEnter(e);
				_entryViewControl.Select();
			}
			finally
			{
				monoOnEnterFix = false;
			}
		}
	}
}