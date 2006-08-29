using System;
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
		/// we have this instead of just using this.Count() because that is not implemented in Mono 1.16
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

		/// <summary>
		/// I want to hide this from clients who would try to touch my controls directly
		/// </summary>
		protected new ControlCollection Controls
		{
			get
			{
				return base.Controls;
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

			this.Controls.Add(panel);
			//reverse order (that's how docking works)

			//HACK: why is this needed?  Need to do the math when I am more awake
			int i = (_rowCount - insertAtRow) - 1;
			i = Math.Max(i, 0);
			this.Controls.SetChildIndex(panel, i);

			++_rowCount;
			return panel;
		}


		public int GetRowOfControl(Control control)
		{
			return (_rowCount - this.Controls.GetChildIndex(control)) - 1;
		}
	}
}
