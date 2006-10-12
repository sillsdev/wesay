using System;
using System.Diagnostics;
using System.Windows.Forms;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.CommonTools
{
	public partial class TaskIndicator : UserControl
	{
		public event EventHandler selected = delegate {};

		private ITask _task;

		public TaskIndicator(ITask task)
		{
			if (task == null)
			{
				throw new ArgumentNullException();
			}
			InitializeComponent();
			_task = task;
			this._count.Text = task.Status;
			this._btnName.Text = task.Label;
			this._textShortDescription.Text = task.Description;
		}

		public ITask Task
		{
			get { return this._task; }
		}

		private void TaskIndicator_BackColorChanged(object sender, EventArgs e)
		{
		   Debug.Assert(BackColor != System.Drawing.Color.Transparent);
		   this._textShortDescription.BackColor = BackColor;
		}

		private void OnBtnNameClick(object sender, EventArgs e)
		{
			selected(this, e);
		}
	}
}
