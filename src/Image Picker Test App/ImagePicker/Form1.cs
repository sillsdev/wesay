using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ImagePicker
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			imageDisplayWidget1.StorageFolderPath = @"c:\TestWeSayImages";
			imageDisplayWidget2.StorageFolderPath = @"c:\TestWeSayImages";
			imageDisplayWidget3.StorageFolderPath = @"c:\TestWeSayImages";
			imageDisplayWidget3.FileName = "";
			imageDisplayWidget2.FileName = @"furlough2005-1.jpg";
			imageDisplayWidget3.FileName = @"nonexistant.jpg";
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}
	}
}