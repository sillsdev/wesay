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
			Controls.Add(content);
			Controls.SetChildIndex(content, 0);
		}

		public void UpdateColors()
		{
			this._shapeControl.BackColor = DisplaySettings.Default.CurrentIndicatorColor;
			label1.BackColor = _shapeControl.BackColor;
			_content.BackColor = _shapeControl.BackColor;
		}

		private void _shapeControl_Resize(object sender, EventArgs e)
		{
			_content.Width = Width - (_content.Left + Padding.Right + _shapeControl.BorderWidth);
		}
	}
}
