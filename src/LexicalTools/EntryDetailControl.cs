using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
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
//        private CachedSortedDb4oList<string, LexEntry> _originalRecordList;

		private readonly ViewTemplate _viewTemplate;
		public event EventHandler SelectedIndexChanged;

		public EntryDetailControl()
		{
			Debug.Assert(DesignMode);
			InitializeComponent();
		}

		public string KeyProvider(LexEntry entry)
		{
			return entry.LexicalForm.GetFirstAlternative();
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

			StringComparer comparer;
			try
			{
				string WritingSystemId = viewTemplate.GetField("EntryLexicalForm").WritingSystemIds[0];
				comparer = StringComparer.Create(CultureInfo.GetCultureInfo(WritingSystemId), false);
			}
			catch
			{
				comparer = StringComparer.InvariantCulture;
			}

			_records = new CachedSortedDb4oList<string, LexEntry>((Db4oRecordList<LexEntry>)recordManager.GetListOfType<LexEntry>(),
																			 new LexicalFormToEntryIdInitializer((Db4oRecordListManager)recordManager).Initializer,
																			 KeyProvider,
																			 comparer,
																			 ((Db4oRecordListManager)recordManager).DataPath);

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
			_findText.KeyDown += new KeyEventHandler(_findText_KeyDown);
			int originalHeight = _findText.Height;
			_findText.Font = _recordsListBox.Font;
			_findText.WritingSystem = _viewTemplate[Field.FieldNames.EntryLexicalForm.ToString()].WritingSystems[0];
			int heightDifference = _findText.Height - originalHeight;
			_recordsListBox.Height -= heightDifference;
			_recordsListBox.Location = new Point(_recordsListBox.Location.X,
												 _recordsListBox.Location.Y + heightDifference);
			_btnFind.Height += heightDifference;
			this._findText.ItemFilterer = ApproximateMatcher.FindClosestAndNextClosestAndPrefixedForms;
			this._findText.Items = (CachedSortedDb4oList<string, LexEntry>)_records;
		}

		void _findText_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.Modifiers == Keys.None &&
			   e.KeyData == Keys.Enter)
			{
				Find(this._findText.Text);
			}
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
