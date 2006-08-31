using System;
using System.Drawing;
using System.Windows.Forms;

namespace WeSay.UI
{
	public partial class WeSayTextBox : TextBox
	{
	   // private bool _isGhost;

		public WeSayTextBox()
		{
		 //   _isGhost = false;
			InitializeComponent();
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
			int interval = 3;
			if (this.BackColor.R < System.Drawing.SystemColors.Window.R)
			{
				interval = Math.Min(interval, 255 - this.BackColor.R);

				this.BackColor = System.Drawing.Color.FromArgb(this.BackColor.R + interval,
															 this.BackColor.G + interval,
															 this.BackColor.B + interval);
			}
			else if( this.BackColor != System.Drawing.SystemColors.Window)
			{
				this.BackColor = System.Drawing.SystemColors.Window;
			}
		}

		public void PrepareForFadeIn()
		{
				this.Text = ""; //ready for the next one
				this.BackColor = System.Drawing.SystemColors.Control;
		}
	}
}
