using System;
using System.Windows.Forms;

namespace WeSay.LexicalTools
{
	public partial class CongratulationsControl: UserControl
	{
		public CongratulationsControl()
		{
			InitializeComponent();
			// We don't want the user to edit/modify what we display!
			// This prevents https://jira.sil.org/browse/WS-92.
			_messageText.ReadOnly = true;
			_messageText.HideSelection = true;
			_messageText.TabStop = false;
		}

		public void Show(string message)
		{
			char[] checkmark_Char = new char[1];
			checkmark_Char[0] = '\u2714';
			checkmarkLabel.Text = new string(checkmark_Char);
			_messageText.Text = message;
			BringToFront();
			Visible = true;
		}

		private void CongratulationsControl_BackColorChanged(object sender, EventArgs e)
		{
			_messageText.BackColor = BackColor;
		}
	}
}
