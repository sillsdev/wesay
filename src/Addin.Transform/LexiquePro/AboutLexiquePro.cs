using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Addin.Transform.LexiquePro
{
	public partial class AboutLexiquePro : Form
	{
		public AboutLexiquePro()
		{
			InitializeComponent();
		}

		private void OnLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start(((LinkLabel)sender).Text);
			}
			catch(Exception error)
			{
				Palaso.Reporting.ErrorReport.ReportNonFatalMessage("Your operating system could not follow the link.\r\n\r\n{0}", error.Message);
			}
		}
	}
}