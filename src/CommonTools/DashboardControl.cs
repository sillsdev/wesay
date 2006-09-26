using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeSay.UI;

namespace WeSay.CommonTools
{
	public partial class DashboardControl : UserControl, ITask
	{
		private IBindingList _records;

		public DashboardControl(IBindingList records, string unusedLabel) :this(records)
		{

		}

		public DashboardControl(IBindingList records)
		{
			_records = records;
			InitializeComponent();
			this._dictionarySizeLabel.Text = String.Format(StringCatalog.Get(this._dictionarySizeLabel.Text), records.Count);
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
			exporter.Add((IList<LexicalModel.LexEntry>) _records);
			exporter.End();
		}

		public string Description
		{
			get
			{
				return "Switch tasks and see current status of tasks";
			}
		}

		public Predicate<object> Filter
		{
			get
			{
				return delegate(object o){
					return true;
				};
			}
		}

		#endregion
	}
}
