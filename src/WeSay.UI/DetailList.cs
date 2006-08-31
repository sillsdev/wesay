using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace WeSay.UI
{
	/// <summary>
	/// This control displays a simple list of {label, control} pairs, one per row.
	/// It supports dynamically removing and inserting new rows, to support
	/// "ghost" fields
	/// </summary>
	public partial class DetailList : UserControl
	{
		/// <summary>
		/// we have this instead of just using this.Count() because  Count not implemented in Mono 1.16
		/// </summary>
		protected int _rowCount = 0;

		public DetailList()
		{
			InitializeComponent();

			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				this.Margin = new Padding(5, 5, 5, 5); //not implemented inn mono 1.16
			}
		}

		public void Clear()
		{
			_rowCount = 0;
			base.Controls.Clear();
		}

	   /// <summary>
		/// I want to hide this from clients who would try to touch my controls directly
		/// </summary>
		///doesn't work to hide       protected new ControlCollection Controls
		public new ControlCollection Controls
		{
			get
			{
				throw new ApplicationException("Please don't access my Controls property externally.");
			}
		}

		public Control AddWidgetRow(string label, bool isHeader, Control control)
		{
			return AddWidgetRow(label, isHeader, control, _rowCount);
		}


		public Control AddWidgetRow(string fieldLabel, bool isHeader, Control control, int insertAtRow)
		{
			control.Size = new Size(200, 50);
			if (insertAtRow < 0)
			{
				  insertAtRow = _rowCount;
			}

			Panel panel = new Panel();
			panel.Size = new Size(25, 25);
			panel.Dock = DockStyle.Top;

			Label label = new Label();
			if (isHeader)
			{
				label.Font = new Font(label.Font, FontStyle.Bold);
			}



			label.Text = fieldLabel;
			label.Size = new Size(60, 50);
			label.Dock = DockStyle.Left;
			panel.Controls.Add(label);

			control.Dock = DockStyle.Right;

			control.Width = this.Width - (label.Width + 30);

			// in mono 1.16,  controls with was something like zero,
			//such that the the child width was something like -3.
			//we tried various things to work around this and couldn't get anything smart to work.
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				//didn't work
				//ResumeLayout(true);
				//this.Refresh();

				////for mono debugging. Indications were that this was coming out negative on mono.
				if (control.Width < 0)
					control.Width = 400;

				//didn't help    control.Dock = DockStyle.Fill;
			}

			control.Left = label.Width + 10;
			control.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			panel.Controls.Add(control);

			base.Controls.Add(panel);
			//reverse order (that's how docking works)

			//HACK: why is this needed?  Need to do the math when I am more awake
			int i = (_rowCount - insertAtRow);// -1;
			Debug.Assert(i >= 0, "A negative insertion value will fail under Mono.");

			base.Controls.SetChildIndex(panel, i);

			++_rowCount;
			return panel;
		}


		public int GetRowOfControl(Control control)
		{
			return (_rowCount - base.Controls.GetChildIndex(control)) - 1;
		}

		private void _fadeInTimer_Tick(object sender, EventArgs e)
		{
			foreach (Control c in base.Controls)
			{
				if (c.Controls.Count < 2)
					continue;
				TextBox tb = c.Controls[1] as TextBox;
				if (tb == null)
					continue;

				int interval = 1;
				if (tb.BackColor.R < 255)
				{
					interval = Math.Min(interval, 255 - tb.BackColor.R);

					//tb.BackColor = System.Drawing.Color.FromArgb(tb.BackColor.A+1,tb.BackColor);
					tb.BackColor = System.Drawing.Color.FromArgb(tb.BackColor.R + interval,
					 tb.BackColor.G + interval,
					 tb.BackColor.B + interval);

				}
				else
				{
					tb.BorderStyle = BorderStyle.FixedSingle;
				 Label l = c.Controls[0] as Label;
				 l.Visible = true;
			   }
			}
		}
	}
}
