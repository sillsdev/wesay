using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeSay.UI;

namespace Admin
{
	public partial class AdminWindow : Form
	{
		public AdminWindow()
		{

		   InitializeComponent();

		   if (this.DesignMode)
			   return;

		   BasilProject.InitializeForTests();
		}
	}
}