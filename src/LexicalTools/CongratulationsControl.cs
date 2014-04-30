using System;
using System.Windows.Forms;

namespace WeSay.LexicalTools
{
	public partial class CongratulationsControl: UserControl
	{
		public CongratulationsControl()
		{
			InitializeComponent();
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
