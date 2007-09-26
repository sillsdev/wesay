using System.Windows.Forms;

namespace WeSay.LexicalTools
{
	public partial class CongratulationsControl : UserControl
	{
		public CongratulationsControl()
		{
			InitializeComponent();
		}


		public void Show(string message)
		{
			_messageText.Text = message;
			this.BringToFront();
			this.Visible = true;
		}

		private void CongratulationsControl_BackColorChanged(object sender, System.EventArgs e)
		{
			_messageText.BackColor = this.BackColor;
		}

	}
}
