using System;
using System.Windows.Forms;
using WeSay.LexicalModel;
using WeSay.UI;
using WeSay.Data;

namespace WeSay.CommonTools
{
	public partial class DashboardControl : UserControl, ITask
	{
		private IRecordList<LexicalModel.LexEntry> _records;

		public DashboardControl(IRecordListManager recordListManager)
		{
			_records = recordListManager.Get<LexicalModel.LexEntry>();
			InitializeComponent();
			this._projectNameLabel.Text = BasilProject.Project.Name;
			DictionaryStatusControl status = new DictionaryStatusControl(recordListManager.Get<LexEntry>());
			this._vbox.AddControlToBottom(status);
			_vbox.AddControlToBottom(new CurrentTaskIndicatorControl(new TaskIndicator()));
			_vbox.AddControlToBottom(new TaskIndicator());
			_vbox.AddControlToBottom(new TaskIndicator());
			_vbox.AddControlToBottom(new TaskIndicator());
		}

		#region ITask
		public void Activate()
		{

		}

		public void Deactivate()
		{

		}

		public string Label
		{
			get { return "Dashboard";
				 }
		}

		public Control Control
		{
			get { return this; }
		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		public string Description
		{
			get
			{
				return "Switch tasks and see current status of tasks";
			}
		}

		#endregion
	}
}
