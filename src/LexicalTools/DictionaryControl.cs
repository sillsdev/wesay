using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Palaso.Reporting;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.LexicalTools.Properties;
using WeSay.Project;
using WeSay.UI;
using WeSay.UI.AutoCompleteTextBox;

namespace WeSay.LexicalTools
{
	public partial class DictionaryControl: UserControl
	{
		private readonly ViewTemplate _viewTemplate;
		private readonly ContextMenu _cmWritingSystems;
		private WritingSystem _listWritingSystem;
		private bool _recordListBoxActive;
		private readonly Db4oRecordListManager _recordManager;
		private IBindingList _records;

		public DictionaryControl()
		{
			Debug.Assert(DesignMode);
			InitializeComponent();
		}

		public DictionaryControl(IRecordListManager recordManager,
								 ViewTemplate viewTemplate)
		{
			if (recordManager == null)
			{
				throw new ArgumentNullException("recordManager");
			}
			if (viewTemplate == null)
			{
				throw new ArgumentNullException("viewTemplate");
			}
			_viewTemplate = viewTemplate;
			_recordManager = (Db4oRecordListManager) recordManager;

			WritingSystem listWritingSystem = null;
			_cmWritingSystems = new ContextMenu();

			Field field =
					viewTemplate.GetField(
							Field.FieldNames.EntryLexicalForm.ToString());
			if (field != null)
			{
				if (field.WritingSystems.Count > 0)
				{
					listWritingSystem = field.WritingSystems[0];
					foreach (WritingSystem writingSystem in field.WritingSystems
							)
					{
						RegisterWritingSystemAndField(field, writingSystem);
					}
				}
				else
				{
					MessageBox.Show(
							String.Format(
									"There are no writing systems enabled for the Field '{0}'",
									field.FieldName),
							"Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation); //review
				}
			}
			Field glossfield =
					viewTemplate.GetField(Field.FieldNames.SenseGloss.ToString());
			if (glossfield != null)
			{
				foreach (
						WritingSystem writingSystem in glossfield.WritingSystems
						)
				{
					RegisterWritingSystemAndField(glossfield, writingSystem);
				}
			}

			if (listWritingSystem == null)
			{
				listWritingSystem =
						BasilProject.Project.WritingSystems.
								UnknownVernacularWritingSystem;
			}

			InitializeComponent();
			InitializeDisplaySettings();
			InitializeMonoWorkarounds();

			//this._btnNewWord.Font = StringCatalog.ModifyFontForLocalization(_btnNewWord.Font);
			//this._btnDeleteWord.Font = StringCatalog.ModifyFontForLocalization(_btnDeleteWord.Font);

			_writingSystemChooser.Image =
					Resources.Expand.GetThumbnailImage(6,
													   6,
													   ReturnFalse,
													   IntPtr.Zero);
			_btnFind.Image =
					Resources.Find.GetThumbnailImage(18,
													 18,
													 ReturnFalse,
													 IntPtr.Zero);
			_btnDeleteWord.Image =
					Resources.DeleteWord.GetThumbnailImage(18,
														   18,
														   ReturnFalse,
														   IntPtr.Zero);
			_btnNewWord.Image =
					Resources.NewWord.GetThumbnailImage(18,
														18,
														ReturnFalse,
														IntPtr.Zero);

			Control_EntryDetailPanel.ViewTemplate = _viewTemplate;
			Control_EntryDetailPanel.RecordListManager = this._recordManager;

			SetListWritingSystem(listWritingSystem);

			_findText.KeyDown += _findText_KeyDown;
			_recordsListBox.SelectedIndexChanged +=OnRecordSelectionChanged;
			_recordsListBox.Enter += _recordsListBox_Enter;
			_recordsListBox.Leave += _recordsListBox_Leave;
			UpdateDisplay();
		}

		public EntryViewControl Control_EntryDetailPanel
		{
			get { return _entryViewControl; }
		}

		public LexEntry CurrentRecord
		{
			get
			{
				if (_records.Count == 0 || CurrentIndex == -1)
				{
					return null;
				}
				return
						((CachedSortedDb4oList<string, LexEntry>) _records).
								GetValue(CurrentIndex);
			}
		}

		protected int CurrentIndex
		{
			get { return _recordsListBox.SelectedIndex; }
		}

		public event EventHandler SelectedIndexChanged = delegate { };

		private void InitializeMonoWorkarounds()
		{
			// Mono bug 82081
#if !MONO
			if (Type.GetType("Mono.Runtime") != null)
#endif
			{
				// this is not allowed in .Net but it is in Mono and is the only way to get
				// it to work (setting width to 0 doesn't)
				_btnFind.FlatAppearance.BorderColor = Color.Transparent;
				_writingSystemChooser.FlatAppearance.BorderColor =
						Color.Transparent;
				_btnDeleteWord.FlatAppearance.BorderColor = Color.Transparent;
				_btnNewWord.FlatAppearance.BorderColor = Color.Transparent;
			}
		}

		private void InitializeDisplaySettings()
		{
			BackColor = DisplaySettings.Default.BackgroundColor;
			this._findWritingSystemId.ForeColor = DisplaySettings.Default.WritingSystemLabelColor;

		}

		private void RegisterWritingSystemAndField(Field field,
												   WritingSystem writingSystem)
		{
			MenuItem item =
					new MenuItem(
							writingSystem.Id + "\t" +
							StringCatalog.Get(field.DisplayName),
							OnCmWritingSystemClicked);
			item.RadioCheck = true;
			item.Tag = writingSystem;
			_cmWritingSystems.MenuItems.Add(item);

			LexEntrySortHelper sortHelper =
					new LexEntrySortHelper(_recordManager.DataSource,
										   writingSystem,
										   IsWritingSystemUsedInLexicalForm(
												   writingSystem));
			_recordManager.GetSortedList(sortHelper);
		}

		private bool IsWritingSystemUsedInLexicalForm(
				WritingSystem writingSystem)
		{
			Field field =
					_viewTemplate.GetField(
							Field.FieldNames.EntryLexicalForm.ToString());
			if (field != null)
			{
				return field.WritingSystems.Contains(writingSystem);
			}
			return false;
		}

		public void SetListWritingSystem(WritingSystem writingSystem)
		{
			if (writingSystem == null)
			{
				throw new ArgumentNullException();
			}
			if (_listWritingSystem == writingSystem)
			{
				return;
			}
			_listWritingSystem = writingSystem;

			LexEntrySortHelper sortHelper =
					new LexEntrySortHelper(_recordManager.DataSource,
										   _listWritingSystem,
										   IsWritingSystemUsedInLexicalForm(
												   _listWritingSystem));
			CachedSortedDb4oList<string, LexEntry> cachedSortedDb4oList = this._recordManager.GetSortedList(sortHelper);
			_records = cachedSortedDb4oList;

			_recordsListBox.DataSource = _records;

			Control_EntryDetailPanel.DataSource = CurrentRecord;
			_recordsListBox.WritingSystem = _listWritingSystem;

			_findText.ItemFilterer = FindClosestAndNextClosestAndPrefixedForms;
			_findText.Items = _records;
			int originalHeight = _findText.Height;
			_findText.WritingSystem = _listWritingSystem;

			_findWritingSystemId.Text = _listWritingSystem.Id;
			int width = _findWritingSystemId.Width;
			_findWritingSystemId.AutoSize = false;
			_findWritingSystemId.Height = _findText.Height;
			_findWritingSystemId.Width = Math.Min(width, 25);

			int heightDifference = _findText.Height - originalHeight;
			_recordsListBox.Location = new Point(_recordsListBox.Location.X,
												 _recordsListBox.Location.Y +
												 heightDifference);
			_recordsListBox.Height -= heightDifference;
			_btnFind.Height = _findText.Height;
			_writingSystemChooser.Height = _findText.Height;
			_btnFind.Image =
					Resources.Find.GetThumbnailImage(_btnFind.Width - 2,
													 _btnFind.Width - 2,
													 ReturnFalse,
													 IntPtr.Zero);

			_btnFind.Left = _writingSystemChooser.Left - _btnFind.Width;
			_findText.Width = _btnFind.Left - _findText.Left;
			_findText.PopupWidth = _recordsListBox.Width;
		}

		private static IEnumerable FindClosestAndNextClosestAndPrefixedForms(string text, IEnumerable items, IDisplayStringAdaptor adaptor)
		{
			return ApproximateMatcher.FindClosestForms(items,
															   text,
															   ApproximateMatcherOptions.IncludePrefixedAndNextClosestForms);
		}

		private void OnCmWritingSystemClicked(object sender, EventArgs e)
		{
			MenuItem item = (MenuItem) sender;
			if (_listWritingSystem != item.Tag)
			{
				SetListWritingSystem((WritingSystem) item.Tag);
			}
		}

		private void OnWritingSystemChooser_Click(object sender, EventArgs e)
		{
			Logger.WriteMinorEvent("WritingSystemChooser_Click");

			foreach (MenuItem menuItem in _cmWritingSystems.MenuItems)
			{
				menuItem.Checked = (_listWritingSystem == menuItem.Tag);
			}
			_cmWritingSystems.Show(_writingSystemChooser,
								   new Point(_writingSystemChooser.Width,
											 _writingSystemChooser.Height));
		}

		private void OnFindWritingSystemId_MouseClick(object sender,
													  MouseEventArgs e)
		{
			Logger.WriteMinorEvent("FindWritingSystemId_MouseClick");
			_findText.Focus();
		}

		// primarily for testing

		private void _findText_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers == Keys.None &&
				e.KeyData == Keys.Enter)
			{
				e.Handled = true;
				if (Type.GetType("Mono.Runtime") == null)
						// Work around not yet implemented in Mono
				{
					SetSuppressKeyPressTrue(e);
				}
				Find(_findText.Text);
			}
		}

