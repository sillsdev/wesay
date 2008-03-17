using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WeSay.App
{
	public partial class LongStartupNotification : Form
	{
		public LongStartupNotification()
		{
			InitializeComponent();
		}
		public string Message
		{
			set { _message.Text = value; }
		}
	}
}