using System;
using System.Windows.Forms;

namespace WeSay.LexicalTools
{
	public partial class CongratulationsControl : UserControl
	{
		public CongratulationsControl()
		{
			InitializeComponent();
			// We don't want the user to edit/modify what we display!
			// This prevents https://jira.sil.org/browse/WS-92.
			_messageText.ReadOnly = true;
			_messageText.HideSelection = true;
			_messageText.TabStop = false;
			_messageText.Font = new System.Drawing.Font(SIL.i18n.StringCatalog.LabelFont.FontFamily, 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
		}

		public void Show(string message)
		{
			checkmarkLabel.Text = "\u2714 ";    // The trailing space seems critical at times for proper display. (?)
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
