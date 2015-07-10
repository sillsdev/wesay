using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using SIL.i18n;
using WeSay.Project;

namespace WeSay.LexicalTools.Dashboard
{
	public partial class TaskIndicator: UserControl
	{
		public event EventHandler Selected = delegate { };

		private readonly ITask _task;

		public TaskIndicator()
		{
			//if(!DesignMode)
			//{
			//    throw new NotSupportedException("Only allowed in Design Mode");
			//}
			InitializeComponent();
		}

		public TaskIndicator(ITask task)
		{
			if (task == null)
			{
				throw new ArgumentNullException();
			}
			InitializeComponent();
			_task = task;
			_intray.Count = task.GetRemainingCount();
			_intray.ReferenceCount = task.GetReferenceCount();

			_btnName.Font = (Font)StringCatalog.LabelFont.Clone();
			_btnName.Text = task.Label; //these have already gone through the StringCatalog
			_textShortDescription.Text = task.Description;
			_textShortDescription.Font = (Font)StringCatalog.LabelFont.Clone();
		}

		public ITask Task
		{
			get { return _task; }
		}

		private void TaskIndicator_BackColorChanged(object sender, EventArgs e)
		{
			Debug.Assert(BackColor != Color.Transparent);
			_textShortDescription.BackColor = BackColor;
			_intray.BackColor = BackColor;
			_layout.BackColor = BackColor;
		}

		private void OnBtnNameClick(object sender, EventArgs e)
		{
			Selected(this, e);
		}
	}
}