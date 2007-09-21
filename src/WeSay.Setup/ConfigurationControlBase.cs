using System.Drawing;
using System.Windows.Forms;

namespace WeSay.Setup
{
	public  class ConfigurationControlBase :UserControl
	{
		private string _header;
		public ConfigurationControlBase(string header)
		{
			_header = header;
			SetStyle(ControlStyles.ResizeRedraw, true);
		}

		//design-time
		public ConfigurationControlBase()
		{
			_header = string.Empty;
		}

		public void SetOtherStuff()
		{
			Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
			BorderStyle = System.Windows.Forms.BorderStyle.None;
		   // Padding = new Padding(15);
		}

		public string Header
		{
			get
			{
				return _header;
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
  //          ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid);
		}

	}
}