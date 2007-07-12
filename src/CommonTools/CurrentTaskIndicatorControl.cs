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
			content.BackColor = BackColor;// System.Drawing.Color.Transparent; // set here to avoid assertion
			content.Dock = DockStyle.Fill;
			_indicatorPanel.DockPadding.Right = 10;

			this._indicatorPanel.Controls.Add(content);
			label1.BackColor = _s.BackColor;
			_indicatorPanel.BackColor = _s.BackColor;
			_content.BackColor = _s.BackColor;

			if (Type.GetType("Mono.Runtime") == null) // Work around not yet implemented in Mono
			{
				SetAutoSizeToGrowAndShrink();
			}
		}
		private void SetAutoSizeToGrowAndShrink()
		{
#if !MONO
			this._indicatorPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
#endif
		}
	}
}
