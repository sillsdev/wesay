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