using System;
using System.Drawing;
using System.Windows.Forms;

namespace Addin.Transform.LexiquePro
{
	public partial class AboutLexiquePro : Form
	{
		public AboutLexiquePro()
		{
			Font = SystemFonts.MessageBoxFont;
			InitializeComponent();
		}

		private void OnLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start(((LinkLabel)sender).Text);
			}
			catch (Exception error)
			{
				SIL.Reporting.ErrorReport.NotifyUserOfProblem("Your operating system could not follow the link.\r\n\r\n{0}", error.Message);
			}
		}
	}
}