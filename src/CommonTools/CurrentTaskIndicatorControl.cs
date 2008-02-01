using System;
using System.Windows.Forms;
using WeSay.UI;

namespace WeSay.CommonTools
{
	public partial class CurrentTaskIndicatorControl : UserControl
	{
		private readonly TaskIndicator _content;

		public CurrentTaskIndicatorControl()
		{
			if(!DesignMode)
			{
				throw new NotSupportedException("Only allowed in Design Mode");
			}
			_content = new TaskIndicator();
			InitializeComponent();
		}

		public CurrentTaskIndicatorControl(TaskIndicator content)
		{
			_content = content;
			InitializeComponent();

			UpdateColors();
			content.Location = new System.Drawing.Point(70, 35);
			content.AutoSize = false;
			content.Width = Width - 80;
			content.Height = 100;
			content.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			Controls.Add(content);
			Controls.SetChildIndex(content, 0);
		}

		public void UpdateColors()
		{
			this._shapeControl.CenterColor = DisplaySettings.Default.CurrentIndicatorColor;
			label1.BackColor = _shapeControl.CenterColor;
			_content.BackColor = _shapeControl.CenterColor;
		}

		private void _shapeControl_Resize(object sender, EventArgs e)
		{
			_content.Width = Width - (_content.Left + Padding.Right + _shapeControl.BorderWidth);
		}
	}
}
