using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WeSay.UI
{
	public partial class DetailList : UserControl
	{
		public DetailList()
		{
			InitializeComponent();
			this.Margin = new Padding(5, 5, 5, 5);
		}

		public Control AddWidgetRow(string label, bool isHeader, Control control)
		{
			return AddWidgetRow(label, isHeader, control, this.Controls.Count);
		}


		public Control AddWidgetRow(string label, bool isHeader, Control control, int row)
		{
			if (row < 0)
			{
			   row= this.Controls.Count;
			}
		   Panel p = new Panel();
			p.Size = new Size(25, 25);
			p.Dock = DockStyle.Top;

			Label l = new Label();
			if (isHeader)
			{
				l.Font = new Font(l.Font, FontStyle.Bold);
			}
			l.Text = label;
			l.Width = 60;
			l.Dock = DockStyle.Left;
			p.Controls.Add(l);

			control.Dock = DockStyle.Right;
			control.Width = this.Width - (l.Width + 30);
			control.Left = l.Width + 10;
			control.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			p.Controls.Add(control);

			this.Controls.Add(p);
			//reverse order (that's how docking works)
			this.Controls.SetChildIndex(p, (this.Controls.Count-row) - 1);

			return p;
		}

		public int GetRowOfControl(Control control)
		{
			return (this.Controls.Count - this.Controls.GetChildIndex(control) ) -1;
		}

		//public void AddLabelRow(string label)
		//{
		//    Label l = new Label();
		//    l.Dock = DockStyle.Top;
		//    l.Text = label;
		//    this.Controls.Add(l);
		//}
	}
}
