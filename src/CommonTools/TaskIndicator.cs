using System;
using System.Diagnostics;
using System.Windows.Forms;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Foundation;
using WeSay.Project;

namespace WeSay.CommonTools
{
	public partial class TaskIndicator : UserControl
	{
		public event EventHandler Selected = delegate {};

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
			_intray.Count = task.Count;
			_intray.ReferenceCount = task.ReferenceCount;

			//_btnName.Font = StringCatalog.LabelFont;
			this._btnName.Text = task.Label;//these have already gone through the StringCatalog
			this._textShortDescription.Text = task.Description;//these have already gone through the StringCatalog
			_textShortDescription.Font = StringCatalog.ModifyFontForLocalization(_textShortDescription.Font);
		}

		public ITask Task
		{
			get { return this._task; }
		}

		private void TaskIndicator_BackColorChanged(object sender, EventArgs e)
		{
		   Debug.Assert(BackColor != System.Drawing.Color.Transparent);
		   this._textShortDescription.BackColor = BackColor;
		   _intray.BackColor = BackColor;
		   _layout.BackColor = BackColor;
	   }

		private void OnBtnNameClick(object sender, EventArgs e)
		{
			Selected(this, e);
		}
	}
}
