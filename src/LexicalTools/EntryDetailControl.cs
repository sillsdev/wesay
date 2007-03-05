using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Data;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;
using Debug=System.Diagnostics.Debug;

namespace WeSay.LexicalTools
{
	public partial class EntryDetailControl : UserControl
	{
		private IBindingList _records;
		WritingSystem _listWritingSystem;
		//        private CachedSortedDb4oList<string, LexEntry> _originalRecordList;

		private readonly ViewTemplate _viewTemplate;
		public event EventHandler SelectedIndexChanged;

		public EntryDetailControl()
		{
			Debug.Assert(DesignMode);
			InitializeComponent();
		}

		public EntryDetailControl(IRecordListManager recordManager, ViewTemplate viewTemplate)
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

			Field field = viewTemplate.GetField(Field.FieldNames.EntryLexicalForm.ToString());
			if(field.WritingSystems.Count > 0)
			{
				_listWritingSystem = field.WritingSystems[0];
			}
			else
			{
				MessageBox.Show(String.Format("There are no writing systems enabled for the Field '{0}'",field.FieldName),"Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);//review
				_listWritingSystem = BasilProject.Project.WritingSystems.TestGetWritingSystemVern;
			}

			LexEntrySortHelper sortHelper = new LexEntrySortHelper(((Db4oRecordListManager) recordManager).DataSource,
																	viewTemplate,
																	_listWritingSystem.Id);
			_records = ((Db4oRecordListManager)recordManager).GetSortedList(sortHelper);

			InitializeComponent();

			_recordsListBox.DataSource = _records;


			BackColor = WeSay.UI.DisplaySettings.Default.BackgroundColor;
			this.Control_EntryDetailPanel.ViewTemplate = _viewTemplate;
			this.Control_EntryDetailPanel.BackColor = WeSay.UI.DisplaySettings.Default.BackgroundColor;
			this.Control_EntryDetailPanel.DataSource = CurrentRecord;

			_btnFind.Text = StringCatalog.Get("Find");


			_recordsListBox.Font = _listWritingSystem.Font;

			_recordsListBox.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);

			_findText.KeyDown += new KeyEventHandler(_findText_KeyDown);
			_findText.ItemFilterer = ApproximateMatcher.FindClosestAndNextClosestAndPrefixedForms;
			_findText.Items = (CachedSortedDb4oList<string, LexEntry>)_records;
			_findText.Font = _listWritingSystem.Font;
			_findText.WritingSystem = _listWritingSystem;


			int originalHeight = _findText.Height;

			int heightDifference = _findText.Height - originalHeight;
			_recordsListBox.Height -= heightDifference;
			_recordsListBox.Location = new Point(_recordsListBox.Location.X,
												 _recordsListBox.Location.Y + heightDifference);
			_btnFind.Height += heightDifference;
		}

		// primarily for testing
		public LexPreviewWithEntryControl Control_EntryDetailPanel
		{
			get
			{
				return this._entryDetailPanel;
			}
		}

		void _findText_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.Modifiers == Keys.None &&
			   e.KeyData == Keys.Enter)
			{
				e.Handled = true;
				e.SuppressKeyPress = true; // otherwise it beeps!
				Find(this._findText.Text);
			}
		}

		void _findText_AutoCompleteChoiceSelected(object sender, System.EventArgs e)
		{
			Find(this._findText.Text);
		}



		void _btnFind_Click(object sender, EventArgs e)
		{
			Find(this._findText.Text);
		}


		private void Find(string text) {
			int index = ((CachedSortedDb4oList<string, LexEntry>) _records).BinarySearch(text);
			if (index < 0)
			{
				index = ~index;
				if (index == _records.Count && index != 0)
				{
					index--;
				}
			}
			if(index >=0)
			{
				_recordsListBox.SelectedIndex = index;
			}
		}


		void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			if (this.Control_EntryDetailPanel.DataSource == CurrentRecord)
			{
				//we were getting 3 calls to this for each click on a new word
				return;
			}
			this.Control_EntryDetailPanel.DataSource = CurrentRecord;
			_btnDeleteWord.Enabled = (CurrentRecord != null);
			if (SelectedIndexChanged != null)
			{
				SelectedIndexChanged.Invoke(this,null);
			}
		}

		public LexEntry CurrentRecord
		{
			get
			{
				if (_records.Count == 0)
				{
					return null;
				}
				return ((CachedSortedDb4oList<string, LexEntry>)_records).GetValue(CurrentIndex);
			}
		}

		protected int CurrentIndex
		{
			get
			{
				if (_records.Count > 0 && _recordsListBox.SelectedIndex == -1)
				{
					return 0;
				}
				return _recordsListBox.SelectedIndex;
			}
		}

		private void _btnNewWord_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			LexEntry entry = new LexEntry();
			bool NoPriorSelection = _recordsListBox.SelectedIndex == -1;
			_records.Add(entry);
			_recordsListBox.SelectedIndex = _records.IndexOf(entry);
			if(NoPriorSelection)
			{
				// Windows.Forms.Listbox does not consider it a change of Selection
				// index if the index was -1 and a record is added.
				// (No event is sent so we must do it ourselves)
				OnRecordSelectionChanged(this, new EventArgs());
			}
			_entryDetailPanel.Focus();
		}

		private void _btnDeleteWord_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Debug.Assert(CurrentIndex >= 0);
			_records.RemoveAt(CurrentIndex);
			//hack until we can get selection change events sorted out in BindingGridList
			OnRecordSelectionChanged(this, null);
			_recordsListBox.Refresh();
			_entryDetailPanel.Focus();
		}
	}
}
