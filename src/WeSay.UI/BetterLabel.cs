using System.Drawing;
using System.Windows.Forms;

namespace WeSay.UI
{
	/// <summary>
	/// Labels are fairly limitted even in .net, but on mono so far, multi-line
	/// labels are trouble.  This class uses TextBox to essentially be a better
	/// cross-platform label.
	/// </summary>
	public partial class BetterLabel : TextBox
	{
		public BetterLabel():base()
		{
			Font = SystemFonts.MessageBoxFont;
			InitializeComponent();
		}

		//make it transparent
		private void BetterLabel_ParentChanged(object sender, System.EventArgs e)
		{
			BackColor = Parent.BackColor;
			Parent.BackColorChanged += ((x, y) => BackColor = Parent.BackColor);
		}

	}
}
