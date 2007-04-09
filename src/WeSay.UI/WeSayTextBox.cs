using System;
using System.ComponentModel;
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
			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				_keymanLink = new KeymanLink.KeymanLink();
				if (!_keymanLink.Initialize(false))
				{
					_keymanLink = null;
				}
			}

			CalculateMinimumSize();

		}

		void OnResize(object sender, EventArgs e)
		{
			UpdateHeight();
		}
		void OnTextChanged(object sender, EventArgs e)
		{
			UpdateHeight();
		}

		private void CalculateMinimumSize()
		{
			using (Graphics g = CreateGraphics())
			{
				SizeF sz = g.MeasureString("x", Font, Width, StringFormat.GenericTypographic);
				int margin = 12;
				MinimumSize = new Size(Width, margin + (int)sz.Height);
			}
		}


		private void UpdateHeight()
		{
			using (Graphics g = CreateGraphics())
			{
				//we add an extran line feed here because this always trims off the last line if it was empty
				SizeF sz = g.MeasureString(Text+"\n", Font, Width, StringFormat.GenericTypographic);
				Height = 5+ (int) Math.Ceiling(sz.Height);
			}
		}

		public WeSayTextBox(WritingSystem ws):this()
		{
			 WritingSystem = ws;
		}

		[Browsable(false)]
		public override string Text
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

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WritingSystem WritingSystem
		{
			get
			{
				if(_writingSystem == null)
				{
					throw new InvalidOperationException("WritingSystem must be initialized prior to use.");
				}
				return _writingSystem;
			}
			set
			{
				if(value == null)
				{
					throw new ArgumentNullException();
				}
				_writingSystem = value;
				Font = value.Font;
				if (value.RightToLeft)
				{
					RightToLeft = RightToLeft.Yes;
				}
				else
				{
					RightToLeft = RightToLeft.No;
				}
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
			AssignKeyboardFromWritingSystem();
		}

		public void AssignKeyboardFromWritingSystem()
		{
			if (_writingSystem == null)
			{
				throw new InvalidOperationException("WritingSystem must be initialized prior to use.");
			}
//
//            if (_writingSystem.KeyboardName == null || _writingSystem.KeyboardName == string.Empty)
//            {
//                InputLanguage.CurrentInputLanguage = InputLanguage.DefaultInputLanguage;
//                return;
//            }

			InputLanguage inputLanguage = FindInputLanguage(this._writingSystem.KeyboardName);
			if (inputLanguage != null)
			{
				InputLanguage.CurrentInputLanguage = inputLanguage;
			}
			else
			{
				//set the windows back to default so it doesn't interfere
				//nice idea but is unneeded... perhaps keyman is calling this too
				//InputLanguage.CurrentInputLanguage = InputLanguage.DefaultInputLanguage;

				if (_keymanLink != null && !string.IsNullOrEmpty(_writingSystem.KeyboardName))
				{
					_keymanLink.SelectKeymanKeyboard(_writingSystem.KeyboardName, true);
				}
			}
		}

		private InputLanguage FindInputLanguage(string name)
		{
			if(InputLanguage.InstalledInputLanguages != null) // as is the case on Linux
			{
				foreach (InputLanguage  l in InputLanguage.InstalledInputLanguages )
				{
					if (l.LayoutName == name)
					{
						return l;
					}
				}
			}
			return null;
		}

		private void WeSayTextBox_Leave(object sender, EventArgs e)
		{
		   // this.BackColor = System.Drawing.Color.White;
			ClearKeyboard();
		}

		public void ClearKeyboard() {
			if (_writingSystem == null)
			{
				throw new InvalidOperationException("WritingSystem must be initialized prior to use.");
			}
			if (FindInputLanguage(this._writingSystem.KeyboardName) != null)//just a weird way to know if we changed the keyboard when we came in
			{
				InputLanguage.CurrentInputLanguage = InputLanguage.DefaultInputLanguage;
			}
			else if (this._keymanLink != null)
			{
				this._keymanLink.SelectKeymanKeyboard(null, false);
			}
		}

		/// <summary>
		/// for automated tests
		/// </summary>
		public void PretendLostFocus()
		{
			OnLostFocus(new EventArgs());
		}
	}
}
