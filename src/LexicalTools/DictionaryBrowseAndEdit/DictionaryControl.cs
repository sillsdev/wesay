using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Palaso.Data;
using Palaso.Code;
using Palaso.i18n;
using Palaso.UiBindings;
using Palaso.UI.WindowsForms.Miscellaneous;
using Palaso.Reporting;
using Palaso.Text;
using Palaso.WritingSystems;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation;
using WeSay.Project;
using WeSay.UI;
using WeSay.UI.AutoCompleteTextBox;
using Palaso.DictionaryServices.Model;

namespace WeSay.LexicalTools.DictionaryBrowseAndEdit
{
	public partial class DictionaryControl: UserControl
	{
		//autofac generates a factory which comes up with all the other needed parameters from its container
		public delegate DictionaryControl Factory(IUserInterfaceMemory memory);

		private readonly ViewTemplate _viewTemplate;
		private readonly ILogger _logger;
		private WritingSystemDefinition _listWritingSystem;
		private readonly LexEntryRepository _lexEntryRepository;
		private ResultSet<LexEntry> _records;
		private readonly ResultSetToListOfStringsAdapter _findTextAdapter;
		private EntryViewControl _entryViewControl;

		public DictionaryControl()
		{
			Debug.Assert(DesignMode);
			InitializeComponent();
		}

		public DictionaryControl(EntryViewControl.Factory entryViewControlFactory, LexEntryRepository lexEntryRepository,
			ViewTemplate viewTemplate, IUserInterfaceMemory memory, ILogger logger)
		{
			if (lexEntryRepository == null)
			{
				throw new ArgumentNullException("lexEntryRepository");
			}
			if (viewTemplate == null)
			{
				throw new ArgumentNullException("viewTemplate");
			}
			_viewTemplate = viewTemplate;
			_logger = logger;
			_lexEntryRepository = lexEntryRepository;

			InitializeComponent();

			if (DesignMode)
				return;


			SetupPickerControlWritingSystems();

			InitializeDisplaySettings();


			_findTextAdapter = new ResultSetToListOfStringsAdapter("Form", _records);
			SearchTextBox.Items = _findTextAdapter;

			SetListWritingSystem(
					_viewTemplate.GetDefaultWritingSystemForField(
							Field.FieldNames.EntryLexicalForm.ToString()));

			_searchTextBoxControl.TextBox.KeyDown += OnFindText_KeyDown;
			_searchTextBoxControl.TextBox.AutoCompleteChoiceSelected += OnSearchText_AutoCompleteChoiceSelected;
			_searchTextBoxControl.FindButton.Click += OnFind_Click;

			_recordsListBox.ItemSelectionChanged += OnRecordsListBoxItemSelectionChanged;

			_splitter.SetMemory(memory);
			SetupEntryViewControl(entryViewControlFactory);
			_entryViewControl.SetMemory(memory.CreateNewSection("entryView"));

			UpdateDisplay();
		}

