using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WeSay.LexicalTools.Dashboard
{
	public partial class GroupHeader: UserControl
	{
		public GroupHeader()
		{
			InitializeComponent();
		}

		public GroupHeader(string name)
		{
			InitializeComponent();
			Name = name;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			//    base.OnPaint(e);
			//e.Graphics.FillRectangle(Brushes.White, this.ClientRectangle);
			Point start = new Point(ClientRectangle.Left, ClientRectangle.Top + 3);
			Point end = new Point(ClientRectangle.Right, ClientRectangle.Top + 3);
			Pen pen = new Pen(Brushes.Black, 2);
			pen.DashStyle = DashStyle.Dot;
			e.Graphics.DrawLine(pen, start, end);
		}

		private void GroupHeader_Load(object sender, EventArgs e)
		{
			_title.Text = Name;
		}
	}
}