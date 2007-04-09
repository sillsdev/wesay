using System;
using System.Windows.Forms;

namespace WeSay.CommonTools
{
	public partial class CurrentTaskIndicatorControl : UserControl
	{
		private readonly Control _content;

		public CurrentTaskIndicatorControl(Control content)
		{
			_content = content;
			InitializeComponent();
			content.BackColor = BackColor;// System.Drawing.Color.Transparent;
			this._indicatorPanel.Controls.Add(content);
			ShapeControl.ShapeControl s = new ShapeControl.ShapeControl();
			s.TabStop = false;
			s.Shape = ShapeControl.ShapeType.RoundedRectangle;
			s.BorderStyle = System.Drawing.Drawing2D.DashStyle.Dot;
			s.BorderWidth = 1;
			s.BorderColor = System.Drawing.Color.Black;
			s.BackColor = BackColor;
			BackColor = System.Drawing.Color.White;
			s.Dock = DockStyle.Fill;
//            s.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			Controls.Add(s);
			label1.BackColor = s.BackColor;
			_indicatorPanel.BackColor = s.BackColor;
			_content.BackColor = s.BackColor;


			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				SetAutoSizeToGrowAndShrink();
			}

		}
		private void SetAutoSizeToGrowAndShrink()
		{
			this._indicatorPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		}


	   //  //I don't know why this was needed, but it works
	   //private void CurrentTaskIndicatorControl_SizeChanged(object sender, EventArgs e)
	   // {
	   //     ((TaskIndicator)_content).RecalcSize(this, null);

	   // }


	}
}