		private static void SetSuppressKeyPressTrue(KeyEventArgs e)
		{
#if !MONO
			e.SuppressKeyPress = true; // otherwise it beeps!
#endif
		}

		private void _findText_AutoCompleteChoiceSelected(object sender,
														  EventArgs e)
		{
			Find(_findText.Text);
		}

		private void OnFind_Click(object sender, EventArgs e)
		{
			Logger.WriteMinorEvent("FindButton_Click");

			Find(_findText.Text);
		}

		private void Find(string text)
		{
			Logger.WriteMinorEvent("Find");
			int index =
					((CachedSortedDb4oList<string, LexEntry>) _records).
							BinarySearch(text);
			if (index < 0)
			{
				index = ~index;
				if (index == _records.Count && index != 0)
				{
					index--;
				}
			}
			if (0 <= index && index < _recordsListBox.Items.Count)
			{
				_recordListBoxActive = true; // allow onRecordSelectionChanged
				_recordsListBox.SelectedIndex = index;
				_recordListBoxActive = false;
			}
		}

		private void _recordsListBox_Enter(object sender, EventArgs e)
		{
			_recordListBoxActive = true;
		}

		private void _recordsListBox_Leave(object sender, EventArgs e)
		{
			_recordListBoxActive = false;
		}

		private void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			if (Control_EntryDetailPanel.DataSource == CurrentRecord)
			{
				//we were getting 3 calls to this for each click on a new word
				return;
			}
			if (!_recordListBoxActive)
			{
				// When we change the content of the displayed string,
				// Windows.Forms.ListBox removes the item (and sends
				// the new selection event) then adds it in to the right
				// place (and sends the new selection event again)
				// We don't want to know about this case
				// We only want to know about the case where the user
				// has selected a record in the list box itself (so has to enter
				// the list box first)
				return;
			}

