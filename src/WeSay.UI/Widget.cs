using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WeSay.UI
{
	public class Widget : UserControl
	{
		private FlagButton _flagButton;

		[Browsable(false)]
		public bool FlagIsOn
		{
			get
			{
				if (_flagButton != null)
				{
					return _flagButton.IsSetOn;
				}
				return false;
			}

			set
			{
				if (_flagButton != null)
				{
					_flagButton.IsSetOn = value;
				}
			}
		}

		protected FlagButton MakeFlagButton(Size panelSize)
		{
			_flagButton = new FlagButton();
			_flagButton.Size = new Size(20, 20);
			_flagButton.Location = new Point(
				-1 + panelSize.Width - _flagButton.Width,
				-1 + panelSize.Height - _flagButton.Height);
			_flagButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			_flagButton.Click += new EventHandler(OnClickFlagButton);
			_flagButton.TabStop = false;


			//            Panel panel = new Panel();
			//            panel.Size = flagButton.Size;
			//            panel.Location = flagButton.Location;
			//            panel.Anchor = flagButton.Anchor;
			//            panel.BackColor = System.Drawing.Color.Red;

			return _flagButton;
		}

		private void OnClickFlagButton(object sender, EventArgs e)
		{
			FlagButton b = (FlagButton)sender;
			b.IsSetOn = !b.IsSetOn;
		}
	}
}