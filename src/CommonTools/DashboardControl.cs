using System;
using System.Windows.Forms;
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
			this._dictionarySizeLabel.Text = String.Format(StringCatalog.Get(this._dictionarySizeLabel.Text), _records.Count);
			this._projectNameLabel.Text = BasilProject.Project.Name;
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

		private void exportLIFT_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string path = string.Format(@"c:\{0}-lift.xml", BasilProject.Project.Name);
			LexicalModel.LiftExporter exporter = new WeSay.LexicalModel.LiftExporter(path);
			exporter.Add(_records);
			exporter.End();
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
