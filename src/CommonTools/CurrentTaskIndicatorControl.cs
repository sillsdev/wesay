using System;
using System.Windows.Forms;
using WeSay.UI;

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
  //          _indicatorPanel.DockPadding.Right = 10;

			UpdateColors();
			this._indicatorPanel.Controls.Add(content);

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

		public void UpdateColors()
		{
			this._shapeControl.BackColor = DisplaySettings.Default.CurrentIndicatorColor;
			label1.BackColor = _shapeControl.BackColor;
			_indicatorPanel.BackColor = _shapeControl.BackColor;
			_content.BackColor = _shapeControl.BackColor;
		}

		private void _shapeControl_Resize(object sender, EventArgs e)
		{
			_indicatorPanel.Width = this.Width - (_indicatorPanel.Left  +10/*HACK*/);
		}

		private void _indicatorPanel_Resize(object sender, EventArgs e)
		{

		}

		private void CurrentTaskIndicatorControl_Resize(object sender, EventArgs e)
		{

		}

		private void _indicatorPanel_Click(object sender, EventArgs e)
		{

		}
	}
}
