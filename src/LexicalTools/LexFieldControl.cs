using System;
using System.Drawing;
using System.Windows.Forms;
using WeSay.LexicalModel;
using WeSay.UI;
using WeSay.Data;

namespace WeSay.LexicalTools
{
	public partial class LexFieldControl : UserControl
	{
		private IRecordList<LexEntry> _records;
		private readonly FieldInventory _fieldInventory;
		public event EventHandler SelectedIndexChanged;

		public LexFieldControl(IRecordList<LexEntry> records, FieldInventory fieldInventory)
		{
			if (records == null)
			{
				throw new ArgumentNullException("records");
			}
			if(fieldInventory == null)
			{
				throw new ArgumentNullException("fieldInventory");
			}
			_records = records;
			_fieldInventory = fieldInventory;

			InitializeComponent();
			_lexFieldDetailPanel.BackColor = SystemColors.Control;//we like it to stand out at design time, but not runtime
			_lexFieldDetailPanel.DataSource = CurrentRecord;

			_recordsListBox.DataSource = _records;
			_recordsListBox.Font = BasilProject.Project.WritingSystems.VernacularWritingSystemDefault.Font;
			_recordsListBox.AutoSize();
			_recordsListBox.Columns.StretchToFit();


			_recordsListBox.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);

		}

		void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			_lexFieldDetailPanel.DataSource = CurrentRecord;
			if (SelectedIndexChanged != null)
			{
				SelectedIndexChanged.Invoke(this,null);
			}
		}

		protected int CurrentIndex
		{
			get { return _recordsListBox.SelectedIndex; }
		}

		public LexPreviewWithEntryControl ControlDetails
		{
			get
			{
				return _lexFieldDetailPanel;
			}
		}
		/// <summary>
		/// Gets current record as selected in record list
		/// </summary>
		/// <value>null if record list is empty</value>
		private LexEntry CurrentRecord
		{
			get
			{
				if (_records.Count == 0)
				{
					return null;
				}
				return _records[CurrentIndex];// _currentIndex];
			}
		}
	}
}
