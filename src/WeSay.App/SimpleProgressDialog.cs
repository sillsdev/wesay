using System.Windows.Forms;

namespace WeSay.App
{
	public partial class SimpleProgressDialog: Form
	{
		public SimpleProgressDialog(string msg)
		{
			InitializeComponent();
			label1.Text = msg;
		}
	}
}