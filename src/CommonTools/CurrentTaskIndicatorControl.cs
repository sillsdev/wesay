using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
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
			content.BackColor = this.BackColor;// System.Drawing.Color.Transparent;
			this._indicatorPanel.Controls.Add(content);
		}
	}
}
