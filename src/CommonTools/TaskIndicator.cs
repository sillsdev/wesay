using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Project;

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

		//hack 'cause it wasn't resizing (well, it grew, just never shrank (no, not a simple case of wrong AutoSizeMode))
		public void RecalcSize(object sender, EventArgs e)
		{
			this.Size = new System.Drawing.Size(this.Parent.Width - this.Left, this.Parent.Height - this.Top);
			//the following kinda worked, but the panelenclosing this ignored our new size, so that would
			//be need to be worked out to make this worth doing.  It would allow us to increase the distance
			//between indicators when the box was thin enough to need 2 lines for description
//            using(Graphics g = this.CreateGraphics())
//            {
//               SizeF sz= g.MeasureString(this._textShortDescription.Text, this._textShortDescription.Font);
//               if (this.Width < sz.Width)
//               {
//                   this._textShortDescription.Height = (int)sz.Height*2;
//                   this.Height = this._textShortDescription.Bottom + 30;
//               }
			// notice nothing has been written yet to shrink it back if it gets wider
//            }
		}
	}
}
