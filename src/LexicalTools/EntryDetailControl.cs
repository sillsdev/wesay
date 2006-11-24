using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
//using Db4objects.Db4o;
//using Db4objects.Db4o.Ext;
//using Db4objects.Db4o.Query;
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
		private readonly FieldInventory _fieldInventory;
		public event EventHandler SelectedIndexChanged;
		private IRecordListManager _recordManager;

		public EntryDetailControl()
		{
			Debug.Assert(DesignMode);
			InitializeComponent();
		}

		public EntryDetailControl(IRecordListManager recordManager, FieldInventory fieldInventory)
		{
			if (recordManager == null)
			{
				throw new ArgumentNullException("recordManager");
			}
			if (fieldInventory == null)
			{
				throw new ArgumentNullException("fieldInventory");
			}
			_recordManager = recordManager;
			_records = recordManager.GetListOfType<LexEntry>();
			_fieldInventory = fieldInventory;
			InitializeComponent();
			BackColor = WeSay.UI.DisplaySettings.Default.BackgroundColor;
			_entryDetailPanel.FieldInventory = _fieldInventory;
			_entryDetailPanel.BackColor = WeSay.UI.DisplaySettings.Default.BackgroundColor;
			_entryDetailPanel.DataSource = CurrentRecord;
			this._btnFind.Text = StringCatalog.Get("Find");


			_recordsListBox.DataSource = _records;
			_recordsListBox.Font = BasilProject.Project.WritingSystems.VernacularWritingSystemDefault.Font;
			_recordsListBox.AutoSize();
			_recordsListBox.Columns[0].Width = _recordsListBox.Width- _recordsListBox.VScrollBar.Width;

			_recordsListBox.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);
			_findText.TextChanged += new EventHandler(_findText_TextChanged);
			_findText.KeyDown += new KeyEventHandler(_findText_KeyDown);
			int originalHeight = _findText.Height;
			_findText.Font = _recordsListBox.Font;
			_findText.WritingSystem = _fieldInventory[Field.FieldNames.EntryLexicalForm.ToString()].WritingSystems[0];
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
			records.AddRange(ApproximateMatcher.FindClosestAndNextClosest(text,_recordManager,_records));
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