			if (CurrentRecord != null)
			{
				Logger.WriteEvent("RecordSelectionChanged to " +
								  CurrentRecord.LexicalForm.GetFirstAlternative());
				Control_EntryDetailPanel.DataSource = CurrentRecord;
			}
			else
			{
				Logger.WriteEvent(
						"RecordSelectionChanged Skipping because record is null");
			}

			SelectedIndexChanged.Invoke(this, null);
			UpdateDisplay();
		}

		private void OnNewWord_Click(object sender, EventArgs e)
		{
			Logger.WriteEvent("NewWord_Click");

			if (!_btnNewWord.Focused)
			{
				// if we use a hot key, it may not have received the focus
				// but we assume it has the focus when we do our selection change event
				_btnNewWord.Focus();
			}
			LexEntry entry = new LexEntry();
			bool NoPriorSelection = _recordsListBox.SelectedIndex == -1;
			_recordListBoxActive = true; // allow onRecordSelectionChanged
			_records.Add(entry);
			_recordsListBox.SelectedIndex = _records.IndexOf(entry);
			if (NoPriorSelection)
			{
				// Windows.Forms.Listbox does not consider it a change of Selection
				// index if the index was -1 and a record is added.
				// (No event is sent so we must do it ourselves)
				OnRecordSelectionChanged(this, null);
			}
			_recordListBoxActive = false;
			UpdateDisplay();
			_entryViewControl.Focus();
		}

		private void OnDeleteWord_Click(object sender, EventArgs e)
		{
			Logger.WriteEvent("DeleteWord_Clicked");

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
			_recordListBoxActive = true; // allow onRecordSelectionChanged
			_records.RemoveAt(CurrentIndex);
			OnRecordSelectionChanged(this, null);
			_recordListBoxActive = false;

			if (CurrentRecord == null)
			{
				Control_EntryDetailPanel.DataSource = CurrentRecord;
			}
			UpdateDisplay();
			_entryViewControl.Focus();
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
				_showAllFieldsToggleButton.Text = StringCatalog.Get("~Hide Uncommon Fields");
			}
			else
			{
				_showAllFieldsToggleButton.Text = StringCatalog.Get("~Show Uncommon Fields");
			}
		}
	}
}