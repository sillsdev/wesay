using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WeSay.CommonTools
{
	public partial class TaskIndicator : UserControl
	{
		public TaskIndicator()
		{
			InitializeComponent();
		}

		private void TaskIndicator_BackColorChanged(object sender, EventArgs e)
		{
		   Debug.Assert(this.BackColor != System.Drawing.Color.Transparent);
		   this._textShortDescription.BackColor = this.BackColor;
		}

		private void _btnName_Click(object sender, EventArgs e)
		{

		}
	}
}
