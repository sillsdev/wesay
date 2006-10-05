using System;
using System.Drawing;
using System.Windows.Forms;
using WeSay.LexicalModel;
using WeSay.UI;
using WeSay.Data;

namespace WeSay.LexicalTools
{
	public partial class LexFieldTool : UserControl
	{
		private IRecordList<LexEntry> _records;
		public event EventHandler SelectedIndexChanged;

		//private int _currentIndex;
		public LexFieldTool(IRecordList<LexEntry> records, Predicate<string> fieldFilter)
		{
			if (records == null)
			{
				throw new ArgumentNullException();
			}
			_records = records;

			InitializeComponent();
			_lexFieldDetailPanel.BackColor = SystemColors.Control;//we like it to stand out at design time, but not runtime
			_lexFieldDetailPanel.ShowField = fieldFilter;
			_lexFieldDetailPanel.DataSource = CurrentRecord;

			_recordsListBox.DataSource = _records;
			_recordsListBox.Font = BasilProject.Project.WritingSystems.VernacularWritingSystemDefault.Font;
			_recordsListBox.AutoSize();
			_recordsListBox.Columns.StretchToFit();


			_recordsListBox.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);

		}

		void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			//_currentIndex = _recordsListBox.SelectedIndex;
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

		public LexFieldControl ControlDetails
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
