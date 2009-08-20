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
			label1.Visible = showChangeDefaultWsMessage;
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			MessageBox.Show(
				"Feel free to close this program and move that folder anywhere you like.  Once you move it, you'll need to open it at least once using this tool in order for WeSay to know where it went.");
		}

		private void FigureOutLabelSizes(object sender, System.EventArgs e)
		{
			label1.MaximumSize = new Size(this.Width-(10+label1.Left), 0);
			label3.MaximumSize = new Size(this.Width - (10 + label3.Left), 0);
			_whereLabel.MaximumSize = new Size(this.Width - (10 + _whereLabel.Left), 0);
		}
	}
}
