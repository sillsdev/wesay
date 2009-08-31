using System.Drawing;
using System.Windows.Forms;

namespace WeSay.ConfigTool.NewProjectCreation
{
	public partial class NewProjectInformationDialog : Form
	{
		public NewProjectInformationDialog(string pathToProject, bool showChangeDefaultWsMessage)
		{
			this.Font = SystemFonts.MessageBoxFont; // use the default os font
			InitializeComponent();
			_whereLabel.Text = string.Format(_whereLabel.Text, pathToProject);
			_changeVMessage.Visible = showChangeDefaultWsMessage;
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			MessageBox.Show(
				"Feel free to close this program and move that folder anywhere you like.  Once you move it, you'll need to open it at least once using this tool in order for WeSay to know where it went.");
		}
	}
}
