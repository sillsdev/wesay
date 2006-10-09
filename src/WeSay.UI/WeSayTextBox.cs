using System;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Language;

namespace WeSay.UI
{
	public partial class WeSayTextBox : TextBox
	{
		private KeymanLink.KeymanLink _keymanLink;
		// private bool _isGhost;
		public WeSayTextBox()
		{
			InitializeComponent();
		}
		public WeSayTextBox(WritingSystem ws)
		{
		 //   _isGhost = false;
			InitializeComponent();
			this.Font = ws.Font;
			_keymanLink = new KeymanLink.KeymanLink();

		}

		public new string Text
		{
			set
			{
				base.Text = value;

				Bitmap  bitmap = new System.Drawing.Bitmap(10, 10);
				System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);
				SizeF sz = graphics.MeasureString(value, this.Font);
			  //  sz.Height += 5;
				if (this.Height < sz.Height)
				{
					this.Height = (int)sz.Height;
				}
			}
			get
			{
				return base.Text;
			}
		}

		//public bool IsGhost
		//{
		//    get
		//    {
		//        return _isGhost;
		//    }
		//    set
		//    {
		//        if (_isGhost != value)
		//        {
		//            if (value) // prepare for fade-in
		//            {
		//                this.Text = ""; //ready for the next one
		//                this.BackColor = System.Drawing.SystemColors.Control;
		//                this.BorderStyle = BorderStyle.None;
		//            }
		//            else  // show as "real"
		//            {
		//                //no change currently
		//            }
		//        }
		//        _isGhost = value;
		//    }
		//}

		public void FadeInSomeMore(Label label)
		{
			int interval = 2;
			if (this.BackColor.R < SystemColors.Window.R)
			{
				interval = Math.Min(interval, 255 - this.BackColor.R);

				this.BackColor = Color.FromArgb(this.BackColor.R + interval,
															 this.BackColor.G + interval,
															 this.BackColor.B + interval);
			}
			else if( this.BackColor != SystemColors.Window)
			{
				this.BackColor = SystemColors.Window;
			}
		}

		public void PrepareForFadeIn()
		{
				this.Text = ""; //ready for the next one
				this.BackColor = SystemColors.Control;
		}

		private void WeSayTextBox_Enter(object sender, EventArgs e)
		{
			_keymanLink.SelectKeymanKeyboard("IPA Unicode 1.0.5",true );
		}

		private void WeSayTextBox_Leave(object sender, EventArgs e)
		{
			_keymanLink.SelectKeymanKeyboard(null, false);

		}
	}
}
