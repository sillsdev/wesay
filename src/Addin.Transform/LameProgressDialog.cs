using System.Windows.Forms;

namespace Addin.Transform
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