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
			_statusMessage.Text = message;
			this.BringToFront();
			this.Visible = true;
		}

//        public void Hide()
//        {
//            this.Visible = false;
//        }
	}
}