		private void SetupEntryViewControl(EntryViewControl.Factory factory)
		{
			this._entryViewControl = factory();
			this.panelDetail.Controls.Add(this._entryViewControl);

			this._entryViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._entryViewControl.Name = "_entryViewControl";
			this._entryViewControl.ShowNormallyHiddenFields = false;
			this._entryViewControl.TabIndex = 0;

			//TODO: remove these, move to ctor
			Control_EntryDetailPanel.ViewTemplate = _viewTemplate;
			Control_EntryDetailPanel.LexEntryRepository = _lexEntryRepository;

		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public EntryViewControl Control_EntryDetailPanel
		{
			get { return _entryViewControl; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LexEntry CurrentRecord
		{
			get
			{
				if (_records.Count == 0 || CurrentIndex == -1)
				{
					return null;
				}
				try
				{
					RepositoryId id = _records[CurrentIndex].Id;
					return _lexEntryRepository.GetItem(id);
				}
				catch (Exception e)
				{
					WeSayWordsProject.Project.HandleProbableCacheProblem(e);
					return null;
				}
			}
		}

		protected int CurrentIndex
		{
			get { return _recordsListBox.SelectedIndex; }
		}



		public event EventHandler SelectedIndexChanged = delegate { };

		private void InitializeDisplaySettings()
		{
			BackColor = DisplaySettings.Default.BackgroundColor;

		}

		private void SetupPickerControlWritingSystems()
		{
			RegisterFieldWithPicker(Field.FieldNames.EntryLexicalForm.ToString());
			RegisterFieldWithPicker(LexSense.WellKnownProperties.Gloss); //Reversal
		}


		private void RegisterFieldWithPicker(string fieldName)
		{
			Field field = _viewTemplate.GetField(fieldName);
			if (field != null)
			{
				if (field.WritingSystemIds.Count > 0)
				{
					IList<WritingSystemDefinition> writingSystems =
							BasilProject.Project.WritingSystemsFromIds(field.WritingSystemIds);
					foreach (WritingSystemDefinition writingSystem in writingSystems)
					{
						if (!WritingSystemExistsInPicker(writingSystem))
						{
							AddWritingSystemToPicker(writingSystem, field);
						}
					}
				}
				else
				{
					ErrorReport.NotifyUserOfProblem(
							"There are no input systems enabled for the Field '{0}'",
							field.FieldName);
				}
			}
		}

		private void AddWritingSystemToPicker(WritingSystemDefinition writingSystem, Field field)
		{
			var item = new MenuItem(
				writingSystem.Abbreviation + "\t" + StringCatalog.Get(field.DisplayName),
				OnWritingSystemMenuItemClicked
			);
			item.RadioCheck = true;
			item.Tag = writingSystem;
			SearchModeMenu.MenuItems.Add(item);
		}

		private bool WritingSystemExistsInPicker(WritingSystemDefinition writingSystem)
		{
			foreach (MenuItem item in SearchModeMenu.MenuItems)
			{
				if (writingSystem.Id == ((WritingSystemDefinition) item.Tag).Id)
				{
					return true;
				}
			}
			return false;
		}

		protected ContextMenu SearchModeMenu
		{
			get { return _searchTextBoxControl.SearchModeMenu; }
		}

		private bool IsWritingSystemUsedInLexicalForm(string writingSystemId)
		{
			return _viewTemplate.IsWritingSystemUsedInField(writingSystemId,
															Field.FieldNames.EntryLexicalForm.
																	ToString());
		}

		public void SetListWritingSystem(WritingSystemDefinition writingSystem)
		{
			Guard.AgainstNull(writingSystem,"writingSystem");


			if (_listWritingSystem == writingSystem)
			{
				return;
			}
			_listWritingSystem = writingSystem;

			_recordsListBox.WritingSystem = _listWritingSystem;

			LoadRecords();

			_recordsListBox.RetrieveVirtualItem -= OnRetrieveVirtualItemEvent;
			_recordsListBox.RetrieveVirtualItem += OnRetrieveVirtualItemEvent;

			//WHy was this here (I'm (JH) scared to remove it)?
			// it is costing us an extra second, as we set the record
			// to the first one, then later set it to the one we actually want.
			//  SetRecordToBeEdited(CurrentRecord);

			ConfigureSearchBox();
		}

		private void ConfigureSearchBox()
		{
			if(DesignMode )
				return;

			_searchTextBoxControl.SetWritingSystem(_listWritingSystem);
			SearchTextBox.ItemFilterer = FindClosestAndNextClosestAndPrefixedForms;
			SearchTextBox.Items = _findTextAdapter;

			var top = _searchTextBoxControl.Bounds.Bottom + 10;
			_recordsListBox.SetBounds(
				_recordsListBox.Bounds.X,
				top,
				_recordsListBox.Bounds.Width,
				(Bottom - _bottomButtonTable.Height) - top - 10
			);
		}

		private void SetRecordToBeEdited(LexEntry record)
		{
			if (record == Control_EntryDetailPanel.DataSource)
				return;

			SaveAndCleanUpPreviousEntry();
			Control_EntryDetailPanel.DataSource = record;
			if (record != null)
			{
				record.PropertyChanged += OnEntryChanged;
			}
		}

		private void SaveAndCleanUpPreviousEntry() {
			LexEntry previousEntry = Control_EntryDetailPanel.DataSource;
			if (previousEntry != null)
			{
				previousEntry.PropertyChanged -= OnEntryChanged;
				if (!previousEntry.IsBeingDeleted)
				{
					_lexEntryRepository.SaveItem(previousEntry);
				}
			}
		}

		private void OnEntryChanged(object sender, PropertyChangedEventArgs e)
		{
			_lexEntryRepository.NotifyThatLexEntryHasBeenUpdated((LexEntry)sender);
		}

		private void LoadRecords()
		{
			if (IsWritingSystemUsedInLexicalForm(_listWritingSystem.Id))
			{
				_records = _lexEntryRepository.GetAllEntriesSortedByLexicalFormOrAlternative(_listWritingSystem);
			}
			else
			{
				_records = _lexEntryRepository.GetAllEntriesSortedByDefinitionOrGloss(_listWritingSystem);
			}
			 _findTextAdapter.Items = _records;
			_recordsListBox.DataSource = new List<RecordToken<LexEntry>>(_records);
		}

		private void OnRetrieveVirtualItemEvent(object sender, RetrieveVirtualItemEventArgs e)
		{
			RecordToken<LexEntry> recordToken = _records[e.ItemIndex];
			var displayString = (string) recordToken["Form"];
			e.Item = new ListViewItem(displayString);

			if ((string) recordToken["WritingSystem"] != _listWritingSystem.Id)
			{
				displayString = (string) recordToken["Form"];
				e.Item.Font = new Font(e.Item.Font, FontStyle.Italic);
				//!!! TODO: Get the correct font from the respective writingsystem and maybe put the writingsystem id behind the form!! --TA 8.9.08
			}

			if (string.IsNullOrEmpty(displayString))
			{
				displayString = "(";
				if (IsWritingSystemUsedInLexicalForm(_listWritingSystem.Id))
				{
					displayString += StringCatalog.Get("~Empty",
													   "This is what shows for a word in a list when the user hasn't yet typed anything in for the word.  Like if you click the 'New Word' button repeatedly.");
				}
				else
				{
					displayString += StringCatalog.Get("~No Gloss",
													   "This is what shows if the user is listing words by the glossing language, but the word doesn't have a gloss.");
				}
				displayString += ")";
			}
			e.Item.Text = displayString;
		}

		private static IEnumerable FindClosestAndNextClosestAndPrefixedForms(string text,
																			 IEnumerable items,
																			 IDisplayStringAdaptor
																					 adaptor)
		{
			return ApproximateMatcher.FindClosestForms(items,
													   text,
													   ApproximateMatcherOptions.IncludePrefixedAndNextClosestForms);
		}

		private void OnWritingSystemMenuItemClicked(object sender, EventArgs e)
		{
			var item = (MenuItem) sender;
			if (_listWritingSystem != item.Tag)
			{
				SetListWritingSystem((WritingSystemDefinition) item.Tag);
			}
		}

		// primarily for testing
		private void OnFindText_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers == Keys.None && e.KeyData == Keys.Enter)
			{
				e.Handled = true;
				e.SuppressKeyPress = true; // otherwise it beeps!
				SelectItemWithDisplayString(SearchTextBox.Text);
			}
		}

		private void OnSearchText_AutoCompleteChoiceSelected(object sender, EventArgs e)
		{
			SelectItemWithDisplayString(SearchTextBox.Text);
		}

		private void OnFind_Click(object sender, EventArgs e)
		{
			Logger.WriteMinorEvent("FindButton_Click");

			SelectItemWithDisplayString(SearchTextBox.Text);
		}


		public void GoToEntryWithId(Guid entryGuid)
		{
			LexEntry entry = _lexEntryRepository.GetLexEntryWithMatchingGuid(entryGuid);
			if (entry == null)
			{
				throw new NavigationException("Could not find the entry with guid " + entryGuid);
			}
			_recordsListBox.SelectedIndex = _records.FindFirstIndex(entry);
		}

		public void GotoFirstEntry()
		{
			if(_recordsListBox.Items.Count>0)
				_recordsListBox.SelectedIndex = 0;

		}

		public void GoToEntryWithId(string entryId)
		{
			//NB: this was written in Dec 2009 while we were discussing getting rid of non-guid ids.

			Guid g=default(Guid);
			try
			{
				g = new Guid(entryId);
			}
			catch
			{
			}
			LexEntry entry;

			if(g!=default(Guid))
			{
				entry = _lexEntryRepository.GetLexEntryWithMatchingGuid(g);
			}
			else
			{
				entry = _lexEntryRepository.GetLexEntryWithMatchingId(entryId);
			}

			if (entry == null)
			{
				throw new NavigationException("Could not find the entry with id " + entryId);
			}
			_recordsListBox.SelectedIndex = _records.FindFirstIndex(entry);
		}

		public void GoToUrl(string url)
		{
			//there was a time when this wasn't a real url, but rather just and id
			if (!url.Contains("lift://"))
			{
				GoToEntryWithId(url);
			}
			else
			{
				GoToEntryWithId(GetIdFromUrl(url));
			}
		}

		private static string GetIdFromUrl(string url)
		{
			var regEx = new Regex("id=([{|\\(]?[0-9a-fA-F]{8}[-]?([0-9a-fA-F]{4}[-]?){3}[0-9a-fA-F]{12}[\\)|}]?)");
			var match = regEx.Match(url);
			if (match.Value == String.Empty)
			{
				return "";
			}
			return match.Groups[1].Value;
		}


		private void SelectItemWithDisplayString(string text)
		{
			Logger.WriteMinorEvent("SelectItemWithDisplayString");
			_recordsListBox.SelectedIndex = _records.FindFirstIndex(
				token => (string) token["Form"] == text
			);
		}

		private void OnRecordsListBoxItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if (e.IsSelected)
			{
				var selectedRecord = CurrentRecord;
				SetRecordToBeEdited(CurrentRecord);

				if (CurrentRecord != null)
				{
					Logger.WriteEvent("RecordSelectionChanged to " +
									  CurrentRecord.LexicalForm.GetFirstAlternative());
				}
				else
				{
					Logger.WriteEvent("RecordSelectionChanged Skipping because record is null");
				}

				LoadRecords();
				_recordsListBox.SelectedIndex = _records.FindFirstIndex(selectedRecord);

				UpdateDisplay();
			}
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			//Catching ctrl-n here is a problem from the localisation perspective
			//What if the language does not lend itself to "n" as a representation of the word "new"
			//We could solve this by using an accelerator
			if (keyData == (Keys.Control | Keys.N))
			{
				bool FocusWasOnFindTextBox = SearchTextBox.Focused;
				_btnNewWord.Focus(); //this is necassary to cause TextBinding to update it's multitext
				AddNewWord(FocusWasOnFindTextBox);
				return true;
			}

			if (keyData == (Keys.Control | Keys.F))
			{
				if (!SearchTextBox.Focused)
				{
					SearchTextBox.Focus();
					SearchTextBox.Mode = WeSayAutoCompleteTextBox.EntryMode.List;
				}
				else
				{
					SelectItemWithDisplayString(SearchTextBox.Text);
				}

				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		protected WeSayAutoCompleteTextBox SearchTextBox
		{
			get { return _searchTextBoxControl.TextBox; }
		}

		public string CurrentUrl
		{
			get
			{
				var entry = CurrentRecord;
				if(entry==null)
					return string.Empty;

				try
				{
					return WeSayWordsProject.GetUrlFromLexEntry(entry);
				}
				catch (Exception error)
				{
					ErrorReport.NotifyUserOfProblem(error, "Could not generate URL for this entry.");
					return string.Empty;
				}
			}
		}



		private void OnNewWord_Click(object sender, EventArgs e)
		{
			AddNewWord(false);
		}

		internal void AddNewWord(bool FocusWasOnFindTextBox)
		{
			Logger.WriteMinorEvent("NewWord_Click");

			// only create a new word when there is not an empty word already
			int emptyWordIndex = GetEmptyWordIndex();
			int selectIndex;
			if (emptyWordIndex == -1)
			{
				LexEntry entry = _lexEntryRepository.CreateItem();
				//bool NoPriorSelection = _recordsListBox.SelectedIndex == -1;
				//_recordListBoxActive = true; // allow onRecordSelectionChanged
				if (FocusWasOnFindTextBox && !string.IsNullOrEmpty(SearchTextBox.Text) &&
					IsWritingSystemUsedInLexicalForm(_listWritingSystem.Id))
				{
					entry.LexicalForm[_listWritingSystem.Id] = SearchTextBox.Text.Trim();
					_lexEntryRepository.SaveItem(entry);
				}
				//review: Revert (remove) below for WS-950
				// _lexEntryRepository.SaveItem(entry);
				LoadRecords();
				selectIndex = _records.FindFirstIndex(entry);
			}
			else
			{
				selectIndex = emptyWordIndex;
			}

			if (!_btnNewWord.Focused)
			{
				// if we use a hot key, it may not have received the focus
				// but we assume it has the focus when we do our selection change event
				_btnNewWord.Focus();
			}
			Debug.Assert(selectIndex != -1);
			_recordsListBox.SelectedIndex = selectIndex;
			//_entryViewControl.Focus();
			_entryViewControl.SelectOnCorrectControl();

			_logger.WriteConciseHistoricalEvent("Added Word");

		}

		/// <summary>
		/// really helpful for debugging focus problems
		/// </summary>
		/// <returns></returns>
		public Control GetWhoHasFocus()
		{
			return FindFocus(this);
		}

		private Control FindFocus(Control parent)
		{
			if(parent.Focused)
				return parent;

			foreach (Control child in parent.Controls)
			{
				var c = FindFocus(child);
				if(c!=null)
					return c;
			}
			return null;
		}

		private int GetEmptyWordIndex()
		{
			// empty forms will always sort to the top
			for (int i = 0;i < _records.Count;++i)
			{
				if (!string.IsNullOrEmpty((string) _records[i]["Form"]))
				{
					break;
				}

				if (_records[i].RealObject.IsEmpty)
				{
					return i;
				}
			}
			return -1; // there is no empty word
		}

		private void OnDeleteWord_Click(object sender, EventArgs e)
		{
			Logger.WriteEvent("DeleteWord_Clicked");

			using (var dlg = new ConfirmDelete())
			{
				dlg.ShowDialog(this);
				if (dlg.DialogResult != DialogResult.OK)
				{
					Logger.WriteEvent("DeleteWord_Cancelled");
					return;
				}
			}
			DeleteWord();
		}

		internal void DeleteWord()
		{
			Debug.Assert(CurrentIndex >= 0);
			if (CurrentIndex == -1)
			{
				return;
			}
			if (!_btnDeleteWord.Focused)
			{
				// if we use a hot key, it may not have received the focus
				// but we assume it has the focus when we do our selection change event
				_btnDeleteWord.Focus();
			}
			//review: This save isn't necessary, but the possibility of deleting unsave records currently doesn't work.
			//_lexEntryRepository.SaveItem(CurrentRecord);

			_logger.WriteConciseHistoricalEvent("Deleted '{0}'",CurrentRecord.GetSimpleFormForLogging());
			CurrentRecord.IsBeingDeleted = true;
			RecordToken<LexEntry> recordToken = _records[CurrentIndex];
			_lexEntryRepository.DeleteItem(recordToken.Id);
			int index = _recordsListBox.SelectedIndex;
			LoadRecords();
			if (index >= _records.Count)
			{
				index = _records.Count - 1;
			}
			_recordsListBox.SelectedIndex = index;
			_entryViewControl.SelectOnCorrectControl();
		}



		private void OnShowAllFields_Click(object sender, EventArgs e)
		{
			_entryViewControl.ToggleShowNormallyHiddenFields();
			UpdateDisplay();
		}

		private void UpdateDisplay()
		{
			_btnDeleteWord.Enabled = (CurrentRecord != null);
			if (_entryViewControl.ShowNormallyHiddenFields)
			{
				_showAllFieldsToggleButton.Text = StringCatalog.Get("~Hide &Uncommon Fields");
			}
			else
			{
				_showAllFieldsToggleButton.Text = StringCatalog.Get("~Show &Uncommon Fields");
			}
		}

		private void DictionaryControl_Leave(object sender, EventArgs e)
		{
			SaveAndCleanUpPreviousEntry();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && !IsDisposed)
			{
				_recordsListBox.ItemSelectionChanged -= OnRecordsListBoxItemSelectionChanged;

				SaveAndCleanUpPreviousEntry();

				//_recordsListBox.Enter -= _recordsListBox_Enter;
				//_recordsListBox.Leave -= _recordsListBox_Leave;
				//_recordsListBox.DataSource = null; // without this, the currency manager keeps trying to work

				SearchTextBox.KeyDown -= OnFindText_KeyDown;
			}
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
		{

		}
	}
}
