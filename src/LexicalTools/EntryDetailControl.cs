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
		private readonly ViewTemplate _viewTemplate;
		public event EventHandler SelectedIndexChanged;
		private IRecordListManager _recordManager;

		public EntryDetailControl()
		{
			Debug.Assert(DesignMode);
			InitializeComponent();
		}

		public int CompareMultiText(MultiText x, MultiText y)
		{
			if(x == null)
			{
				if (y == null)
				{
					return 0; // both null so equal
				}
				else
				{
					return -1; // x is null so y is greater
				}
			}
			else
			{
				if(y == null)
				{
					return 1; // y is null so x is greater
				}
				else
				{
					return x.GetFirstAlternative().CompareTo(y.GetFirstAlternative());
				}
			}
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
			_recordManager = recordManager;
			_records = recordManager.GetListOfType<LexEntry>();

			_viewTemplate = viewTemplate;
			InitializeComponent();
			BackColor = WeSay.UI.DisplaySettings.Default.BackgroundColor;
			_entryDetailPanel.ViewTemplate = _viewTemplate;
			_entryDetailPanel.BackColor = WeSay.UI.DisplaySettings.Default.BackgroundColor;
			_entryDetailPanel.DataSource = CurrentRecord;
			_btnFind.Text = StringCatalog.Get("Find");


			_recordsListBox.DataSource = _records;
			_recordsListBox.Font = BasilProject.Project.WritingSystems.VernacularWritingSystemDefault.Font;
			_recordsListBox.AutoSize();
			_recordsListBox.Columns[0].Width = _recordsListBox.Width- _recordsListBox.VScrollBar.Width;

			_recordsListBox.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);
			_findText.TextChanged += new EventHandler(_findText_TextChanged);
			_findText.KeyDown += new KeyEventHandler(_findText_KeyDown);
			int originalHeight = _findText.Height;
			_findText.Font = _recordsListBox.Font;
			_findText.WritingSystem = _viewTemplate[Field.FieldNames.EntryLexicalForm.ToString()].WritingSystems[0];
			int heightDifference = _findText.Height - originalHeight;
			_recordsListBox.Height -= heightDifference;
			_recordsListBox.Location = new Point(_recordsListBox.Location.X,
												 _recordsListBox.Location.Y + heightDifference);
			_btnFind.Height += heightDifference;
		}

		void _findText_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.Modifiers == Keys.None &&
			   e.KeyData == Keys.Enter)
			{
				Find(this._findText.Text);
			}
		}

		void _findText_TextChanged(object sender, EventArgs e)
		{
			if (_btnFind.Text == StringCatalog.Get("Clear"))
			{
				_btnFind.Text = StringCatalog.Get("Find");
			}
		}

		void _btnFind_Click(object sender, EventArgs e)
		{
			if (this._btnFind.Text == StringCatalog.Get("Find"))
			{
				Find(this._findText.Text);
			}
			else
			{
				ClearLastFind();
			}
		}


		private void ClearLastFind() {
			// reset to original records
			this._records = this._recordManager.GetListOfType<LexEntry>();
			this._recordsListBox.DataSource = this._records;
			this._btnFind.Text = StringCatalog.Get("Find");

			// toggle state between clear and find
			this._findText.ResetText();
		}

		private void Find(string text) {
			Cursor currentCursor = Cursor;
			Cursor = Cursors.WaitCursor;
			InMemoryBindingList<object> records = new InMemoryBindingList<object>();
			records.AddRange(ApproximateMatcher.FindClosestAndNextClosestAndPrefixed(text, _recordManager, _records));
			this._records = records;
			this._recordsListBox.DataSource = this._records;

			// toggle state between find and clear
			this._btnFind.Text = StringCatalog.Get("Clear");
			Cursor = currentCursor;
		}


		void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			if (_entryDetailPanel.DataSource == CurrentRecord)
			{
				//we were getting 3 calls to this for each click on a new word
				return;
			}
			_entryDetailPanel.DataSource = CurrentRecord;
			_btnDeleteWord.Enabled = (CurrentRecord != null);
			if (SelectedIndexChanged != null)
			{
				SelectedIndexChanged.Invoke(this,null);
			}
		}

		private LexEntry CurrentRecord
		{
			get
			{
				if (_records.Count == 0)
				{
					return null;
				}
				return (LexEntry)_records[CurrentIndex];
			}
		}

		protected int CurrentIndex
		{
			get { return _recordsListBox.SelectedIndex; }
		}

		private void _btnNewWord_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			LexEntry entry = new LexEntry();
			_records.Add(entry);
			_recordsListBox.SelectedIndex = _records.IndexOf(entry);
		}

		private void _btnDeleteWord_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Debug.Assert(CurrentIndex >= 0);
			_records.RemoveAt(CurrentIndex);
			//hack until we can get selection change events sorted out in BindingGridList
			OnRecordSelectionChanged(this, null);
			_recordsListBox.Refresh();
		}
	}
}
