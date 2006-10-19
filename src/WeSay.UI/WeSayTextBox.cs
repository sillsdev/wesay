using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Language;

namespace WeSay.UI
{
	public partial class WeSayTextBox : TextBox
	{
		private WritingSystem _writingSystem;
		private KeymanLink.KeymanLink _keymanLink;

		public WeSayTextBox()
		{
			InitializeComponent();
		  //  Debug.Assert(DesignMode);
			BorderStyle = BorderStyle.None;
			BackColor = System.Drawing.Color.White;
			_keymanLink = new KeymanLink.KeymanLink();
			if(!_keymanLink.Initialize(false))
			{
				_keymanLink = null;
			}
		}



		public WeSayTextBox(WritingSystem ws):this()
		{
			 WritingSystem = ws;
		}


		public new string Text
		{
			set
			{
				base.Text = value;
			}
			get
			{
				return base.Text;
			}
		}

  //      private void ComputeHeight()
  //      {
  //          System.Drawing.Graphics graphics = this.CreateGraphics();
  //        //  SizeF sz = graphics.MeasureString(this.Text, Font);

  //          Graphics g = this.CreateGraphics();
  //  //        int h = this.Font.FontFamily.GetEmHeight(this.Font.Style) - this.Font.FontFamily.GetLineSpacing(this.Font.Style);
  //          int h = this.Font.FontFamily.GetCellAscent(this.Font.Style) + this.Font.FontFamily.GetCellDescent(this.Font.Style);
  //          int hInPixels = (int)(this.Font.Size * h / this.Font.FontFamily.GetEmHeight(this.Font.Style));


  ////          Height = hInPixels;

  //      }

		public WritingSystem WritingSystem
		{
			get { return _writingSystem; }
			set
			{
				_writingSystem = value;
				Font = value.Font;
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
			if (BackColor.R < SystemColors.Window.R)
			{
				interval = Math.Min(interval, 255 - BackColor.R);

				BackColor = Color.FromArgb(BackColor.R + interval,
															 BackColor.G + interval,
															 BackColor.B + interval);
			}
			else if( BackColor != SystemColors.Window)
			{
				BackColor = SystemColors.Window;
			}
		}

		public void PrepareForFadeIn()
		{
				Text = ""; //ready for the next one
				BackColor = SystemColors.Control;
		}

		private void WeSayTextBox_Enter(object sender, EventArgs e)
		{
			//this.BackColor = System.Drawing.Color.Yellow;

			//the following fixes some uknown problem where the cursor would be in this box,
			//but when you type the text would always go to the firs ws alternative in the enclosing
			//multitext
		 //   Focus();


			if (FindInputLanguage(_writingSystem.KeyboardName) != null)
			{
				InputLanguage.CurrentInputLanguage= FindInputLanguage(_writingSystem.KeyboardName);
			}
			else if (_keymanLink != null && _writingSystem.KeyboardName != null && _writingSystem.KeyboardName !="")
			{
				_keymanLink.SelectKeymanKeyboard(_writingSystem.KeyboardName, true);
			}
		}

		private InputLanguage FindInputLanguage(string name)
		{
			foreach (InputLanguage  l in InputLanguage.InstalledInputLanguages )
			{
				if (l.LayoutName == name)
				{
					return l;
				}
			}
			return null;
		}

		private void WeSayTextBox_Leave(object sender, EventArgs e)
		{
		   // this.BackColor = System.Drawing.Color.White;

			if (FindInputLanguage(_writingSystem.KeyboardName) != null)//just a weird way to know if we changed the keyboard when we came in
			{
				InputLanguage.CurrentInputLanguage = InputLanguage.DefaultInputLanguage;
			}
			else if (_keymanLink != null)
			{
				_keymanLink.SelectKeymanKeyboard(null, false);
			}

		}

	//        protected override Padding DefaultMargin
//        {
//            get
//            {
//                return new Padding(0);
//            }
//        }
//        public override Size GetPreferredSize(Size proposedSize)
//        {
//            Size preferredSize = base.GetPreferredSize(proposedSize);
//            preferredSize.Height = this.FontHeight;
//            return preferredSize;
//        }

//        protected override Size DefaultSize
//        {
//            get
//            {
//                Size defaultSize =base.DefaultSize;
//                defaultSize.Height = this.FontHeight;
//                return defaultSize;
//            }
//        }
//        protected override void OnFontChanged(EventArgs e)
//        {
//            base.OnFontChanged(e);
//            Height = FontHeight;
//        }
//        protected override void OnSizeChanged(EventArgs e)
//        {
//            base.OnSizeChanged(e);
//            Height = FontHeight;
//        }
	}
}
