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

		public void AddWidgetRow(string label, bool isHeader, Control control)
		{
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
			this.Controls.SetChildIndex(p, 0);//stick at end
		}

		public void AddLabelRow(string label)
		{
			Label l = new Label();
			l.Dock = DockStyle.Top;
			l.Text = label;
			this.Controls.Add(l);
		}
	}
}
