using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WeSay.CommonTools
{
	public partial class GroupHeader : UserControl
	{
		public GroupHeader()
		{
			InitializeComponent();
		}


		protected override void OnPaint(PaintEventArgs e)
		{
		//    base.OnPaint(e);
			//e.Graphics.FillRectangle(Brushes.White, this.ClientRectangle);
			Point start = new Point(this.ClientRectangle.Left, this.ClientRectangle.Top + 3);
			Point end = new Point(this.ClientRectangle.Right, this.ClientRectangle.Top + 3);
			Pen pen = new Pen(Brushes.Black, 2);
			pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
			e.Graphics.DrawLine(pen, start, end);
		}

		private void GroupHeader_Load(object sender, EventArgs e)
		{
			_title.Text = Name;
		}
	}
}
