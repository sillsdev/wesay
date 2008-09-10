using System.Windows.Forms;

namespace WeSay.Foundation
{
	public partial class LameProgressDialog: Form
	{
		public LameProgressDialog(string msg)
		{
			InitializeComponent();
			label1.Text = msg;
		}
	}
}