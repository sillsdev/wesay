using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WeSay.Foundation
{
	public partial class LameProgressDialog : Form
	{
		public LameProgressDialog(string msg)
		{
			InitializeComponent();
			this.label1.Text = msg;
		}
	}
}